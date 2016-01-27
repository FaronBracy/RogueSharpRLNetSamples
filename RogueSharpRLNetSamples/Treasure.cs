using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Inventory;

namespace RogueSharpRLNetSamples
{
   public class Treasure : IDrawable
   {
      public int Gold { get; set; }
      public Equipment Equipment { get; set; } 
      public Ability Ability { get; set; }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      public Treasure( int x, int y, int gold, Equipment equipment = null, Ability ability = null )
      {
         char symbol = '$';
         if ( equipment != null )
         {
            symbol = '!';
         }
         if ( ability != null )
         {
            symbol = '*';
         }

         Gold = gold;
         Equipment = equipment;
         Ability = ability;
         Color = RLColor.Yellow;
         Symbol = symbol;
         X = x;
         Y = y;
      }

      public void Draw( RLConsole mapConsole, IMap map )
      {
         if ( !map.IsExplored( X, Y ) )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            mapConsole.Set( X, Y, Color, null, Symbol );
         }
         else
         {
            mapConsole.Set( X, Y, RLColor.Blend( Color, RLColor.Gray, 0.5f ), Colors.FloorBackground, Symbol );
         }
      }
   }
}
