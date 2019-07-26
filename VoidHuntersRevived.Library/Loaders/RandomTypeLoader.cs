using Guppy.Loaders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoidHuntersRevived.Library.Loaders
{
    /// <summary>
    /// Simple loader used to bind a string to a group.
    /// This is used when selecting a random entity based
    /// on the entity type. For instance, the handle of ship-part:hull
    /// would have entity:shi-part:hull:square and entity:shi-part:hull:triangle
    /// bound to it.
    /// 
    /// When the request random function is called, a random string bound to
    /// the input handle will be returned.
    /// </summary>
    public class RandomTypeLoader : Loader<String, String, String[]>
    {
        private Dictionary<String, List<String>> _registeredValues;

        public RandomTypeLoader(ILogger logger) : base(logger)
        {
            _registeredValues = new Dictionary<String, List<String>>();
        }

        public override void Register(string handle, string value, ushort priority = 100)
        {
            if (!_registeredValues.ContainsKey(handle))
                _registeredValues[handle] = new List<string>();

            _registeredValues[handle].Add(value);
        }

        protected override Dictionary<string, string[]> BuildValuesTable()
        {
            return _registeredValues.ToDictionary(
                keySelector: kvp => kvp.Key,
                elementSelector: kvp => kvp.Value.ToArray());
        }

        public String GetRandomValue(String handle, Random rand)
        {
            return this.valuesTable[handle].ElementAt(rand.Next(this.valuesTable[handle].Length));
        }
    }
}
