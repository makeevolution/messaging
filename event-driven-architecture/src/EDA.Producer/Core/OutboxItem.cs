using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace EDA.Producer.Core;

[PrimaryKey("Id")]
public class OutboxItem
{
    public int Id { get; set; }
    public DateTime EventTime { get; set; }
    public bool Processed { get; set; }
    public string EventData { get; set; }
    
    public string EventType { get; set; }
}