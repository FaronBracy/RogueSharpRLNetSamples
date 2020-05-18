using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class ShoutForHelp : IBehavior
   {
      public bool Act( Monster monster, CommandSystem commandSystem )
      {
         bool didShoutForHelp = false;
         DungeonMap dungeonMap = Game.DungeonMap;
         FieldOfView<DungeonCell> monsterFov = new FieldOfView<DungeonCell>( dungeonMap );

         monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
         foreach ( var monsterLocation in dungeonMap.GetMonsterLocations() )
         {
            if ( monsterFov.IsInFov( monsterLocation.X, monsterLocation.Y ) )
            {
               Monster alertedMonster = dungeonMap.GetMonsterAt( monsterLocation.X, monsterLocation.Y );
               if ( !alertedMonster.TurnsAlerted.HasValue )
               {
                  alertedMonster.TurnsAlerted = 1;
                  didShoutForHelp = true;
               }
            }
         }

         if ( didShoutForHelp )
         {
            Game.MessageLog.Add( $"{monster.Name} shouts for help!" );
         }

         return didShoutForHelp;
      }
   }
}
