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
      public IItem Item { get; set; }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      public Treasure( int x, int y, Equipment equipment ) 
         : this( x, y, 0, equipment )
      {
      }

      public Treasure( int x, int y, Ability ability ) 
         : this( x, y, 0, null, ability )
      {
      }

      public Treasure( int x, int y, IItem item ) 
         : this( x, y, 0, null, null, item )
      {
      }

      public Treasure( int x, int y, int gold, Equipment equipment = null, Ability ability = null, IItem item = null )
      {
         char symbol = '$';
         if ( item != null )
         {
            symbol = '!';
         }
         if ( equipment != null )
         {
            symbol = ']';
         }
         if ( ability != null )
         {
            symbol = '*';
         }

         Gold = gold;
         Item = item;
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
