﻿namespace RedMango_API.Services
{
    public interface IBlobService
    {
        Task<string> GetBlob(string blobName, string containterName);
        Task<bool> DeleteBlob(string blobName, string containterName);
        Task<string> UploadBlob(string blobName, string containterName, IFormFile file);
    }
}
