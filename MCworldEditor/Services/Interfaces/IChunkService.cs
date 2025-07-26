namespace MCworldEditor.Services.Interfaces
{
    public interface IChunkService
    {
        bool CheckIfBlock(int worldId, int x, int y, int z);
        int FindByteInChunkByCoords(int x, int y, int z);
        (int x, int y, int z) FindFirstBlockOfChunkByCoords(int x, int y, int z);
    }
}
