using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class MagicMissile : Ability, ITargetable
   {
      public MagicMissile()
      {
         Name = "Magic Missile";
         TurnsToRefresh = 10;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility()
      {
         return Game.TargetingService.SelectMonster( this );
      }

      public void SelectTarget( Point target )
      {
         DungeonMap map = Game.CommandService.DungeonMap;
         Player player = map.GetPlayer();
         Monster monster = map.GetMonsterAt( target.X, target.Y );
         if ( monster != null )
         {
            Game.Messages.Add( $"{player.Name} casts a Magic Missle at {monster.Name}" );
            Actor magicMissleActor = new Actor
            {
               Attack = 3, AttackChance = 80, Name = "Magic Missle"
            };
            Game.CommandService.Attack( magicMissleActor, monster );
         }
      }
   }
}
