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
         if ( !monster.IsAlerted )
         {
            monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
            if ( monsterFov.IsInFov( player.X, player.Y ) )
            {
               monster.IsAlerted = true;
            }
         }
         if ( monster.IsAlerted )
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
         }
         return true;
      }
   }
}
