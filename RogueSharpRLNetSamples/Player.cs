using RLNET;

namespace RogueSharpRLNetSamples
{
   public class Player
   {
      public int X { get; set; }
      public int Y { get; set; }

      public int Gold { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public int Armor { get; set; }
      public int Attack { get; set; }

      public void DrawStats( RLConsole statConsole )
      {
         statConsole.Print( 1, 1, string.Format( "Health:  {0}/{1}", Health, MaxHealth ), RLColor.White );
         statConsole.Print( 1, 3, string.Format( "Attack:  {0}", Attack ), RLColor.White );
         statConsole.Print( 1, 5, string.Format( "Armor:   {0}", Armor ), RLColor.White );
         statConsole.Print( 1, 7, string.Format( "Gold:    {0}", Armor ), RLColor.Yellow );
      }

      public void Draw( RLConsole mapConsole )
      {
         mapConsole.Set( X, Y, Colors.Player, null, '@' );
      }
   }
}