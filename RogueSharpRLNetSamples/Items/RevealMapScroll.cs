using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class RevealMapScroll : IItem
   {
      public string Name { get; }
      public int RemainingUses { get; private set; }

      public RevealMapScroll()
      {
         Name = "Magic Map";
         RemainingUses = 1;
      }

      public bool Use()
      {
         DungeonMap map = Game.CommandSystem.DungeonMap;
         Player player = map.GetPlayer();

         Game.Messages.Add( $"{player.Name} reads a {Name} and gains knowledge of the surrounding area" );

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
