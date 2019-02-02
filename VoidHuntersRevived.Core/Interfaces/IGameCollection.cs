using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Interfaces
{
    public interface IGameCollection<TObject> : IEnumerable
    {

        event EventHandler<TObject> OnAdded;
        event EventHandler<TObject> OnRemove;

        /// <summary>
        /// Used to determin if items can be added or removed from the collection
        /// </summary>
        /// <returns></returns>
        Boolean CanAlter();

        /// <summary>
        /// Add an item to the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TObject Add(TObject item);

        /// <summary>
        /// Remove an item from the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TObject Remove(TObject item);

        /// <summary>
        /// Convert the object into a list
        /// </summary>
        /// <returns></returns>
        List<TObject> ToList();

        /// <summary>
        /// Return the current colletion size
        /// </summary>
        /// <returns></returns>
        Int32 Count();

        /// <summary>
        /// Return an element at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TObject ElementAt(Int32 index);
    }
}
