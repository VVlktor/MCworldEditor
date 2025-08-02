using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class TimeService : ITimeService
    {
        private IFileService _fileService;

        public TimeService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public long ReadTime(int worldId)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            long timeTag = nbtFile.RootTag.Get<NbtCompound>("Data")!.Get<NbtLong>("Time")!.Value;
            return timeTag;
        }

        public string FormatTime(long time, bool isRaw)
        {
            if (isRaw)
                return time.ToString();
                
            var totalTime = TimeSpan.FromSeconds(time / 20);
            return $"{Math.Floor(totalTime.TotalHours)}:{totalTime.Minutes}:{totalTime.Seconds}";
        }

        public int SetTime(int worldId, int tickTime)
        {
            string path = _fileService.GetLevelDatPath(worldId);
            NbtFile nbtFile = _fileService.ReadDatFile(path);
            var dataTag = nbtFile.RootTag.Get<NbtCompound>("Data");
            var xSpawn = dataTag!.Get<NbtLong>("Time");
            long time = xSpawn!.Value % 24000;
            xSpawn.Value -= time;
            xSpawn.Value += tickTime;
            nbtFile.SaveToFile(path, NbtCompression.GZip);
            return 0;
        }
    }
}
