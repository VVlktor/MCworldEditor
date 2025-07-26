using fNbt;

namespace MCworldEditor.Services.Interfaces
{
    public interface IFileService
    {
        NbtFile ReadDatFile(string path);
        string GetLevelDatPath(int worldId);
        string GetChunkPathByCoordinates(int worldNumber, int x, int y, int z);
    }
}
