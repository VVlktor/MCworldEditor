using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCworldEditor.Services.Interfaces
{
    public interface ITimeService
    {
        long ReadTime(int worldId);
        string FormatTime(long time, bool isRaw);
    }
}
