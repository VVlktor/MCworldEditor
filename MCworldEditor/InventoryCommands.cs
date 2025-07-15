namespace MCworldEditor
{
    public class InventoryCommands
    {
        public void AddItemToInventory(int worldNumber, int itemId, int count)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = Path.Combine(appData, ".minecraft", "saves", $"World{worldNumber}");
            Console.WriteLine(path);
            Console.WriteLine($"Dziala: itemid - {itemId}, count - {count}");
        }

        public void ClearInventory(int worldNumber)
        {
            Console.WriteLine(worldNumber);
        }
    }
}
