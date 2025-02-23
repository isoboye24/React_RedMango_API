
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RedMango_API.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;
        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        // containerName is the container created in the Azure account and the blobName is each file name in the container
        public async Task<bool> DeleteBlob(string blobName, string containterName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containterName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetBlob(string blobName, string containterName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containterName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadBlob(string blobName, string containterName, IFormFile file)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containterName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };
            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
            if (result != null)
            {
                return await GetBlob(blobName, containterName);
            }
            else
            {
                return "";
            }
        }
    }
}
