// This controller is for Adding Tages, (Desired/Reported)properties to devices and Query device twins.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Linq;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;


[ApiController]
[Route("[controller]")]
public class DevicetwinController : ControllerBase
{
    private IConfiguration Configuration;
    private static RegistryManager registryManager;
    private static DeviceClient Client = null;
    private readonly ILogger<DevicetwinController> _logger;

    public DevicetwinController(IConfiguration _configuration, ILogger<DevicetwinController> logger)
    {
        Configuration = _configuration;
        _logger = logger;
        registryManager = RegistryManager.CreateFromConnectionString(this.Configuration.GetConnectionString("NxTIoTHubSAP"));//shared access policies connection string.
        Client = DeviceClient.CreateFromConnectionString(this.Configuration.GetConnectionString("DeviceConnectionString"), Microsoft.Azure.Devices.Client.TransportType.Mqtt);

    }

    [HttpPut]
    [Route("AddTags")]
    //the service app : adds location metadata to the device twin associated with DeviceId.
    //retrieves the device twin for myDeviceId, and finally updates its tags with the desired location information. 
    public async Task<IActionResult> AddTags(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));
        }
        try
        {
            var twin = await registryManager.GetTwinAsync(deviceId);
            if (twin == null)
            {
                throw new Exception("Devicetwin Not found for : " + deviceId);
            }
            var patch =
                @"{
            tags: {
                location: {
                    Region: 'India',
                    plant: 'Banglore'
                }                           
            }
        }";
            var updatedTwin = await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
            return Ok("Device " + deviceId + " tag updated ");
        }
        catch (Exception ex)
        {
            throw new Exception("couldn't AddTag for Device " + deviceId + " due to : " + ex);
        }

    }

    // the device app :that connects to your hub as (IoT device), and then updates its reported properties that is connected using a cellular network.
    [HttpPut]
    [Route("Properties/Reported")]
    public async Task<IActionResult> UpdateReportedProperties(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));
        }
        try
        {
            await Client.GetTwinAsync();
            //Console.WriteLine("Sending connectivity data as reported property");
            TwinCollection reportedProperties, connectivity;
            reportedProperties = new TwinCollection();
            connectivity = new TwinCollection();            
            connectivity["type"] = "cellular";
            reportedProperties["connectivity"] = connectivity;
            reportedProperties["temperature"] = "30";
            reportedProperties["pressure"] = "string";
            reportedProperties["accuracy"] = "High";
            reportedProperties["frequency"] = "23 HZ";
            reportedProperties["sensorType"] = "Touch";
            await Client.UpdateReportedPropertiesAsync(reportedProperties);
        }
        catch (Exception ex)
        {
            throw new Exception("couldn't Update Reported Property for Device : " + deviceId + " due to : " + ex);
        }

        return Ok("the device app : " + deviceId + " Reported Property updated successfully");
    }


    [HttpPut]
    [Route("Properties/Desired")]
    public async Task<IActionResult> UpdateDesiredProperties(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));
        }
        try
        {
            var twin = await registryManager.GetTwinAsync(deviceId);
            if (twin == null)
            {
                throw new Exception("Devicetwin Not found for : " + deviceId);
            }
            twin.Properties.Desired["DeviceDesiredProperty"] = "value";

            var updatedTwin = await registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
            return Ok("Device : " + deviceId + " Desired Property updated");
        }
        catch (Exception ex)
        {
            throw new Exception("couldn't Update Desired Property for Device : " + deviceId + " due to : " + ex);
        }
    }

    [HttpPut]
    [Route("Properties/Telemetric")]    
    public async Task<IActionResult> SendTelemetricMessage(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));

        }
        try
        {
        var device = await Client.GetTwinAsync();
        //var twinProperties = new ReportedProperties()
        TwinCollection reportedProperties = device.Properties.Reported;
            var telemetryDataPoint = new
            {
                temperature = "30",//reportedProperties["temperature"],

                pressure = "20Pa"//reportedProperties["pressure"]

            };
            string messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
            await Client.SendEventAsync(message);
            return Ok("Device App Id: " + deviceId + " Telemetry message sent successfully!!!");

        }
        catch (Exception ex)
        {

            throw new Exception("couldn't Update Reported Property for Device : " + deviceId + " due to : " + ex);

        }
    }
    //  public static async void InitClient()
    // {
    //     try
    //     {
    //         Console.WriteLine("Connecting to hub");
    //        // Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString,TransportType.Mqtt);
    //         Console.WriteLine("Retrieving twin");

    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine();
    //         Console.WriteLine("Error in sample: {0}", ex.Message);
    //     }
    // }

}

