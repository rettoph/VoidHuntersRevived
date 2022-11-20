using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class TickBuffer
    {
        [DebuggerDisplay("Id: {Data.Id}")]
        private sealed class Node
        {
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

                while (tick.IsReady(parent.Data.Id))
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

            public bool IsReady(int parentId)
            {
                if (parentId == this.Data.Id - 1)
                {
                    return true;
                }

                return false;
            }

            private bool TryAdd(Node child)
            {
                if (this.Child is null)
                {
                    this.Child = child;
                    return true;
                }

                if (child.Data.Id == this.Child.Data.Id)
                {
                    this.Child.Data = child.Data;
                    return true;
                }

                if (child.Data.Id == this.Data.Id + 1)
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

        private int _currentId;

        public Tick? Head => _head?.Data;
        public Tick? Tail => _tail?.Data;

        public int CurrentId
        {
            get => _currentId;
            set => _currentId = value;
        }

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

        public TickBuffer(int currentId)
        {
            _currentId = currentId;
        }

        public bool TryPop([MaybeNullWhen(false)] out Tick tick)
        {
            if(_head is null)
            {
                tick = null;
                return false;
            }

            if (_head.IsReady(_currentId))
            {
                tick = _head.Data;
                _currentId = _head.Data.Id;
                _head = _head.Child;

                if(_head is null)
                {
                    _tail = null;
                }

                return true;
            }

            tick = null;
            return false;
        }

        public void Enqueue(Tick tick)
        {
            if(_head is null)
            {
                _head = new Node(tick);
                this.UpdateTail();
                return;
            }

            if(tick.Id < _head.Data.Id)
            {
                var old = _head;
                _head = new Node(tick);
                _head.Add(old);
                this.UpdateTail();
                return;
            }

            _head.Add(new Node(tick));
            this.UpdateTail();
        }

        private void UpdateTail()
        {
            if(_tail is null || _tail.Data.Id < _head?.Data.Id)
            {
                _tail = _head;
            }

            _tail = _tail?.GetTail();
        }
    }
}
