namespace MCworldEditor.Services.Interfaces
{
    public interface IInventoryService
    {
        int CleanInventory(int worldId);
        int AddItemToInventory(int worldId, int itemId, int count);
        List<(int count, short id)> ReadItemsInInventory(int worldId);
    }
}
