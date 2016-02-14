﻿using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Actors
{
   public class Goblin : Monster
   {
      private int? _turnsSpentRunning = null;
      private bool _shoutedForHelp = false;

      public static Goblin Create( int level )
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
            Symbol = 'g'
         };
      }

      public override void PerformAction( CommandService commandService )
      {
         var fullyHealBehavior = new FullyHeal();
         var runAwayBehavior = new RunAway();
         var shoutForHelpBehavior = new ShoutForHelp();

         if ( _turnsSpentRunning.HasValue && _turnsSpentRunning.Value > 15 )
         {
            fullyHealBehavior.Act( this, commandService );
            _turnsSpentRunning = null;
         }
         else if ( Health < MaxHealth )
         {
            runAwayBehavior.Act( this, commandService );
            if ( _turnsSpentRunning.HasValue )
            {
               _turnsSpentRunning += 1;
            }
            else
            {
               _turnsSpentRunning = 1;
            }

            if ( !_shoutedForHelp )
            {
               _shoutedForHelp = shoutForHelpBehavior.Act( this, commandService );
            }
         }
         else
         {
            base.PerformAction( commandService );
         }
      }
   }
}
