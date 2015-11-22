using RLNET;

namespace RogueSharpRLNetSamples
{
   public interface IActor
   {
      int Attack { get; set; }
      int AttackChance { get; set; }
      int Awareness { get; set; }
      int Defense { get; set; }
      int DefenseChance { get; set; }
      int Gold { get; set; }
      int Health { get; set; }
      int MaxHealth { get; set; }
      string Name { get; set; }
      int Speed { get; set; }
   }

   public interface IDrawable
   {
      RLColor Color { get; set; }
      char Symbol { get; set; }
      int X { get; set; }
      int Y { get; set; }
   }

   public class Actor : IActor, IDrawable
   {
      public int Attack { get; set; }
      public int AttackChance { get; set; }
      public int Awareness { get; set; }
      public RLColor Color { get; set; }
      public int Defense { get; set; }
      public int DefenseChance { get; set; }
      public int Gold { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public string Name { get; set; }
      public int Speed { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
   }
}