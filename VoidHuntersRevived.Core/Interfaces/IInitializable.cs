using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Enums;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IInitializable
    {
        InitializationState InitializationState { get; }

        event EventHandler<IInitializable> OnBoot;
        event EventHandler<IInitializable> OnPreInitialize;
        event EventHandler<IInitializable> OnInitialize;
        event EventHandler<IInitializable> OnPostInitialize;

        void TryBoot();
        void TryPreInitialize();
        void TryInitialize();
        void TryPostInitialize();
    }
}
