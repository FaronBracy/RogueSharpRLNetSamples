using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behavior
{
   public class RunAway : IBehavior
   {
      public bool Act( Monster monster, CommandService commandService )
      {
         DungeonMap dungeonMap = commandService.DungeonMap;

         // Set the cell the monster is on to be walkable temporarily so that pathfinder won't bail early
         // TODO: This functionality should be automatically done by the GoalMap pathfinder and not required to do manually here.
         dungeonMap.SetIsWalkable( monster.X, monster.Y, true );

         Player player = dungeonMap.GetPlayer();
         GoalMap goalMap = new GoalMap( dungeonMap );
         goalMap.AddGoal( player.X, player.Y, 0 );
         Path path = goalMap.FindPathAvoidingGoals( monster.X, monster.Y );

         // Reset the cell the monster is on back to not walkable
         dungeonMap.SetIsWalkable( monster.X, monster.Y, false );
         try
         {
            commandService.MoveMonster( monster, path.StepForward() );
         }
         catch ( NoMoreStepsException )
         {
            Game.Messages.Add( string.Format( "{0} cowers in fear", monster.Name ) );
         }
         return true;
      }
   }
}
