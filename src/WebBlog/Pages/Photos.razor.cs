using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBlog.Data.Services;

namespace WebBlog.Pages
{
    public class PhotosBase : ComponentBase
    {
        [Inject] private PhotoService PhotoService { get; set; }

        protected List<BlobClient> blobs;

        protected override async Task OnInitializedAsync()
        {
            blobs = await PhotoService.Load();
        }
    }
}
