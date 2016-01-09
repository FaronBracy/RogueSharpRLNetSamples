using RogueSharp;
using RogueSharp.DiceNotation;

namespace RogueSharpRLNetSamples.Actors
{
   public class Kobold : Monster
   {
      public static Monster Create( int level, Point location )
      {
         int health = Dice.Roll( "2D5" );
         return new Monster {
            Attack = Dice.Roll( "1D3" ) + level / 3,
            AttackChance = Dice.Roll( "25D3" ),
            Awareness = 10,
            Color = Colors.KoboldColor,
            Defense = Dice.Roll( "1D3" ) + level / 3,
            DefenseChance = Dice.Roll( "10D4" ),
            Gold = Dice.Roll( "5D5" ),
            Health = health,
            MaxHealth = health,
            Name = "Kobold",
            Speed = 14,
            Symbol = 'k',
            X = location.X,
            Y = location.Y
         };
      }
   }
}
