using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Fireball : Ability, ITargetable
   {
      private readonly int _attack;
      private readonly int _attackChance;
      private readonly int _area;

      public Fireball( int attack, int attackChance, int area )
      {
         Name = "Fireball";
         TurnsToRefresh = 40;
         TurnsUntilRefreshed = 0;
         _attack = attack;
         _attackChance = attackChance;
         _area = area;
      }

      protected override bool PerformAbility()
      {
         return Game.TargetingSystem.SelectArea( this, _area );
      }

      public void SelectTarget( Point target )
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;
         Game.MessageLog.Add( $"{player.Name} casts a {Name}" );
         Actor fireballActor = new Actor {
            Attack = _attack,
            AttackChance = _attackChance,
            Name = Name
         };
         foreach ( Cell cell in map.GetCellsInSquare( target.X, target.Y, _area ) )
         {
            Monster monster = map.GetMonsterAt( cell.X, cell.Y );
            if ( monster != null )
            {
               Game.CommandSystem.Attack( fireballActor, monster );
            }
         }
      }
   }
}
