//we will create our C# application to upload a file to this file share we have just created.
//get the nugget packeg 
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace IotApplication.Controllers{
[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly string _connectionString;

    public FileController(IConfiguration _configuration)
    {
        _connectionString = _configuration.GetValue<string>("ConnectionStrings:StorageConnectionString");

    }

    //The following method creates a file share if it doesn't already exist. 
    // by creating a ShareClient object from a connection string

    [HttpPost("CreateFileShare")]
    public async Task<IActionResult> createFileShareAysnc(string fileShareName)
    {
        try
        {
            // Instantiate a ShareClient which will be used to create and manipulate the file share
            ShareClient share = new ShareClient(_connectionString, fileShareName);
            // Create the share if it doesn't already exist
            await share.CreateIfNotExistsAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to create File share due to : ", ex);

        }
        return Ok(fileShareName + " : created successfully");
    }

    [HttpPost("CreateDirectory")]
    public async Task<IActionResult> createDirectoryAysnc(string fileShareName, string directoryName)
    {
        try
        {
            // Instantiate a ShareClient which will be used to create and manipulate the file share
            ShareClient share = new ShareClient(_connectionString, fileShareName);
            // Ensure that the share exists.
            if (await share.ExistsAsync())
            {
                //if fileShare present then 
                // Get a reference to the sample directory
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                // Create the directory if it doesn't already exist
                await directory.CreateIfNotExistsAsync();
            }
            else
            {
                throw new FileNotFoundException("Unable to find Fileshare : ", fileShareName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to create Directory due to : ", ex);

        }
        return Ok(directoryName + " : created successfully");
    }

    [HttpPost("UploadFile")]
    public async Task<IActionResult> UploadFile(string fileShareName, string directoryName, string localFilePath)
    {
        try
        {
            //string localFilePath=@"C:\Users\M1089675\Documents\FileStorageTest.txt";

            //get file share and storage connection
            ShareClient share = new(_connectionString, fileShareName);
            //get the directory
            var directory = share.GetDirectoryClient(directoryName);
            string fileName = Path.GetFileName(localFilePath);
            var file = directory.GetFileClient(fileName);
            using FileStream stream = System.IO.File.Open(localFilePath, FileMode.OpenOrCreate);
            file.Create(stream.Length);
            stream.Close();
            stream.Dispose();
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to upload a file to this fileshare due to : ", ex);
        }
        return Ok("file uploadted successfully");
    }

    [HttpPost("DownloadFile")]
    public async Task<IActionResult> GetFile(string fileShareName, string directoryName, string fileName)
    {
        try
        {

            //get fileShare and storage connection
            ShareClient share = new(_connectionString, fileShareName);
            // Ensure that the share exists
            if (await share.ExistsAsync())
            {
                // Get a reference to the directory
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                // Ensure that the directory exists.
                if (await directory.ExistsAsync())
                {
                    // Get a reference to a file object
                    ShareFileClient file = directory.GetFileClient(fileName);
                    // Ensure that the file exists
                    if (await file.ExistsAsync())
                    {
                        // Download the file
                        ShareFileDownloadInfo download = await file.DownloadAsync();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Unable to download file due to : ", ex);
        }
        return Ok("file Downloaded successfully");
    }

    [HttpDelete("DeleteFile")]
    public async Task<IActionResult> DeleteFile(string fileShareName, string directoryName, string fileName)
    {
        try
        {
            //get fileShare and storage connection
            ShareClient share = new(_connectionString, fileShareName);
            // Ensure that the share exists
            if (await share.ExistsAsync())
            {
                // Get a reference to the directory
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                // Ensure that the directory exists.
                if (await directory.ExistsAsync())
                {
                    // Get a reference to a file object
                    ShareFileClient file = directory.GetFileClient(fileName);
                    // Ensure that the file exists
                    if (await file.ExistsAsync())
                    {
                        //delete file
                        await file.DeleteAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to delete file due to : ", ex);
        }
        return Ok("file deleted successfully");
    }

    [HttpDelete("DeleteDirectory")]
    public async Task<IActionResult> DeleteDirectoryAysnc(string fileShareName, string directoryName)
    {
        try
        {
            // Instantiate a ShareClient which will be used to create and manipulate the file share
            ShareClient share = new ShareClient(_connectionString, fileShareName);
            // Ensure that the share exists.
            if (await share.ExistsAsync())
            {
                //if fileShare present then 
                // Get a reference to the sample directory
                ShareDirectoryClient directory = share.GetDirectoryClient(directoryName);
                // Create the directory if it doesn't already exist
                await directory.DeleteAsync();
            }
            else
            {
                throw new FileNotFoundException("Unable to find Fileshare : ", fileShareName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to Delete Directory due to : ", ex);

        }
        return Ok(directoryName + " : Deleted successfully");
    }

    [HttpDelete("DeleteFileShare")]
    public async Task<IActionResult> deleteFileShareAysnc(string fileShareName)
    {
        try
        {
            // Instantiate a ShareClient which will be used to create and manipulate the file share
            ShareClient share = new ShareClient(_connectionString, fileShareName);
            // Create the share if it doesn't already exist
            await share.DeleteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Unable to Delete File share due to : ", ex);

        }
        return Ok(fileShareName + " : Deleted successfully");
    }

}
}