using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class HealingPotion : IItem
   {
      public string Name { get; }
      public int RemainingUses { get; private set; }

      public HealingPotion()
      {
         Name = "Healing Potion";
         RemainingUses = 1;
      }

      public bool Use()
      {
         int healAmount = 15;
         Player player = Game.CommandSystem.DungeonMap.GetPlayer();
         Game.Messages.Add( $"{player.Name} consumes a {Name} and recovers {healAmount} health" );  

         Heal heal = new Heal( healAmount );

         RemainingUses--;

         return heal.Perform();
      }
   }
}
