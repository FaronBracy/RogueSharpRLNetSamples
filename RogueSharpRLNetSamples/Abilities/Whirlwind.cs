using System.Collections.Generic;
using RogueSharp;
using RogueSharpRLNetSamples.Core;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Whirlwind : Ability
   {
      public Whirlwind()
      {
         Name = "Whirlwind";
         TurnsToRefresh = 20;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility()
      {
         DungeonMap map = Game.DungeonMap;
         Player player = Game.Player;

         Game.MessageLog.Add( $"{player.Name} performs a whirlwind attack against all adjacent enemies" );

         List<Point> monsterLocations = new List<Point>();

         foreach ( Cell cell in map.GetAdjacentCells( player.X, player.Y, true ) )
         {
            foreach ( Point monsterLocation in map.GetMonsterLocations() )
            {
               if ( cell.X == monsterLocation.X && cell.Y == monsterLocation.Y )
               {
                  monsterLocations.Add( monsterLocation );
               }
            }
         }

         foreach ( Point monsterLocation in monsterLocations )
         {
            Monster monster = map.GetMonsterAt( monsterLocation.X, monsterLocation.Y );
            if ( monster != null )
            {
               Game.CommandSystem.Attack( player, monster );
            }
         }

         return true;
      }
   }
}
