﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class TickBuffer : IEnumerable<Tick>
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

            public EnqueueTickResponse Add(Node child)
            {
                Node parent = this;
                EnqueueTickResponse response = EnqueueTickResponse.NotEnqueued;

                while ((response = parent.TryAdd(child)) == EnqueueTickResponse.NotEnqueued)
                {
                    if (parent.Child is null)
                    {
                        break;
                    }

                    parent = parent.Child;
                }

                return response;
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

            private EnqueueTickResponse TryAdd(Node child)
            {
                if (child.Data.Id == Data.Id)
                {
                    if(Data.Hash != child.Data.Hash)
                    {
                        Data = child.Data;
                        return EnqueueTickResponse.DuplicateMismatch;
                    }
                    
                    return EnqueueTickResponse.DuplicateMatch;
                }

                if (Child is null)
                {
                    Child = child;
                    return EnqueueTickResponse.Enqueued;
                }

                if (child.Data.Id < Child.Data.Id)
                {
                    var old = Child;
                    Child = child;
                    Child.Child = old;
                    return EnqueueTickResponse.Enqueued;
                }

                return EnqueueTickResponse.NotEnqueued;
            }
        }

        public enum EnqueueTickResponse
        {
            Enqueued,
            NotEnqueued,
            DuplicateMatch,
            DuplicateMismatch
        }
        private Node? _head;
        private Node? _tail;
        private Node? _popped;

        public Tick? Head => _head?.Data;
        public Tick? Tail => _tail?.Data;
        public Tick? Popped => _popped?.Data;

        public int Count { get; private set; }

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
                _popped = _head;

                _head = _head.Child;

                if (_head is null)
                {
                    _tail = null;
                }

                this.Count--;

                return true;
            }

            if (_head.Id < id)
            { // Sometimes we double send a message, this should fix that.
                _head = _head.Child;
                this.Count--;

                return TryPop(id, out tick);
            }

            tick = null;
            return false;
        }

        public EnqueueTickResponse TryEnqueue(Tick tick)
        {
            var node = new Node(tick);
            if (_head is null || tick.Queue != _head.Data.Queue)
            {
                _head = node;
                this.UpdateTail();
                this.Count++;
                return EnqueueTickResponse.Enqueued;
            }

            EnqueueTickResponse response = EnqueueTickResponse.NotEnqueued;

            if (tick.Id < _head.Id)
            {
                var old = _head;
                _head = node;
                if((response = _head.Add(old)) == EnqueueTickResponse.Enqueued)
                {
                    this.UpdateTail();
                    this.Count++;
                }

                return response;
            }

            if((response = _head.Add(node)) == EnqueueTickResponse.Enqueued)
            {
                this.UpdateTail();
                this.Count++;
            }

            return response;
        }

        private void UpdateTail()
        {
            if (_tail is null || _tail.Id < _head?.Id)
            {
                _tail = _head;
            }

            _tail = _tail?.GetTail();
        }

        public void Clear()
        {
            _head = null;
            _tail = null;
            _popped = null;

            this.Count = 0;
        }

        public int? IndexOf(int id)
        {
            if(_head is null || id < _head.Id)
            {
                return null;
            }

            int index = 0;
            Node? node = _head;

            while(node is not null)
            {
                if(node.Id > id)
                {
                    return null;
                }

                if(node.Id == id)
                {
                    return index;
                }

                node = node.Child;
                index++;
            }

            return null;
        }

        public Tick? Previous(int id)
        {
            if (_head is null || id < _head.Id)
            {
                return null;
            }

            Node? previous = null;
            Node? node = _head;

            while (node is not null)
            {
                if (node.Id > id)
                {
                    return previous?.Data;
                }

                if (node.Id == id)
                {
                    return previous?.Data;
                }

                previous = node;
                node = node.Child;
            }

            return previous?.Data;
        }

        public Tick? Next(int id)
        {
            if (_head is null || id < _head.Id)
            {
                return null;
            }

            Node? node = _head;

            while (node is not null)
            {
                if (node.Id > id)
                {
                    return null;
                }

                if (node.Id == id)
                {
                    return node.Child?.Data;
                }

                node = node.Child;
            }

            return null;
        }

        public IEnumerator<Tick> GetEnumerator()
        {
            Node? node = _head;

            while(node is not null)
            {
                yield return node.Data;

                node = node.Child;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
