namespace VoidHuntersRevived.Domain.Common.Providers
{
    public interface IUniqueNumberProvider
    {
        uint GetUInt32();

        int GetInt32();
    }
}