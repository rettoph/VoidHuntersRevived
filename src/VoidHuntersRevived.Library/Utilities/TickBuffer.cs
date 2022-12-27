using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class TickBuffer
    {
        [DebuggerDisplay("Id: {Data.Id}")]
        private sealed class Node
        {
            public int Id => this.Data.Id;
            public int ParentId => this.Id - 1;
            public int ChildId => this.Id + 1;

            public Tick Data { get; private set; }
            public Node? Child { get; private set; }

            public Node(Tick data)
            {
                this.Data = data;
            }

            public void Add(Node child)
            {
                Node head = this;

                while (head.TryAdd(child) == false)
                {
                    if (head.Child is null)
                    {
                        break;
                    }

                    head = head.Child;
                }
            }

            public Node GetTail()
            {
                if(this.Child is null)
                {
                    return this;
                }

                Node parent = this;
                Node tick = this.Child;

                while (parent.ChildId == tick.Id)
                {
                    if (tick.Child is null)
                    {
                        return tick;
                    }

                    parent = tick;
                    tick = tick.Child;
                }

                return parent ?? tick;
            }

            private bool TryAdd(Node child)
            {
                if (this.Child is null)
                {
                    this.Child = child;
                    return true;
                }

                if (child.Data.Id == this.Data.Id)
                {
                    this.Data = child.Data;
                    return true;
                }

                if (child.Data.Id < this.Child.Data.Id)
                {
                    var old = this.Child;
                    this.Child = child;
                    this.Child.Child = old;
                    return true;
                }

                return false;
            }
        }

        private Node? _head;
        private Node? _tail;

        public Tick? Head => _head?.Data;
        public Tick? Tail => _tail?.Data;
        public Tick? Latest { get; private set; }

        public Tick? this[int index]
        {
            get
            {
                Node? result = _head;

                for(int i=0; i<index; i++)
                {
                    if (result is null)
                    {
                        break;
                    }

                    result = result.Child;
                }

                return result?.Data;
            }
        }

        public TickBuffer()
        {
        }

        public bool TryPop(int id, [MaybeNullWhen(false)] out Tick tick)
        {
            if(_head is null)
            {
                tick = null;
                return false;
            }

            if (_head.Id == id)
            {
                tick = _head.Data;
                _head = _head.Child;

                if(_head is null)
                {
                    _tail = null;
                }

                return true;
            }

            if(_head.Id < id)
            { // Sometimes we double send a message, this should fix that.
                _head = _head.Child; 

                return this.TryPop(id, out tick);
            }

            tick = null;
            return false;
        }

        public void Enqueue(Tick tick)
        {
            var node = new Node(tick);
            if (_head is null)
            {
                _head = node;
                this.UpdateTail();
                return;
            }

            if(tick.Id < _head.Id)
            {
                var old = _head;
                _head = node;
                _head.Add(old);
                this.UpdateTail();
                return;
            }

            _head.Add(node);
            this.UpdateTail();
            this.Latest = tick;
        }

        private void UpdateTail()
        {
            if(_tail is null || _tail.Id < _head?.Id)
            {
                _tail = _head;
            }

            _tail = _tail?.GetTail();
        }
    }
}
