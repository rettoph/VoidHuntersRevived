using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public class TickBuffer : IEnumerable<Tick>
    {
        [DebuggerDisplay("Id: {Data.Id}")]
        private sealed class Node(Tick data)
        {
            public int Id => this.Data.Id;
            public int ParentId => this.Id - 1;
            public int ChildId => this.Id + 1;

            public Tick Data { get; private set; } = data;
            public Node? Child { get; private set; }

            public EnqueueTickResponseEnum Add(Node child)
            {
                Node parent = this;
                EnqueueTickResponseEnum response;
                while ((response = parent.TryAdd(child)) == EnqueueTickResponseEnum.NotEnqueued)
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
                if (this.Child is null)
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

            private EnqueueTickResponseEnum TryAdd(Node child)
            {
                if (child.Data.Id == this.Data.Id)
                {
                    if (this.Data.Hash != child.Data.Hash)
                    {
                        this.Data = child.Data;
                        return EnqueueTickResponseEnum.DuplicateMismatch;
                    }

                    return EnqueueTickResponseEnum.DuplicateMatch;
                }

                if (this.Child is null)
                {
                    this.Child = child;
                    return EnqueueTickResponseEnum.Enqueued;
                }

                if (child.Data.Id < this.Child.Data.Id)
                {
                    var old = this.Child;
                    this.Child = child;
                    this.Child.Child = old;
                    return EnqueueTickResponseEnum.Enqueued;
                }

                return EnqueueTickResponseEnum.NotEnqueued;
            }
        }

        public enum EnqueueTickResponseEnum
        {
            Enqueued,
            NotEnqueued,
            DuplicateMatch,
            DuplicateMismatch
        }
        private Node? _head;
        private Node? _tail;
        private Node? _popped;

        public Tick? Head => this._head?.Data;
        public Tick? Tail => this._tail?.Data;
        public Tick? Popped => this._popped?.Data;

        public int Count { get; private set; }

        public Tick? this[int index]
        {
            get
            {
                Node? result = this._head;

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
            if (this._head is null)
            {
                tick = null;
                return false;
            }

            if (this._head.Id == id)
            {
                tick = this._head.Data;
                this._popped = this._head;

                this._head = this._head.Child;

                if (this._head is null)
                {
                    this._tail = null;
                }

                this.Count--;

                return true;
            }

            if (this._head.Id < id)
            { // Sometimes we double send a message, this should fix that.
                this._head = this._head.Child;
                this.Count--;

                return this.TryPop(id, out tick);
            }

            tick = null;
            return false;
        }

        public EnqueueTickResponseEnum TryEnqueue(Tick tick)
        {
            var node = new Node(tick);
            if (this._head is null)
            {
                this._head = node;
                this.UpdateTail();
                this.Count++;
                return EnqueueTickResponseEnum.Enqueued;
            }

            EnqueueTickResponseEnum response;
            if (tick.Id < this._head.Id)
            {
                var old = this._head;
                this._head = node;
                if ((response = this._head.Add(old)) == EnqueueTickResponseEnum.Enqueued)
                {
                    this.UpdateTail();
                    this.Count++;
                }

                return response;
            }

            if ((response = this._head.Add(node)) == EnqueueTickResponseEnum.Enqueued)
            {
                this.UpdateTail();
                this.Count++;
            }

            return response;
        }

        private void UpdateTail()
        {
            if (this._tail is null || this._tail.Id < this._head?.Id)
            {
                this._tail = this._head;
            }

            this._tail = this._tail?.GetTail();
        }

        public void Clear()
        {
            this._head = null;
            this._tail = null;
            this._popped = null;

            this.Count = 0;
        }

        public int? IndexOf(int id)
        {
            if (this._head is null || id < this._head.Id)
            {
                return null;
            }

            int index = 0;
            Node? node = this._head;

            while (node is not null)
            {
                if (node.Id > id)
                {
                    return null;
                }

                if (node.Id == id)
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
            if (this._head is null || id < this._head.Id)
            {
                return null;
            }

            Node? previous = null;
            Node? node = this._head;

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
            if (this._head is null || id < this._head.Id)
            {
                return null;
            }

            Node? node = this._head;

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
            Node? node = this._head;

            while (node is not null)
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