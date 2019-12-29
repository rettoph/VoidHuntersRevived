using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Teams represent groups of players
    /// that have no friendly fire & share
    /// base color.
    /// </summary>
    public class Team : NetworkEntity
    {
        #region Private Fields
        private List<Player> _players;
        private List<Team> _teams;
        #endregion

        #region Public Attributes
        public Color Color { get; set; }
        public IReadOnlyCollection<Player> Players { get => _players.AsReadOnly(); }
        #endregion

        #region Events 
        public event EventHandler<Player> OnPlayerAdded;
        #endregion

        #region Constructor
        public Team(List<Team> teams)
        {
            _teams = teams;
            _players = new List<Player>();
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _teams.Add(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            _teams.Remove(this);
        }
        #endregion

        #region Event Handlers
        public void AddPlayer(Player player)
        {
            if(player.Team != this)
            {
                player?.Team?.RemovePlayer(player);
                _players.Add(player);
                player.SetTeam(this);

                this.OnPlayerAdded?.Invoke(this, player);
            }
        }

        private void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }
        #endregion

        #region Network Methods
        public void ReadColor(NetIncomingMessage im)
        {
            this.Color = im.ReadColor();
        }

        public void WriteColor(NetOutgoingMessage om)
        {
            om.Write(this.Color);
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadColor(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteColor(om);
        }
        #endregion
    }
}
