using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace WebBlog.Components
{
    /// <summary>
    /// Document
    /// </summary>
    public class Document : ComponentBase, IDisposable
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && NavigationManager != null)
                NavigationManager.LocationChanged -= LocationChanged;
        }

        protected override void OnInitialized()
        {
            if (NavigationManager != null)
            {
                NavigationManager.LocationChanged += LocationChanged;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;
            if (NavigationManager != null)
            {
                await SetTitle(new Uri(NavigationManager.Uri)).ConfigureAwait(false);
            }
        }

        private async Task SetTitle(Uri uri)
        {
            var pageName = uri.AbsolutePath.Replace("/", "-", StringComparison.CurrentCultureIgnoreCase);
            if (JsRuntime != null && pageName != "-")
            {
                await JsRuntime.InvokeVoidAsync("JsFunctions.setDocumentTitle", PageTitleGenerator.Create(pageName)).ConfigureAwait(false);
            }
        }

        private async void LocationChanged(object sender, LocationChangedEventArgs e)
        {
            await SetTitle(new Uri(e.Location));
        }
    }
}