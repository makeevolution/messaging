using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Anko.OrdersService.Infrastructure;

[PrimaryKey("Id")]
public class OutboxItem
{
    public OutboxItem()
    {
        EventTime = DateTime.UtcNow;
        TraceParent = Activity.Current?.Id;  // .NET activity ID nicely has same format as traceparent e.g.
                                             // 00-19e89a2e7b4096ece7ddcd78821a0397-518c033d28cb90a0-01
    }
    public int Id { get; set; }
    public DateTime EventTime { get; set; }
    public bool Processed { get; set; }
    public string EventData { get; set; }
    
    public string EventType { get; set; }
    public string TraceParent { get; set; }
}