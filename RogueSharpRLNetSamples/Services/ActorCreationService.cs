using RogueSharp;
using RogueSharp.DiceNotation;

namespace RogueSharpRLNetSamples.Services
{
   public static class ActorCreationService
   {
      public static Monster MakeMonster( int level, Point location )
      {
         if ( level == 1 )
         {
            if ( Dice.Roll( "1D100" ) > 75 )
            {
               return MakeOrc( location );
            }
         }

         return MakeGoblin( location );
      }

      private static Monster MakeGoblin( Point location )
      {
         int health = Dice.Roll( "1D5" );
         return new Monster {
            Attack = Dice.Roll( "1D2" ),
            AttackChance = Dice.Roll( "10D5" ),
            Awareness = 10,
            Color = Colors.GoblinColor,
            Defense = Dice.Roll( "1D2" ),
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

      private static Monster MakeOrc( Point location )
      {
         int health = Dice.Roll( "2D5" );
         return new Monster {
            Attack = Dice.Roll( "1D3" ),
            AttackChance = Dice.Roll( "25D3" ),
            Awareness = 10,
            Color = Colors.GoblinColor,
            Defense = Dice.Roll( "1D3" ),
            DefenseChance = Dice.Roll( "10D4" ),
            Gold = Dice.Roll( "5D5" ),
            Health = health,
            MaxHealth = health,
            Name = "Orc",
            Speed = 14,
            Symbol = 'o',
            X = location.X,
            Y = location.Y
         };
      }
   }
}