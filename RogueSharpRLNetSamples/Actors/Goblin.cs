using System;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace RogueSharpRLNetSamples.Actors
{
   public class Goblin : Monster
   {
      public static Monster Create( int level, Point location )
      {
         int health = Dice.Roll( "1D5" );
         return new Monster {
            Attack = Dice.Roll( "1D2" ) + level / 3,
            AttackChance = Dice.Roll( "10D5" ),
            Awareness = 10,
            Color = Colors.GoblinColor,
            Defense = Dice.Roll( "1D2" ) + level / 3,
            DefenseChance = Dice.Roll( "10D4" ),
            Gold = Dice.Roll( "1D20" ),
            Health = health,
            MaxHealth = health,
            Name = "Goblin",
            Speed = 12,
            Symbol = 'g',
            X = location.X,
            Y = location.Y
         };
      }
   }
}
