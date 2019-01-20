using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    public class GameCollection<TObject> : IGameCollection<TObject>
    {
        protected List<TObject> _list;

        public event EventHandler<IGameCollection<TObject>> OnAdd;
        public event EventHandler<IGameCollection<TObject>> OnRemove;

        #region Methods
        protected virtual Boolean add(TObject item)
        {
            _list.Add(item);
            return true;
        }
        protected virtual Boolean remove(TObject item) {
            return _list.Remove(item);
        }
        #endregion

        #region IGameCollection Implementation
        public virtual Boolean CanAlter()
        {
            return true;
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
            return _list.GetEnumerator();
        }
        #endregion
    }
}
