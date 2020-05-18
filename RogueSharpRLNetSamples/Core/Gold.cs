using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
   public class Gold : ITreasure, IDrawable 
   {
      public int Amount { get; set; }

      public Gold( int amount )
      {
         Amount = amount;
         Symbol = '$';
         Color = RLColor.Yellow;
      }

      public bool PickUp( IActor actor )
      {
         actor.Gold += Amount;
         Game.MessageLog.Add( $"{actor.Name} picked up {Amount} gold" );
         return true;
      }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public void Draw( RLConsole console, DungeonMap map )
      {
         if ( !map.IsExplored( X, Y ) )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol );
         }
         else
         {
            console.Set( X, Y, RLColor.Blend( Color, RLColor.Gray, 0.5f ), Colors.FloorBackground, Symbol );
         }
      }
   }
}
