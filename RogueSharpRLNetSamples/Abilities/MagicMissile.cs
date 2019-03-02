using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class MagicMissile : Ability, ITargetable
   {
      private readonly int _attack;
      private readonly int _attackChance;

      public MagicMissile( int attack, int attackChance)
      {
         Name = "Magic Missile";
         TurnsToRefresh = 10;
         TurnsUntilRefreshed = 0;
         _attack = attack;
         _attackChance = attackChance;
      }

      protected override bool PerformAbility()
      {
         return Game.TargetingSystem.SelectMonster( this );
      }

      public void SelectTarget( Point target )
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;
         Monster monster = map.GetMonsterAt( target.X, target.Y );
         if ( monster != null )
         {
            Game.MessageLog.Add( $"{player.Name} casts a {Name} at {monster.Name}" );
            Actor magicMissleActor = new Actor
            {
               Attack = _attack, AttackChance = _attackChance, Name = Name
            };
            Game.CommandSystem.Attack( magicMissleActor, monster );
         }
      }
   }
}
