using Guppy.Core.Assets.Common;
using Guppy.Game.Graphics.Common.Assets;
using Guppy.Game.ImGui.Common.Styling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Common
{
    public static class Assets
    {
        public static class SpriteFonts
        {
            public static readonly AssetKey<SpriteFont> Default = AssetKey<SpriteFont>.Get($"{nameof(SpriteFont)}.{nameof(Default)}");
        }

        public static class EffectCodes
        {
            public static readonly AssetKey<IEffectCode> ShaderAntiAliasing = AssetKey<IEffectCode>.Get($"EffectCode.{nameof(ShaderAntiAliasing)}");
            public static readonly AssetKey<IEffectCode> Visible = AssetKey<IEffectCode>.Get($"EffectCode.{nameof(Visible)}");
        }

        public static class Strings
        {
            public static readonly AssetKey<string> TeamZeroName = AssetKey<string>.Get($"{nameof(String)}.{nameof(TeamZeroName)}");
            public static readonly AssetKey<string> TeamOneName = AssetKey<string>.Get($"{nameof(String)}.{nameof(TeamOneName)}");
        }

        public static class Colors
        {
            public static readonly AssetKey<Color> None = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(None)}");

            public static readonly AssetKey<Color> HullPrimaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(HullPrimaryColor)}");
            public static readonly AssetKey<Color> HullSecondaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(HullSecondaryColor)}");

            public static readonly AssetKey<Color> ThrusterPrimaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(ThrusterPrimaryColor)}");
            public static readonly AssetKey<Color> ThrusterSecondaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(ThrusterSecondaryColor)}");

            public static readonly AssetKey<Color> TeamOnePrimaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(TeamOnePrimaryColor)}");
            public static readonly AssetKey<Color> TeamOneSecondaryColor = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(TeamOneSecondaryColor)}");

            public static readonly AssetKey<Color> TractorBeamHighlight = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
            public static readonly AssetKey<Color> ActiveThrustableHighlight = AssetKey<Color>.Get($"{nameof(Color)}.{nameof(TractorBeamHighlight)}");
        }

        public static class ImGuiStyles
        {
            public static readonly AssetKey<ImStyle> ButtonGreen = AssetKey<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonGreen)}");
            public static readonly AssetKey<ImStyle> ButtonRed = AssetKey<ImStyle>.Get($"{nameof(ImStyle)}.{nameof(ButtonRed)}");
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