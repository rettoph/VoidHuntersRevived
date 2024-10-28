using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Registration.Simulations.Extensions
{
    public static class ISimulationExtensions
    {
        public static void Run(this ISimulation simulation, Func<IStrategy, IEnumerator<int>> coroutine)
        {
            var coroutines = simulation.Strategies.Select(x => coroutine(x)).ToArray();

            bool running = false;
            do
            {
                running = false;
                for (int i = 0; i < coroutines.Length; i++)
                {
                    running |= coroutines[i].MoveNext();
                }
            } while (running == true);

            for (int i = 0; i < coroutines.Length; i++)
            {
                coroutines[i].Dispose();
            }
        }
    }
}
