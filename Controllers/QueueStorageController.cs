using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


[ApiController]
[Route("[controller]")]
public class QueueStorageController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public string connectionString;

     public QueueStorageController(IConfiguration configuration)
    {
        _configuration = configuration;
        // Get the connection string from app settings
         this.connectionString = _configuration.GetValue<string>("ConnectionStrings:StorageConnectionString"); 

    }
    
// Create a message queue

[HttpPost]
public bool CreateQueue(string queueName)
{
    try
    {
      
        // Instantiate a QueueClient which will be used to create and manipulate the queue
        QueueClient queueClient = new QueueClient(this.connectionString, queueName);
        Console.WriteLine(connectionString);

        // Create the queue
        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            Console.WriteLine($"Queue created: '{queueClient.Name}'");
            return true;
        }
        else
        {
            Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}\n\n");
        Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
        return false;
    }
}

//-------------------------------------------------
// Insert a message into a queue
//-------------------------------------------------
[HttpPost("InsertMessage")]
public void InsertMessage(string queueName, string message)
{
    // Get the connection string from app settings
    //string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

    // Instantiate a QueueClient which will be used to create and manipulate the queue
    QueueClient queueClient = new QueueClient(this.connectionString, queueName);

    // Create the queue if it doesn't already exist
    queueClient.CreateIfNotExists();

    if (queueClient.Exists())
    {
        // Send a message to the queue
        queueClient.SendMessage(message);
    }

    Console.WriteLine($"Inserted: {message}");
}

//-------------------------------------------------
// Peek at a message in the queue
//-------------------------------------------------
[HttpGet]
public void PeekMessage(string queueName)
{
    // Get the connection string from app settings
    //string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

    // Instantiate a QueueClient which will be used to manipulate the queue
    QueueClient queueClient = new QueueClient(this.connectionString, queueName);

    if (queueClient.Exists())
    { 
        // Peek at the next message
        PeekedMessage[] peekedMessage = queueClient.PeekMessages();

        // Display the message
        Console.WriteLine($"Peeked message: '{peekedMessage[0].Body}'");
    }
}

//-------------------------------------------------
// Update an existing message in the queue
//-------------------------------------------------
[HttpPut]
public void UpdateMessage(string queueName)
{
    // Get the connection string from app settings
    //string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

    // Instantiate a QueueClient which will be used to manipulate the queue
    QueueClient queueClient = new QueueClient(this.connectionString, queueName);

    if (queueClient.Exists())
    {
        // Get the message from the queue
        QueueMessage[] message = queueClient.ReceiveMessages();

        // Update the message contents
        queueClient.UpdateMessage(message[0].MessageId, 
                message[0].PopReceipt, 
                "Updated contents",
                TimeSpan.FromSeconds(60.0)  // Make it invisible for another 60 seconds
            );
    }
}

//-------------------------------------------------
// Process and remove a message from the queue
//-------------------------------------------------
[HttpDelete]
//-------------------------------------------------
// Delete the queue
//-------------------------------------------------
public void DeleteQueue(string queueName)
{
    // Get the connection string from app settings
    //string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

    // Instantiate a QueueClient which will be used to manipulate the queue
    QueueClient queueClient = new QueueClient(this.connectionString, queueName);

    if (queueClient.Exists())
    {
        // Delete the queue
        queueClient.Delete();
    }

    Console.WriteLine($"Queue deleted: '{queueClient.Name}'");
}

}
