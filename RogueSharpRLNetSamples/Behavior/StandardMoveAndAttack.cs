using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behavior
{
   public class StandardMoveAndAttack : IBehavior
   {
      public bool Act( Monster monster, CommandService commandService )
      {
         DungeonMap dungeonMap = commandService.DungeonMap;
         Player player = dungeonMap.GetPlayer();
         FieldOfView monsterFov = new FieldOfView( dungeonMap );
         if ( !monster.TurnsAlerted.HasValue )
         {
            monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
            if ( monsterFov.IsInFov( player.X, player.Y ) )
            {
               Game.Messages.Add( string.Format( "{0} is eager to fight {1}", monster.Name, player.Name ) );
               monster.TurnsAlerted = 1;
            }
         }
         if ( monster.TurnsAlerted.HasValue )
         {
            PathFinder pathFinder = new PathFinder( dungeonMap );
            Path path = pathFinder.ShortestPath( dungeonMap.GetCell( monster.X, monster.Y ), dungeonMap.GetCell( player.X, player.Y ) );
            try
            {
               commandService.MoveMonster( monster, path.StepForward() );
            }
            catch ( NoMoreStepsException )
            {
               Game.Messages.Add( string.Format( "{0} waits for a turn", monster.Name ) );
            }
            monster.TurnsAlerted++;
            // Lose alerted status every 8 turns. As long as the player is still in FoV the monster will be realerted otherwise the monster will quit chasing the player.
            if ( monster.TurnsAlerted > 15 )
            {
               monster.TurnsAlerted = null;
            }
         }
         return true;
      }
   }
}
