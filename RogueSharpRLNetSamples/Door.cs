using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class Door
   {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsOpen { get; set; }

      public void Draw( RLConsole console, IMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            if ( IsOpen )
            {
               console.Set( X, Y, Colors.DoorFov, Colors.DoorBackgroundFov, '-' );
            }
            else
            {
               console.Set( X, Y, Colors.DoorFov, Colors.DoorBackgroundFov, '+' );
            }
         }
         else
         {
            if ( IsOpen )
            {
               console.Set( X, Y, Colors.Door, Colors.DoorBackground, '-' );
            }
            else
            {
               console.Set( X, Y, Colors.Door, Colors.DoorBackground, '+' );
            }
         }
      }
   }
}