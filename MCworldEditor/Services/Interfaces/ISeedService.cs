namespace MCworldEditor.Services.Interfaces
{
    public interface ISeedService
    {
        long ReadSeed(int worldId);
        void SetSeed(int worldId, long seed);
    }
}
