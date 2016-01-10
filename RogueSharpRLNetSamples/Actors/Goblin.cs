using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behavior;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Actors
{
   public class Goblin : Monster
   {
      public static Goblin Create( int level, Point location )
      {
         int health = Dice.Roll( "1D5" );
         return new Goblin {
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

      public override void PerformAction( CommandService commandService )
      {
         var standardBehavior = new StandardMoveAndAttack();
         var runAwayBehavior = new RunAway();
         if ( Health < MaxHealth )
         {
            runAwayBehavior.Act( this, commandService );
         }
         else
         {
            standardBehavior.Act( this, commandService );
         }
      }
   }
}
