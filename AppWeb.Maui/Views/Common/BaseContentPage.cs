using AppWeb.ViewModels.Core.Base;

namespace AppWeb.Maui.Views.Common
{
    /// <summary>Base ContentPage implementation that handles ViewModel lifecycle and navigation</summary>
    /// <typeparam name="TViewModel">Type of ViewModel for this page</typeparam>
    public abstract class BaseContentPage<TViewModel> : ContentPage where TViewModel : IViewModelBase
    {
        /// <summary>Gets the ViewModel associated with this page</summary>
        protected TViewModel ViewModel => (TViewModel)BindingContext;

        protected BaseContentPage(TViewModel viewModel)
        {
            BindingContext = viewModel;
            //Subscribe to lifecycle events
            Appearing += OnPageAppearing;
            Disappearing += OnPageDisappearing;
        }
        
        private async void OnPageAppearing(object sender, EventArgs e)
        { if (ViewModel != null) await OnViewModelAppearing(); }
        
        private async void OnPageDisappearing(object sender, EventArgs e)
        { if (ViewModel != null) await OnViewModelDisappearing(); }
        
        /// <summary>Called when the page appears and forwards to the ViewModel's OnAppearingAsync method</summary>
        /// <returns>Task representing the asynchronous operation</returns>
        protected virtual Task OnViewModelAppearing()
        { return ViewModel.InitializeAsync(); }
        
        /// <summary>Override to customize ViewModel initialization behavior</summary>
        /// <returns>Task representing the asynchronous operation</returns>
        protected virtual Task OnViewModelDisappearing()
        { return ViewModel.OnDisappearing(); }
        
        /// <summary>Called when the page disappears from screen</summary>
        protected override void OnDisappearing()
        { base.OnDisappearing(); }
        
        /// <summary>Called when the page is being disposed</summary>
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            // If the handler is null and the ViewModel implements IDisposable, dispose the ViewModel when the page is destroyed
            if (Handler == null && ViewModel is IDisposable) ViewModel.Dispose();
        }
        
        /// <summary>Called when the page is being navigated to with parameters.</summary>
        public virtual async Task OnNavigatingTo(object parameter)
        { if (ViewModel != null) await ViewModel.OnNavigatingToAsync(parameter); }
        
        /// <summary>Called when the page has been navigated to.</summary>
        public virtual async Task OnNavigatedTo()
        { if (ViewModel != null) await ViewModel.OnNavigatedToAsync(); }
        
        /// <summary>Called when navigating away from the page.</summary>
        public virtual async Task OnNavigatedFrom()
        { if (ViewModel != null)await ViewModel.OnNavigatedFromAsync(); }
    }
} 