using fNbt;
using MCworldEditor.Services;
using MCworldEditor.Services.Interfaces;
using Moq;

namespace MCworldEditor.Tests
{
    public class ChunkServiceTests
    {
        [Fact]
        public void FindFirstBlockOfChunkByCoords_returnsCoordsOfFirstBlockOnChunk()
        {
            var fileServiceMock = new Mock<IFileService>();
            var service = new ChunkService(fileServiceMock.Object, new PlayerPositionService(new FileService()));
            var result = service.FindFirstBlockOfChunkByCoords(115, 5, -19);
            Assert.Equal(112, result.x);
            Assert.Equal(0, result.y);
            Assert.Equal(-31, result.z);
        }

        [Fact]
        public void FindByteInChunkByCoords_returnCorrectByte()
        {
            var fileServiceMock = new Mock<IFileService>();
            var service = new ChunkService(fileServiceMock.Object, new PlayerPositionService(new FileService()));
            var result = service.FindByteInChunkByCoords(21, 65, -43);
            Assert.Equal(10817, result);
        }

        [Fact]
        public void CheckIfBlock_WhenBlock()
        {
            var fileServiceMock = new Mock<IFileService>();
            string fakePath = "path";
            int worldId = 2, x = 0, y = 3, z = 0;
            var index = 3;
            byte expectedBlock = 1;

            var arr = new byte[Math.Max(1, index + 1)];
            arr[index] = expectedBlock;

            var nbtFile = new NbtFile(new NbtCompound("Root")
            {
                new NbtCompound("Level")
                {
                    new NbtByteArray("Blocks", arr)
                }
            });

            fileServiceMock.Setup(d => d.GetChunkPathByCoordinates(worldId, x, y, z)).Returns(fakePath);
            fileServiceMock.Setup(f => f.ReadDatFile(fakePath)).Returns(nbtFile);

            var chunkService = new ChunkService(fileServiceMock.Object, new PlayerPositionService(new FileService()));
            var result = chunkService.CheckIfBlock(worldId, x, y, z);
            Assert.True(result);
        }

        [Fact]
        public void CheckIfBlock_WhenNotBlock()
        {
            var fileServiceMock = new Mock<IFileService>();
            string fakePath = "path";
            int worldId = 2, x = 0, y = 3, z = 1;
            var index = 131;
            byte expectedBlock = 0;

            var arr = new byte[Math.Max(1, index + 1)];
            arr[index] = expectedBlock;

            var nbtFile = new NbtFile(new NbtCompound("Root")
            {
                new NbtCompound("Level")
                {
                    new NbtByteArray("Blocks", arr)
                }
            });

            fileServiceMock.Setup(d => d.GetChunkPathByCoordinates(worldId, x, y, z)).Returns(fakePath);
            fileServiceMock.Setup(f => f.ReadDatFile(fakePath)).Returns(nbtFile);

            var chunkService = new ChunkService(fileServiceMock.Object, new PlayerPositionService(new FileService()));
            var result = chunkService.CheckIfBlock(worldId, x, y, z);
            Assert.False(result);
        }
    }
}
