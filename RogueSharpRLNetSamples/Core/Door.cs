using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
   public class Door : IDrawable
   {
      public Door()
      {
         Symbol = '+';
         Color = Colors.Door;
         BackgroundColor = Colors.DoorBackground;
      }
      public bool IsOpen { get; set; }
      
      public RLColor Color { get; set; }
      public RLColor BackgroundColor { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      public void Draw( RLConsole console, DungeonMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         Symbol = IsOpen ? '-' : '+';
         if ( map.IsInFov( X, Y ) )
         {
            Color = Colors.DoorFov;
            BackgroundColor = Colors.DoorBackgroundFov;
         }
         else
         {
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
         }

         console.Set( X, Y, Color, BackgroundColor, Symbol );
      }
   }
}