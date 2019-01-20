using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using System.Linq;

namespace VoidHuntersRevived.Core.Providers
{
    public class ContentLoader : ILoader
    {
        private ILogger _logger;
        private ContentManager _content;
        private List<RegisteredContentInfo> _registeredContentInfoList;
        private Dictionary<Type, Dictionary<String, Object>> _contentTable;

        #region Structs
        private struct RegisteredContentInfo
        {
            public readonly Type Type;
            public readonly String Name;
            public readonly String AssetName;
            public readonly Int32 Priority;

            public RegisteredContentInfo(Type type, String name, String assetName, Int32 priority)
            {
                this.Type = type;
                this.Name = name;
                this.AssetName = assetName;
                this.Priority = priority;
            }
        }
        #endregion

        public ContentLoader(ILogger logger, ContentManager content = null)
        {
            _logger = logger;
            _content = content;
            _registeredContentInfoList = new List<RegisteredContentInfo>();
        }

        /// <summary>
        /// Register a new asset to a given name. On initialization, 
        /// each name will present the highest priority value and assume
        /// that texture for its given asset
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="name"></param>
        /// <param name="assetName"></param>
        /// <param name="priority"></param>
        public void Register<TAsset>(String name, String assetName, Int32 priority = 0)
        {
            _logger.LogDebug($"Registering new asset<{typeof(TAsset).Name}> => name: '{name}', assetName: '{assetName}', priority: {priority}");

            _registeredContentInfoList.Add(
                new RegisteredContentInfo(
                    typeof(TAsset),
                    name,
                    assetName,
                    priority));
        }

        /// <summary>
        /// Return an asset from the content loader by name
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public TAsset Get<TAsset>(String name)
        {
            var type = typeof(TAsset);

            if(!_contentTable.ContainsKey(type) || !_contentTable[type].ContainsKey(name))
            {
                _logger.LogError($"Unknown Asset<{type.Name}> => name: '{name}'");
                return default(TAsset);
            }

            return (TAsset)_contentTable[type][name];
        }

        public void Initialize()
        {
            _logger.LogDebug("Initializing and loading top priority assets...");

            _contentTable = _registeredContentInfoList
                .GroupBy(rci => rci.Type)
                .ToDictionary(
                    keySelector: g => g.Key,
                    elementSelector: g => g.GroupBy(rci => rci.Name)
                        .ToDictionary(
                            keySelector: ng => ng.Key,
                            elementSelector: ng => _content.Load<Object>(ng.ToList()
                                .OrderByDescending(rci => rci.Priority)
                                .First().AssetName)));
        }
    }
}
