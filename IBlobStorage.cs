using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AzureBlobStorage.Helpers
{
    public interface IBlobStorage
    {

        Task<Azure.Response> DeleteContainer(string _connectionString, string ContainerName);
        Task<BlobContainerClient> CreateContainer(string _connectionString, string ContainerName);
        public Task<List<string>> GetAllDocuments(string connectionString, string containerName);
        Task UploadDocument(string connectionString, string containerName, string fileName, Stream fileContent);
        Task<Stream> GetDocument(string connectionString, string containerName, string fileName);
        Task<bool> DeleteDocument(string connectionString, string containerName, string fileName);

    }
}