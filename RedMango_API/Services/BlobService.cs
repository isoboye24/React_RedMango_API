
namespace RedMango_API.Services
{
    public class BlobService : IBlobService
    {
        public Task<string> DeleteBlob(string blobName, string containterName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetBlob(string blobName, string containterName)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadBlob(string blobName, string containterName, IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
