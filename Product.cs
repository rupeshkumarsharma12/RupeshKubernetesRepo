// C# record type for items in the table

using Azure;
using System;
using Azure.Data.Tables;



public record Product : ITableEntity

{

    public string RowKey { get; set; } = default!;



    public string PartitionKey { get; set; } = default!;



    public string Name { get; init; } = default!;



    public int Quantity { get; init; }



    public bool Sale { get; init; }



    public Azure.ETag ETag { get; set; } = default!;



    public System.DateTimeOffset? Timestamp { get; set; } = default!;

}