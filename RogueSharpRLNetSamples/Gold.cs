using RLNET;

namespace RogueSharpRLNetSamples
{
   public class Gold : IDrawable
   {
      public int Amount { get; set; }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      public Gold( int x, int y, int amount )
      {
         Amount = amount;
         Color = RLColor.Yellow;
         Symbol = '$';
         X = x;
         Y = y;
      }

      public void Draw( RLConsole mapConsole )
      {
         mapConsole.Set( X, Y, Color, null, Symbol );
      }
   }
}
