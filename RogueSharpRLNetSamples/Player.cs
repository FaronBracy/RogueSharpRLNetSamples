using RLNET;

namespace RogueSharpRLNetSamples
{
   public class Player : Actor
   {
      public void DrawStats( RLConsole statConsole )
      {
         statConsole.Print( 1, 1, string.Format( "Health:  {0}/{1}", Health, MaxHealth ), RLColor.White );
         statConsole.Print( 1, 3, string.Format( "Attack:  {0}", Attack ), RLColor.White );
         statConsole.Print( 1, 5, string.Format( "Armor:   {0}", Armor ), RLColor.White );
         statConsole.Print( 1, 7, string.Format( "Gold:    {0}", Gold ), RLColor.Yellow );
      }

      public void Draw( RLConsole mapConsole )
      {
         mapConsole.Set( X, Y, Color, null, Symbol );
      }
   }
}