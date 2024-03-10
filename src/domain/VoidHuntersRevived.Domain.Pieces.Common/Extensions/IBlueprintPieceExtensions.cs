using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Pieces.Extensions
{
    internal static class IBlueprintPieceExtensions
    {
        public static VhId CalculateHash(this IBlueprintPiece blueprintPiece)
        {
            VhId hash = blueprintPiece.PieceType.Id.Value;

            for (int i = 0; i < blueprintPiece.Children.Length; i++)
            {
                for (int ii = 0; ii < blueprintPiece.Children[i].Length; ii++)
                {
                    hash = hash.Create(blueprintPiece.Children[i][ii].CalculateHash());
                }
            }

            return hash;
        }
    }
}
