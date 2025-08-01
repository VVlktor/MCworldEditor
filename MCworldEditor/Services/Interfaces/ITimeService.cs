﻿namespace MCworldEditor.Services.Interfaces
{
    public interface ITimeService
    {
        long ReadTime(int worldId);
        string FormatTime(long time, bool isRaw);
        int SetTime(int worldId, int tickTime);
    }
}
