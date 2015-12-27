using RLNET;

namespace RogueSharpRLNetSamples.Actors
{
   public class Player : Actor
   {
      public void DrawStats( RLConsole statConsole )
      {
         statConsole.Print( 1, 1, string.Format( "Name:    {0}", Name ), RLColor.White );
         statConsole.Print( 1, 3, string.Format( "Health:  {0}/{1}", Health, MaxHealth ), RLColor.White );
         statConsole.Print( 1, 5, string.Format( "Attack:  {0} ({1}%)", Attack, AttackChance ), RLColor.White );
         statConsole.Print( 1, 7, string.Format( "Defense: {0} ({1}%)", Defense, DefenseChance ), RLColor.White );
         statConsole.Print( 1, 9, string.Format( "Gold:    {0}", Gold ), RLColor.Yellow );
      }

      public void DrawInventory( RLConsole inventoryConsole )
      {
         inventoryConsole.Print( 1, 1, "Equipment", RLColor.White );
         inventoryConsole.Print( 1, 3, "Head: Plate", RLColor.LightGray );
         inventoryConsole.Print( 1, 5, "Body: Chain", RLColor.LightGray );
         inventoryConsole.Print( 1, 7, "Hand: None", RLColor.LightGray );
         inventoryConsole.Print( 1, 9, "Feet: Leather", RLColor.LightGray );

         inventoryConsole.Print( 28, 1, "Abilities", RLColor.White );
         inventoryConsole.Print( 28, 3, "Q - Charge", RLColor.LightGray );
         inventoryConsole.Print( 28, 5, "W - Whirlwind Attack", RLColor.LightGray );
         inventoryConsole.Print( 28, 7, "E - Fireball", RLColor.LightGray );
         inventoryConsole.Print( 28, 9, "R - Lightning Bolt", RLColor.LightGray );

         inventoryConsole.Print( 55, 1, "Items", RLColor.White );
         inventoryConsole.Print( 55, 3, "1 - Health Potion", RLColor.LightGray );
         inventoryConsole.Print( 55, 5, "2 - Mana Potion", RLColor.LightGray );
         inventoryConsole.Print( 55, 7, "3 - Scroll", RLColor.LightGray );
         inventoryConsole.Print( 55, 9, "4 - Wand", RLColor.LightGray );
      }

      public void Draw( RLConsole mapConsole )
      {
         mapConsole.Set( X, Y, Color, null, Symbol );
      }
   }
}