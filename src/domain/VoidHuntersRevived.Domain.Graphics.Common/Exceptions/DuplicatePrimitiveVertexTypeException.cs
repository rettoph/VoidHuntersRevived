namespace VoidHuntersRevived.Domain.Graphics.Common.Exceptions
{
    public class DuplicatePrimitiveVertexTypeException(Type vertexType, IPrimitive[] duplicates) : Exception(GetMessage(vertexType, duplicates))
    {
        public readonly Type VertexType = vertexType;
        public readonly IPrimitive[] Primitives = duplicates;

        private static string GetMessage(Type vertexType, IPrimitive[] duplicates)
        {
            return $"Duplicate Primitive VertexType - {vertexType.Name}. Found {duplicates.Length} primitives with duplicate vertex types.";
        }
    }
}
