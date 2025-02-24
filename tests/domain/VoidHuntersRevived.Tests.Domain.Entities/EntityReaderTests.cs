using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityReaderTests
    {
        [Fact]
        public void GenericReadByte_MatchesInput()
        {
            byte[] input = [1, 0, 2, 3, 255, 69, 100];
            EntityReader reader = new(VhId.Empty, input, 0);

            for (int i = 0; i < input.Length; i++)
            {
                Assert.True(reader.DataAvailable);
                Assert.Equal(reader.Length, input.Length);
                Assert.Equal(reader.Position, i);

                byte result = reader.Read<byte>();
                Assert.Equal(result, input[i]);
            }

            Assert.False(reader.DataAvailable);
            Assert.Equal(reader.Length, reader.Position);
        }

        [Fact]
        public void GenericReadInt32_MatchesInput()
        {
            int[] input = [0, 1, 2, 10, 50, 69, 420, int.MaxValue, int.MinValue];

            EntityReader reader = new(VhId.Empty, input.ToByteArray(BitConverter.GetBytes), 0);

            for (int i = 0; i < input.Length; i++)
            {
                Assert.True(reader.DataAvailable);
                Assert.Equal(reader.Length, input.Length * sizeof(int));
                Assert.Equal(reader.Position, i * sizeof(int));

                int result = reader.Read<int>();
                Assert.Equal(result, input[i]);
            }

            Assert.False(reader.DataAvailable);
            Assert.Equal(reader.Length, reader.Position);
        }

        [Fact]
        public void GenericWriteByte_MatchesInput()
        {
            List<byte> output = [];
            byte[] input = [1, 0, 2, 3, 255, 69, 100];
            EntityWriter writer = new(output, []);

            for (int i = 0; i < input.Length; i++)
            {
                writer.Write(input[i]);
            }


            Assert.Equal(output.Count, input.Length);

            for (int i = 0; i < output.Count; i++)
            {
                Assert.Equal(output[i], input[i]);
            }
        }
    }
}