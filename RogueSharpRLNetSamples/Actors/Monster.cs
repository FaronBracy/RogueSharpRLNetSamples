using System;
using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples.Actors
{
   public class Monster : Actor
   {
      public void DrawStats( RLConsole statConsole, int position )
      {
         int yPosition = 13 + ( position * 2 );
         statConsole.Print( 1, yPosition, Symbol.ToString(), Color );
         int width = Convert.ToInt32( ( (double) Health / (double) MaxHealth ) * 16.0 );
         int remainingWidth = 16 - width;
         statConsole.SetBackColor( 3, yPosition, width, 1, Swatch.Primary );
         statConsole.SetBackColor( 3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );  
         statConsole.Print( 2, yPosition, string.Format( ": {0}", Name ), RLColor.White );
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