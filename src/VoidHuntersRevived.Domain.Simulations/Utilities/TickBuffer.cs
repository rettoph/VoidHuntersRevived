using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Utilities
{
    public sealed class TickBuffer
    {
        [DebuggerDisplay("Id: {Data.Id}")]
        private sealed class Node
        {
            public int Id => Data.Id;
            public int ParentId => Id - 1;
            public int ChildId => Id + 1;

            public Tick Data { get; private set; }
            public Node? Child { get; private set; }

            public Node(Tick data)
            {
                Data = data;
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
                if (Child is null)
                {
                    return this;
                }

                Node parent = this;
                Node tick = Child;

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
                if (Child is null)
                {
                    Child = child;
                    return true;
                }

                if (child.Data.Id == Data.Id)
                {
                    Data = child.Data;
                    return true;
                }

                if (child.Data.Id < Child.Data.Id)
                {
                    var old = Child;
                    Child = child;
                    Child.Child = old;
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

                for (int i = 0; i < index; i++)
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
            if (_head is null)
            {
                tick = null;
                return false;
            }

            if (_head.Id == id)
            {
                tick = _head.Data;
                _head = _head.Child;

                if (_head is null)
                {
                    _tail = null;
                }

                return true;
            }

            if (_head.Id < id)
            { // Sometimes we double send a message, this should fix that.
                _head = _head.Child;

                return TryPop(id, out tick);
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
                UpdateTail();
                return;
            }

            if (tick.Id < _head.Id)
            {
                var old = _head;
                _head = node;
                _head.Add(old);
                UpdateTail();
                return;
            }

            _head.Add(node);
            UpdateTail();
            Latest = tick;
        }

        private void UpdateTail()
        {
            if (_tail is null || _tail.Id < _head?.Id)
            {
                _tail = _head;
            }

            _tail = _tail?.GetTail();
        }
    }
}
