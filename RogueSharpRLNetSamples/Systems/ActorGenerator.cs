using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Monsters;

namespace RogueSharpRLNetSamples.Systems
{
   public static class ActorGenerator
   {
      private static Player _player = null;

      public static Monster CreateMonster( int level, Point location )
      {
         Pool<Monster> monsterPool = new Pool<Monster>();
         monsterPool.Add( Kobold.Create( level ), 25 );
         monsterPool.Add( Ooze.Create( level ), 25 );
         monsterPool.Add( Goblin.Create( level ), 50 );

         Monster monster = monsterPool.Get();
         monster.X = location.X;
         monster.Y = location.Y;

         return monster;
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