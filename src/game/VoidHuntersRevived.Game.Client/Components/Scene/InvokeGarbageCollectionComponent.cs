using Guppy.Core.Messaging.Common;
using Guppy.Game.Common.Components;
using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Game.Client.Messages;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    internal class InvokeGarbageCollectionComponent(ILogger logger) : ISceneComponent,
        ISubscriber<Input_Invoke_Garbage_Collection>
    {
        private readonly ILogger _logger = logger;
        private DateTime _lastInvocation;
        private DateTime _lastWarning;

        public void Process(in Guid messageId, Input_Invoke_Garbage_Collection message)
        {
            if (DateTime.Now - this._lastInvocation < TimeSpan.FromMilliseconds(100))
            {
                return;
            }

            if (DateTime.Now - this._lastInvocation < TimeSpan.FromSeconds(5))
            {
                if (DateTime.Now - this._lastWarning < TimeSpan.FromMilliseconds(100))
                {
                    this._lastWarning = DateTime.Now;
                    return;
                }

                this._logger.Warning("Too soon, try again later.");

                return;
            }

            long preAllocatedBytes = GC.GetTotalMemory(true);
            this._logger.Debug("Invoking garbage collection.");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long postAllocatedBytes = GC.GetTotalMemory(true);

            this._logger.Debug("Done. Cleared {Memory}", BytesToString(preAllocatedBytes - postAllocatedBytes));

            this._lastInvocation = DateTime.Now;
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = ["B", "KB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}