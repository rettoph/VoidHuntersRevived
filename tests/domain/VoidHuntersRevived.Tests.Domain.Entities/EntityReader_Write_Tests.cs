using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityReader_Write_Tests
    {
        [Fact]
        public void EntityReader_GenericWrite_Byte()
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