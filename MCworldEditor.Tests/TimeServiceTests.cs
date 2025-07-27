using fNbt;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;
using Moq;

namespace MCworldEditor.Tests
{
    public class TimeServiceTests
    {
        [Fact]
        public void ReadTime_returnCorrectTimeValue()
        {
            int worldId = 2;
            string expectedPath = "somepath";
            long expectedTime = 1234;

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(d => d.GetLevelDatPath(worldId)).Returns(expectedPath);

            var nbtFile = new NbtFile();
            var timeTag = new NbtLong("Time", expectedTime);
            var dataCompound = new NbtCompound("Data") { timeTag };
            var root = new NbtCompound("") { dataCompound };
            nbtFile.RootTag = root;

            fileServiceMock.Setup(d => d.ReadDatFile(expectedPath)).Returns(nbtFile);

            var service = new TimeService(fileServiceMock.Object);
            var result = service.ReadTime(worldId);

            Assert.Equal(expectedTime, result);
        }

        [Fact]
        public void FormatTime_returnCorrectFormat()
        {
            var fileServiceMock = new Mock<IFileService>();
            var service = new TimeService(fileServiceMock.Object);

            long timeValue = 34256;
            bool isRaw = false;

            var result = service.FormatTime(timeValue, isRaw);
            var expectedFormat = "0:28:32";

            Assert.Equal(expectedFormat, result);
        }
    }
}
