using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Musix4u_API.Services
{
    public class StorageService
    {
        private readonly string _connectionString;

        public StorageService(IConfiguration configuration)
        {
            _connectionString = configuration["StorageAccount"];
        }

        public async Task<List<string>> GetBlobs(string containerName)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobs = new List<string>();
            foreach (var blobItem in containerClient.GetBlobs())
            {
                blobs.Add($"{containerClient.Uri.AbsoluteUri}/{blobItem.Name}");
            }

            return blobs;
        }

        public async Task<string> UploadFile(string containerName, string fileName, IFormFile file, bool overwrite = true)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            BlobServiceClient serviceClient = new BlobServiceClient(_connectionString);
            var props = (await serviceClient.GetPropertiesAsync()).Value;
            props.DefaultServiceVersion = "2019-07-07";
            await serviceClient.SetPropertiesAsync(props);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (!overwrite)
            {
                string newFileName;
                int count = 1;
                while (await blobClient.ExistsAsync())
                {
                    newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{count++}{Path.GetExtension(fileName)}";
                    blobClient = containerClient.GetBlobClient(newFileName);
                }
            }

            // Open the file and upload its data
            var headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                ContentDisposition = file.ContentDisposition
            };
            await using var uploadFileStream = file.OpenReadStream();
            await blobClient.UploadAsync(uploadFileStream, headers);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadFile(string containerName, string fileName, byte[] bytes, bool overwrite = true)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (!overwrite)
            {
                string newFileName;
                int count = 1;
                while (await blobClient.ExistsAsync())
                {
                    newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{count++}{Path.GetExtension(fileName)}";
                    blobClient = containerClient.GetBlobClient(newFileName);
                }
            }

            // Open the file and upload its data
            await using var uploadFileStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(uploadFileStream, true);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadFile(string containerName, string fileName, string base64String)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Open the file and upload its data
            await using var memoryStream = new MemoryStream(Convert.FromBase64String(base64String));
            await blobClient.UploadAsync(memoryStream, true);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<byte[]> DownloadFile(string containerName, string fileName)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            await using var downloadFileStream = new MemoryStream();
            await download.Content.CopyToAsync(downloadFileStream);

            return downloadFileStream.GetBuffer();
        }

        public async Task DownloadFile(string containerName, string fileName, string downloadFilePath)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            await using var downloadFileStream = File.OpenWrite(downloadFilePath);
            await download.Content.CopyToAsync(downloadFileStream);
        }

        public async Task<bool> DeleteFile(string containerName, string fileName)
        {
            // Create the container if not exists and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Delete the blob
            var response = await blobClient.DeleteIfExistsAsync();

            return response.Value;
        }
    }
}