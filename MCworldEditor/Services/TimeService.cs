using fNbt;
using MCworldEditor.Services.Interfaces;

namespace MCworldEditor.Services
{
    public class TimeService
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
    }
}
