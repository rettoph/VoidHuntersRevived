namespace VoidHuntersRevived.Domain.Graphics.Common.Exceptions
{
    public class DuplicatePrimitiveVertexTypeException : Exception
    {
        public readonly Type VertexType;
        public readonly IPrimitive[] Primitives;

        public DuplicatePrimitiveVertexTypeException(Type vertexType, IPrimitive[] duplicates) : base(GetMessage(vertexType, duplicates))
        {
            this.VertexType = vertexType;
            this.Primitives = duplicates;
        }

        private static string GetMessage(Type vertexType, IPrimitive[] duplicates)
        {
            return $"Duplicate Primitive VertexType - {vertexType.Name}. Found {duplicates.Length} primitives with duplicate vertex types.";
        }
    }
}
