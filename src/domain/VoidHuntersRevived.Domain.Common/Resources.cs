using Guppy.Core.Resources.Common;
using Guppy.Game.Graphics.Common.Resources;
using Guppy.Game.ImGui.Common.Styling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Common
{
    public static class Resources
    {
        public static class SpriteFonts
        {
            public static readonly ResourceKey<SpriteFont> Default = ResourceKey<SpriteFont>.Get($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly ResourceKey<IEffectCode> ShaderAntiAliasing = ResourceKey<IEffectCode>.Get($"EffectCode.{nameof(ShaderAntiAliasing)}");
            public static readonly ResourceKey<IEffectCode> Visible = ResourceKey<IEffectCode>.Get($"EffectCode.{nameof(Visible)}");
        }

        public static class Strings
        {
            public static readonly ResourceKey<string> TeamZeroName = ResourceKey<string>.Get($"{nameof(String)}.{nameof(TeamZeroName)}");
            public static readonly ResourceKey<string> TeamOneName = ResourceKey<string>.Get($"{nameof(String)}.{nameof(TeamOneName)}");
        }

        public static class Colors
        {
            public static readonly ResourceKey<Color> None = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(None)}");

            public static readonly ResourceKey<Color> HullPrimaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(HullPrimaryColor)}");
            public static readonly ResourceKey<Color> HullSecondaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(HullSecondaryColor)}");

            public static readonly ResourceKey<Color> ThrusterPrimaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(ThrusterPrimaryColor)}");
            public static readonly ResourceKey<Color> ThrusterSecondaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(ThrusterSecondaryColor)}");

            public static readonly ResourceKey<Color> TeamOnePrimaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(TeamOnePrimaryColor)}");
            public static readonly ResourceKey<Color> TeamOneSecondaryColor = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(TeamOneSecondaryColor)}");

            public static readonly ResourceKey<Color> TractorBeamHighlight = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
            public static readonly ResourceKey<Color> ActiveThrustableHighlight = ResourceKey<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
        }

        public static class ImGuiStyles
        {
            public static readonly ResourceKey<ImStyle> ButtonGreen = ResourceKey<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonGreen)}");
            public static readonly ResourceKey<ImStyle> ButtonRed = ResourceKey<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonRed)}");
        }

        public static class EntityTemplates
        {
            public static class Team
            {
                public static readonly Key<IEntityTemplate> TeamEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Team.TeamEntityTemplate");
                public static readonly Key<IEntityTemplate> DefaultTeamEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Team.DefaultTeamEntityTemplate");
                public static readonly Key<IEntityTemplate> TeamMemberEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Team.TeamMemberEntityTemplate");
            }

            public static class Physics
            {
                public static readonly Key<IEntityTemplate> BodyEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Physics.BodyEntityTemplate");
                public static readonly Key<IEntityTemplate> FixtureEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Physics.FixtureEntityTemplate");
            }

            public static class Piece
            {
                public static readonly Key<IEntityTemplate> TreeEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Piece.TreeEntityTemplate");
                public static readonly Key<IEntityTemplate> PieceEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Piece.PieceEntityTemplate");
                public static readonly Key<IEntityTemplate> HullEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Piece.HullEntityTemplate");
                public static readonly Key<IEntityTemplate> ThrusterEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Piece.ThrusterEntityTemplate");
            }

            public static class Ship
            {
                public static readonly Key<IEntityTemplate> ChainEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Ship.ChainEntityTemplate");
                public static readonly Key<IEntityTemplate> ShipEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Ship.ShipEntityTemplate");
                public static readonly Key<IEntityTemplate> UserShipEntityTemplate = Key<IEntityTemplate>.GetByName("Entity.Ship.UserShipEntityTemplate");
            }
        }
    }
}
