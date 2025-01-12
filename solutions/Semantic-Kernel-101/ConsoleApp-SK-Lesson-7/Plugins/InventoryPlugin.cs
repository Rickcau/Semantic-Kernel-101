using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;
using AzureOpenAISearchConfiguration;
using Azure.Identity;
using Azure.AI.OpenAI;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents;
using Runbook.Models;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Azure.Core;
using MyAgent.Models;

namespace Plugins
{
    public class InventoryPlugin
    {
        private readonly Configuration _configuration;
        private List<InventoryItem> _inventory;
        private readonly ILogger<InventoryPlugin> _logger;

        public InventoryPlugin(Configuration configuration, ILogger<InventoryPlugin>? logger = null)
        {
            _configuration = configuration;
            _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<InventoryPlugin>();
            _inventory = new List<InventoryItem>
            {
                new InventoryItem { ItemName = "Blue Jeans", Size = "32", Quantity = 5 },
                new InventoryItem { ItemName = "Red Sneakers", Size = "10", Quantity = 3 },
                new InventoryItem { ItemName = "Black T-Shirt", Size = "Large", Quantity = 8 },
                new InventoryItem { ItemName = "White Hoodie", Size = "Medium", Quantity = 2 },
                new InventoryItem { ItemName = "Baseball Cap", Color = "Black", Quantity = 10 },
                new InventoryItem { ItemName = "Brown Leather Jacket", Size = "Medium", Quantity = 1 },
                new InventoryItem { ItemName = "Running Shorts", Size = "Small", Quantity = 7 },
                new InventoryItem { ItemName = "Sunglasses", Type = "Aviator", Quantity = 4 },
                new InventoryItem { ItemName = "Backpack", Color = "Gray", Quantity = 6 },
                new InventoryItem { ItemName = "Wristwatch", Type = "Digital", Quantity = 9 }
            };
            _logger.LogDebug("Constructor for the InventoryPlugin as been called!");
        }

        [KernelFunction, Description("Check inventory stork")]
        public async Task<string> CheckInventory(
            [Description("The ItemName of the item to check in our inventory List")]
            string searchItemName
            )
        {
            _logger.LogDebug("InventoryPlugin::CheckInventory Called with '{searchItemName}'",searchItemName);
            StringBuilder content = new StringBuilder();
            try
            {

                // Perform the search directly
                InventoryItem? foundItem = _inventory.FirstOrDefault(item =>
                    item.ItemName != null && item.ItemName.Equals(searchItemName, StringComparison.OrdinalIgnoreCase));

                if (foundItem != null)
                {
                    content.AppendLine($"Item Found: {foundItem.ItemName}, Size: {foundItem.Size ?? "N/A"}, Color: {foundItem.Color ?? "N/A"}, Type: {foundItem.Type ?? "N/A"}, Quantity: {foundItem.Quantity}");
                }
                else
                {
                    content.AppendLine($"Item '{searchItemName}' not found in the inventory.");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error searching for item: {ex.Message}";
                content.Append(errorMessage);
                _logger.LogError(ex, "Unexpected error searching for item: {ErrorMessage}", errorMessage);
            }
            await Task.Delay(10);
            return content.ToString();
        }

        [KernelFunction, Description("List the items we have in inventory")]
        public async Task<string> ListInventory()
        {
            _logger.LogDebug("InventoryPlugin::ListInventory Called");
            StringBuilder content = new StringBuilder();
            try
            {
                foreach (var item in _inventory)
                {
                    content.AppendLine($"Item: {item.ItemName}, Size: {item.Size ?? "N / A"}, Color: {item.Color ?? "N / A"}, Type: {item.Type ?? "N / A"}, Quantity: {item.Quantity}");
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error listing inventory: {ex.Message}";
                content.Append(errorMessage);
                _logger.LogError(ex, "Unexpected error listing inventory items: {ErrorMessage}", errorMessage);
            }
            await Task.Delay(10);
            return content.ToString();
        }

        [KernelFunction, Description("Update quantity for an inventory item")]
        public async Task<string> UpdateInventory(
            [Description("The ItemName of the item to update in our inventory list")]
            string itemNameToUpdate,
                    [Description("The new quantity to set for the item")]
            int newQuantity)
        {
            _logger.LogDebug("InventoryPlugin::UpdateInventory Called with '{itemNameToUpdate}' update quanity to: '{newQuantity}'", itemNameToUpdate, newQuantity);
            StringBuilder content = new StringBuilder();
            try
            {
                // Find the inventory item by name
                var itemToUpdate = _inventory.FirstOrDefault(item =>
                    item.ItemName != null && item.ItemName.Equals(itemNameToUpdate, StringComparison.OrdinalIgnoreCase));

                if (itemToUpdate != null)
                {
                    // Update the item's quantity
                    itemToUpdate.Quantity = newQuantity;

                    // Log and return confirmation
                    content.AppendLine($"Updated Item: {itemToUpdate.ItemName}, New Quantity: {itemToUpdate.Quantity}");
                    _logger.LogInformation("Inventory updated for item: {ItemName}, New Quantity: {Quantity}", itemToUpdate.ItemName, newQuantity);
                }
                else
                {
                    // Item not found
                    content.AppendLine($"Item '{itemNameToUpdate}' not found in the inventory.");
                    _logger.LogWarning("Attempted to update non-existent item: {ItemName}", itemNameToUpdate);
                }
            }
            catch (Exception ex)
            {
                // Handle and log any exceptions
                var errorMessage = $"Error updating inventory: {ex.Message}";
                content.Append(errorMessage);
                _logger.LogError(ex, "Unexpected error updating inventory items: {ErrorMessage}", errorMessage);
            }

            await Task.Delay(10); // Simulate asynchronous operation
            return content.ToString();
        }

        [KernelFunction, Description("Reduce quantity for an inventory item after a purchase")]
        public async Task<string> ReduceInventory(
        [Description("The ItemName of the item to reduce in our inventory list")]
        string itemNameToReduce,
        [Description("The quantity to reduce from the item")]
        int quantityToReduce)
            {
            _logger.LogDebug("InventoryPlugin::ReduceInventory Called with '{itemNameToReduce}' quanity to reduce by: '{quantityToReduce}'", itemNameToReduce, quantityToReduce);
            StringBuilder content = new StringBuilder();
                try
                {
                    // Find the inventory item by name
                    var itemToReduce = _inventory.FirstOrDefault(item =>
                        item.ItemName != null && item.ItemName.Equals(itemNameToReduce, StringComparison.OrdinalIgnoreCase));

                    if (itemToReduce != null)
                    {
                        // Check if there's enough inventory to reduce
                        if (itemToReduce.Quantity >= quantityToReduce)
                        {
                            // Reduce the item's quantity
                            itemToReduce.Quantity -= quantityToReduce;

                            // Log and return confirmation
                            content.AppendLine($"Reduced Item: {itemToReduce.ItemName}, Quantity Reduced: {quantityToReduce}, Remaining Quantity: {itemToReduce.Quantity}");
                            _logger.LogInformation("Inventory reduced for item: {ItemName}, Quantity Reduced: {QuantityReduced}, Remaining Quantity: {RemainingQuantity}", itemToReduce.ItemName, quantityToReduce, itemToReduce.Quantity);
                        }
                        else
                        {
                            // Not enough stock available
                            content.AppendLine($"Not enough stock for item '{itemNameToReduce}'. Available Quantity: {itemToReduce.Quantity}, Requested Reduction: {quantityToReduce}.");
                            _logger.LogWarning("Attempted to reduce inventory for item: {ItemName}, but insufficient stock. Available: {AvailableQuantity}, Requested: {RequestedQuantity}", itemToReduce.ItemName, itemToReduce.Quantity, quantityToReduce);
                        }
                    }
                    else
                    {
                        // Item not found
                        content.AppendLine($"Item '{itemNameToReduce}' not found in the inventory.");
                        _logger.LogWarning("Attempted to reduce inventory for non-existent item: {ItemName}", itemNameToReduce);
                    }
                }
                catch (Exception ex)
                {
                    // Handle and log any exceptions
                    var errorMessage = $"Error reducing inventory: {ex.Message}";
                    content.Append(errorMessage);
                    _logger.LogError(ex, "Unexpected error reducing inventory items: {ErrorMessage}", errorMessage);
                }

                await Task.Delay(10); // Simulate asynchronous operation
                return content.ToString();
        }
    }
}
