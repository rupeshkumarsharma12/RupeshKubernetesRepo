using Azure;
using Azure.Storage.Blobs; //  to operate on the service, containers, and blobs
using Azure.Storage.Blobs.Models;//All other utility classes, structures, and enumeration types.
using Azure.Storage.Blobs.Specialized;//to perform operations specific to a blob type 
using AzureBlobStorage.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

[ApiController]
[Route("[controller]")]
public class BlobStorageController : ControllerBase
{
    private readonly IBlobStorage _storage;
    private readonly string _connectionString;
    private readonly string _container;
    public BlobStorageController(IConfiguration _configuration, IBlobStorage storage)
    {
        _storage = storage;
        _connectionString = _configuration.GetValue<string>("ConnectionStrings:StorageAccountConnection");
        _container = _configuration.GetValue<string>("ConnectionStrings:ContainerName");
    }
    [HttpPost("CreateContainer")]
    public async Task<IActionResult> CreateContainer(string containerName)
    {
        BlobContainerClient container;
        try
        {
            container = await _storage.CreateContainer(_connectionString, containerName);
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to create Container due to : ", ex);
        }
        return Ok("Container " + containerName + " :" + container + " Created : Successfully");
    }
    [HttpGet("ListAllFiles")]
    public async Task<List<string>> ListAllFiles(string containerName)
    {
        return await _storage.GetAllDocuments(_connectionString, containerName);
    }

    //Upload by using a Stream
    //The following example uploads a blob by creating a Stream object, and then uploading that stream.
    [Route("UploadFile")]
    [HttpPost]
    public async Task<IActionResult> UploadFile(string localFilePath, string containerName)
    {
        //string localFilePath=@"C:\Users\M1089675\Documents\TestBlobStorage";
        if (string.IsNullOrEmpty(localFilePath))
        {
            throw new ArgumentException($"'{nameof(localFilePath)}' cannot be null or empty.", nameof(localFilePath));
        }
        string fileName = Path.GetFileName(localFilePath);
        try
        {
            if (!string.IsNullOrEmpty(localFilePath))
            {
                FileStream fileStream = System.IO.File.Open(localFilePath, FileMode.OpenOrCreate);
                await _storage.UploadDocument(_connectionString, containerName, fileName, fileStream);
                fileStream.Close();
                fileStream.Dispose();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to Upload file : " + fileName + " due to : " + ex); ;
        }
        return Created("File : " + fileName + " uploaded to Block BLOB storage", "successfully");

    }


    [HttpGet("DownloadFile/{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName, string containerName)
    {
        var content = await _storage.GetDocument(_connectionString, containerName, fileName);
        return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }

    [HttpDelete("DeleteContainer")]
    public async Task<IActionResult> DeleteContainer(string containerName)
    {
        Azure.Response response;
        try
        {
            response = await _storage.DeleteContainer(_connectionString, containerName);
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to Delete Container due to : ", ex);
        }
        return Ok("Container : " + containerName + " Deleted Successfully " + response);
    }
    [Route("DeleteFile/{fileName}")]
    [HttpDelete]
    public async Task<bool> DeleteFile(string fileName, string containerName)
    {
        return await _storage.DeleteDocument(_connectionString, containerName, fileName);
    }

}


