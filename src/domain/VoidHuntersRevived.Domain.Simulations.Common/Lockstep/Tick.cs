using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Lockstep
{
    public sealed class Tick
    {
        public readonly int Id;
        public readonly EnqueuedStepInput[] Inputs;
        public readonly VhId Hash;

        private Tick(int id, EnqueuedStepInput[] inputs)
        {
            this.Id = id;
            this.Inputs = inputs;
            this.Hash = HashBuilder<Tick, int>.Instance.Calculate(id);

            foreach (EnqueuedStepInput input in inputs)
            {
                this.Hash = this.Hash.Create(input.Id.Value);
            }
        }

        public override string ToString()
        {
            return $"Id = {this.Id}, Events: {this.Inputs.Length}, Hash = {this.Hash}";
        }

        public Tick Next(EnqueuedStepInput[] inputs)
        {
            return new(this.Id + 1, inputs);
        }

        public static Tick First(EnqueuedStepInput[] inputs)
        {
            return new(0, inputs);
        }

        public static Tick Empty(int id)
        {
            return new(id, []);
        }

        public static Tick Create(int id, EnqueuedStepInput[] inputs)
        {
            return new(id, inputs);
        }
    }
}