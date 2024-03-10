namespace VoidHuntersRevived.Common.Core
{
    public static class NameSpace<T>
    {
        private static readonly VhId _nameSpace = new VhId("51BEE28A-C264-417E-B814-A8826570B825");
        public static readonly VhId Instance = _nameSpace.Create(typeof(T).AssemblyQualifiedName ?? throw new Exception());
    }
}
