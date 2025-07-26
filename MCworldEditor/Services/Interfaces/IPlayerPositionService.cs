using fNbt;

namespace MCworldEditor.Services.Interfaces
{
    public interface IPlayerPositionService
    {
        (int X, int Y, int Z) GetPlayerPositionInt(int worldId);
        (double X, double Y, double Z) GetPlayerPositionDouble(int worldId);
    }
}
