using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        void SetMainPage(Page page);
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
    /// Defines scoped navigation options
    /// </summary>
    public interface INavigationPageOptions
    {
        /// <summary>
        /// Clear navigation stack on navigation
        /// </summary>
        bool ClearNavigationStackOnNavigation { get;}
    }

    /// <summary>
    /// Defines some page or vm to initialize
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// Occours when page/vm is initialized
        /// </summary>
        /// <param name="parameters">Navigation parameters</param>
        /// <returns></returns>
        Task InitializeAsync(IDictionary<string, object> parameters);
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
        private const string SELECTEDTAB = "SelectTab";

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

        public static async Task OnInitializedAsync(BindableObject page, IDictionary<string, object> parameters)
        {
            if (page is null) return;

            await InvokeViewAndViewModelActionAsync<IInitialize>(page, v => v.InitializeAsync(parameters)).ConfigureAwait(false);
        }

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

            await ProcessNavigation(uri, parameters, options).ConfigureAwait(true);

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
            _page = _pageProvider.ResolvePage(nextSegment);
            await ProcessNavigation(uri, parameters, options).ConfigureAwait(true);

            var currentPage = _applicationProvider.GetMainPage();
            var modalStack = currentPage?.Navigation.ModalStack.ToList();

            await DoNavigateAction(
                GetCurrentPage(), uri, _page, parameters, () => DoPush(null, _page, options)).ConfigureAwait(true);

            if (currentPage != null)
                DestroyWithModalStack(currentPage, modalStack);
        }

        private async Task ProcessForContentPage(Page currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            var nextPageType = _pageProvider.GetPageType(uri.GetSegmentName());
            var useReverse = UseReverseNavigation(currentPage, nextPageType) && !(options.Modal.HasValue && options.Modal.Value);

            if (!useReverse)
            {
                _page = _pageProvider.ResolvePage(nextSegment);
                await ProcessNavigation(uri, parameters, options);

                await DoNavigateAction(currentPage, uri, _page, parameters, async () =>
                {
                    await DoPush(currentPage, _page, options);
                });
            }
            else
            {
                await DoReverseNavigation(currentPage, nextSegment, uri, parameters, options);
            }
        }

        private async Task ProcessForNavigationPage(NavigationPage currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            if (currentPage.Navigation.NavigationStack.Count == 0)
            {
                await DoReverseNavigation(currentPage, nextSegment, uri, parameters, options);
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
                    await DoReverseNavigation(topPage, uri.NextSegment(), uri, parameters, options).ConfigureAwait(true);

                await DoNavigateAction(topPage, uri, topPage, parameters, onNavigationActionCompleted: (p) =>
                {
                    if (nextSegment.Contains(SELECTEDTAB))
                    {
                        var segmentParams = uri.GetCurrentSegmentParameters();
                        SelectPageTab(topPage, segmentParams);
                    }
                }).ConfigureAwait(false);
            }
            else
            {
                await DoReverseNavigation(currentPage, nextSegment, uri, parameters, options).ConfigureAwait(true);

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
            await ProcessNavigation(uri, parameters, options).ConfigureAwait(true);
            await DoNavigateAction(currentPage, uri, _page, parameters, () => DoPush(currentPage, _page, options)).ConfigureAwait(false);
        }

        private async Task GoBackInternal(IDictionary<string, object> parameters, NavigationOptions options)
        {
            var page = GetCurrentPage();
            var canNavigate = await CanNavigateAsync(page, parameters).ConfigureAwait(true);
            if (!canNavigate)
                return;

            var useModalForDoPop = UseModalGoBack(page, options.Modal);
            var previousPage = GetOnNavigatedToTarget(page, _applicationProvider.GetMainPage(), useModalForDoPop);

            var poppedPage = await DoPop(page.Navigation, options);
            if (poppedPage != null)
            {
                HandleOnPush(page, parameters);
                HandleOnPop(previousPage, parameters);
                DisposePageAndViewModel(poppedPage);
            }
        }

        private async Task DoNavigateAction(Page fromPage, NavUri uri, Page toPage, IDictionary<string, object> parameters, Func<Task> navigationAction = null, Action<IDictionary<string, object>> onNavigationActionCompleted = null)
        {
            var segmentParameters = uri.GetCurrentSegmentParameters(parameters);

            var canNavigate = await CanNavigateAsync(fromPage, segmentParameters).ConfigureAwait(true);
            if (!canNavigate)
                return;

            await OnInitializedAsync(toPage, segmentParameters).ConfigureAwait(true);

            if (navigationAction != null)
                await navigationAction().ConfigureAwait(true);

            HandleOnPop(fromPage, segmentParameters);

            onNavigationActionCompleted?.Invoke(segmentParameters);

            HandleOnPush(toPage, segmentParameters);
        }

        private async Task DoReverseNavigation(Page currentPage, string nextSegment, NavUri uri, IDictionary<string, object> parameters, NavigationOptions options)
        {
            var navigationStack = new Stack<string>();

            if (!String.IsNullOrWhiteSpace(nextSegment))
                navigationStack.Push(nextSegment);

            var illegalSegments = new Queue<string>();

            bool illegalPageFound = false;
            foreach (var item in uri.GetSegments())
            {
                //if we run into an illegal page, we need to create new navigation segments to properly handle the deep link
                if (illegalPageFound)
                {
                    illegalSegments.Enqueue(item);
                    continue;
                }

                var pageType = _pageProvider.GetPageType(item);
                if (IsSameOrSubclassOf<MasterDetailPage>(pageType))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }

            var pageOffset = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 2)
                pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

            var onNavigatedFromTarget = currentPage;
            if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
                onNavigatedFromTarget = navPage.CurrentPage;

            var insertBefore = false;
            while (navigationStack.Count > 0)
            {
                var segment = navigationStack.Pop();
                var nextPage = _pageProvider.ResolvePage(segment);
                await DoNavigateAction(onNavigatedFromTarget, uri, nextPage, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, options, insertBefore, pageOffset);
                });
                insertBefore = true;
            }

            //if an illegal page is found, we force a Modal navigation
            if (illegalSegments.Count > 0)
            {
                options.Modal = true;
                uri.UpdateSegments(illegalSegments);
                _page = currentPage.Navigation.NavigationStack.Last();
                await ProcessNavigation(uri, parameters, options);
            }

        }

        private Task DoPush(Page currentPage, Page page, NavigationOptions options, bool insertBeforeLast = false, int navigationOffset = 0)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (currentPage == null)
            {
                _applicationProvider.SetMainPage(page);
                return Task.CompletedTask;
            }
            else
            {
                bool useModalForPush = UseModalNavigation(currentPage, options.Modal);

                if (useModalForPush)
                {
                    return currentPage.Navigation.PushModalAsync(page, options.Animated);
                }
                else
                {
                    if (insertBeforeLast)
                    {
                        InsertPageBefore(currentPage, page, navigationOffset);
                        return Task.CompletedTask;
                    }
                    else
                    {
                        return currentPage.Navigation.PushAsync(page, options.Animated);
                    }
                }
            }
        }

        private Task<Page> DoPop(INavigation navigation, NavigationOptions options)
        {
            if (options.Modal == true)
                return navigation.PopModalAsync(options.Animated);
            else
                return navigation.PopAsync(options.Animated);
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

        private void SelectPageTab(Page page, IDictionary<string, object> parameters)
        {
            if (page is TabbedPage tabbedPage)
            {
                TabbedPageSelectTab(tabbedPage, parameters);
            }
            else if (page is CarouselPage carouselPage)
            {
                CarouselPageSelectTab(carouselPage, parameters);
            }
        }

        private void TabbedPageSelectTab(TabbedPage tabbedPage, IDictionary<string, object> parameters)
        {
            var selectedTab = string.Empty;
            if(parameters != null)
            {
                if(parameters.TryGetValue(SELECTEDTAB, out var parameter))
                    selectedTab = parameter.ToString();
            }
            
            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var pageName = selectedTab.Split('?')[0];
                var selectedTabType = _pageProvider.GetPageType(pageName);

                var childFound = false;
                foreach (var child in tabbedPage.Children)
                {
                    if (!childFound && child.GetType() == selectedTabType)
                    {
                        tabbedPage.CurrentPage = child;
                        childFound = true;
                    }

                    if (child is NavigationPage)
                    {
                        if (!childFound && ((NavigationPage)child).CurrentPage.GetType() == selectedTabType)
                        {
                            tabbedPage.CurrentPage = child;
                            childFound = true;
                        }
                    }
                }
            }
        }

        private void CarouselPageSelectTab(CarouselPage carouselPage, IDictionary<string, object> parameters)
        {
            var selectedTab = string.Empty;
            if (parameters != null)
            {
                if (parameters.TryGetValue(SELECTEDTAB, out var parameter))
                    selectedTab = parameter.ToString();
            }

            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var pageName = selectedTab.Split('?')[0];
                var selectedTabType = _pageProvider.GetPageType(pageName);

                foreach (var child in carouselPage.Children)
                {
                    if (child.GetType() == selectedTabType)
                        carouselPage.CurrentPage = child;
                }
            }
        }

        private void InsertPageBefore(Page currentPage, Page page, int pageOffset)
        {
            var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
            currentPage.Navigation.InsertPageBefore(page, firstPage);
        }

        private bool HasNavigationParent(Page page) =>
            HasNavigationParent(page, out _);

        private bool HasNavigationParent(Page page, out NavigationPage navigationPage)
        {
            if (page?.Parent != null)
            {
                if (page.Parent is NavigationPage navParent)
                {
                    navigationPage = navParent;
                    return true;
                }
                else if ((page.Parent is TabbedPage || page.Parent is CarouselPage) && page.Parent?.Parent is NavigationPage navigationParent)
                {
                    navigationPage = navigationParent;
                    return true;
                }
            }

            navigationPage = null;
            return false;
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

        private static void DisposePageAndViewModel(Page page) =>
            InvokeViewAndViewModelAction<IDisposable>(page, e => e.Dispose());

        private static void InvokeViewAndViewModelAction<T>(BindableObject element, Action<T> action) where T : class
        {
            if (element is T viewAsT)
                action(viewAsT);

            if (element.BindingContext is T viewModelAsT)
                action(viewModelAsT);
        }

        private static async Task InvokeViewAndViewModelActionAsync<T>(BindableObject element, Func<T, Task> action) where T : class
        {
            if (element is T viewAsT)
                await action(viewAsT).ConfigureAwait(false);

            if (element.BindingContext is T viewModelAsT)
                await action(viewModelAsT).ConfigureAwait(false);
        }

        public static Page GetOnNavigatedToTarget(Page page, Page mainPage, bool useModalNavigation)
        {
            Page target;
            if (useModalNavigation)
            {
                var previousPage = GetPreviousPage(page, page.Navigation.ModalStack);

                //MainPage is not included in the navigation stack, so if we can't find the previous page above
                //let's assume they are going back to the MainPage
                target = GetOnNavigatedToTargetFromChild(previousPage ?? mainPage);
            }
            else
            {
                target = GetPreviousPage(page, page.Navigation.NavigationStack);
                if (target != null)
                    target = GetOnNavigatedToTargetFromChild(target);
                else
                    target = GetOnNavigatedToTarget(page, mainPage, true);
            }

            return target;
        }

        public static Page GetOnNavigatedToTargetFromChild(Page target)
        {
            Page child = null;

            if (target is MasterDetailPage)
                child = ((MasterDetailPage)target).Detail;
            else if (target is TabbedPage)
                child = ((TabbedPage)target).CurrentPage;
            else if (target is CarouselPage)
                child = ((CarouselPage)target).CurrentPage;
            else if (target is NavigationPage)
                child = target.Navigation.NavigationStack.Last();

            if (child != null)
                target = GetOnNavigatedToTargetFromChild(child);

            return target;
        }

        public static Page GetPreviousPage(Page currentPage, IReadOnlyList<Page> navStack)
        {
            Page previousPage = null;

            int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
            int previousPageIndex = currentPageIndex - 1;
            if (navStack.Count >= 0 && previousPageIndex >= 0)
                previousPage = navStack[previousPageIndex];

            return previousPage;
        }

        public static int GetCurrentPageIndex(Page currentPage, IReadOnlyList<Page> navStack)
        {
            int stackCount = navStack.Count;
            for (int x = 0; x < stackCount; x++)
            {
                var view = navStack[x];
                if (view == currentPage)
                    return x;
            }

            return stackCount - 1;
        }

        private bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
        {
            if (useModalNavigationDefault.HasValue)
                return useModalNavigationDefault.Value;
            else if (currentPage is NavigationPage)
                return false;

            return !HasNavigationParent(currentPage);
        }

        private bool UseModalGoBack(Page currentPage, bool? useModalNavigationDefault)
        {
            if (useModalNavigationDefault.HasValue)
                return useModalNavigationDefault.Value;
            else if (currentPage is NavigationPage navPage)
                return GoBackModal(navPage);
            else if (HasNavigationParent(currentPage, out var navParent))
                return GoBackModal(navParent);

            return true;
        }

        private bool GetClearNavigationPageNavigationStack(NavigationPage page)
        {
            if (page is INavigationPageOptions iNavigationPage)
                return iNavigationPage.ClearNavigationStackOnNavigation;

            if (page.BindingContext is INavigationPageOptions iNavigationPageBindingContext)
                return iNavigationPageBindingContext.ClearNavigationStackOnNavigation;

            return true;
        }

        private bool UseReverseNavigation(Page currentPage, Type nextPageType) =>
            HasNavigationParent(currentPage) && IsSameOrSubclassOf<ContentPage>(nextPageType);

        private bool IsSameOrSubclassOf<T>(Type potentialDescendant)
        {
            if (potentialDescendant == null)
                return false;

            Type potentialBase = typeof(T);

            return potentialDescendant.GetTypeInfo().IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        private bool GoBackModal(NavigationPage navPage)
        {
            if (navPage.CurrentPage != navPage.RootPage)
                return false;
            else if (navPage.CurrentPage == navPage.RootPage && navPage.Parent is Application && _applicationProvider.GetMainPage() != navPage)
                return true;
            else if (navPage.Parent is TabbedPage tabbed && tabbed != _applicationProvider.GetMainPage())
                return true;
            else if (navPage.Parent is CarouselPage carousel && carousel != _applicationProvider.GetMainPage())
                return true;

            return false;
        }

        public struct NavUri
        {
            private const string LOCAL_HOST = "http://localhost";
            private static readonly char[] _pathDelimiter = { '/' };

            private Uri uri;
            private bool? isAbsolute;
            private string currentSegment;
            private Queue<string> segments;

            public bool IsAbsoluteUri { get => isAbsolute ?? uri.IsAbsoluteUri; }

            public NavUri(string url)
            {
                uri = Parse(url);
                segments = GetUriSegments(uri);
                isAbsolute = null;
                currentSegment = string.Empty;
            }

            public NavUri(Uri uri)
            {
                this.uri = uri;
                segments = GetUriSegments(this.uri);
                isAbsolute = null;
                currentSegment = string.Empty;
            }

            public bool IsEmpty() => segments.Count == 0;

            public string NextSegment()
            {
                currentSegment = segments.Dequeue();
                return currentSegment;
            }

            public string GetCurrentSegment() => currentSegment;

            public bool CanRemoveAndPush()
            {
                if (segments.All(x => x == REMOVE))
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

            public string GetSegmentName() =>
                GetSegmentNameInternal(currentSegment);

            private string GetSegmentNameInternal(string segment) =>
                segment.Split('?')[0];

            public IDictionary<string, object> GetCurrentSegmentParameters()
            {
                var query = string.Empty;
                if (string.IsNullOrWhiteSpace(currentSegment))
                    return ParseParameters(query).ToDictionary(q => q.Item1, q => q.Item2);

                var indexOfQuery = currentSegment.IndexOf('?');
                if (indexOfQuery > 0)
                    query = currentSegment.Substring(indexOfQuery);

                return ParseParameters(query).ToDictionary(q => q.Item1, q => q.Item2);
            }

            public IDictionary<string, object> GetCurrentSegmentParameters(IDictionary<string, object> parameters)
            {
                var navParameters = GetCurrentSegmentParameters();

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, object> navigationParameter in parameters)
                    {
                        navParameters.Add(navigationParameter);
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

            private IEnumerable<(string, object)> ParseParameters(string query)
            {
                if (!string.IsNullOrWhiteSpace(query))
                    yield break;

                int num = query.Length;
                for (int i = ((query.Length > 0) && (query[0] == '?')) ? 1 : 0; i < num; i++)
                {
                    int startIndex = i;
                    int num4 = -1;
                    while (i < num)
                    {
                        char ch = query[i];
                        if (ch == '=')
                        {
                            if (num4 < 0)
                            {
                                num4 = i;
                            }
                        }
                        else if (ch == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value;
                    if (num4 >= 0)
                    {
                        key = query.Substring(startIndex, num4 - startIndex);
                        value = query.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        value = query.Substring(startIndex, i - startIndex);
                    }

                    if (key != null)
                        yield return (Uri.UnescapeDataString(key), Uri.UnescapeDataString(value));
                }
            }

            public IReadOnlyCollection<string> GetSegments()
            {
                var instance = this;
                return segments.Select(s => instance.GetSegmentNameInternal(s)).ToList();
            }

            public void UpdateSegments(Queue<string> illegalSegments)
            {
                segments = illegalSegments;
                currentSegment = string.Empty;
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
