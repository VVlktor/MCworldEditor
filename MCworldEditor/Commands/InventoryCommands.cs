using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.CommandsToCall
{
    public class InventoryCommands
    {
        private IInventoryService _inventoryService;

        public InventoryCommands(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public int AddItemToInventory(int worldNumber, int itemId, int count)
        {
            int response = _inventoryService.AddItemToInventory(worldNumber, itemId, count);
            return response;
        }

        public int ClearInventory(int worldNumber)
        {
            int response = _inventoryService.CleanInventory(worldNumber);
            return response;
        }

        public int ReadInventory(int worldId)
        {
            var inventoryData = _inventoryService.ReadItemsInInventory(worldId);
            foreach (var item in inventoryData)
                Console.WriteLine($"Id: {item.id}, Count: {item.count}");
            return 0;
        }
    }
}
