using RogueSharpRLNetSamples.Abilities;
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
         Heal heal = new Heal( 15 );

         RemainingUses--;

         return heal.Perform();
      }
   }
}
