using MCworldEditor.Services;

namespace MCworldEditor.Tests
{
    public class FileServiceTests
    {
        [Fact]
        public void GetLevelDatPath_ReturnsCorrectPath()
        {
            var fileService = new FileService();
            int worldId = 3;
            string expectedAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string expectedPath = Path.Combine(expectedAppData, ".minecraft", "saves", "World3", "level.dat");
            string actualPath = fileService.GetLevelDatPath(worldId);

            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void GetChunkPathByCoordinates_ReturnsCorrectPath()
        {
            var fileService = new FileService();
            int worldNumber = 2, x = -54, y = 34, z = 12;
            string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves", $"World2", "1o", "0", "c.-4.0.dat");
            string result = fileService.GetChunkPathByCoordinates(worldNumber, x, y, z);

            Assert.Equal(expectedPath, result);
        }
    }
}
