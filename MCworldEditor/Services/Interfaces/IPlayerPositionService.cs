namespace MCworldEditor.Services.Interfaces
{
    public interface IPlayerPositionService
    {
        (int X, int Y, int Z) GetPlayerPositionInt(int worldId);
        (double X, double Y, double Z) GetPlayerPositionDouble(int worldId);
        (int x, int y, int z) GetValidCoords(int worldId, int? x, int? z);
        int SetPlayerPosition(int worldId, int x, int y, int z);
    }
}
