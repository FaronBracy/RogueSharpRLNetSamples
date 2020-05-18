using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples.Core
{
   public class Stairs
   {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsUp { get; set; }

      public void Draw( RLConsole console, DungeonMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            if ( IsUp )
            {
               console.Set( X, Y, Colors.Player, null, '<' );
            }
            else
            {
               console.Set( X, Y, Colors.Player, null, '>' );
            }
         }
         else
         {
            if ( IsUp )
            {
               console.Set( X, Y, Colors.Floor, null, '<' );
            }
            else
            {
               console.Set( X, Y, Colors.Floor, null, '>' );
            }
         }
      }
   }
}