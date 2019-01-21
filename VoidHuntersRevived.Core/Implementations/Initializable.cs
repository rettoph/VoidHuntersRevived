using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Enums;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class Initializable : IInitializable
    {
        private ILogger _logger;
        public InitializationState InitializationState { get; private set; }

        public event EventHandler<IInitializable> OnBoot;
        public event EventHandler<IInitializable> OnPreInitialize;
        public event EventHandler<IInitializable> OnInitialize;
        public event EventHandler<IInitializable> OnPostInitialize;
        

        public Initializable(ILogger logger)
        {
            _logger = logger;
            InitializationState = InitializationState.UnInitialized;
        }

        public void TryBoot()
        {
            if (InitializationState >= InitializationState.Booting)
                throw new Exception("Unable to call Initializable.Boot() after a greater or equal initialization method has been called.");
            else
            {
                InitializationState = InitializationState.Booting;
                _logger.LogDebug("Calling Initializable.Boot()");
                this.Boot();
                this.OnBoot?.Invoke(this, this);
            }
        }

        public void TryPreInitialize()
        {
            if (InitializationState >= InitializationState.Preinitializing)
                _logger.LogError("Unable to call Initializable.PreInitialize() after a greater or equal initialization method has been called.");
            else
            {
                InitializationState = InitializationState.Preinitializing;
                _logger.LogDebug("Calling Initializable.PreInitialize()");
                this.PreInitialize();
                this.OnPreInitialize?.Invoke(this, this);
            }
        }

        public void TryInitialize()
        {
            if (InitializationState >= InitializationState.Initializing)
                _logger.LogError("Unable to call Initializable.Initialize() after a greater or equal initialization method has been called.");
            else
            {
                InitializationState = InitializationState.Initializing;
                _logger.LogDebug("Calling Initializable.Initialize()");
                this.Initialize();
                this.OnInitialize?.Invoke(this, this);
            }
        }

        public void TryPostInitialize()
        {
            if (InitializationState >= InitializationState.PostInitializing)
                _logger.LogError("Unable to call Initializable.PostInitialize() after a greater or equal initialization method has been called.");
            else
            {
                InitializationState = InitializationState.PostInitializing;
                _logger.LogDebug("Calling Initializable.PostInitialize()");
                this.PostInitialize();
                this.OnPostInitialize?.Invoke(this, this);
            }
        }

        protected abstract void Boot();
        protected abstract void PreInitialize();
        protected abstract void Initialize();
        protected abstract void PostInitialize();
    }
}
