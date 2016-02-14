using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class ShoutForHelp : IBehavior
   {
      public bool Act( Monster monster, CommandService commandService )
      {
         bool didShoutForHelp = false;
         DungeonMap dungeonMap = commandService.DungeonMap;
         FieldOfView monsterFov = new FieldOfView( dungeonMap );

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
            Game.Messages.Add( string.Format( "{0} shouts for help!", monster.Name ) );
         }

         return didShoutForHelp;
      }
   }
}
