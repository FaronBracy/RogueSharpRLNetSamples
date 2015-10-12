using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class Monster
   {
      public int X { get; set; }
      public int Y { get; set; }

      public char Symbol { get; set; }
      public RLColor Color { get; set; }

      public string Name { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public int Armor { get; set; }
      public int Attack { get; set; }

      public void DrawStats( RLConsole statConsole, int position )
      {
         int yPosition = 9 + ( position * 2 );
         statConsole.Print( 1, yPosition, Symbol.ToString(), Color );
         statConsole.Print( 2, yPosition, string.Format( ": {0} {1}/{2}", Name, Health, MaxHealth ), RLColor.White );
      }

      public void Draw( RLConsole mapConsole, IMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            mapConsole.Set( X, Y, Color, null, Symbol );
         }
         else
         {
            mapConsole.Set( X, Y, Colors.Floor, Colors.FloorBackground, '.' );
         }
      }
   }
}