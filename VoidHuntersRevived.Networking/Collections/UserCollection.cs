using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Networking.Implementations;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Collections
{
    public class UserCollection : GameCollection<IUser>
    {
        private Dictionary<Int64, IUser> _userTable;

        public UserCollection()
        {
            _userTable = new Dictionary<Int64, IUser>();
        }

        protected override bool add(IUser item)
        {
            if(base.add(item))
            {
                _userTable.Add(item.Id, item);
                item.OnDisconnect += this.HandleUserDisconnect;

                return true;
            }

            return false;
        }

        protected override bool remove(IUser item)
        {
            if (base.remove(item))
            {
                _userTable.Remove(item.Id);
                item.OnDisconnect -= this.HandleUserDisconnect;

                return true;
            }

            return false;
        }

        public IUser GetById(Int64 id)
        {
            if (_userTable.ContainsKey(id))
                return _userTable[id];

            return default(IUser);
        }

        public IUser RemoveById(Int64 id)
        {
            return this.Remove(this.GetById(id));
        }

        #region Event Handlers
        private void HandleUserDisconnect(object sender, IUser e)
        {
            // Automatically remove the user when they disconnect
            this.Remove(e);
        }
        #endregion
    }
}
