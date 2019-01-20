using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Enums;

namespace VoidHuntersRevived.Core.Collections
{
    public class InitializableGameCollection<TObject> : Initializable, IGameCollection<TObject>
    {
        private ILogger _logger;
        private Boolean _cemented;
        protected List<TObject> _list;
        protected TObject[] _array;

        public event EventHandler<IGameCollection<TObject>> OnAdd;
        public event EventHandler<IGameCollection<TObject>> OnRemove;

        public InitializableGameCollection(ILogger logger) : base(logger)
        {
            _logger = logger;
            _cemented = false;
            _list = new List<TObject>();
        }

        #region Initializable methods
        protected override void Boot()
        {
            // On boot we can convert the list into an array
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            // throw new NotImplementedException();
        }
        #endregion

        #region Methods
        protected virtual Boolean add(TObject item)
        {
            if (item == null)
                return false;

            _list.Add(item);
            return true;
        }
        protected virtual Boolean remove(TObject item) {
            if (item == null)
                return false;

            return _list.Remove(item);
        }

        public void Cement()
        {
            if (!_cemented)
            {
                _array = _list.ToArray();

                _list.Clear();
                _list = null;

                _cemented = true;
            }
        }
        #endregion

        #region IGameCollection Implementation
        public virtual Boolean CanAlter()
        {
            if (this.InitializationState >= InitializationState.Booting)
                _logger.LogError("Unable to alter InitializableGameCollection after Initializable.Initialize has been called.");
            else
                return true;

            return false;
        }

        public TObject Add(TObject item)
        {
            if(this.CanAlter())
            {
                this.add(item);
                this.OnAdd?.Invoke(this, this);
            }

            return item;
        }

        public TObject Remove(TObject item)
        {
            if (this.CanAlter())
            {
                this.remove(item);
                this.OnRemove?.Invoke(this, this);
            }

            return item;
        }

        public List<TObject> ToList()
        {
            return _list;
        }
        #endregion


        #region IEnumerable Implementation
        public virtual IEnumerator GetEnumerator()
        {
            if (_cemented)
                return _array.GetEnumerator();
            else
                return _list.GetEnumerator();
        }
        #endregion
    }
}
