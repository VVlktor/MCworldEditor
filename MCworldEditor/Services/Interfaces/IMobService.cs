namespace MCworldEditor.Services.Interfaces
{
    public interface IMobService
    {
        int SpawnMob(int worldOption, string mobId, int? x, int? y, int? z, int? hp, int? count);
        int RemoveMobs(int worldOption, bool passive, bool hostile, int? area);
    }
}
