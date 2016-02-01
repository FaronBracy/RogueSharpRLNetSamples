using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class NoItem : IItem
   {
      public string Name { get; }

      public int RemainingUses { get; }

      public NoItem()
      {
         Name = "None";
         RemainingUses = 1;
      }

      public bool Use()
      {
         return false;
      }
   }
}
