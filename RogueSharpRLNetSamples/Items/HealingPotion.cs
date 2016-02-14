﻿using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Actors;

namespace RogueSharpRLNetSamples.Items
{
   public class HealingPotion : Item
   {
      public HealingPotion()
      {
         Name = "Healing Potion";
         RemainingUses = 1;
      }

      protected override bool UseItem()
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
