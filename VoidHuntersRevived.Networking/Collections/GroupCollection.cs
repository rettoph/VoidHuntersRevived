using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Collections;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Collections
{
    public class GroupCollection : GameCollection<IGroup>
    {
        private Dictionary<Int64, IGroup> _groupTable;
        private IPeer _peer;
        private Type _groupType;

        public GroupCollection(IPeer peer, Type groupType)
        {
            _groupTable = new Dictionary<Int64, IGroup>();
            _peer = peer;
            _groupType = groupType;
        }

        protected override bool add(IGroup item)
        {
            if (base.add(item))
            {
                _groupTable.Add(item.Id, item);

                return true;
            }

            return false;
        }

        protected override bool remove(IGroup item)
        {
            if (base.remove(item))
            {
                _groupTable.Remove(item.Id);

                return true;
            }

            return false;
        }

        public IGroup GetById(Int64 id)
        {
            if (!_groupTable.ContainsKey(id))
            { // If that particular group doesnt exist, we can just create it now
                this.Add(
                    (IGroup)Activator.CreateInstance(
                        _groupType,
                        id,
                        _peer));
            }

            // Return the coresponding group as is   
            return _groupTable[id];
        }
    }
}
