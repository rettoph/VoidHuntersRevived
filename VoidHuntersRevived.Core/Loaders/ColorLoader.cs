using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Loaders
{
    /// <summary>
    /// Used to store colors via handle, allowing
    /// easy changing for internal colors values
    /// </summary>
    public class ColorLoader : ILoader
    {
        private readonly ILogger _logger;
        private List<RegisterdColorInfo> _registeredColorList;
        private Dictionary<String, Color> _colorTable;

        #region Structs
        private struct RegisterdColorInfo
        {
            public readonly Int32 Priority;
            public readonly String Key;
            public readonly Color Value;
            
            public RegisterdColorInfo(String key, Color value, Int32 priority)
            {
                this.Key = key;
                this.Value = value;
                this.Priority = priority;
            }
        }
        #endregion

        public ColorLoader(ILogger logger)
        {
            _logger = logger;
            _registeredColorList = new List<RegisterdColorInfo>();
        }

        public void Register(String key, Color value, Int32 priority = 0)
        {
            _logger.LogDebug($"Registering new Color => key: '{key}', value: '{value}', priority: {priority}");

            _registeredColorList.Add(
                new RegisterdColorInfo(
                    key, 
                    value,
                    priority));
        }

        public Color Get(String handle)
        {
            return _colorTable[handle];
        }

        public void Initialize()
        {
            _logger.LogDebug("Initializing top priority colors...");

            _colorTable = _registeredColorList
                .GroupBy(rsi => rsi.Key)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: g => g.ToList()
                        .OrderByDescending(rsi => rsi.Priority)
                        .First()
                        .Value);
        }
    }
}
