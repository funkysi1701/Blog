using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebBlog.Data.Services
{
    public class PhotoService
    {
        private readonly IConfiguration configuration;

        public PhotoService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<BlobClient>> Load()
        {
            BlobServiceClient blobServiceClient = new(configuration.GetValue<string>("BlobConnectionString"));

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("dev");
            await container.CreateIfNotExistsAsync();
            var blobs = new List<BlobClient>();
            foreach (BlobItem blob in container.GetBlobs(BlobTraits.None, BlobStates.None, string.Empty))
            {
                var file = container.GetBlobClient(blob.Name);

                blobs.Add(file);
            }
            return blobs;
        }
    }
}
