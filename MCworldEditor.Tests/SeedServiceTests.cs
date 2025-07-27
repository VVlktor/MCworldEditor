using fNbt;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;
using Moq;

namespace MCworldEditor.Tests
{
    public class SeedServiceTests
    {
        [Fact]
        public void ReadSeed_ReturnsCorrectSeed()
        {
            var worldId = 1;
            var expectedPath = "path";
            long expectedSeed = 123456789;

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock.Setup(f => f.GetLevelDatPath(worldId)).Returns(expectedPath);

            var root = new NbtCompound("root", [ new NbtCompound("Data", [ new NbtLong("RandomSeed", expectedSeed) ]) ]);
            var nbtFile = new NbtFile(root);
            fileServiceMock.Setup(f => f.ReadDatFile(expectedPath)).Returns(nbtFile);

            var worldService = new SeedService(fileServiceMock.Object);
            long actualSeed = worldService.ReadSeed(worldId);

            Assert.Equal(expectedSeed, actualSeed);
        }
    }
}
