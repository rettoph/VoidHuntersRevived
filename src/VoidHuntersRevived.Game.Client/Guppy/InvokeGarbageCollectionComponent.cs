using Guppy;
using Guppy.Attributes;
using Guppy.Common;
using Guppy.Messaging;
using Guppy.MonoGame;
using Microsoft.Xna.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Client.Engines;
using VoidHuntersRevived.Game.Client.Messages;

namespace VoidHuntersRevived.Game.Client.GameComponents
{
    [AutoLoad]
    internal class InvokeGarbageCollectionComponent : IGuppyComponent,
        ISubscriber<Input_Invoke_Garbage_Collection>
    {
        private ILogger _logger;
        private DateTime _lastInvocation;
        private DateTime _lastWarning;

        public InvokeGarbageCollectionComponent(ILogger logger)
        {
            _logger = logger;
        }

        public void Initialize(IGuppy guppy)
        {
            //
        }

        public void Process(in Guid messageId, Input_Invoke_Garbage_Collection message)
        {
            if(DateTime.Now - _lastInvocation < TimeSpan.FromMilliseconds(100))
            {
                return;
            }

            if (DateTime.Now - _lastInvocation < TimeSpan.FromSeconds(5))
            {
                if(DateTime.Now - _lastWarning < TimeSpan.FromMilliseconds(100))
                {
                    _lastWarning = DateTime.Now;
                    return;
                }
                
                _logger.Warning("{ClassName}::{MethodName} - Too soon, try again later.", nameof(InvokeGarbageCollectionComponent), nameof(Process));

                return;
            }

            long preAllocatedBytes = GC.GetTotalMemory(true);
            _logger.Debug("{ClassName}::{MethodName} - Invoking garbage collection.", nameof(InvokeGarbageCollectionComponent), nameof(Process));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long postAllocatedBytes = GC.GetTotalMemory(true);

            _logger.Debug("{ClassName}::{MethodName} - Done. Cleared {Memory}", nameof(InvokeGarbageCollectionComponent), nameof(Process), BytesToString(preAllocatedBytes - postAllocatedBytes));

            _lastInvocation = DateTime.Now;
        }

        static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
