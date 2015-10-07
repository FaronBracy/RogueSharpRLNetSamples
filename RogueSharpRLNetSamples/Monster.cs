using RLNET;

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

      public void Draw( RLConsole console )
      {
         console.Set( X, Y, Color, null, Symbol );
      }
   }
}