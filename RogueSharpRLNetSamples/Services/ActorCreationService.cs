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
         int result = Dice.Roll( "1D100" );

         if ( result < 25 )
         {
            return Kobold.Create( level, location );
         }
         if ( result < 50 )
         {
            return Ooze.Create( level, location );
         }
         return Goblin.Create( level, location );
      }


      public static Player CreatePlayer()
      {
         if ( _player == null )
         {
            _player = new Player {
               Attack = 2,
               AttackChance = 50,
               Awareness = 15,
               Color = Colors.Player,
               Defense = 2,
               DefenseChance = 40,
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