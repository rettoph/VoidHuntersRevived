using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Assets.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    /// <summary>
    /// <see cref="ITickService"/> implementation capable of automatically
    /// sorting incoming ticks. It is assumed that ticks may be out of order
    /// like in the instance of incoming messages
    /// </summary>
    public class ClientLinkedListLockstepTickService(
        ISettingService settingService
    ) : BaseLockstepTickService(settingService),
        ITickService
    {
        /// <summary>
        /// A singular node contains current <see cref="Tick"/> data
        /// and a reference to the latest known <see cref="Node.Child"/>.
        /// This is the baseline of a functional linked list
        /// </summary>
        /// <param name="data"></param>
        [DebuggerDisplay("Id: {Data.Id}")]
        private sealed class Node(Tick data)
        {
            /// <summary>
            /// The current tick id
            /// </summary>
            public int Id => this.Data.Id;

            /// <summary>
            /// The previous tick id
            /// </summary>
            public int ParentId => this.Id - 1;

            /// <summary>
            /// The expected child id. Note, this may not match the value stored by <see cref="Node.Child"/> as
            /// there may be gaps in the linked list data
            /// </summary>
            public int ChildId => this.Id + 1;

            /// <summary>
            /// The current tick data.
            /// </summary>
            public Tick Data { get; private set; } = data;

            /// <summary>
            /// The latest known child. Ideally the direct next tick, but in the
            /// event of dropped ticks this may be a few ticks down the line. Eventualy,
            /// when the dropped ticks are recieved, the liked list will self sort
            /// </summary>
            public Node? Child { get; private set; }

            /// <summary>
            /// Attempt to add a new node to the linked list.
            /// </summary>
            /// <param name="child"></param>
            /// <returns></returns>
            public EnqueueTickResponseEnum Add(Node child, out Node parent)
            {
                parent = this;
                EnqueueTickResponseEnum response;

                // the NodeEnqueued response indicates that we have not found a proper parent
                // for the given child node yet. Iterate through the linked list until we
                // get a response indicating action.
                while ((response = parent.TryAdd(child)) == EnqueueTickResponseEnum.NotEnqueued)
                {
                    if (parent.Child is null)
                    {
                        // This likely happens because the 'child' node
                        // declares itself to be a parent of the current node
                        // This should never organically happen. 
                        // TODO: Test this and ensure it cant happen
                        // TODO: Figure out what to do here...
                        break;
                    }

                    parent = parent.Child;
                }

                // The loop has been broken - we can safely assume the child has been added to the 
                // linked list in a sorted postition
                return response;
            }

            /// <summary>
            /// Gets the lastest tick in the current direct line
            /// Gaps in tick data will result in the tail being
            /// returned early
            /// </summary>
            /// <returns></returns>
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

            /// <summary>
            /// Attempt to add the given node as a direct child of the current node.
            /// </summary>
            /// <param name="child"></param>
            /// <returns></returns>
            private EnqueueTickResponseEnum TryAdd(Node child)
            {
                // If the given node id matches the current node id
                // we either have a duplicate tick or some sort of
                // desync
                if (child.Data.Id == this.Data.Id)
                {
                    if (this.Data.Hash != child.Data.Hash)
                    {
                        this.Data = child.Data;
                        return EnqueueTickResponseEnum.DuplicateMismatch;
                    }

                    return EnqueueTickResponseEnum.DuplicateMatch;
                }

                // If the current node does not currently have a child
                // we can store the given node as a child directly
                // without any more advanced sorting
                if (this.Child is null)
                {
                    this.Child = child;
                    return EnqueueTickResponseEnum.Enqueued;
                }

                // If the given node has an id less than the current node's child
                // Then the given node should become the new child. Insert it into
                // the linked list
                if (child.Data.Id < this.Child.Data.Id)
                {
                    var old = this.Child;
                    this.Child = child;
                    this.Child.Child = old;
                    return EnqueueTickResponseEnum.Enqueued;
                }

                // This indicates the current child is actually a parent
                // of the given node. The node was not added to the current
                // node as a child - to avoid recursion. A flat method will
                // handle this result
                return EnqueueTickResponseEnum.NotEnqueued;
            }
        }

        /// <summary>
        /// The current head (start) of the linked list
        /// </summary>
        private Node? _head;

        /// <summary>
        /// Indicates how many nodes are currently in the list
        /// </summary>
        private int _count;

        /// <inheritdoc />
        public override EnqueueTickResponseEnum TryEnqueue(Tick tick)
        {
            Node node = new(tick);

            if (tick.Id < this.NextTickId)
            {
                // TODO: Investigate why this is here and why this is needed.
                // Sometimes we double send a message, this should fix that.
                return EnqueueTickResponseEnum.NotEnqueued;
            }

            // If the current head is null then no need to add
            // to the linked list. We just store the current node as
            // the head and tail
            if (this._head is null)
            {
                this._head = node;
                this._count = 1;

                return EnqueueTickResponseEnum.Enqueued;
            }

            Node? tail = null;
            EnqueueTickResponseEnum response;

            // The incoming tick should replace the head
            if (tick.Id < this._head.Id)
            {
                var old = this._head;
                this._head = node;
                // Attempt to add the old head to the new node
                response = this._head.Add(old, out tail);
                if (response == EnqueueTickResponseEnum.Enqueued)
                {
                    this._count++;
                }

                return response;
            }

            // If we've made it this far we know the new node comes after the current head
            // Attempt to add it to the current head
            response = this._head.Add(node, out tail);
            if (response == EnqueueTickResponseEnum.Enqueued)
            {
                this._count++;
            }

            return response;
        }

        protected override bool TryDequeue([MaybeNullWhen(false)] out Tick tick)
        {
            if (this._head is null)
            {
                tick = null;
                return false;
            }

            if (this._head.Id == this.NextTickId)
            {
                tick = this._head.Data;

                this._head = this._head.Child;
                this._count--;

                return true;
            }

            if (this._head.Id < this.NextTickId)
            {
                // TODO: Investigate why this is here and why this is needed.
                throw new NotImplementedException();

                // Sometimes we double send a message, this should fix that.
                this._head = this._head.Child;

                return this.TryDequeue(out tick);
            }

            tick = null;
            return false;
        }

        /// <summary>
        /// Return the last known tick before the given <paramref name="id"/>.
        /// 
        /// If the buffer has 1, 2, 3, 4 stored and we pass in 5 we should get tick 4 back.
        /// 
        /// If the buffer has 1, 2, 3, 5, 6, 7 stored and we paass in 4 we should get tick 3 back
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        public override void Reset()
        {
            base.Reset();

            this._head = null;
            this._count = 0;
        }

        public override bool ShouldStep(bool timeSinceLastStepLessThanStepInterval)
        {
            return this._count > 0;
        }
    }
}
