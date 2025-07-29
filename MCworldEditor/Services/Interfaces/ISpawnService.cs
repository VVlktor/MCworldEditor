namespace MCworldEditor.Services.Interfaces
{
    public interface ISpawnService
    {
        int SetSpawnPoint(int worldId, int x, int y, int z, bool safe);
        (int x, int y, int z) ReadSpawnPoint(int worldId);
    }
}
