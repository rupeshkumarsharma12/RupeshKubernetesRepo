using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Azure.Data.Tables;
using System.Collections.Generic;

namespace MyIOTDevice.Controllers
{

[ApiController]
[Route("[controller]")]
public class MYIOTTableStorageController : ControllerBase
{
    private readonly string connectionString;
    // New instance of the TableClient class
    TableServiceClient tableServiceClient;
    public MYIOTTableStorageController(IConfiguration _configuration)
    {
        connectionString = _configuration.GetValue<string>("ConnectionStrings:CosmosDBTableAPI");
        this.tableServiceClient = new TableServiceClient(connectionString);
    }
    //Use the TableClient.CreateIfNotExistsAsync method on the TableClient to create a new table if it doesn't already exist.
    // This method will return a reference to the existing or newly created table.

    [HttpPost("CreateTable")]
    public async Task<IActionResult> CreateTableAsync(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
        }
        // New instance of TableClient class referencing the server-side table
        TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);
        try
        {
            await tableClient.CreateIfNotExistsAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to create table due to : {0}", ex);
        }

        return Created($"{0} Table created Successfuly", tableName);
    }
    /*Create an item
The easiest way to create a new item in a table is to create a class that implements the ITableEntity interface. 
You can then add your own properties to the class to populate columns of data in that table row.*/

    [HttpPost("AddItem")]
    public async Task<IActionResult> AddItemAsync(string tableName)
    {


        // New instance of TableClient class referencing the server-side table
        TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);
        Azure.Response response;
        try
        {
            var prod3 = new Product()
            {
                RowKey = "001222001001",
                PartitionKey = "2gear-surf-surfboardsnew",
                Name = "Ocean Surfboard",
                Quantity = 8,
                Sale = true
            };
            // Add new item to server-side table
            await tableClient.AddEntityAsync<Product>(prod3);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to create Item due to : {0}", ex);
        }

        return Ok(" Created successfully");
    }
    /*Get an item
   You can retrieve a specific item from a table using the TableEntity.GetEntityAsync<T> method.
   Provide the partitionKey and rowKey as parameters to identify the correct row
  to perform a quick point read of that item.*/
    /// Read a single item from container
    [HttpGet("GetItem")]
    public async Task<IActionResult> GetItemAsync(string tableName, string RowKey, string PartitionKey)
    {
        // New instance of TableClient class referencing the server-side table
        TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);
        Azure.Response<Product> product;
        try
        {
            // Read a single item from container
            product = await tableClient.GetEntityAsync<Product>(
                rowKey: RowKey,
                partitionKey: PartitionKey
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to Get Item due to : {0}", ex);
        }

        return Ok(product + " Item Retrived Successfully!! ");
    }
    //Delete an Azure table
    //Individual tables can be deleted from the service.

    [HttpDelete("DeleteTable")]
    public async Task<IActionResult> DeleteTableAsync(string tableName)
    {
        // New instance of TableClient class referencing the server-side table
        TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

        try
        {
            await tableServiceClient.DeleteTableAsync(tableName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to delete table due to : {0}", ex);
        }
        return Ok("Table " + tableName + " deleted Successfully!! ");
    }
    //Delete table entities
    //If we no longer need our new table entity, it can be deleted.
    [HttpDelete("DeleteEntities")]
    public async Task<IActionResult> DeleteEntitiesAsync(string tableName, string RowKey, string PartitionKey)
    {
        // New instance of TableClient class referencing the server-side table
        TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

        try
        {
            // Delete the entity given the partition and row key.
            await tableClient.DeleteEntityAsync(PartitionKey, RowKey);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to delete Entities due to : {0}", ex);
        }
        return Ok("Entity deleted Successfully!! ");
    }
}
}



