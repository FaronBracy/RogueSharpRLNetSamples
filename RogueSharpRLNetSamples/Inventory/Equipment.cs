using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Inventory
{
   public class Equipment : IHasStats
   {
      public int Attack { get; set; }
      public int AttackChance { get; set; }
      public int Awareness { get; set; }
      public int Defense { get; set; }
      public int DefenseChance { get; set; }
      public int Gold { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public string Name { get; set; }
      public int Speed { get; set; }
   }
}