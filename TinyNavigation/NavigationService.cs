using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TinyNavigation
{
    /// <summary>
    /// Resolve pages instances
    /// </summary>
    public interface IPageProvider
    {
        /// <summary>
        /// Resolve page instance by name
        /// </summary>
        /// <param name="page">Page name</param>
        /// <returns>Page instance</returns>
        Page ResolvePage(string page);

        /// <summary>
        /// Resolve page type by name
        /// </summary>
        /// <param name="page">Page name</param>
        /// <returns>Page type</returns>
        Type GetPageType(string page);
    }

    /// <summary>
    /// Resolve Xamarin Forms application instance
    /// </summary>
    public interface IApplicationProvider
    {
        /// <summary>
        /// Resolve MainPage instance
        /// </summary>
        /// <returns>Current application MainPage</returns>
        Page GetMainPage();
    }

    /// <summary>
    /// Ask for permission before navigate
    /// </summary>
    public interface IAskForNavigation
    {
        /// <summary>
        /// Return true when navigation service is allowed to handle navigation
        /// </summary>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns></returns>
        Task<bool> CanNavigateAsync(IDictionary<string, object> parameters);
    }

    /// <summary>
    /// Intercept navigation events
    /// </summary>
    public interface IInterceptNavigation
    {
        /// <summary>
        /// Occours after navigation push this page/viewmodel
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns></returns>
        Task OnPushedAsync(CancellationToken token, IDictionary<string, object> parameters);

        /// <summary>
        /// Occours after navigation pop this page/viewmodel
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns></returns>
        Task OnPopedAsync(CancellationToken token, IDictionary<string, object> parameters);
    }

    /// <summary>
    /// Navigation service abstraction
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to page using URL string
        /// </summary>
        /// <param name="url">Navigation url</param>
        /// <returns>Task</returns>
        Task NavigateAsync(string url);

        /// <summary>
        /// Navigate to page using URL string
        /// </summary>
        /// <param name="url">Navigation url</param>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns>Task</returns>
        Task NavigateAsync(string url, IDictionary<string, object> parameters);

        /// <summary>
        /// Navigate to page using URL string
        /// </summary>
        /// <param name="url">Navigation url</param>
        /// <param name="parameters">Navigation parameters</param>
        /// <param name="options">Navigation options</param>
        /// <returns></returns>
        Task NavigateAsync(string url, IDictionary<string, object> parameters, NavigationOptions options);

        /// <summary>
        /// Navigate back
        /// </summary>
        /// <returns></returns>
        Task GoBackAsync();

        /// <summary>
        /// Navigate back
        /// </summary>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns></returns>
        Task GoBackAsync(IDictionary<string, object> parameters);

        /// <summary>
        /// Navigate back
        /// </summary>
        /// <param name="parameters">Navigation parameters</param>
        /// <param name="options">Navigation options</param>
        /// <returns></returns>
        Task GoBackAsync(IDictionary<string, object> parameters, NavigationOptions options);
    }

    /// <summary>
    /// Defines some navigation options
    /// </summary>
    public struct NavigationOptions
    {
        public bool Animated { get; private set; }
        public bool? Modal { get; set; }

        public NavigationOptions(bool animated, bool? modal = null)
        {
            Animated = animated;
            Modal = modal;
        }

        public static NavigationOptions Default = new NavigationOptions(true);
    }

    public sealed class NavigationService : INavigationService
    {
        private const string RELATIVE_URL = "/";
        private const string POP_URL = "../";
        private const string REMOVE = "__Remove";
        private const string QUERY_DELIMITATOR = "?";

        private readonly IPageProvider _pageProvider;
        private readonly IApplicationProvider _applicationProvider;

        private CancellationTokenSource _popTokenSource;
        private CancellationTokenSource _pushTokenSource;

        private Page _page;

        private Page GetCurrentPage() =>
            _page ?? _applicationProvider.GetMainPage();

        [Preserve]
        public NavigationService(IPageProvider pageProvider,
                                 IApplicationProvider applicationProvider)
        {
            _pageProvider = pageProvider;
            _applicationProvider = applicationProvider;
        }

        public Task NavigateAsync(string name) =>
            NavigateAsync(name, null);

        public Task NavigateAsync(string name, IDictionary<string, object> parameters) =>
            NavigateInternal(name, parameters, NavigationOptions.Default);

        public Task NavigateAsync(string name, IDictionary<string, object> parameters, NavigationOptions options) =>
            NavigateInternal(name, parameters, options);

        public Task GoBackAsync() =>
            GoBackAsync(null);

        public Task GoBackAsync(IDictionary<string, object> parameters) =>
            GoBackInternal(parameters, NavigationOptions.Default);

        public Task GoBackAsync(IDictionary<string, object> parameters, NavigationOptions options) =>
            GoBackInternal(parameters, options);

        private Task NavigateInternal(string name, IDictionary<string, object> parameters, NavigationOptions options)
        {
            if (name.StartsWith(POP_URL))
                name = name.Replace(POP_URL, REMOVE);

            return ProcessNavigation(new NavUri(name), parameters, options);
        }

        private async Task ProcessNavigation(NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            if (uri.IsEmpty())
                return;

            await Task.Yield();

            var nextSegment = uri.NextSegment();
            var currentPage = !uri.IsAbsoluteUri ? GetCurrentPage() : null;

            if (nextSegment == REMOVE)
            {
                await ProcessForRemoveSegment(currentPage, uri, parameters, options).ConfigureAwait(false);
            }
            else if (currentPage == null)
            {
                await ProcessForRootPage(nextSegment, uri, parameters, options).ConfigureAwait(false);
            }
            else if (currentPage is ContentPage contentPage)
            {
                await ProcessForContentPage(contentPage, nextSegment, uri, parameters, options).ConfigureAwait(false);
            }
            else if (currentPage is NavigationPage navigationPage)
            {
                await ProcessForNavigationPage(navigationPage, nextSegment, uri, parameters, options).ConfigureAwait(false);
            }
            else if (currentPage is TabbedPage tabbedPage)
            {
                await ProcessForTabbedPage(tabbedPage, nextSegment, uri, parameters, options).ConfigureAwait(false);
            }
        }

        private Task ProcessForRemoveSegment(Page currentPage, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            if (!HasNavigationParent(currentPage))
                throw new InvalidOperationException("Relative navigation requires navigation page, currentPage");

            if (uri.CanRemoveAndPush())
                return RemoveAndPush(currentPage, uri, parameters, options);
            else
                return RemoveAndGoBack(currentPage, uri, parameters);
        }

        private Task RemoveAndGoBack(Page currentPage, NavUri uri, IDictionary<string, object> parameters)
        {
            var pagesToRemove = GetPagesToRemove(currentPage, uri);

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);

            return GoBackAsync(parameters);
        }

        private async Task RemoveAndPush(Page currentPage, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            var pagesToRemove = GetPagesToRemove(currentPage, uri, true);

            await ProcessNavigation(uri, parameters, options).ConfigureAwait(false);

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);
        }

        private static IEnumerable<Page> GetPagesToRemove(Page currentPage, NavUri uri, bool includeCurrent = false)
        {
            if (includeCurrent)
                yield return currentPage;

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (!uri.IsEmpty())
            {
                currentPageIndex -= 1;
                yield return currentPage.Navigation.NavigationStack[currentPageIndex];
                _ = uri.NextSegment();
            }
        }

        private static void RemovePagesFromNavigationPage(Page currentPage, IEnumerable<Page> pagesToRemove)
        {
            if (currentPage.Parent is NavigationPage navigationPage)
            {
                foreach (var page in pagesToRemove)
                {
                    navigationPage.Navigation.RemovePage(page);
                    DisposePageAndViewModel(page);

                }
            }
        }

        private async Task ProcessForRootPage(string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            var nextPage = _pageProvider.ResolvePage(nextSegment);

            _page = nextPage;
            await ProcessNavigation(uri, parameters, options);

            var currentPage = _applicationProvider.GetMainPage();
            var modalStack = currentPage?.Navigation.ModalStack.ToList();

            await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(null, nextPage, useModalNavigation, animated);
            });

            if (currentPage != null)
            {
                DestroyWithModalStack(currentPage, modalStack);
            }
        }

        private async Task ProcessForContentPage(Page currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            var nextPageType = _pageProvider.GetPageType(uri.GetSegmentName());
            var useReverse = UseReverseNavigation(currentPage, nextPageType) && !(options.Modal.HasValue && options.Modal.Value);

            if (!useReverse)
            {
                _page = _pageProvider.ResolvePage(nextSegment);
                await ProcessNavigation(uri, parameters, options);

                await DoNavigateAction(currentPage, nextSegment, _page, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, useModalNavigation, animated);
                });
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, uri, parameters, options);
            }
        }

        private async Task ProcessForNavigationPage(NavigationPage currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            if (currentPage.Navigation.NavigationStack.Count == 0)
            {
                await UseReverseNavigation(currentPage, nextSegment, uri, parameters, options);
                return;
            }

            var clearNavigationStack = GetClearNavigationPageNavigationStack(currentPage);
            var isEmptyOfNavigationStack = currentPage.Navigation.NavigationStack.Count == 0;
            var destroyPages = Enumerable.Empty<Page>().ToList();

            if (clearNavigationStack && !isEmptyOfNavigationStack)
            {
                destroyPages = currentPage.Navigation.NavigationStack.ToList();
                destroyPages.Reverse();

                await currentPage.Navigation.PopToRootAsync(false);
            }

            var topPage = currentPage.Navigation.NavigationStack.LastOrDefault();
            var nextPageType = _pageProvider.GetPageType(uri.GetSegmentName());
            if (topPage?.GetType() == nextPageType)
            {
                if (clearNavigationStack)
                    destroyPages.Remove(destroyPages.Last());

                if (!uri.IsEmpty())
                    await UseReverseNavigation(topPage, uri.NextSegment(), uri, parameters, options).ConfigureAwait(true);

                await DoNavigateAction(topPage, nextSegment, topPage, parameters, onNavigationActionCompleted: (p) =>
                {
                    if (nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                    {
                        var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                        SelectPageTab(topPage, segmentParams);
                    }
                }).ConfigureAwait(false);
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, uri, parameters, options).ConfigureAwait(true);

                if (clearNavigationStack && !isEmptyOfNavigationStack)
                    currentPage.Navigation.RemovePage(topPage);
            }

            foreach (var destroyPage in destroyPages)
            {
                DisposePageAndViewModel(destroyPage);
            }
        }

        private async Task ProcessForTabbedPage(TabbedPage currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            _page = _pageProvider.ResolvePage(nextSegment);
            await ProcessNavigation(uri, parameters, options);
            await DoNavigateAction(currentPage, nextSegment, _page, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        private async Task GoBackInternal(IDictionary<string, object> parameters, NavigationOptions options)
        {
            var page = GetCurrentPage();
            var canNavigate = await CanNavigateAsync(page, parameters);
            if (!canNavigate)
                return;

            var useModalForDoPop = UseModalGoBack(page, options.Modal);
            Page previousPage = GetOnNavigatedToTarget(page, _applicationProvider.GetMainPage(), useModalForDoPop);

            var poppedPage = await DoPop(page.Navigation, options);
            if (poppedPage != null)
            {
                HandleOnPush(page, parameters);
                HandleOnPop(previousPage, parameters);
                DisposePageAndViewModel(poppedPage);
            }
        }

        private Task<bool> CanNavigateAsync(Page currentPage, IDictionary<string, object> parameters)
        {

            if (currentPage.BindingContext is IAskForNavigation askForNavigation)
                return askForNavigation.CanNavigateAsync(parameters);

            if (currentPage is IAskForNavigation pageAskForNavigation)
                return pageAskForNavigation.CanNavigateAsync(parameters);

            return Task.FromResult(true);
        }

        private static void DestroyWithModalStack(Page page, IList<Page> modalStack)
        {
            foreach (var childPage in modalStack.Reverse())
            {
                DisposePageAndViewModel(childPage);
            }

            DisposePageAndViewModel(page);
        }

        private void InsertPageBefore(Page currentPage, Page page, int pageOffset)
        {
            var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
            currentPage.Navigation.InsertPageBefore(page, firstPage);
        }

        private bool HasNavigationParent(Page page)
        {
            return page.Parent is NavigationPage;
        }

        private void HandleOnPop(Page previousPage, IDictionary<string, object> parameters)
        {
            _popTokenSource?.Cancel();
            _popTokenSource?.Dispose();
            _popTokenSource = new CancellationTokenSource();
            var token = _popTokenSource.Token;

            Task.Run(() => InvokeViewAndViewModelActionAsync<IInterceptNavigation>(previousPage, e => e.OnPopedAsync(token, parameters)));
        }

        private void HandleOnPush(Page page, IDictionary<string, object> parameters)
        {
            _pushTokenSource?.Cancel();
            _pushTokenSource?.Dispose();
            _pushTokenSource = new CancellationTokenSource();
            var token = _pushTokenSource.Token;

            Task.Run(() => InvokeViewAndViewModelActionAsync<IInterceptNavigation>(page, e => e.OnPopedAsync(token, parameters)));
        }

        private static void DisposePageAndViewModel(Page page)
        {
            InvokeViewAndViewModelAction<IDisposable>(page, e => e.Dispose());
        }

        private static void InvokeViewAndViewModelAction<T>(BindableObject element, Action<T> action) where T : class
        {
            if (element is T viewAsT)
            {
                action(viewAsT);
            }

            if (element.BindingContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        private static async Task InvokeViewAndViewModelActionAsync<T>(BindableObject element, Func<T, Task> action) where T : class
        {
            if (element is T viewAsT)
            {
                await action(viewAsT);
            }

            if (element.BindingContext is T viewModelAsT)
            {
                await action(viewModelAsT);
            }
        }

        public struct NavUri
        {
            private const string LOCAL_HOST = "http://localhost";
            private static readonly char[] _pathDelimiter = { '/' };

            private Uri _uri;
            private bool? isAbsolute;
            private Queue<string> _segments;
            private string currentSegment;

            public bool IsAbsoluteUri { get => isAbsolute ?? _uri.IsAbsoluteUri; }

            public NavUri(string url)
            {
                _uri = Parse(url);
                _segments = GetUriSegments(_uri);
                isAbsolute = null;
                currentSegment = string.Empty;
            }

            public NavUri(Uri uri)
            {
                _uri = uri;
                _segments = GetUriSegments(_uri);
                isAbsolute = null;
                currentSegment = string.Empty;
            }

            public bool IsEmpty() => _segments.Count == 0;

            public string NextSegment()
            {
                currentSegment = _segments.Dequeue();
                return currentSegment;
            }

            public string GetCurrentSegment() => currentSegment;

            public bool CanRemoveAndPush()
            {
                if (_segments.All(x => x == REMOVE))
                    return false;
                else
                    return true;
            }

            private static Queue<string> GetUriSegments(Uri uri)
            {
                var segmentStack = new Queue<string>();

                if (!uri.IsAbsoluteUri)
                {
                    uri = EnsureAbsolute(uri);
                }

                var segments = uri.PathAndQuery.Split(_pathDelimiter, StringSplitOptions.RemoveEmptyEntries);
                foreach (var segment in segments)
                {
                    segmentStack.Enqueue(Uri.UnescapeDataString(segment));
                }

                return segmentStack;
            }

            public string GetSegmentName()
            {
                return currentSegment.Split('?')[0];
            }

            public static IDictionary<string, object> GetSegmentParameters(string segment)
            {
                string query = string.Empty;

                if (string.IsNullOrWhiteSpace(segment))
                {
                    return new NavigationParameters(query);
                }

                var indexOfQuery = segment.IndexOf('?');
                if (indexOfQuery > 0)
                    query = segment.Substring(indexOfQuery);

                return new NavigationParameters(query);
            }

            public static IDictionary<string, object> GetSegmentParameters(string uriSegment, IDictionary<string, object> parameters)
            {
                var navParameters = GetSegmentParameters(uriSegment);

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> navigationParameter in parameters)
                    {
                        navParameters.Add(navigationParameter.Key, navigationParameter.Value);
                    }
                }

                return navParameters;
            }

            private static Uri EnsureAbsolute(Uri uri)
            {
                if (uri.IsAbsoluteUri)
                    return uri;

                if (!uri.OriginalString.StartsWith(RELATIVE_URL, StringComparison.Ordinal))
                {
                    return new Uri($"{LOCAL_HOST}/{uri}", UriKind.Absolute);
                }

                return new Uri($"{LOCAL_HOST}{uri}", UriKind.Absolute);
            }

            private static Uri Parse(string uri)
            {
                if (uri == null) throw new ArgumentNullException(nameof(uri));

                if (uri.StartsWith(RELATIVE_URL, StringComparison.Ordinal))
                {
                    return new Uri($"{LOCAL_HOST}{uri}", UriKind.Absolute);
                }


                return new Uri(uri, UriKind.RelativeOrAbsolute);
            }
        }

        public struct SegmentReference
        {

            public string Segment
            {
                get;
                set;
            }

            public SegmentReference(string segment)
            {
                Segment = segment;
            }
        }
    }
}
