using fNbt;

namespace MCworldEditor
{
    public class InventoryCommands
    {
        public int AddItemToInventory(int worldNumber, int itemId, int count)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = Path.Combine(appData, ".minecraft", "saves", $"World{worldNumber}");
            Console.WriteLine(path);
            Console.WriteLine($"Dziala: itemid - {itemId}, count - {count}");
            return 0;
        }

        public int ClearInventory(int worldNumber)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World{worldNumber}", "level.dat");

            NbtFile nbtFile = new NbtFile();

            using (FileStream stream = File.OpenRead(path))
            {
                nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);
            }

            var allTags = nbtFile.RootTag.Names.ToList();
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var playerTag = dataTag!.Get<NbtCompound>("Player");
            var inventoryData = playerTag!.Get<NbtList>("Inventory");

            inventoryData!.Clear();
            nbtFile.SaveToFile(path, NbtCompression.GZip);

            return 0;
        }
    }
}
