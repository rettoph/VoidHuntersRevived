using Guppy.Common;
using Guppy.Network.Enums;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService
    {
        private abstract class Request : IMessage
        {
            public bool Rejected;

            public abstract Type Type { get; }

            public static class Factory
            {
                private static Dictionary<Type, Func<ParallelKey, IData, Request>> _factories = new();
                private static MethodInfo _method = typeof(Factory).GetMethod(nameof(FactoryMethod), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

                public static Request Create(ParallelKey user, IData data)
                {
                    var type = data.GetType();
                    if (!_factories.TryGetValue(type, out var factory))
                    {
                        var method = _method.MakeGenericMethod(type);
                        factory = (Func<ParallelKey, IData, Request>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                        _factories.Add(type, factory);
                    }

                    return factory(user, data);
                }

                private static Func<ParallelKey, IData, Request> FactoryMethod<TData>()
                    where TData : IData
                {
                    Request Factory(ParallelKey user, IData data)
                    {
                        return new SimulationService.Request<TData>(user, (TData)data);
                    }

                    return Factory;
                }
            }
        }

        private class Request<TData> : Request, IRequest<TData>
            where TData : IData
        {
            public ParallelKey PilotKey { get; }

            public TData Data { get; }

            public override Type Type { get; } = typeof(IRequest<TData>);

            public Request(ParallelKey pilotKey, TData data)
            {
                this.Rejected = false;
                this.PilotKey = pilotKey;
                this.Data = data;
            }

            public void Reject()
            {
                this.Rejected = true;
            }
        }
    }
}
