using RogueSharp;
using RogueSharpRLNetSamples.Core;

namespace RogueSharpRLNetSamples.Items
{
   public class RevealMapScroll : Item
   {
      public RevealMapScroll()
      {
         Name = "Magic Map";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         DungeonMap map = Game.DungeonMap;

         Game.MessageLog.Add( $"{Game.Player.Name} reads a {Name} and gains knowledge of the surrounding area" );

         foreach ( Cell cell in map.GetAllCells() )
         {
            if ( cell.IsWalkable )
            {
               map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
         
         RemainingUses--;

         return true;
      }
   }
}
