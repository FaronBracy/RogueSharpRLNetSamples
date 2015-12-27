using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Actors;

namespace RogueSharpRLNetSamples.Services
{
   public static class ActorCreationService
   {
      private static Player _player = null;

      public static Monster CreateMonster( int level, Point location )
      {
         if ( level == 1 )
         {
            if ( Dice.Roll( "1D100" ) > 75 )
            {
               return CreateOrc( location );
            }
         }

         return CreateGoblin( location );
      }

      private static Monster CreateGoblin( Point location )
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

      private static Monster CreateOrc( Point location )
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

      public static Player CreatePlayer()
      {
         if ( _player == null )
         {
            _player = new Player {
               Attack = 4,
               AttackChance = 60,
               Awareness = 15,
               Color = Colors.Player,
               Defense = 4,
               DefenseChance = 50,
               Gold = 0,
               Health = 100,
               MaxHealth = 100,
               Name = "Rogue",
               Speed = 10,
               Symbol = '@'
            };
         }

         return _player;
      }
   }
}