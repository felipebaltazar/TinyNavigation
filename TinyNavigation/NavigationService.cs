
///Uncomment this line to use Unity container
//#define USE_UNITY_CONTAINER

///Uncomment this line to use Dryioc container
//#define USE_DRYIOC_CONTAINER

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using TinyNavigation.Abstractions;
using TinyNavigation.Ioc;
using TinyNavigation.Behaviors;
using TinyNavigation.MVVM;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Transactions;


#if USE_UNITY_CONTAINER && !USE_DRYIOC_CONTAINER

using Unity;
using Unity.Lifetime;
using Unity.Resolution;

#elif USE_DRYIOC_CONTAINER

using DryIoc;

#else

using TinyIoC;

#endif


namespace TinyNavigation.Abstractions
{
    /// <summary>
    /// Register platformspecific dependencies
    /// </summary>
    public interface IPlatformInitializer
    {
        void RegisterTypes(IContainerRegistry containerRegistry);
    }

    /// <summary>
    /// The registering container
    /// </summary>
    public interface IContainerRegistry
    {
        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterInstance(Type type, object instance);

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterInstance(Type type, object instance, string name);

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterSingleton(Type from, Type to);

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterSingleton(Type from, Type to, string name);

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod);

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod);

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementation <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes);

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry Register(Type from, Type to);

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry Register(Type from, Type to, string name);

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry Register(Type type, Func<object> factoryMethod);

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod);

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementing <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes);

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterScoped(Type from, Type to);

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod);

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type"/>.</param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod);

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        bool IsRegistered(Type type);

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        bool IsRegistered(Type type, string name);
    }

    /// <summary>
    /// A strongly typed container extension
    /// </summary>
    /// <typeparam name="TContainer">The underlying root container</typeparam>
    public interface IContainerExtension<TContainer> : IContainerExtension
    {
        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        TContainer Instance { get; }
    }

    /// <summary>
    /// A generic abstraction for what Prism expects from a container
    /// </summary>
    public interface IContainerExtension : IContainerProvider, IContainerRegistry
    {
        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        void FinalizeExtension();
    }

    /// <summary>
    /// Defines a contract for providing Application components.
    /// </summary>
    public interface IApplicationProvider
    {
        /// <summary>
        /// Gets or sets the main page of the Application.
        /// </summary>
        Page MainPage { get; set; }
    }

    /// <summary>
    /// Interface to signify that a class must have knowledge of a specific <see cref="Xamarin.Forms.Page"/> instance in order to function properly.
    /// </summary>
    public interface IPageAware
    {
        /// <summary>
        /// The <see cref="Xamarin.Forms.Page"/> instance.
        /// </summary>
        Page Page { get; set; }
    }

    /// <summary>
    /// Interface that defines if the object instance is active
    /// and notifies when the activity changes.
    /// </summary>
    public interface IActiveAware
    {
        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value><see langword="true" /> if the object is active; otherwise <see langword="false" />.</value>
        bool IsActive { get; set; }

        /// <summary>
        /// Notifies that the value for <see cref="IsActive"/> property has changed.
        /// </summary>
        event EventHandler IsActiveChanged;
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities after the target Page has been added to the navigation stack.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(INavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities after the target Page has been added to the navigation stack.
    /// </summary>
    public interface INavigatedAware
    {
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(INavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in page licfecycle tobe notified after the target Page has appear/disappear.
    /// </summary>
    public interface IPageLifecycleAware
    {
        /// <summary>
        /// On page appearing
        /// </summary>
        void OnAppearing();

        /// <summary>
        /// On page disappearing
        /// </summary>
        void OnDisappearing();
    }

    /// <summary>
    /// Interface to handle OS related events when Application is put to sleep, etc.
    /// </summary>
    public interface IApplicationLifecycleAware
    {
        /// <summary>
        /// Called when application is resumed
        /// </summary>
        void OnResume();

        /// <summary>
        /// Called when application is put to sleep
        /// </summary>
        void OnSleep();
    }

    /// <summary>
    /// Interface for objects that require cleanup of resources prior to Disposal
    /// </summary>
    public interface IDestructible
    {
        /// <summary>
        /// This method allows cleanup of any resources used by your View/ViewModel 
        /// </summary>
        void Destroy();
    }

    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a NavigationPage during navigation.
    /// </summary>
    public interface INavigationPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the NavigationPage should clear the NavigationStack when navigating to a new Page.
        /// </summary>
        /// <remarks>This is equivalent to calling PopToRoot, and then replacing the current Page with the target Page being navigated to.</remarks>
        bool ClearNavigationStackOnNavigation { get; }
    }

    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a MasterDetailPage during navigation.
    /// </summary>
    public interface IMasterDetailPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the MasterDetailPage.Master should be presented after navigation.
        /// </summary>
        bool IsPresentedAfterNavigation { get; }
    }

    public interface IInitialize
    {
        void Initialize(INavigationParameters parameters);
    }

    public interface IInitializeAsync
    {
        Task InitializeAsync(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigation
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <returns><c>True</c> if navigation can continue, <c>False</c> if navigation is not allowed to continue</returns>
        bool CanNavigate(INavigationParameters parameters);
    }

    /// <summary>
    /// Provides a way for ViewModels involved in navigation to asynchronously determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigationAsync
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        /// <returns><c>True</c> if navigation can continue, <c>False</c> if navigation is not allowed to continue</returns>
        Task<bool> CanNavigateAsync(INavigationParameters parameters);
    }

    /// <summary>
    /// Navigation service abstraction
    /// </summary>
    public interface INavigationService : IPageAware
    {
        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        Task<INavigationResult> GoBackAsync();

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated);

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource));
        /// </example>
        Task<INavigationResult> NavigateAsync(Uri uri);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        Task<INavigationResult> NavigateAsync(string name);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PushModalAsync, if <c>false</c> uses PushAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated);
    }

    /// <summary>
    /// Defines a contract for specifying values associated with a unique key.
    /// </summary>
    public interface IParameters : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Adds the specified key and value to the parameter collection.
        /// </summary>
        /// <param name="key">The key of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        void Add(string key, object value);

        /// <summary>
        /// Determines whether the <see cref="IParameters"/> contains the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to search the parameters for existence.</param>
        /// <returns>true if the <see cref="IParameters"/> contains a parameter with the specified key; otherwise, false.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the number of parameters contained in the <see cref="IParameters"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="IParameters"/>.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the parameter associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to get.</typeparam>
        /// <param name="key">The key of the parameter to find.</param>
        /// <returns>A matching value of <typeparamref name="T"/> if it exists.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets the parameter associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to get.</typeparam>
        /// <param name="key">The key of the parameter to find.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all the values referenced by key.</returns>
        IEnumerable<T> GetValues<T>(string key);

        /// <summary>
        /// Gets the parameter associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to get.</typeparam>
        /// <param name="key">The key of the parameter to get.</param>
        /// <param name="value">
        /// When this method returns, contains the parameter associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// </param>
        /// <returns>true if the <see cref="IParameters"/> contains a parameter with the specified key; otherwise, false.</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Gets the parameter associated with the specified key (legacy).
        /// </summary>
        /// <param name="key">The key of the parameter to get.</param>
        /// <returns>A matching value if it exists.</returns>
        object this[string key] { get; }
    }

    /// <summary>
    /// Provides a way for the <see cref="IDialogService"/> to pass parameters when displaying a dialog.
    /// </summary>
    public interface IDialogParameters : IParameters
    {
    }

    /// <summary>
    /// Provides a way for the <see cref="INavigationService"/> to pass parameters during navigation.
    /// </summary>
    public interface INavigationParameters : IParameters
    {
    }

    /// <summary>
    /// Used to set internal parameters used by Prism's <see cref="INavigationService"/>
    /// </summary>
    public interface INavigationParametersInternal
    {
        /// <summary>
        /// Adds the key and value to the parameters Collection
        /// </summary>
        /// <param name="key">The key to reference this value in the parameters collection.</param>
        /// <param name="value">The value of the parameter to store</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks collection for presense of key
        /// </summary>
        /// <param name="key">The key to check in the Collection</param>
        /// <returns><c>true</c> if key exists; else returns <c>false</c>.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Returns the value of the member referenced by key
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <returns>Returns a matching parameter of <typeparamref name="T"/> if one exists in the Collection</returns>
        T GetValue<T>(string key);
    }

    public interface INavigationResult
    {
        bool Success { get; }

        Exception Exception { get; }
    }

    public interface IPageBehaviorFactory
    {
        /// <summary>
        /// Applies behaviors to a page based on the page type.
        /// </summary>
        /// <param name="page">The page to apply the behaviors</param>
        /// <remarks>The PageLifeCycleAwareBehavior is applied to all pages</remarks>
        void ApplyPageBehaviors(Page page);
    }

    /// <summary>
    /// Resolves Services from the Container
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, params (Type Type, object Instance)[] parameters);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, string name);

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters);

        /// <summary>
        /// Creates a new scope
        /// </summary>
        IScopedProvider CreateScope();

        /// <summary>
        /// Gets the Current Scope
        /// </summary>
        IScopedProvider CurrentScope { get; }
    }

    /// <summary>
    /// Defines a Container Scope
    /// </summary>
    public interface IScopedProvider : IContainerProvider, IDisposable
    {
        /// <summary>
        /// Gets or Sets the IsAttached property.
        /// </summary>
        /// <remarks>
        /// Indicates that Prism is tracking the scope
        /// </remarks>
        bool IsAttached { get; set; }
    }

    public enum PageNavigationSource
    {
        NavigationService,
        Device
    }

    /// <summary>
    /// The NavigationMode provides information about the navigation operation that has been invoked.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Indicates that a navigation operation occured that resulted in navigating backwards in the navigation stack.
        /// </summary>
        Back,
        /// <summary>
        /// Indicates that a new navigation operation has occured and a new page has been added to the navigation stack.
        /// </summary>
        New,
        /// <summary>
        /// Indicates that a forward navigation operation has occured to an existing page.
        /// </summary>
        /// <remarks>Not currently supported on Xamarin.Forms</remarks>
        Forward,
        /// <summary>
        /// Indicates that the current page in the navigation stack has been navigated to again, or it's state has been refreshed.
        /// </summary>
        /// <remarks>Not currently supported on Xamarin.Forms</remarks>
        Refresh,
    }
}

namespace TinyNavigation.Ioc
{
    /// <summary>
    /// The <see cref="ContainerLocator" /> tracks the current instance of the Container used by your Application
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ContainerLocator
    {
        private static Lazy<IContainerExtension> _lazyContainer;

        private static IContainerExtension _current;

        /// <summary>
        /// Gets the current <see cref="IContainerExtension" />.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContainerExtension Current =>
            _current ?? (_current = _lazyContainer?.Value);

        /// <summary>
        /// Gets the <see cref="IContainerProvider" />
        /// </summary>
        public static IContainerProvider Container =>
            Current;

        /// <summary>
        /// Sets the Container Factory to use if the Current <see cref="IContainerProvider" /> is null
        /// </summary>
        /// <param name="factory"></param>
        /// <remarks>
        /// NOTE: We want to use Lazy Initialization in case the container is first created
        /// prior to Prism initializing which could be the case with Shiny
        /// </remarks>
        public static void SetContainerExtension(Func<IContainerExtension> factory) =>
            _lazyContainer = new Lazy<IContainerExtension>(factory);

        /// <summary>
        /// Used for Testing to Reset the Current Container
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetContainer()
        {
            _current = null;
            _lazyContainer = null;
        }
    }

    /// <summary>
    /// Provides Generic Type extensions for the <see cref="IContainerRegistry" />
    /// </summary>
    public static class IContainerRegistryExtensions
    {
        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="TInterface">The service <see cref="Type"/> that is being registered</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance);
        }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <typeparam name="TInterface">The service <see cref="Type"/> that is being registered</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterInstance<TInterface>(this IContainerRegistry containerRegistry, TInterface instance, string name)
        {
            return containerRegistry.RegisterInstance(typeof(TInterface), instance, name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.RegisterSingleton(type, type);
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.RegisterSingleton(typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.RegisterSingleton(typeof(T));
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.RegisterSingleton(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterSingleton<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.RegisterSingleton(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <typeparam name="T">The implementation <see cref="Type" />.</typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public static IContainerRegistry RegisterManySingleton<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes)
        {
            return containerRegistry.RegisterManySingleton(typeof(T), serviceTypes);
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.Register(type, type);
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.Register(typeof(T));
        }

        /// <summary>
        /// Registers a Transient with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register(this IContainerRegistry containerRegistry, Type type, string name)
        {
            return containerRegistry.Register(type, type, name);
        }

        /// <summary>
        /// Registers a Singleton with the given <see cref="Type" />.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <typeparam name="T">The concrete <see cref="Type" /></typeparam>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.Register(typeof(T), name);
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<TFrom, TTo>(this IContainerRegistry containerRegistry, string name) where TTo : TFrom
        {
            return containerRegistry.Register(typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.Register(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry Register<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.Register(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <typeparam name="T">The implementing <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public static IContainerRegistry RegisterMany<T>(this IContainerRegistry containerRegistry, params Type[] serviceTypes)
        {
            return containerRegistry.RegisterMany(typeof(T), serviceTypes);
        }

        /// <summary>
        /// Registers a scoped service.
        /// </summary>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="type">The concrete <see cref="Type" />.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped(this IContainerRegistry containerRegistry, Type type)
        {
            return containerRegistry.RegisterScoped(type, type);
        }

        /// <summary>
        /// Registers a scoped service.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.RegisterScoped(typeof(T));
        }

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <typeparam name="TFrom">The service <see cref="Type" /></typeparam>
        /// <typeparam name="TTo">The implementation <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<TFrom, TTo>(this IContainerRegistry containerRegistry)
            where TTo : TFrom
        {
            return containerRegistry.RegisterScoped(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry, Func<object> factoryMethod)
        {
            return containerRegistry.RegisterScoped(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public static IContainerRegistry RegisterScoped<T>(this IContainerRegistry containerRegistry, Func<IContainerProvider, object> factoryMethod)
        {
            return containerRegistry.RegisterScoped(typeof(T), factoryMethod);
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry)
        {
            return containerRegistry.IsRegistered(typeof(T));
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerRegistry">The instance of the <see cref="IContainerRegistry" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerRegistry containerRegistry, string name)
        {
            return containerRegistry.IsRegistered(typeof(T), name);
        }
    }

    /// <summary>
    /// Provides Generic Type extensions for the <see cref="IContainerProvider" />
    /// </summary>
    public static class IContainerProviderExtensions
    {
        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider)
        {
            return (T)provider.Resolve(typeof(T));
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), parameters);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, string name, params (Type Type, object Instance)[] parameters)
        {
            return (T)provider.Resolve(typeof(T), name, parameters);
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type"/></typeparam>
        /// <param name="provider">The current <see cref="IContainerProvider"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public static T Resolve<T>(this IContainerProvider provider, string name)
        {
            return (T)provider.Resolve(typeof(T), name);
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerProvider">The instance of the <see cref="IContainerProvider" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerProvider containerProvider)
        {
            return containerProvider.IsRegistered(typeof(T));
        }

        internal static bool IsRegistered(this IContainerProvider containerProvider, Type type)
        {
            if (containerProvider is IContainerRegistry containerRegistry)
                return containerRegistry.IsRegistered(type);
            return false;
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <typeparam name="T">The service <see cref="Type" /></typeparam>
        /// <param name="containerProvider">The instance of the <see cref="IContainerProvider" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public static bool IsRegistered<T>(this IContainerProvider containerProvider, string name)
        {
            return containerProvider.IsRegistered(typeof(T), name);
        }

        internal static bool IsRegistered(this IContainerProvider containerProvider, Type type, string name)
        {
            if (containerProvider is IContainerRegistry containerRegistry)
                return containerRegistry.IsRegistered(type, name);
            return false;
        }
    }

    /// <summary>
    /// Used to resolve the registered implementation type for a given key
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IContainerInfo
    {
        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="key">Registration Key</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetRegistrationType(string key);

        /// <summary>
        /// Locates the registered implementation <see cref="Type"/> for a give key
        /// </summary>
        /// <param name="serviceType">Service Type</param>
        /// <returns>Implementation <see cref="Type"/></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetRegistrationType(Type serviceType);
    }

#if USE_UNITY_CONTAINER && !USE_DRYIOC_CONTAINER

    /// <summary>
    /// The Unity implementation of the <see cref="IContainerExtension" />
    /// </summary>
    public class UnityContainerExtension : IContainerExtension<IUnityContainer>, IContainerInfo
    {
        private UnityScopedProvider _currentScope;

        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public IUnityContainer Instance { get; }

        /// <summary>
        /// Constructs a default <see cref="UnityContainerExtension" />
        /// </summary>
        [Preserve]
        public UnityContainerExtension()
            : this(new UnityContainer())
        {
        }

        /// <summary>
        /// Constructs a <see cref="UnityContainerExtension" /> with the specified <see cref="IUnityContainer" />
        /// </summary>
        /// <param name="container"></param>
        public UnityContainerExtension(IUnityContainer container)
        {
            Instance = container;
            string currentContainer = "CurrentContainer";
            Instance.RegisterInstance(currentContainer, this);
            Instance.RegisterFactory(typeof(IContainerExtension), c => c.Resolve<UnityContainerExtension>(currentContainer));
            Instance.RegisterFactory(typeof(IContainerProvider), c => c.Resolve<UnityContainerExtension>(currentContainer));
        }

        /// <summary>
        /// Gets the current <see cref="IScopedProvider"/>
        /// </summary>
        public IScopedProvider CurrentScope => _currentScope;

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        public void FinalizeExtension() { }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
            return this;
        }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterInstance(type, name, instance);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.RegisterSingleton(from, to);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.RegisterSingleton(from, to, name);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, _ => factoryMethod(), new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()), new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementation <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            Instance.RegisterSingleton(type);
            return this;
        }

        private IContainerRegistry RegisterManyInternal(Type implementingType, Type[] serviceTypes)
        {
            if (serviceTypes is null || serviceTypes.Length == 0)
            {
                serviceTypes = implementingType.GetInterfaces().Where(x => x != typeof(IDisposable)).ToArray();
            }

            foreach (var service in serviceTypes)
            {
                Instance.RegisterFactory(service, c => c.Resolve(implementingType));
            }

            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.RegisterType(from, to);
            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.RegisterType(from, to, name);
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, _ => factoryMethod());
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()));
            return this;
        }

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementing <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            Instance.RegisterType(type);
            return this;
        }

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            Instance.RegisterType(from, to, new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(), new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type"/>.</param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterFactory(type, c => factoryMethod(c.Resolve<IContainerProvider>()), new HierarchicalLifetimeManager());
            return this;
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type) =>
            Resolve(type, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name) =>
            Resolve(type, name, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var c = _currentScope?.Container ?? Instance;
                var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Length > 0)
                {
                    type = type.GetGenericArguments()[0];
                    return c.ResolveAll(type, overrides);
                }

                return c.Resolve(type, overrides);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(type.FullName, ex);
            }
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var c = _currentScope?.Container ?? Instance;

                // Unity will simply return a new object() for unregistered Views
                if (!c.IsRegistered(type, name))
                    throw new KeyNotFoundException($"No registered type {type.Name} with the key {name}.");

                var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                return c.Resolve(type, name, overrides);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(name, ex);
            }
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type)
        {
            return Instance.IsRegistered(type);
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type, name);
        }

        Type IContainerInfo.GetRegistrationType(string key)
        {
            //First try friendly name registration. If not found, try type registration
            var matchingRegistration = Instance.Registrations.Where(r => key.Equals(r.Name, StringComparison.Ordinal)).FirstOrDefault();
            if (matchingRegistration == null)
            {
                matchingRegistration = Instance.Registrations.Where(r => key.Equals(r.RegisteredType.Name, StringComparison.Ordinal)).FirstOrDefault();
            }

            return matchingRegistration?.MappedToType;
        }

        Type IContainerInfo.GetRegistrationType(Type serviceType)
        {
            var matchingRegistration = Instance.Registrations.Where(x => x.RegisteredType == serviceType).FirstOrDefault();
            return matchingRegistration?.MappedToType;
        }

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual IScopedProvider CreateScope() =>
            CreateScopeInternal();

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>A child <see cref="IUnityContainer" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IScopedProvider CreateScopeInternal()
        {
            var child = Instance.CreateChildContainer();
            _currentScope = new UnityScopedProvider(child);
            return _currentScope;
        }

        private class UnityScopedProvider : IScopedProvider
        {
            public UnityScopedProvider(IUnityContainer container)
            {
                Container = container;
            }

            public IUnityContainer Container { get; private set; }
            public bool IsAttached { get; set; }
            public IScopedProvider CurrentScope => this;

            public IScopedProvider CreateScope() => this;

            public void Dispose()
            {
                Container.Dispose();
                Container = null;
            }

            public object Resolve(Type type) =>
                Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                    return Container.Resolve(type, overrides);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(type.FullName, ex);
                }
            }

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    // Unity will simply return a new object() for unregistered Views
                    if (!Container.IsRegistered(type, name))
                        throw new KeyNotFoundException($"No registered type {type.Name} with the key {name}.");

                    var overrides = parameters.Select(p => new DependencyOverride(p.Type, p.Instance)).ToArray();
                    return Container.Resolve(type, name, overrides);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(name, ex);
                }
            }
        }
    }


#elif USE_DRYIOC_CONTAINER
    /// <summary>
    /// The <see cref="IContainerExtension" /> Implementation to use with DryIoc
    /// </summary>
    public class DryIocContainerExtension : IContainerExtension<DryIoc.IContainer>, IContainerInfo
    {
        private DryIocScopedProvider _currentScope;

        /// <summary>
        /// Gets the Default DryIoc Container Rules used by Prism
        /// </summary>
        public static Rules DefaultRules => Rules.Default.WithConcreteTypeDynamicRegistrations(reuse: Reuse.Transient)
                                                         .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                         .WithFuncAndLazyWithoutRegistration()
                                                         .WithTrackingDisposableTransients()
                                                         .WithoutFastExpressionCompiler()
                                                         .WithFactorySelector(Rules.SelectLastRegisteredFactory());

        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        public DryIoc.IContainer Instance { get; }

        /// <summary>
        /// Constructs a default instance of the <see cref="DryIocContainerExtension" />
        /// </summary>
        [Preserve]
        public DryIocContainerExtension()
            : this(new DryIoc.Container(DefaultRules))
        {
        }

        /// <summary>
        /// Constructs a new <see cref="DryIocContainerExtension" />
        /// </summary>
        /// <param name="container">The <see cref="IContainer" /> instance to use.</param>
        public DryIocContainerExtension(DryIoc.IContainer container)
        {
            Instance = container;
            Instance.RegisterInstanceMany(new[]
            {
                typeof(IContainerExtension),
                typeof(IContainerProvider)
            }, this);
        }

        /// <summary>
        ///  Gets the current scope
        /// </summary>
        public IScopedProvider CurrentScope => _currentScope;

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        public void FinalizeExtension() { }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
            return this;
        }

        /// <summary>
        /// Registers an instance of a given <see cref="Type"/> with the specified name or key
        /// </summary>
        /// <param name="type">The service <see cref="Type"/> that is being registered</param>
        /// <param name="instance">The instance of the service or <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.RegisterInstance(type, instance, ifAlreadyRegistered: IfAlreadyRegistered.Replace, serviceKey: name);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.Singleton);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Register(from, to, Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace, serviceKey: name);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterDelegate(type, r => factoryMethod(), Reuse.Singleton);
            return this;
        }

        /// <summary>
        /// Registers a Singleton with the given service <see cref="Type" /> factory delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterDelegate(type, factoryMethod, Reuse.Singleton);
            return this;
        }

        /// <summary>
        /// Registers a Singleton Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementation <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            if (serviceTypes.Length == 0)
            {
                serviceTypes = type.GetInterfaces();
            }

            Instance.RegisterMany(serviceTypes, type, Reuse.Singleton);
            return this;
        }

        /// <summary>
        /// Registers a scoped service
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.ScopedOrSingleton);
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterDelegate(type, r => factoryMethod(), Reuse.ScopedOrSingleton);
            return this;
        }

        /// <summary>
        /// Registers a scoped service using a delegate method.
        /// </summary>
        /// <param name="type">The service <see cref="Type"/>.</param>
        /// <param name="factoryMethod">The delegate method using the <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterDelegate(type, factoryMethod, Reuse.ScopedOrSingleton);
            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.Register(from, to, Reuse.Transient);
            return this;
        }

        /// <summary>
        /// Registers a Transient with the given service and mapping to the specified implementation <see cref="Type" />.
        /// </summary>
        /// <param name="from">The service <see cref="Type" /></param>
        /// <param name="to">The implementation <see cref="Type" /></param>
        /// <param name="name">The name or key to register the service</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.Register(from, to, Reuse.Transient, ifAlreadyRegistered: IfAlreadyRegistered.Replace, serviceKey: name);
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.RegisterDelegate(type, r => factoryMethod());
            return this;
        }

        /// <summary>
        /// Registers a Transient Service using a delegate method
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="factoryMethod">The delegate method using <see cref="IContainerProvider"/>.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.RegisterDelegate(type, factoryMethod);
            return this;
        }

        /// <summary>
        /// Registers a Transient Service which implements service interfaces
        /// </summary>
        /// <param name="type">The implementing <see cref="Type" />.</param>
        /// <param name="serviceTypes">The service <see cref="Type"/>'s.</param>
        /// <returns>The <see cref="IContainerRegistry" /> instance</returns>
        /// <remarks>Registers all interfaces if none are specified.</remarks>
        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            if (serviceTypes.Length == 0)
            {
                serviceTypes = type.GetInterfaces();
            }

            Instance.RegisterMany(serviceTypes, type, Reuse.Transient);
            return this;
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type) =>
            Resolve(type, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name) =>
            Resolve(type, name, Array.Empty<(Type, object)>());

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var container = _currentScope?.Resolver ?? Instance;
                return container.Resolve(type, args: parameters.Select(p => p.Instance).ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(type.FullName, ex);
            }
        }

        /// <summary>
        /// Resolves a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The service <see cref="Type"/></param>
        /// <param name="name">The service name/key used when registering the <see cref="Type"/></param>
        /// <param name="parameters">Typed parameters to use when resolving the Service</param>
        /// <returns>The resolved Service <see cref="Type"/></returns>
        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            try
            {
                var container = _currentScope?.Resolver ?? Instance;
                return container.Resolve(type, name, args: parameters.Select(p => p.Instance).ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(name, ex);
            }
        }

        /// <summary>
        /// Determines if a given service is registered
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type)
        {
            return Instance.IsRegistered(type);
        }

        /// <summary>
        /// Determines if a given service is registered with the specified name
        /// </summary>
        /// <param name="type">The service <see cref="Type" /></param>
        /// <param name="name">The service name or key used</param>
        /// <returns><c>true</c> if the service is registered.</returns>
        public bool IsRegistered(Type type, string name)
        {
            return Instance.IsRegistered(type, name) || Instance.IsRegistered(type, name, FactoryType.Wrapper); ;
        }

        Type IContainerInfo.GetRegistrationType(string key)
        {
            var matchingRegistration = Instance.GetServiceRegistrations().Where(r => key.Equals(r.OptionalServiceKey?.ToString(), StringComparison.Ordinal)).FirstOrDefault();
            if (matchingRegistration.OptionalServiceKey == null)
                matchingRegistration = Instance.GetServiceRegistrations().Where(r => key.Equals(r.ImplementationType.Name, StringComparison.Ordinal)).FirstOrDefault();

            return matchingRegistration.ImplementationType;
        }

        Type IContainerInfo.GetRegistrationType(Type serviceType)
        {
            var registration = Instance.GetServiceRegistrations().Where(x => x.ServiceType == serviceType).FirstOrDefault();
            return registration.ServiceType is null ? null : registration.ImplementationType;
        }

        /// <summary>
        /// Creates a new Scope
        /// </summary>
        public virtual IScopedProvider CreateScope() =>
            CreateScopeInternal();

        /// <summary>
        /// Creates a new Scope and provides the updated ServiceProvider
        /// </summary>
        /// <returns>The Scoped <see cref="IResolverContext" />.</returns>
        /// <remarks>
        /// This should be called by custom implementations that Implement IServiceScopeFactory
        /// </remarks>
        protected IScopedProvider CreateScopeInternal()
        {
            var resolver = Instance.OpenScope();
            _currentScope = new DryIocScopedProvider(resolver);
            return _currentScope;
        }

        private class DryIocScopedProvider : IScopedProvider
        {
            public DryIocScopedProvider(IResolverContext resolver)
            {
                Resolver = resolver;
            }

            public bool IsAttached { get; set; }

            public IResolverContext Resolver { get; private set; }
            public IScopedProvider CurrentScope => this;

            public IScopedProvider CreateScope() => this;

            public void Dispose()
            {
                Resolver.Dispose();
                Resolver = null;
            }

            public object Resolve(Type type) =>
                Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    return Resolver.Resolve(type, args: parameters.Select(p => p.Instance).ToArray());
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(type.FullName, ex);
                }
            }

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    return Resolver.Resolve(type, name, args: parameters.Select(p => p.Instance).ToArray());
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(name, ex);
                }
            }
        }
    }
#else
    /// <summary>
    /// TinyIoc container extension
    /// </summary>
    public class TinyIocContainerExtension : IContainerExtension<TinyIoCContainer>
    {
        private TinyScopedProvider _tinyIocScoped;

        public TinyIoCContainer Instance { get; }

        /// <summary>
        /// Constructs a default instance of the <see cref="TinyIocContainerExtension" />
        /// </summary>
        [Preserve]
        public TinyIocContainerExtension()
            : this(new TinyIoC.TinyIoCContainer())
        {
        }

        /// <summary>
        /// Constructs a new <see cref="DryIocContainerExtension" />
        /// </summary>
        /// <param name="container">The <see cref="IContainer" /> instance to use.</param>
        public TinyIocContainerExtension(TinyIoCContainer container)
        {
            Instance = container;
            Instance.Register<IContainerExtension>(this);
            Instance.Register<IContainerProvider>(this);
        }

        public IScopedProvider CurrentScope => _tinyIocScoped;

        public IScopedProvider CreateScope() => CreateScopeInternal();

        public void FinalizeExtension()
        {
        }

        public bool IsRegistered(Type type)
        {
            return Instance.CanResolve(type);
        }

        public bool IsRegistered(Type type, string name)
        {
            return Instance.CanResolve(type, name, ResolveOptions.Default);
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            Instance.Register(from, to).AsMultiInstance();
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            Instance.Register(from, to, name).AsMultiInstance();
            return this;
        }

        public IContainerRegistry Register(Type type, Func<object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke()).AsMultiInstance();
            return this;
        }

        public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke(this)).AsMultiInstance();
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            Instance.Register(type, instance);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            Instance.Register(type, instance, name);
            return this;
        }

        public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
        {
            foreach (var serviceType in serviceTypes)
                Instance.Register(serviceType, type).AsMultiInstance();

            return this;
        }

        public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
        {
            foreach (var serviceType in serviceTypes)
                Instance.Register(serviceType, type).AsSingleton();

            return this;
        }

        public IContainerRegistry RegisterScoped(Type from, Type to)
        {
            Instance.Register(from, to).AsMultiInstance();
            return this;
        }

        public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke()).AsMultiInstance();
            return this;
        }

        public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke(this)).AsMultiInstance();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            Instance.Register(from, to).AsSingleton();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            Instance.Register(from, to, name).AsSingleton();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke()).AsSingleton();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
        {
            Instance.Register(type, (c, p) => factoryMethod.Invoke(this)).AsSingleton();
            return this;
        }

        public object Resolve(Type type) =>
            Instance.Resolve(type);

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            if (parameters.Length > 0)
                return Instance.Resolve(type, ResolveParameters(type, parameters));

            return Resolve(type);
        }

        public object Resolve(Type type, string name) =>
            Instance.Resolve(type, name);

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            if (parameters.Length > 0)
                return Instance.Resolve(type, name, ResolveParameters(type, parameters));

            return Resolve(type, name);
        }

        protected virtual IScopedProvider CreateScopeInternal()
        {
            _tinyIocScoped = new TinyScopedProvider(Instance);
            return _tinyIocScoped;
        }

        private static NamedParameterOverloads ResolveParameters(Type type, (Type Type, object Instance)[] parameters)
        {
            var paramTypes = parameters.Select(p => p.Type).ToArray();
            var ctor = type.GetConstructor(paramTypes);
            var ctorParameters = ctor.GetParameters()
                .Select(i => (i.Name, i.ParameterType))
                .ToDictionary(i => i.Name, i => parameters.First(p => p.Type == i.ParameterType).Instance);

            return new NamedParameterOverloads(ctorParameters);
        }

        private class TinyScopedProvider : IScopedProvider
        {
            public TinyScopedProvider(TinyIoCContainer resolver)
            {
                Resolver = resolver;
            }

            public bool IsAttached { get; set; }

            public TinyIoCContainer Resolver { get; private set; }

            public IScopedProvider CurrentScope => this;

            public IScopedProvider CreateScope() => this;

            public void Dispose()
            {
                Resolver.Dispose();
                Resolver = null;
            }

            public object Resolve(Type type) =>
                Resolve(type, Array.Empty<(Type, object)>());

            public object Resolve(Type type, string name) =>
                Resolve(type, name, Array.Empty<(Type, object)>());

            public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    if (parameters.Length > 0)
                        return Resolver.Resolve(type, ResolveParameters(type, parameters));

                    return Resolver.Resolve(type);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(type.FullName, ex);
                }
            }

            public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
            {
                try
                {
                    if (parameters.Length > 0)
                        return Resolver.Resolve(type, name, ResolveParameters(type, parameters));

                    return Resolver.Resolve(type, name);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(name, ex);
                }
            }
        }
    }
#endif
}

namespace TinyNavigation.Behaviors
{
    /// <summary>
    /// Applies behaviors to the Xamarin.Forms pages when they are created during navigation.
    /// </summary>
    public class PageBehaviorFactory : IPageBehaviorFactory
    {
        /// <summary>
        /// Applies behaviors to a CarouselPage.
        /// </summary>
        /// <param name="page">The CarouselPage to apply the behaviors</param>
        /// <remarks>The CarouselPageActiveAwareBehavior is applied by default</remarks>
        protected virtual void ApplyCarouselPageBehaviors(CarouselPage page)
        {
            page.Behaviors.Add(new CarouselPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a ContentPage.
        /// </summary>
        /// <param name="page">The ContentPage to apply the behaviors</param>
        protected virtual void ApplyContentPageBehaviors(ContentPage page)
        {

        }

        /// <summary>
        /// Applies behaviors to a MasterDetailPage.
        /// </summary>
        /// <param name="page">The MasterDetailPage to apply the behaviors</param>
        protected virtual void ApplyMasterDetailPageBehaviors(MasterDetailPage page)
        {

        }

        /// <summary>
        /// Applies behaviors to a NavigationPage.
        /// </summary>
        /// <param name="page">The NavigationPage to apply the behaviors</param>
        /// <remarks>The NavigationPageSystemGoBackBehavior and NavigationPageActiveAwareBehavior are applied by default</remarks>
        protected virtual void ApplyNavigationPageBehaviors(NavigationPage page)
        {
            page.Behaviors.Add(new NavigationPageSystemGoBackBehavior());
            page.Behaviors.Add(new NavigationPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a page based on the page type.
        /// </summary>
        /// <param name="page">The page to apply the behaviors</param>
        /// <remarks>
        /// There is no need to call base.ApplyPageBehaviors when overriding.
        /// All Prism behaviors have already been applied.
        /// </remarks>
        protected virtual void ApplyPageBehaviors(Page page)
        {
        }

        void IPageBehaviorFactory.ApplyPageBehaviors(Page page)
        {
            switch (page)
            {
                case ContentPage contentPage:
                    ApplyContentPageBehaviors(contentPage);
                    break;
                case NavigationPage navPage:
                    ApplyNavigationPageBehaviors(navPage);
                    break;
                case MasterDetailPage masterDetailPage:
                    ApplyMasterDetailPageBehaviors(masterDetailPage);
                    break;
                case TabbedPage tabbedPage:
                    ApplyTabbedPageBehaviors(tabbedPage);
                    break;
                case CarouselPage carouselPage:
                    ApplyCarouselPageBehaviors(carouselPage);
                    break;
            }

            page.Behaviors.Add(new PageLifeCycleAwareBehavior());
            page.Behaviors.Add(new PageScopeBehavior());
            ApplyPageBehaviors(page);
        }

        /// <summary>
        /// Applies behaviors to a TabbedPage.
        /// </summary>
        /// <param name="page">The TabbedPage to apply the behaviors</param>
        /// <remarks>The TabbedPageActiveAwareBehavior is added by default</remarks>
        protected virtual void ApplyTabbedPageBehaviors(TabbedPage page)
        {
            page.Behaviors.Add(new TabbedPageActiveAwareBehavior());
        }
    }

    /// <summary>
    /// Base class that extends on Xamarin Forms Behaviors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BehaviorBase<T> : Behavior<T> where T : BindableObject
    {
        /// <summary>
        /// The Object associated with the Behavior
        /// </summary>
        public T AssociatedObject { get; private set; }

        /// <inheritDoc />
        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;

            if (bindable.BindingContext != null)
            {
                BindingContext = bindable.BindingContext;
            }

            bindable.BindingContextChanged += OnBindingContextChanged;
        }

        /// <inheritDoc />
        protected override void OnDetachingFrom(T bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedObject = null;
        }

        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        /// <inheritDoc />
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }

    public class MultiPageActiveAwareBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
    {
        protected T _lastSelectedPage;

        /// <inheritDoc/>
        protected override void OnAttachedTo(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged += CurrentPageChangedHandler;
            bindable.Appearing += RootPageAppearingHandler;
            bindable.Disappearing += RootPageDisappearingHandler;
            base.OnAttachedTo(bindable);
        }

        /// <inheritDoc/>
        protected override void OnDetachingFrom(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged -= CurrentPageChangedHandler;
            bindable.Appearing -= RootPageAppearingHandler;
            bindable.Disappearing -= RootPageDisappearingHandler;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Event Handler for the MultiPage CurrentPageChanged event
        /// </summary>
        /// <param name="sender">The MultiPage</param>
        /// <param name="e">Event Args</param>
        protected void CurrentPageChangedHandler(object sender, EventArgs e)
        {
            if (_lastSelectedPage == null)
                _lastSelectedPage = AssociatedObject.CurrentPage;

            //inactive 
            SetIsActive(_lastSelectedPage, false);

            _lastSelectedPage = AssociatedObject.CurrentPage;

            //active
            SetIsActive(_lastSelectedPage, true);
        }

        /// <summary>
        /// Event Handler for the MultiPage Appearing event
        /// </summary>
        /// <param name="sender">The MultiPage Appearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageAppearingHandler(object sender, EventArgs e)
        {
            if (_lastSelectedPage == null)
                _lastSelectedPage = AssociatedObject.CurrentPage;

            SetIsActive(_lastSelectedPage, true);
        }

        /// <summary>
        /// Event Handler for the MultiPage Disappearing event
        /// </summary>
        /// <param name="sender">The MultiPage Disappearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageDisappearingHandler(object sender, EventArgs e)
        {
            SetIsActive(_lastSelectedPage, false);
        }

        void SetIsActive(object view, bool isActive)
        {
            var pageToSetIsActive = view is NavigationPage ? ((NavigationPage)view).CurrentPage : view;

            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(pageToSetIsActive, activeAware => activeAware.IsActive = isActive);
        }
    }

    /// <summary>
    /// Controls the Page container Scope
    /// </summary>
    public sealed class PageScopeBehavior : BehaviorBase<Page>
    {
        protected override void OnAttachedTo(Page page)
        {
            base.OnAttachedTo(page);
            // Ensure the scope gets created and NavigationService is created
            Navigation.GetNavigationService(page);
        }

        protected override void OnDetachingFrom(Page page)
        {
            base.OnDetachingFrom(page);
            // This forces the Attached Property to get cleaned up.
            page.SetValue(Navigation.NavigationScopeProperty, null);
        }
    }

    public class PageLifeCycleAwareBehavior : BehaviorBase<Page>
    {
        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Appearing += OnAppearing;
            bindable.Disappearing += OnDisappearing;
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Appearing -= OnAppearing;
            bindable.Disappearing -= OnDisappearing;
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            PageUtilities.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnAppearing());
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            PageUtilities.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnDisappearing());
        }
    }

    public class CarouselPageActiveAwareBehavior : MultiPageActiveAwareBehavior<ContentPage>
    {
    }

    public class NavigationPageActiveAwareBehavior : BehaviorBase<NavigationPage>
    {
        private const string CURRENT_PAGE = "CurrentPage";
        protected override void OnAttachedTo(NavigationPage bindable)
        {
            bindable.PropertyChanging += NavigationPage_PropertyChanging;
            bindable.PropertyChanged += NavigationPage_PropertyChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(NavigationPage bindable)
        {
            bindable.PropertyChanging -= NavigationPage_PropertyChanging;
            bindable.PropertyChanged -= NavigationPage_PropertyChanged;
            base.OnDetachingFrom(bindable);
        }

        private void NavigationPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CURRENT_PAGE == e.PropertyName)
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(AssociatedObject.CurrentPage, (obj) => obj.IsActive = true);
        }

        private void NavigationPage_PropertyChanging(object sender, Xamarin.Forms.PropertyChangingEventArgs e)
        {
            if (CURRENT_PAGE == e.PropertyName)
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(AssociatedObject.CurrentPage, (obj) => obj.IsActive = false);
        }
    }

    public class NavigationPageSystemGoBackBehavior : BehaviorBase<NavigationPage>
    {
        protected override void OnAttachedTo(NavigationPage bindable)
        {
            bindable.Popped += NavigationPage_Popped;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(NavigationPage bindable)
        {
            bindable.Popped -= NavigationPage_Popped;
            base.OnDetachingFrom(bindable);
        }

        private void NavigationPage_Popped(object sender, NavigationEventArgs e)
        {
            if (NavigationService.NavigationSource == PageNavigationSource.Device)
                PageUtilities.HandleSystemGoBack(e.Page, AssociatedObject.CurrentPage);
        }
    }

    public class TabbedPageActiveAwareBehavior : MultiPageActiveAwareBehavior<Page>
    {
    }
}

namespace TinyNavigation.MVVM
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the <see cref="Prism.Mvvm.ViewModelLocationProvider"/>.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        public static readonly BindableProperty AutowireViewModelProperty =
            BindableProperty.CreateAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), null, propertyChanged: OnAutowireViewModelChanged);

        /// <summary>
        /// Gets the AutowireViewModel property value.
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        public static bool? GetAutowireViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }

        /// <summary>
        /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="value"></param>
        public static void SetAutowireViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool? bNewValue = (bool?)newValue;
            if (bNewValue.HasValue && bNewValue.Value)
                ViewModelLocationProvider.AutoWireViewModelChanged(bindable, Bind);
        }

        /// <summary>
        /// Sets the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> of a View
        /// </summary>
        /// <param name="view">The View to set the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> on</param>
        /// <param name="viewModel">The object to use as the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> for the View</param>
        private static void Bind(object view, object viewModel)
        {
            if (view is BindableObject element)
                element.BindingContext = viewModel;
        }
    }

    /// <summary>
    /// The ViewModelLocationProvider class locates the view model for the view that has the AutoWireViewModelChanged attached property set to true.
    /// The view model will be located and injected into the view's DataContext. To locate the view model, two strategies are used: First the ViewModelLocationProvider
    /// will look to see if there is a view model factory registered for that view, if not it will try to infer the view model using a convention based approach.
    /// This class also provides methods for registering the view model factories,
    /// and also to override the default view model factory and the default view type to view model type resolver.
    /// </summary>
    // Documentation on using the MVVM pattern is at http://go.microsoft.com/fwlink/?LinkID=288814&clcid=0x409
    public static class ViewModelLocationProvider
    {
        /// <summary>
        /// A dictionary that contains all the registered factories for the views.
        /// </summary>
        static Dictionary<string, Func<object>> _factories = new Dictionary<string, Func<object>>();

        /// <summary>
        /// A dictionary that contains all the registered ViewModel types for the views.
        /// </summary>
        static Dictionary<string, Type> _typeFactories = new Dictionary<string, Type>();

        /// <summary>
        /// The default view model factory which provides the ViewModel type as a parameter.
        /// </summary>
        static Func<Type, object> _defaultViewModelFactory = type => Activator.CreateInstance(type);

        /// <summary>
        /// ViewModelfactory that provides the View instance and ViewModel type as parameters.
        /// </summary>
        static Func<object, Type, object> _defaultViewModelFactoryWithViewParameter;

        /// <summary>
        /// Default view type to view model type resolver, assumes the view model is in same assembly as the view type, but in the "ViewModels" namespace.
        /// </summary>
        static Func<Type, Type> _defaultViewTypeToViewModelTypeResolver =
            viewType =>
            {
                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
                return Type.GetType(viewModelName);
            };

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory which provides the ViewModel type as a parameter.</param>
        public static void SetDefaultViewModelFactory(Func<Type, object> viewModelFactory)
        {
            _defaultViewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory that provides the View instance and ViewModel type as parameters.</param>
        public static void SetDefaultViewModelFactory(Func<object, Type, object> viewModelFactory)
        {
            _defaultViewModelFactoryWithViewParameter = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view type to view model type resolver.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The view type to view model type resolver.</param>
        public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Automatically looks up the viewmodel that corresponds to the current view, using two strategies:
        /// It first looks to see if there is a mapping registered for that view, if not it will fallback to the convention based approach.
        /// </summary>
        /// <param name="view">The dependency object, typically a view.</param>
        /// <param name="setDataContextCallback">The call back to use to create the binding between the View and ViewModel</param>
        public static void AutoWireViewModelChanged(object view, Action<object, object> setDataContextCallback)
        {
            // Try mappings first
            object viewModel = GetViewModelForView(view);

            // try to use ViewModel type
            if (viewModel == null)
            {
                //check type mappings
                var viewModelType = GetViewModelTypeForView(view.GetType());

                // fallback to convention based
                if (viewModelType == null)
                    viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

                if (viewModelType == null)
                    return;

                viewModel = _defaultViewModelFactoryWithViewParameter != null ? _defaultViewModelFactoryWithViewParameter(view, viewModelType) : _defaultViewModelFactory(viewModelType);
            }


            setDataContextCallback(view, viewModel);
        }

        /// <summary>
        /// Gets the view model for the specified view.
        /// </summary>
        /// <param name="view">The view that the view model wants.</param>
        /// <returns>The ViewModel that corresponds to the view passed as a parameter.</returns>
        private static object GetViewModelForView(object view)
        {
            var viewKey = view.GetType().ToString();

            // Mapping of view models base on view type (or instance) goes here
            if (_factories.ContainsKey(viewKey))
                return _factories[viewKey]();

            return null;
        }

        /// <summary>
        /// Gets the ViewModel type for the specified view.
        /// </summary>
        /// <param name="view">The View that the ViewModel wants.</param>
        /// <returns>The ViewModel type that corresponds to the View.</returns>
        private static Type GetViewModelTypeForView(Type view)
        {
            var viewKey = view.ToString();

            if (_typeFactories.ContainsKey(viewKey))
                return _typeFactories[viewKey];

            return null;
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type.
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register<T>(Func<object> factory)
        {
            Register(typeof(T).ToString(), factory);
        }

        /// <summary>
        /// Registers the ViewModel factory for the specified view type name.
        /// </summary>
        /// <param name="viewTypeName">The name of the view type.</param>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register(string viewTypeName, Func<object> factory)
        {
            _factories[viewTypeName] = factory;
        }

        /// <summary>
        /// Registers a ViewModel type for the specified view type.
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <typeparam name="VM">The ViewModel</typeparam>
        public static void Register<T, VM>()
        {
            var viewType = typeof(T);
            var viewModelType = typeof(VM);

            Register(viewType.ToString(), viewModelType);
        }

        /// <summary>
        /// Registers a ViewModel type for the specified view.
        /// </summary>
        /// <param name="viewTypeName">The View type name</param>
        /// <param name="viewModelType">The ViewModel type</param>
        public static void Register(string viewTypeName, Type viewModelType)
        {
            _typeFactories[viewTypeName] = viewModelType;
        }
    }
}

namespace TinyNavigation
{

    /// <summary>
    /// The Base implementation for a PrismApplication
    /// </summary>
    public abstract class TinyApplicationBase : Application
    {
        /// <summary>
        /// Gets the Current PrismApplication
        /// </summary>
        public new static TinyApplicationBase Current => (TinyApplicationBase)Application.Current;

        /// <summary>
        /// The registration name to create a new transient instance of the <see cref="INavigationService"/>
        /// </summary>
        public const string NavigationServiceName = "PageNavigationService";

        private IContainerExtension _containerExtension;
        private Page _previousPage = null;

        /// <summary>
        /// The dependency injection container used to resolve objects
        /// </summary>
        public IContainerProvider Container => _containerExtension;

        /// <summary>
        /// Gets the <see cref="INavigationService"/> for the application.
        /// </summary>
        protected INavigationService NavigationService { get; set; }

        /// <summary>
        /// Get the Platform Initializer
        /// </summary>
        protected IPlatformInitializer PlatformInitializer { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> using the default constructor
        /// </summary>
        protected TinyApplicationBase() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplicationBase" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        protected TinyApplicationBase(IPlatformInitializer platformInitializer)
        {
            base.ModalPopping += PrismApplicationBase_ModalPopping;
            base.ModalPopped += PrismApplicationBase_ModalPopped;

            PlatformInitializer = platformInitializer;
            InitializeInternal();
        }

        /// <summary>
        /// Run the initialization process.
        /// </summary>
        private void InitializeInternal()
        {
            ConfigureViewModelLocator();
            Initialize();
            OnInitialized();
        }

        /// <summary>
        /// Configures the <see cref="ViewModelLocator"/> used by Prism.
        /// </summary>
        protected virtual void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => Container.Resolve(type));
        }

        private INavigationService CreateNavigationService(object view)
        {
            if (view is Page page)
            {
                return Navigation.GetNavigationService(page);
            }
            else if (view is VisualElement visualElement && visualElement.TryGetParentPage(out var parent))
            {
                return Navigation.GetNavigationService(parent);
            }

            return Container.Resolve<INavigationService>();
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        protected virtual void Initialize()
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            _containerExtension = ContainerLocator.Current;

            RegisterRequiredTypes(_containerExtension);
            PlatformInitializer?.RegisterTypes(_containerExtension);
            RegisterTypes(_containerExtension);

            _containerExtension.FinalizeExtension();
            _containerExtension.CreateScope();

            NavigationService = _containerExtension.Resolve<INavigationService>();
        }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/>
        /// </summary>
        /// <returns></returns>
        protected virtual IContainerExtension CreateContainerExtension()
        {
#if USE_UNITY_CONTAINER && !USE_DRYIOC_CONTAINER
            return new UnityContainerExtension(new UnityContainer());
#elif USE_DRYIOC_CONTAINER
            return new DryIocContainerExtension(new DryIoc.Container(CreateContainerRules()));
#else
            return new TinyIocContainerExtension(new TinyIoCContainer());
#endif
        }

#if USE_DRYIOC_CONTAINER
        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => DryIocContainerExtension.DefaultRules;
#endif

        /// <summary>
        /// Registers all types that are required by Prism to function with the container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IApplicationProvider, ApplicationProvider>();
            containerRegistry.RegisterSingleton<IPageBehaviorFactory, PageBehaviorFactory>();
            containerRegistry.RegisterSingleton<INavigationService, NavigationService>();
            containerRegistry.Register<INavigationService, NavigationService>(NavigationServiceName);
        }

        /// <summary>
        /// Used to register types with the container that will be used by your application.
        /// </summary>
        protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

        /// <summary>
        /// Called when the PrismApplication has completed it's initialization process.
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Application developers override this method to perform actions when the application
        /// resumes from a sleeping state
        /// </summary>
        /// <remarks>
        /// Be sure to call base.OnResume() or you will lose support for IApplicationLifecycleAware
        /// </remarks>
        protected override void OnResume()
        {
            if (MainPage != null)
            {
                var page = PageUtilities.GetCurrentPage(MainPage);
                PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnResume());
            }
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application
        /// enters the sleeping state
        /// </summary>
        /// <remarks>
        /// Be sure to call base.OnSleep() or you will lose support for IApplicationLifecycleAware
        /// </remarks>
        protected override void OnSleep()
        {
            if (MainPage != null)
            {
                var page = PageUtilities.GetCurrentPage(MainPage);
                PageUtilities.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(page, x => x.OnSleep());
            }
        }

        private void PrismApplicationBase_ModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (TinyNavigation.NavigationService.NavigationSource == PageNavigationSource.Device)
            {
                _previousPage = PageUtilities.GetOnNavigatedToTarget(e.Modal, MainPage, true);
            }
        }

        private void PrismApplicationBase_ModalPopped(object sender, ModalPoppedEventArgs e)
        {
            if (TinyNavigation.NavigationService.NavigationSource == PageNavigationSource.Device)
            {
                PageUtilities.HandleSystemGoBack(e.Modal, _previousPage);
                _previousPage = null;
            }
        }
    }

    public struct PageNavigationInfo
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }

    public class NavigationResult : INavigationResult
    {
        public bool Success { get; set; }

        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Provides Application components.
    /// </summary>
    public class ApplicationProvider : IApplicationProvider
    {
        /// <inheritdoc/>
        public Page MainPage
        {
            get => Application.Current.MainPage;
            set => Application.Current.MainPage = value;
        }
    }

    /// <summary>
    /// This is a generic parameters base class used for Dialog Parameters and Navigation Parameters.
    /// </summary>
    public abstract class ParametersBase : IParameters, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> _entries = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ParametersBase()
        {
        }

        /// <summary>
        /// Constructs a list of parameters.
        /// </summary>
        /// <param name="query">Query string to be parsed.</param>
        protected ParametersBase(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
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
                        Add(Uri.UnescapeDataString(key), Uri.UnescapeDataString(value));
                }
            }
        }

        /// <summary>
        /// Searches Parameter collection and returns value if Collection contains key.
        /// Otherswise returns null.
        /// </summary>
        /// <param name="key">The key for the value to be returned.</param>
        /// <returns>The value of the parameter referenced by the key; otherwise <c>null</c>.</returns>
        public object this[string key]
        {
            get
            {
                foreach (var entry in _entries)
                {
                    if (string.Compare(entry.Key, key, StringComparison.Ordinal) == 0)
                    {
                        return entry.Value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// The count, or number, of parameters in collection.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Returns an IEnumerable of the Keys in the collection.
        /// </summary>
        public IEnumerable<string> Keys =>
            _entries.Select(x => x.Key);

        /// <summary>
        /// Adds the key and value to the parameters collection.
        /// </summary>
        /// <param name="key">The key to reference this value in the parameters collection.</param>
        /// <param name="value">The value of the parameter to store.</param>
        public void Add(string key, object value) =>
            _entries.Add(new KeyValuePair<string, object>(key, value));

        /// <summary>
        /// Checks collection for presence of key.
        /// </summary>
        /// <param name="key">The key to check in the collection.</param>
        /// <returns><c>true</c> if key exists; else returns <c>false</c>.</returns>
        public bool ContainsKey(string key) =>
            _entries.ContainsKey(key);

        /// <summary>
        /// Gets an enumerator for the KeyValuePairs in parameter collection.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
            _entries.GetEnumerator();

        /// <summary>
        /// Returns the value of the member referenced by key.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="key">The key for the value to be returned.</param>
        /// <returns>Returns a matching parameter of <typeparamref name="T"/> if one exists in the Collection.</returns>
        public T GetValue<T>(string key) =>
            _entries.GetValue<T>(key);

        /// <summary>
        /// Returns an IEnumerable of all parameters.
        /// </summary>
        /// <typeparam name="T">The type for the values to be returned.</typeparam>
        /// <param name="key">The key for the values to be returned.</param>
        ///<returns>Returns a IEnumerable of all the instances of type <typeparamref name="T"/>.</returns>
        public IEnumerable<T> GetValues<T>(string key) =>
            _entries.GetValues<T>(key);

        /// <summary>
        /// Checks to see if the parameter collection contains the value.
        /// </summary>
        /// <typeparam name="T">The type for the values to be returned.</typeparam>
        /// <param name="key">The key for the value to be returned.</param>
        /// <param name="value">Value of the returned parameter if it exists.</param>
        public bool TryGetValue<T>(string key, out T value) =>
            _entries.TryGetValue(key, out value);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Converts parameter collection to a parameter string.
        /// </summary>
        /// <returns>A string representation of the parameters.</returns>
        public override string ToString()
        {
            var queryBuilder = new StringBuilder();

            if (_entries.Count > 0)
            {
                queryBuilder.Append('?');
                var first = true;

                foreach (var kvp in _entries)
                {
                    if (!first)
                    {
                        queryBuilder.Append('&');
                    }
                    else
                    {
                        first = false;
                    }

                    queryBuilder.Append(Uri.EscapeDataString(kvp.Key));
                    queryBuilder.Append('=');
                    queryBuilder.Append(Uri.EscapeDataString(kvp.Value != null ? kvp.Value.ToString() : ""));
                }
            }

            return queryBuilder.ToString();
        }

        /// <summary>
        /// Adds a collection of parameters to the local parameter list.
        /// </summary>
        /// <param name="parameters">An IEnumerable of KeyValuePairs to add to the current parameter list.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void FromParameters(IEnumerable<KeyValuePair<string, object>> parameters) =>
            _entries.AddRange(parameters);
    }

    /// <summary>
    /// Represents Navigation parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during Navigation. 
    /// </remarks>
    public class NavigationParameters : ParametersBase, INavigationParameters, INavigationParametersInternal
    {
        private readonly Dictionary<string, object> _internalParameters = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
        /// </summary>
        public NavigationParameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationParameters"/> class with a query string.
        /// </summary>
        /// <param name="query">The query string.</param>
        public NavigationParameters(string query)
            : base(query)
        {
        }

#region INavigationParametersInternal
        void INavigationParametersInternal.Add(string key, object value)
        {
            _internalParameters.Add(key, value);
        }

        bool INavigationParametersInternal.ContainsKey(string key)
        {
            return _internalParameters.ContainsKey(key);
        }

        T INavigationParametersInternal.GetValue<T>(string key)
        {
            return _internalParameters.GetValue<T>(key);
        }
#endregion
    }

    public class DialogParameters : ParametersBase, IDialogParameters
    {
        public DialogParameters()
        {
        }

        public DialogParameters(string query)
            : base(query)
        {
        }
    }

    public class NavigationService : INavigationService
    {
        internal const string RemovePageRelativePath = "../";
        internal const string RemovePageInstruction = "__RemovePage/";
        internal const string RemovePageSegment = "__RemovePage";

        protected internal static PageNavigationSource NavigationSource { get; protected set; } = PageNavigationSource.Device;

        private readonly IContainerProvider _container;
        protected readonly IApplicationProvider _applicationProvider;
        protected readonly IPageBehaviorFactory _pageBehaviorFactory;

        protected Page _page;

        Page IPageAware.Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="NavigationService"/>.
        /// </summary>
        /// <param name="container">The <see cref="IContainerProvider"/> that will be used to resolve pages for navigation.</param>
        /// <param name="applicationProvider">The <see cref="IApplicationProvider"/> that will let us ensure the Application.MainPage is set.</param>
        /// <param name="pageBehaviorFactory">The <see cref="IPageBehaviorFactory"/> that will apply base and custom behaviors to pages created in the <see cref="PageNavigationService"/>.</param>
        public NavigationService(IContainerProvider container, IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory)
        {
            _container = container;
            _applicationProvider = applicationProvider;
            _pageBehaviorFactory = pageBehaviorFactory;
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public virtual Task<INavigationResult> GoBackAsync() =>
            GoBackAsync(null);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        public virtual Task<INavigationResult> GoBackAsync(INavigationParameters parameters) =>
            GoBackInternal(parameters, null, true);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public virtual Task<INavigationResult> GoBackAsync(INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
            GoBackInternal(parameters, useModalNavigation, animated);

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
        protected async virtual Task<INavigationResult> GoBackInternal(INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();
            Page page = null;
            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                page = GetCurrentPage();
                var segmentParameters = UriParsingHelper.GetSegmentParameters(null, parameters);
                segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

                var canNavigate = await PageUtilities.CanNavigateAsync(page, segmentParameters);
                if (!canNavigate)
                {
                    result.Exception = new InvalidOperationException($"IConfirmNavigation returned false | {page.GetType().FullName}");
                    return result;
                }

                bool useModalForDoPop = UseModalGoBack(page, useModalNavigation);
                Page previousPage = PageUtilities.GetOnNavigatedToTarget(page, _applicationProvider.MainPage, useModalForDoPop);

                var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated);
                if (poppedPage != null)
                {
                    PageUtilities.OnNavigatedFrom(page, segmentParameters);
                    PageUtilities.OnNavigatedTo(previousPage, segmentParameters);
                    PageUtilities.DestroyPage(poppedPage);

                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }

            result.Exception = GetGoBackException(page, _applicationProvider.MainPage);
            return result;
        }

        private static Exception GetGoBackException(Page currentPage, Page mainPage)
        {
            if (IsMainPage(currentPage, mainPage))
            {
                return new InvalidOperationException($"Cannot pop application MainPage | {currentPage.GetType().FullName}");
            }
            else if ((currentPage is NavigationPage navPage && IsOnNavigationPageRoot(navPage)) ||
                (currentPage.Parent is NavigationPage navParent && IsOnNavigationPageRoot(navParent)))
            {
                return new InvalidOperationException($"Cannot go back from root | {currentPage.GetType().FullName}");
            }

            return new InvalidOperationException($"Unknown Exception | {currentPage.GetType().FullName}");
        }

        private static bool IsOnNavigationPageRoot(NavigationPage navigationPage) =>
            navigationPage.CurrentPage == navigationPage.RootPage;

        private static bool IsMainPage(Page currentPage, Page mainPage)
        {
            if (currentPage == mainPage)
            {
                return true;
            }
            else if (mainPage is MasterDetailPage mdp && mdp.Detail == currentPage)
            {
                return true;
            }
            else if (currentPage.Parent is TabbedPage tabbed && mainPage == tabbed)
            {
                return true;
            }
            else if (currentPage.Parent is CarouselPage carousel && mainPage == carousel)
            {
                return true;
            }
            else if (currentPage.Parent is NavigationPage navPage && navPage.CurrentPage == navPage.RootPage)
            {
                return IsMainPage(navPage, mainPage);
            }

            return false;
        }

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        public virtual Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
        {
            return GoBackToRootInternal(parameters);
        }

        /// <summary>
        /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
        /// </summary>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Only works when called from a View within a NavigationPage</remarks>
        protected async virtual Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters)
        {
            var result = new NavigationResult();
            Page page = null;
            try
            {
                if (parameters == null)
                    parameters = new NavigationParameters();

                parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

                page = GetCurrentPage();
                var canNavigate = await PageUtilities.CanNavigateAsync(page, parameters);
                if (!canNavigate)
                {
                    result.Exception = new InvalidOperationException($"IConfirmNavigation returned false | {page.GetType().FullName}");
                    return result;
                }

                List<Page> pagesToDestroy = page.Navigation.NavigationStack.ToList(); // get all pages to destroy
                pagesToDestroy.Reverse(); // destroy them in reverse order
                var root = pagesToDestroy.Last();
                pagesToDestroy.Remove(root); //don't destroy the root page

                await page.Navigation.PopToRootAsync();

                foreach (var destroyPage in pagesToDestroy)
                {
                    PageUtilities.OnNavigatedFrom(destroyPage, parameters);
                    PageUtilities.DestroyPage(destroyPage);
                }

                PageUtilities.OnNavigatedTo(root, parameters);

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        public virtual Task<INavigationResult> NavigateAsync(string name) =>
            NavigateAsync(name, null);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        public virtual Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters) =>
            NavigateInternal(name, parameters, null, true);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PushModalAsync, if <c>false</c> uses PushAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        public Task<INavigationResult> NavigateAsync(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
            NavigateInternal(name, parameters, useModalNavigation, animated);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the target to navigate to.</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        protected virtual Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (name.StartsWith(RemovePageRelativePath))
                name = name.Replace(RemovePageRelativePath, RemovePageInstruction);

            return NavigateInternal(UriParsingHelper.Parse(name), parameters, useModalNavigation, animated);
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource));
        /// </example>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri) =>
            NavigateAsync(uri, null);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters) =>
            NavigateInternal(uri, parameters, null, true);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// NavigateAsync(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        public virtual Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
            NavigateInternal(uri, parameters, useModalNavigation, animated);

        /// <summary>
        /// Initiates navigation to the target specified by the <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri to navigate to</param>
        /// <param name="parameters">The navigation parameters</param>
        /// <param name="useModalNavigation">If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync</param>
        /// <param name="animated">If <c>true</c> the transition is animated, if <c>false</c> there is no animation on transition.</param>
        /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
        /// <example>
        /// Navigate(new Uri("MainPage?id=3&amp;name=brian", UriKind.RelativeSource), parameters);
        /// </example>
        protected async virtual Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var result = new NavigationResult();
            try
            {
                NavigationSource = PageNavigationSource.NavigationService;

                var navigationSegments = UriParsingHelper.GetUriSegments(uri);

                if (uri.IsAbsoluteUri)
                {
                    await ProcessNavigationForAbsoulteUri(navigationSegments, parameters, useModalNavigation, animated);
                    result.Success = true;
                    return result;
                }
                else
                {
                    await ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, useModalNavigation, animated);
                    result.Success = true;
                    return result;
                }

            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }
            finally
            {
                NavigationSource = PageNavigationSource.Device;
            }
        }

        /// <summary>
        /// Processes the Navigation for the Queued navigation segments
        /// </summary>
        /// <param name="currentPage">The Current <see cref="Page"/> that we are navigating from.</param>
        /// <param name="segments">The Navigation <see cref="Uri"/> segmenets.</param>
        /// <param name="parameters">The <see cref="INavigationParameters"/>.</param>
        /// <param name="useModalNavigation"><see cref="Nullable{Boolean}"/> flag if we should force Modal Navigation.</param>
        /// <param name="animated">If <c>true</c>, the navigation will be animated.</param>
        /// <returns></returns>
        protected virtual async Task ProcessNavigation(Page currentPage, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (segments.Count == 0)
                return;

            await Task.Yield();
            var nextSegment = segments.Dequeue();

            var pageParameters = UriParsingHelper.GetSegmentParameters(nextSegment);
            if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
                useModalNavigation = pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation);

            if (nextSegment == RemovePageSegment)
            {
                await ProcessNavigationForRemovePageSegments(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
                return;
            }

            if (currentPage == null)
            {
                await ProcessNavigationForRootPage(nextSegment, segments, parameters, useModalNavigation, animated);
                return;
            }

            if (currentPage is ContentPage)
            {
                await ProcessNavigationForContentPage(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is NavigationPage)
            {
                await ProcessNavigationForNavigationPage((NavigationPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is TabbedPage)
            {
                await ProcessNavigationForTabbedPage((TabbedPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is CarouselPage)
            {
                await ProcessNavigationForCarouselPage((CarouselPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
            else if (currentPage is MasterDetailPage)
            {
                await ProcessNavigationForMasterDetailPage((MasterDetailPage)currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
        }

        protected virtual Task ProcessNavigationForRemovePageSegments(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (!PageUtilities.HasDirectNavigationPageParent(currentPage))
                throw new InvalidOperationException($"Relative navigation requires NavigationPage | {currentPage.GetType().FullName}");

            if (CanRemoveAndPush(segments))
                return RemoveAndPush(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            else
                return RemoveAndGoBack(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }

        private bool CanRemoveAndPush(Queue<string> segments)
        {
            if (segments.All(x => x == RemovePageSegment))
                return false;
            else
                return true;
        }

        private Task RemoveAndGoBack(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            List<Page> pagesToRemove = new List<Page>();

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Count != 0)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);

            return GoBackAsync(parameters);
        }

        private async Task RemoveAndPush(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var pagesToRemove = new List<Page>
            {
                currentPage
            };

            var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 0)
                currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;

            while (segments.Peek() == RemovePageSegment)
            {
                currentPageIndex -= 1;
                pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
                nextSegment = segments.Dequeue();
            }

            await ProcessNavigation(currentPage, segments, parameters, useModalNavigation, animated);

            RemovePagesFromNavigationPage(currentPage, pagesToRemove);
        }

        private static void RemovePagesFromNavigationPage(Page currentPage, List<Page> pagesToRemove)
        {
            var navigationPage = (NavigationPage)currentPage.Parent;
            foreach (var page in pagesToRemove)
            {
                navigationPage.Navigation.RemovePage(page);
                PageUtilities.DestroyPage(page);
            }
        }

        protected virtual Task ProcessNavigationForAbsoulteUri(Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated) =>
            ProcessNavigation(null, segments, parameters, useModalNavigation, animated);

        protected virtual async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            var currentPage = _applicationProvider.MainPage;
            var modalStack = currentPage?.Navigation.ModalStack.ToList();
            await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(null, nextPage, useModalNavigation, animated);
            });
            if (currentPage != null)
            {
                PageUtilities.DestroyWithModalStack(currentPage, modalStack);
            }
        }

        protected virtual async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            bool useReverse = UseReverseNavigation(currentPage, nextPageType) && !(useModalNavigation.HasValue && useModalNavigation.Value);
            if (!useReverse)
            {
                var nextPage = CreatePageFromSegment(nextSegment);

                await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

                await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, useModalNavigation, animated);
                });
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
            }
        }

        protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            if (currentPage.Navigation.NavigationStack.Count == 0)
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);
                return;
            }

            var clearNavigationStack = GetClearNavigationPageNavigationStack(currentPage);
            var isEmptyOfNavigationStack = currentPage.Navigation.NavigationStack.Count == 0;

            List<Page> destroyPages;
            if (clearNavigationStack && !isEmptyOfNavigationStack)
            {
                destroyPages = currentPage.Navigation.NavigationStack.ToList();
                destroyPages.Reverse();

                await currentPage.Navigation.PopToRootAsync(false);
            }
            else
            {
                destroyPages = new List<Page>();
            }

            var topPage = currentPage.Navigation.NavigationStack.LastOrDefault();
            var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));
            if (topPage?.GetType() == nextPageType)
            {
                if (clearNavigationStack)
                    destroyPages.Remove(destroyPages.Last());

                if (segments.Count > 0)
                    await UseReverseNavigation(topPage, segments.Dequeue(), segments, parameters, false, animated);

                await DoNavigateAction(topPage, nextSegment, topPage, parameters, onNavigationActionCompleted: (p) =>
                {
                    if (nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                    {
                        var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                        SelectPageTab(topPage, segmentParams);
                    }
                });
            }
            else
            {
                await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);

                if (clearNavigationStack && !isEmptyOfNavigationStack)
                    currentPage.Navigation.RemovePage(topPage);
            }

            foreach (var destroyPage in destroyPages)
            {
                PageUtilities.DestroyPage(destroyPage);
            }
        }

        protected virtual async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForCarouselPage(CarouselPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }

        protected virtual async Task ProcessNavigationForMasterDetailPage(MasterDetailPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            bool isPresented = GetMasterDetailPageIsPresented(currentPage);

            var detail = currentPage.Detail;
            if (detail == null)
            {
                var newDetail = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newDetail, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
                {
                    currentPage.IsPresented = isPresented;
                    currentPage.Detail = newDetail;
                });
                return;
            }

            if (useModalNavigation.HasValue && useModalNavigation.Value)
            {
                var nextPage = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
                {
                    currentPage.IsPresented = isPresented;
                    await DoPush(currentPage, nextPage, true, animated);
                });
                return;
            }

            var nextSegmentType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(nextSegment));

            //we must recreate the NavigationPage everytime or the transitions on iOS will not work properly, unless we meet the two scenarios below
            bool detailIsNavPage = false;
            bool reuseNavPage = false;
            if (detail is NavigationPage navPage)
            {
                detailIsNavPage = true;

                //we only care if we the next segment is also a NavigationPage.
                if (PageUtilities.IsSameOrSubclassOf<NavigationPage>(nextSegmentType))
                {
                    //first we check to see if we are being forced to reuse the NavPage by checking the interface
                    reuseNavPage = !GetClearNavigationPageNavigationStack(navPage);

                    if (!reuseNavPage)
                    {
                        //if we weren't forced to reuse the NavPage, then let's check the NavPage.CurrentPage against the next segment type as we don't want to recreate the entire nav stack
                        //just in case the user is trying to navigate to the same page which may be nested in a NavPage
                        var nextPageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(segments.Peek()));
                        var currentPageType = navPage.CurrentPage.GetType();
                        if (nextPageType == currentPageType)
                        {
                            reuseNavPage = true;
                        }
                    }
                }
            }

            if ((detailIsNavPage && reuseNavPage) || (!detailIsNavPage && detail.GetType() == nextSegmentType))
            {
                await ProcessNavigation(detail, segments, parameters, useModalNavigation, animated);
                await DoNavigateAction(null, nextSegment, detail, parameters, onNavigationActionCompleted: (p) =>
                {
                    if (detail is TabbedPage && nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                    {
                        var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment);
                        SelectPageTab(detail, segmentParams);
                    }

                    currentPage.IsPresented = isPresented;
                });
                return;
            }
            else
            {
                var newDetail = CreatePageFromSegment(nextSegment);
                await ProcessNavigation(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
                await DoNavigateAction(detail, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
                {
                    if (detailIsNavPage)
                        OnNavigatedFrom(((NavigationPage)detail).CurrentPage, p);

                    currentPage.IsPresented = isPresented;
                    currentPage.Detail = newDetail;
                    PageUtilities.DestroyPage(detail);
                });
                return;
            }
        }

        protected static bool GetMasterDetailPageIsPresented(MasterDetailPage page)
        {
            if (page is IMasterDetailPageOptions iMasterDetailPage)
                return iMasterDetailPage.IsPresentedAfterNavigation;

            if (page.BindingContext is IMasterDetailPageOptions iMasterDetailPageBindingContext)
                return iMasterDetailPageBindingContext.IsPresentedAfterNavigation;

            return false;
        }

        protected static bool GetClearNavigationPageNavigationStack(NavigationPage page)
        {
            if (page is INavigationPageOptions iNavigationPage)
                return iNavigationPage.ClearNavigationStackOnNavigation;

            if (page.BindingContext is INavigationPageOptions iNavigationPageBindingContext)
                return iNavigationPageBindingContext.ClearNavigationStackOnNavigation;

            return true;
        }

        protected static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, INavigationParameters parameters, Func<Task> navigationAction = null, Action<INavigationParameters> onNavigationActionCompleted = null)
        {
            var segmentParameters = UriParsingHelper.GetSegmentParameters(toSegment, parameters);
            segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

            var canNavigate = await PageUtilities.CanNavigateAsync(fromPage, segmentParameters);
            if (!canNavigate)
            {
                throw new InvalidOperationException($"IConfirmNavigation returned false | {toPage.GetType()}");
            }

            await OnInitializedAsync(toPage, segmentParameters);

            if (navigationAction != null)
                await navigationAction();

            OnNavigatedFrom(fromPage, segmentParameters);

            onNavigationActionCompleted?.Invoke(segmentParameters);

            OnNavigatedTo(toPage, segmentParameters);
        }

        private static async Task OnInitializedAsync(Page toPage, INavigationParameters parameters)
        {
            await PageUtilities.OnInitializedAsync(toPage, parameters);

            if (toPage is TabbedPage tabbedPage)
            {
                foreach (var child in tabbedPage.Children)
                {
                    if (child is NavigationPage navigationPage)
                    {
                        await PageUtilities.OnInitializedAsync(navigationPage.CurrentPage, parameters);
                    }
                    else
                    {
                        await PageUtilities.OnInitializedAsync(child, parameters);
                    }
                }
            }
            else if (toPage is CarouselPage carouselPage)
            {
                foreach (var child in carouselPage.Children)
                {
                    await PageUtilities.OnInitializedAsync(child, parameters);
                }
            }
        }

        private static void OnNavigatedTo(Page toPage, INavigationParameters parameters)
        {
            PageUtilities.OnNavigatedTo(toPage, parameters);

            if (toPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    PageUtilities.OnNavigatedTo(navigationPage.CurrentPage, parameters);
                }
                else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
                {
                    PageUtilities.OnNavigatedTo(tabbedPage.CurrentPage, parameters);
                }
            }
            else if (toPage is CarouselPage carouselPage)
            {
                PageUtilities.OnNavigatedTo(carouselPage.CurrentPage, parameters);
            }
        }

        private static void OnNavigatedFrom(Page fromPage, INavigationParameters parameters)
        {
            PageUtilities.OnNavigatedFrom(fromPage, parameters);

            if (fromPage is TabbedPage tabbedPage)
            {
                if (tabbedPage.CurrentPage is NavigationPage navigationPage)
                {
                    PageUtilities.OnNavigatedFrom(navigationPage.CurrentPage, parameters);
                }
                else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
                {
                    PageUtilities.OnNavigatedFrom(tabbedPage.CurrentPage, parameters);
                }
            }
            else if (fromPage is CarouselPage carouselPage)
            {
                PageUtilities.OnNavigatedFrom(carouselPage.CurrentPage, parameters);
            }
        }

        protected virtual Page CreatePage(string segmentName)
        {
            try
            {
                _container.CreateScope();
                var page = (Page)_container.Resolve<object>(segmentName);

                if (page is null)
                    throw new NullReferenceException($"The resolved type for {segmentName} was null. You may be attempting to navigate to a Non-Page type");

                return SetNavigationServiceForPage(page);
            }
            catch (Exception ex)
            {
                if (((IContainerRegistry)_container).IsRegistered<object>(segmentName))
                    throw new InvalidOperationException($"Error creating page | {_page.GetType().FullName}", ex);

                throw new InvalidOperationException($"No page is registered | {_page.GetType().FullName}", ex);
            }
        }

        protected virtual Page CreatePageFromSegment(string segment)
        {
            string segmentName = UriParsingHelper.GetSegmentName(segment);
            var page = CreatePage(segmentName);
            if (page == null)
            {
                var innerException = new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));
                throw new InvalidOperationException($"No page is registered {_page.GetType().FullName}", innerException);
            }

            PageUtilities.SetAutowireViewModel(page);
            _pageBehaviorFactory.ApplyPageBehaviors(page);
            ConfigurePages(page, segment);

            return page;
        }

        private Page SetNavigationServiceForPage(Page page)
        {
            // Someone explicitly set Autowire ViewModel
            if (page.GetValue(Navigation.NavigationServiceProperty) != null)
                return page;

            // This will wireup the Navigation Service in case you have something injected that
            // actually required the Nav Service
            var childNavService = _container.Resolve<INavigationService>();
            if (childNavService is IPageAware pa)
                pa.Page = page;

            page.SetValue(Navigation.NavigationServiceProperty, childNavService);
            return page;
        }

        void ConfigurePages(Page page, string segment)
        {
            if (page is TabbedPage)
            {
                ConfigureTabbedPage((TabbedPage)page, segment);
            }
            else if (page is CarouselPage)
            {
                ConfigureCarouselPage((CarouselPage)page, segment);
            }
        }

        private void ConfigureTabbedPage(TabbedPage tabbedPage, string segment)
        {
            foreach (var child in tabbedPage.Children)
            {
                PageUtilities.SetAutowireViewModel(child);
                _pageBehaviorFactory.ApplyPageBehaviors(child);
                if (child is NavigationPage navPage)
                {
                    PageUtilities.SetAutowireViewModel(navPage.CurrentPage);
                    _pageBehaviorFactory.ApplyPageBehaviors(navPage.CurrentPage);
                }
            }

            var parameters = UriParsingHelper.GetSegmentParameters(segment);

            var tabsToCreate = parameters.GetValues<string>(KnownNavigationParameters.CreateTab);
            if (tabsToCreate.Count() > 0)
            {
                foreach (var tabToCreate in tabsToCreate)
                {
                    //created tab can be a single view or a view nested in a NavigationPage with the syntax "NavigationPage|ViewToCreate"
                    var tabSegements = tabToCreate.Split('|');
                    if (tabSegements.Length > 1)
                    {
                        var navigationPage = CreatePageFromSegment(tabSegements[0]) as NavigationPage;
                        if (navigationPage != null)
                        {
                            var navigationPageChild = CreatePageFromSegment(tabSegements[1]);

                            navigationPage.PushAsync(navigationPageChild);

                            //when creating a NavigationPage w/ DI, a blank Page object is injected into the ctor. Let's remove it
                            if (navigationPage.Navigation.NavigationStack.Count > 1)
                                navigationPage.Navigation.RemovePage(navigationPage.Navigation.NavigationStack[0]);

                            //set the title because Xamarin doesn't do this for us.
                            navigationPage.Title = navigationPageChild.Title;
                            navigationPage.IconImageSource = navigationPageChild.IconImageSource;

                            tabbedPage.Children.Add(navigationPage);
                        }
                    }
                    else
                    {
                        var tab = CreatePageFromSegment(tabToCreate);
                        tabbedPage.Children.Add(tab);
                    }
                }
            }

            TabbedPageSelectTab(tabbedPage, parameters);
        }

        private void ConfigureCarouselPage(CarouselPage carouselPage, string segment)
        {
            foreach (var child in carouselPage.Children)
            {
                PageUtilities.SetAutowireViewModel(child);
            }

            var parameters = UriParsingHelper.GetSegmentParameters(segment);

            CarouselPageSelectTab(carouselPage, parameters);
        }

        private static void SelectPageTab(Page page, INavigationParameters parameters)
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

        private static void TabbedPageSelectTab(TabbedPage tabbedPage, INavigationParameters parameters)
        {
            var selectedTab = parameters?.GetValue<string>(KnownNavigationParameters.SelectedTab);
            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var selectedTabType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(selectedTab));

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

        private static void CarouselPageSelectTab(CarouselPage carouselPage, INavigationParameters parameters)
        {
            var selectedTab = parameters?.GetValue<string>(KnownNavigationParameters.SelectedTab);
            if (!string.IsNullOrWhiteSpace(selectedTab))
            {
                var selectedTabType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(selectedTab));

                foreach (var child in carouselPage.Children)
                {
                    if (child.GetType() == selectedTabType)
                        carouselPage.CurrentPage = child;
                }
            }
        }

        protected virtual async Task UseReverseNavigation(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            var navigationStack = new Stack<string>();

            if (!String.IsNullOrWhiteSpace(nextSegment))
                navigationStack.Push(nextSegment);

            var illegalSegments = new Queue<string>();

            bool illegalPageFound = false;
            foreach (var item in segments)
            {
                //if we run into an illegal page, we need to create new navigation segments to properly handle the deep link
                if (illegalPageFound)
                {
                    illegalSegments.Enqueue(item);
                    continue;
                }

                //if any page decide to go modal, we need to consider it and all pages after it an illegal page
                var pageParameters = UriParsingHelper.GetSegmentParameters(item);
                if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
                {
                    if (pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation))
                    {
                        illegalSegments.Enqueue(item);
                        illegalPageFound = true;
                    }
                    else
                    {
                        navigationStack.Push(item);
                    }
                }
                else
                {
                    var pageType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(item));
                    if (PageUtilities.IsSameOrSubclassOf<MasterDetailPage>(pageType))
                    {
                        illegalSegments.Enqueue(item);
                        illegalPageFound = true;
                    }
                    else
                    {
                        navigationStack.Push(item);
                    }
                }
            }

            var pageOffset = currentPage.Navigation.NavigationStack.Count;
            if (currentPage.Navigation.NavigationStack.Count > 2)
                pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

            var onNavigatedFromTarget = currentPage;
            if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
                onNavigatedFromTarget = navPage.CurrentPage;

            bool insertBefore = false;
            while (navigationStack.Count > 0)
            {
                var segment = navigationStack.Pop();
                var nextPage = CreatePageFromSegment(segment);
                await DoNavigateAction(onNavigatedFromTarget, segment, nextPage, parameters, async () =>
                {
                    await DoPush(currentPage, nextPage, useModalNavigation, animated, insertBefore, pageOffset);
                });
                insertBefore = true;
            }

            //if an illegal page is found, we force a Modal navigation
            if (illegalSegments.Count > 0)
                await ProcessNavigation(currentPage.Navigation.NavigationStack.Last(), illegalSegments, parameters, true, animated);
        }

        protected virtual Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool animated, bool insertBeforeLast = false, int navigationOffset = 0)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            if (currentPage == null)
            {
                _applicationProvider.MainPage = page;
                return Task.FromResult<object>(null);
            }
            else
            {
                bool useModalForPush = UseModalNavigation(currentPage, useModalNavigation);

                if (useModalForPush)
                {
                    return currentPage.Navigation.PushModalAsync(page, animated);
                }
                else
                {
                    if (insertBeforeLast)
                    {
                        return InsertPageBefore(currentPage, page, navigationOffset);
                    }
                    else
                    {
                        return currentPage.Navigation.PushAsync(page, animated);
                    }
                }

            }
        }

        protected virtual Task InsertPageBefore(Page currentPage, Page page, int pageOffset)
        {
            var navigationPage = currentPage.Parent as NavigationPage;
            var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
            currentPage.Navigation.InsertPageBefore(page, firstPage);
            return Task.FromResult(true);
        }

        protected virtual Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
        {
            if (useModalNavigation)
                return navigation.PopModalAsync(animated);
            else
                return navigation.PopAsync(animated);
        }

        protected virtual Page GetCurrentPage()
        {
            return _page != null ? _page : _applicationProvider.MainPage;
        }

        public static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
        {
            if (useModalNavigationDefault.HasValue)
                return useModalNavigationDefault.Value;
            else if (currentPage is NavigationPage)
                return false;

            return !PageUtilities.HasNavigationPageParent(currentPage);
        }

        public bool UseModalGoBack(Page currentPage, bool? useModalNavigationDefault)
        {
            if (useModalNavigationDefault.HasValue)
                return useModalNavigationDefault.Value;
            else if (currentPage is NavigationPage navPage)
                return GoBackModal(navPage);
            else if (PageUtilities.HasNavigationPageParent(currentPage, out var navParent))
                return GoBackModal(navParent);
            else
                return true;
        }

        private bool GoBackModal(NavigationPage navPage)
        {
            if (navPage.CurrentPage != navPage.RootPage)
                return false;
            else if (navPage.CurrentPage == navPage.RootPage && navPage.Parent is Application && _applicationProvider.MainPage != navPage)
                return true;
            else if (navPage.Parent is TabbedPage tabbed && tabbed != _applicationProvider.MainPage)
                return true;
            else if (navPage.Parent is CarouselPage carousel && carousel != _applicationProvider.MainPage)
                return true;

            return false;
        }

        public static bool UseReverseNavigation(Page currentPage, Type nextPageType) =>
            PageUtilities.HasNavigationPageParent(currentPage) && PageUtilities.IsSameOrSubclassOf<ContentPage>(nextPageType);
    }

    public static class KnownNavigationParameters
    {
        /// <summary>
        /// Used to dynamically create a Page that will be used as a Tab when navigating to a TabbedPage.
        /// </summary>
        public const string CreateTab = "createTab";

        /// <summary>
        /// Used to select an existing Tab when navigating to a Tabbedpage.
        /// </summary>
        public const string SelectedTab = "selectedTab";

        /// <summary>
        /// Used to control the navigation stack. If <c>true</c> uses PopModalAsync, if <c>false</c> uses PopAsync.
        /// </summary>
        public const string UseModalNavigation = "useModalNavigation";

        /// <summary>
        /// Used to define a navigation parameter that is bound directly to a CommandParameter via <code>{Binding .}</code>.
        /// </summary>
        public const string XamlParam = "xamlParam";
    }

    public static class KnownInternalParameters
    {
        public const string NavigationMode = "__NavigationMode";
    }

    public static class UriParsingHelper
    {
        private static readonly char[] _pathDelimiter = { '/' };

        public static Queue<string> GetUriSegments(Uri uri)
        {
            var segmentStack = new Queue<string>();

            if (!uri.IsAbsoluteUri)
            {
                uri = EnsureAbsolute(uri);
            }

            string[] segments = uri.PathAndQuery.Split(_pathDelimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                segmentStack.Enqueue(Uri.UnescapeDataString(segment));
            }

            return segmentStack;
        }

        public static string GetSegmentName(string segment)
        {
            return segment.Split('?')[0];
        }

        public static INavigationParameters GetSegmentParameters(string segment)
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

        public static INavigationParameters GetSegmentParameters(string uriSegment, INavigationParameters parameters)
        {
            var navParameters = UriParsingHelper.GetSegmentParameters(uriSegment);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    navParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return navParameters;
        }

        public static IDialogParameters GetSegmentDialogParameters(string segment)
        {
            string query = string.Empty;

            if (string.IsNullOrWhiteSpace(segment))
            {
                return new DialogParameters(query);
            }

            var indexOfQuery = segment.IndexOf('?');
            if (indexOfQuery > 0)
                query = segment.Substring(indexOfQuery);

            return new DialogParameters(query);
        }

        public static IDialogParameters GetSegmentParameters(string uriSegment, IDialogParameters parameters)
        {
            var dialogParameters = UriParsingHelper.GetSegmentDialogParameters(uriSegment);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    dialogParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return dialogParameters;
        }

        public static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            if (!uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost/" + uri, UriKind.Absolute);
            }
            return new Uri("http://localhost" + uri, UriKind.Absolute);
        }

        public static Uri Parse(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (uri.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost" + uri, UriKind.Absolute);
            }
            else
            {
                return new Uri(uri, UriKind.RelativeOrAbsolute);
            }
        }
    }

    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
            {
                action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                action(viewModelAsT);
            }
        }

        public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
        {
            if (view is T viewAsT)
            {
                await action(viewAsT);
            }

            if (view is BindableObject element && element.BindingContext is T viewModelAsT)
            {
                await action(viewModelAsT);
            }
        }

        public static void DestroyPage(Page page)
        {
            try
            {
                DestroyChildren(page);

                InvokeViewAndViewModelAction<IDestructible>(page, v => v.Destroy());

                page.Behaviors?.Clear();
                page.BindingContext = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot destroy {page}.", ex);
            }
        }

        private static void DestroyChildren(Page page)
        {
            switch (page)
            {
                case MasterDetailPage mdp:
                    DestroyPage(mdp.Master);
                    DestroyPage(mdp.Detail);
                    break;
                case TabbedPage tabbedPage:
                    foreach (var item in tabbedPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case CarouselPage carouselPage:
                    foreach (var item in carouselPage.Children.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
                case NavigationPage navigationPage:
                    foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
                    {
                        DestroyPage(item);
                    }
                    break;
            }
        }

        public static void DestroyWithModalStack(Page page, IList<Page> modalStack)
        {
            foreach (var childPage in modalStack.Reverse())
            {
                DestroyPage(childPage);
            }
            DestroyPage(page);
        }


        public static Task<bool> CanNavigateAsync(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigationAsync confirmNavigationItem)
                return confirmNavigationItem.CanNavigateAsync(parameters);

            if (page is BindableObject bindableObject)
            {
                if (bindableObject.BindingContext is IConfirmNavigationAsync confirmNavigationBindingContext)
                    return confirmNavigationBindingContext.CanNavigateAsync(parameters);
            }

            return Task.FromResult(CanNavigate(page, parameters));
        }

        public static bool CanNavigate(object page, INavigationParameters parameters)
        {
            if (page is IConfirmNavigation confirmNavigationItem)
                return confirmNavigationItem.CanNavigate(parameters);

            if (page is BindableObject bindableObject)
            {
                if (bindableObject.BindingContext is IConfirmNavigation confirmNavigationBindingContext)
                    return confirmNavigationBindingContext.CanNavigate(parameters);
            }

            return true;
        }

        public static void OnNavigatedFrom(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
        }

        public static async Task OnInitializedAsync(object page, INavigationParameters parameters)
        {
            if (page is null) return;

            InvokeViewAndViewModelAction<IInitialize>(page, v => v.Initialize(parameters));
            await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, async v => await v.InitializeAsync(parameters));
        }

        private static bool HasKey(this IEnumerable<KeyValuePair<string, object>> parameters, string name, out string key)
        {
            key = parameters.Select(x => x.Key).FirstOrDefault(k => k.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return !string.IsNullOrEmpty(key);
        }

        public static void OnNavigatedTo(object page, INavigationParameters parameters)
        {
            if (page != null)
                InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
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

        public static Page GetPreviousPage(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
        {
            Page previousPage = null;

            int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
            int previousPageIndex = currentPageIndex - 1;
            if (navStack.Count >= 0 && previousPageIndex >= 0)
                previousPage = navStack[previousPageIndex];

            return previousPage;
        }

        public static int GetCurrentPageIndex(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
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

        public static Page GetCurrentPage(Page mainPage)
        {
            var page = mainPage;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            return GetOnNavigatedToTargetFromChild(page);
        }

        public static void HandleSystemGoBack(Page previousPage, Page currentPage)
        {
            var parameters = new NavigationParameters();
            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
            OnNavigatedFrom(previousPage, parameters);
            OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
            DestroyPage(previousPage);
        }

        internal static bool HasDirectNavigationPageParent(Page page)
        {
            return page?.Parent != null && page?.Parent is NavigationPage;
        }

        internal static bool HasNavigationPageParent(Page page) =>
            HasNavigationPageParent(page, out var _);

        internal static bool HasNavigationPageParent(Page page, out NavigationPage navigationPage)
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

        internal static bool IsSameOrSubclassOf<T>(Type potentialDescendant)
        {
            if (potentialDescendant == null)
                return false;

            Type potentialBase = typeof(T);

            return potentialDescendant.GetTypeInfo().IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        /// <summary>
        /// Sets the AutowireViewModel property on the View to <c>true</c> if there is currently
        /// no BindingContext and the AutowireViewModel property has not been set.
        /// </summary>
        /// <param name="element">The View typically a <see cref="Page"/> or <see cref="View"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetAutowireViewModel(VisualElement element)
        {
            if (element.BindingContext is null && ViewModelLocator.GetAutowireViewModel(element) is null)
                ViewModelLocator.SetAutowireViewModel(element, true);
        }
    }

    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Selects a Tab of the TabbedPage parent.
        /// </summary>
        /// <param name="navigationService">Service for handling navigation between views</param>
        /// <param name="name">The name of the tab to select</param>
        /// <param name="parameters">The navigation parameters</param>
        public static async Task<INavigationResult> SelectTabAsync(this INavigationService navigationService, string name, INavigationParameters parameters = null)
        {
            try
            {
                var currentPage = ((IPageAware)navigationService).Page;

                var canNavigate = await PageUtilities.CanNavigateAsync(currentPage, parameters);
                if (!canNavigate)
                    throw new Exception($"IConfirmNavigation for {currentPage} returned false");

                TabbedPage tabbedPage = null;

                if (currentPage.Parent is TabbedPage parent)
                {
                    tabbedPage = parent;
                }
                else if (currentPage.Parent is NavigationPage navPage)
                {
                    if (navPage.Parent != null && navPage.Parent is TabbedPage parent2)
                    {
                        tabbedPage = parent2;
                    }
                }

                if (tabbedPage == null)
                    throw new Exception("No parent TabbedPage could be found");

                var tabToSelectedType = PageNavigationRegistry.GetPageType(UriParsingHelper.GetSegmentName(name));
                if (tabToSelectedType is null)
                    throw new Exception($"No View Type has been registered for '{name}'");

                Page target = null;
                foreach (var child in tabbedPage.Children)
                {
                    if (child.GetType() == tabToSelectedType)
                    {
                        target = child;
                        break;
                    }

                    if (child is NavigationPage childNavPage)
                    {
                        if (childNavPage.CurrentPage.GetType() == tabToSelectedType ||
                            childNavPage.RootPage.GetType() == tabToSelectedType)
                        {
                            target = child;
                            break;
                        }
                    }
                }

                if (target is null)
                    throw new Exception($"Could not find a Child Tab for '{name}'");

                var tabParameters = UriParsingHelper.GetSegmentParameters(name, parameters);

                tabbedPage.CurrentPage = target;
                PageUtilities.OnNavigatedFrom(currentPage, tabParameters);
                PageUtilities.OnNavigatedTo(target, tabParameters);
            }
            catch (Exception ex)
            {
                return new NavigationResult { Exception = ex };
            }

            return new NavigationResult { Success = true };
        }
    }

    public static class NavigationParametersExtensions
    {
        public static NavigationMode GetNavigationMode(this INavigationParameters parameters)
        {
            var internalParams = (INavigationParametersInternal)parameters;
            if (internalParams.ContainsKey(KnownInternalParameters.NavigationMode))
                return internalParams.GetValue<NavigationMode>(KnownInternalParameters.NavigationMode);

            throw new System.ArgumentNullException("Navigation mode not available");
        }

        internal static INavigationParametersInternal GetNavigationParametersInternal(this INavigationParameters parameters)
        {
            return (INavigationParametersInternal)parameters;
        }
    }

    /// <summary>
    /// Extension methods for Navigation or Dialog parameters
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ParametersExtensions
    {
        /// <summary>
        /// Searches <paramref name="parameters"/> for <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">The type of the parameter to return</typeparam>
        /// <param name="parameters">A collection of parameters to search</param>
        /// <param name="key">The key of the parameter to find</param>
        /// <returns>A matching value of <typeparamref name="T"/> if it exists</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T GetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            (T)GetValue(parameters, key, typeof(T));

        /// <summary>
        /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>
        /// </summary>
        /// <param name="parameters">A collection of parameters to search</param>
        /// <param name="key">The key of the parameter to find</param>
        /// <param name="type">The type of the parameter to return</param>
        /// <returns>A matching value of <paramref name="type"/> if it exists</returns>
        /// <exception cref="InvalidCastException">Unable to convert the value of Type</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetValue(this IEnumerable<KeyValuePair<string, object>> parameters, string key, Type type)
        {
            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (TryGetValueInternal(kvp, type, out var value))
                        return value;

                    throw new InvalidCastException($"Unable to convert the value of Type '{kvp.Value.GetType().FullName}' to '{type.FullName}' for the key '{key}' ");
                }
            }

            return GetDefault(type);
        }

        /// <summary>
        /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">The type of the parameter to return</typeparam>
        /// <param name="parameters">A collection of parameters to search</param>
        /// <param name="key">The key of the parameter to find</param>
        /// <param name="value">The value of parameter to return</param>
        /// <returns>Success if value is found; otherwise returns <c>false</c></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool TryGetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key, out T value)
        {
            var type = typeof(T);

            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    var success = TryGetValueInternal(kvp, typeof(T), out object valueAsObject);
                    value = (T)valueAsObject;
                    return success;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Searches <paramref name="parameters"/> for value referenced by <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">The type of the parameter to return</typeparam>
        /// <param name="parameters">A collection of parameters to search</param>
        /// <param name="key">The key of the parameter to find</param>
        /// <returns>An IEnumberable{T} of all the values referenced by key</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> GetValues<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key)
        {
            List<T> values = new List<T>();
            var type = typeof(T);

            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    TryGetValueInternal(kvp, type, out var value);
                    values.Add((T)value);
                }
            }

            return values.AsEnumerable();
        }

        private static bool TryGetValueInternal(KeyValuePair<string, object> kvp, Type type, out object value)
        {
            value = GetDefault(type);
            var success = false;
            if (kvp.Value == null)
            {
                success = true;
            }
            else if (kvp.Value.GetType() == type)
            {
                success = true;
                value = kvp.Value;
            }
            else if (type.IsAssignableFrom(kvp.Value.GetType()))
            {
                success = true;
                value = kvp.Value;
            }
            else if (type.IsEnum)
            {
                var valueAsString = kvp.Value.ToString();
                if (Enum.IsDefined(type, valueAsString))
                {
                    success = true;
                    value = Enum.Parse(type, valueAsString);
                }
                else if (int.TryParse(valueAsString, out var numericValue))
                {
                    success = true;
                    value = Enum.ToObject(type, numericValue);
                }
            }

            if (!success && type.GetInterface("System.IConvertible") != null)
            {
                success = true;
                value = Convert.ChangeType(kvp.Value, type);
            }

            return success;
        }

        /// <summary>
        /// Checks to see if key exists in parameter collection
        /// </summary>
        /// <param name="parameters">IEnumberable to search</param>
        /// <param name="key">The key to search the <paramref name="parameters"/> for existence</param>
        /// <returns><c>true</c> if key exists; <c>false</c> otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool ContainsKey(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            parameters.Any(x => string.Compare(x.Key, key, StringComparison.Ordinal) == 0);

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }

    public static class PageNavigationRegistry
    {
        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo
            {
                Name = name,
                Type = pageType
            };

            if (!_pageRegistrationCache.ContainsKey(name))
                _pageRegistrationCache.Add(name, info);
        }

        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return default(PageNavigationInfo);
        }

        public static PageNavigationInfo GetPageNavigationInfo(Type pageType)
        {
            foreach (var item in _pageRegistrationCache)
            {
                if (item.Value.Type == pageType)
                    return item.Value;
            }

            return default(PageNavigationInfo);
        }

        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name).Type;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearRegistrationCache()
        {
            _pageRegistrationCache.Clear();
        }
    }

    /// <summary>
    /// Provides Attachable properties for Navigation
    /// </summary>
    public static class Navigation
    {
        internal static readonly BindableProperty NavigationServiceProperty =
            BindableProperty.CreateAttached("NavigationService",
                typeof(INavigationService),
                typeof(Navigation),
                default(INavigationService));

        internal static readonly BindableProperty NavigationScopeProperty =
            BindableProperty.CreateAttached("NavigationScope",
                typeof(IScopedProvider),
                typeof(Navigation),
                default(IScopedProvider),
                propertyChanged: OnNavigationScopeChanged);

        private static void OnNavigationScopeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            if (oldValue != null && newValue is null && oldValue is IScopedProvider oldProvider)
            {
                oldProvider.Dispose();
                return;
            }

            if (newValue != null && newValue is IScopedProvider scopedProvider)
            {
                scopedProvider.IsAttached = true;
            }
        }

        /// <summary>
        /// Provides bindable CanNavigate Bindable Property
        /// </summary>
        public static readonly BindableProperty CanNavigateProperty =
            BindableProperty.CreateAttached("CanNavigate",
                typeof(bool),
                typeof(Navigation),
                true,
                propertyChanged: OnCanNavigatePropertyChanged);

        internal static readonly BindableProperty RaiseCanExecuteChangedInternalProperty =
            BindableProperty.CreateAttached("RaiseCanExecuteChangedInternal",
                typeof(Action),
                typeof(Navigation),
                default(Action));

        /// <summary>
        /// Gets the Bindable Can Navigate property for an element
        /// </summary>
        /// <param name="view">The bindable element</param>
        public static bool GetCanNavigate(BindableObject view) => (bool)view.GetValue(CanNavigateProperty);

        /// <summary>
        /// Sets the Bindable Can Navigate property for an element
        /// </summary>
        /// <param name="view">The bindable element</param>
        /// <param name="value">The Can Navigate value</param>
        public static void SetCanNavigate(BindableObject view, bool value) => view.SetValue(CanNavigateProperty, value);

        /// <summary>
        /// Gets the instance of <see cref="INavigationService"/> for the given <see cref="Page"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <returns>The <see cref="INavigationService"/></returns>
        /// <remarks>Do not use... this is an internal use API</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService GetNavigationService(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            var navService = (INavigationService)page.GetValue(NavigationServiceProperty);
            if (navService is null)
            {
                var currentScope = (IScopedProvider)page.GetValue(NavigationScopeProperty) ?? ContainerLocator.Container.CurrentScope;

                if (currentScope is null)
                    currentScope = ContainerLocator.Container.CreateScope();

                if (!currentScope.IsAttached)
                    page.SetValue(NavigationScopeProperty, currentScope);

                currentScope.IsAttached = true;

                navService = currentScope.Resolve<INavigationService>();
                if (navService is IPageAware pa)
                {
                    pa.Page = page;
                }

                page.SetValue(NavigationServiceProperty, navService);
            }

            return navService;
        }

        internal static Action GetRaiseCanExecuteChangedInternal(BindableObject view) => (Action)view.GetValue(RaiseCanExecuteChangedInternalProperty);

        internal static void SetRaiseCanExecuteChangedInternal(BindableObject view, Action value) => view.SetValue(RaiseCanExecuteChangedInternalProperty, value);

        private static void OnCanNavigatePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var action = GetRaiseCanExecuteChangedInternal(bindable);
            action?.Invoke();
        }
    }

    public static class VisualElementExtensions
    {
        public static bool TryGetParentPage(this VisualElement element, out Page page)
        {
            page = GetParentPage(element);
            return page != null;
        }

        private static Page GetParentPage(Element visualElement)
        {
            switch (visualElement.Parent)
            {
                case Page page:
                    return page;
                case null:
                    return null;
                default:
                    return GetParentPage(visualElement.Parent);
            }
        }
    }
}
