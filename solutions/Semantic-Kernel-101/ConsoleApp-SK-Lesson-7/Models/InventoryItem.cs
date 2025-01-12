using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAgent.Models
{
public class InventoryItem
{
    public string ItemName { get; set; }
    public string? Size { get; set; } // Nullable for items without size
    public string? Color { get; set; } // Nullable for items without color
    public string? Type { get; set; } // Nullable for specific types
    public int Quantity { get; set; }
}
}
