namespace VoidHuntersRevived.Common.Entities.Systems
{
    public interface IStepSystem
    {
        void Step(Step step);
    }

    public interface IStepSystem<T1>
        where T1 : unmanaged
    {
        void Step(Step step, in Guid id, ref T1 component1);
    }

    public interface IStepSystem<T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        void Step(Step step, in Guid id, ref T1 component1, ref T2 component2);
    }

    public interface IStepSystem<T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        void Step(Step step, in Guid id, ref T1 component1, ref T2 component2, ref T3 component3);
    }

    public interface IStepSystem<T1, T2, T3, T4>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        void Step(Step step, in Guid id, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);
    }
}
