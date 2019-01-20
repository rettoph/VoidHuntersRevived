using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Loaders
{
    /// <summary>
    /// Used to store strings via handle, allowing
    /// easy changing for internal string values
    /// </summary>
    public class StringLoader : ILoader
    {
        private readonly ILogger _logger;
        private List<RegisterdStringInfo> _registeredStringList;
        private Dictionary<String, String> _stringTable;

        #region Structs
        private struct RegisterdStringInfo
        {
            public readonly Int32 Priority;
            public readonly String Key;
            public readonly String Value;
            
            public RegisterdStringInfo(String key, String value, Int32 priority)
            {
                this.Key = key;
                this.Value = value;
                this.Priority = priority;
            }
        }
        #endregion

        public StringLoader(ILogger logger)
        {
            _logger = logger;
            _registeredStringList = new List<RegisterdStringInfo>();
        }

        public void Register(String key, String value, Int32 priority = 0)
        {
            _logger.LogDebug($"Registering new String => key: '{key}', value: '{value}', priority: {priority}");

            _registeredStringList.Add(
                new RegisterdStringInfo(
                    key, 
                    value,
                    priority));
        }

        public String Get(String handle)
        {
            return _stringTable[handle];
        }

        public void Initialize()
        {
            _logger.LogDebug("Initializing top priority strings...");

            _stringTable = _registeredStringList
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
