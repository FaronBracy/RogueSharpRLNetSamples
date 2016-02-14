using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class RunAway : IBehavior
   {
      public bool Act( Monster monster, CommandSystem commandSystem )
      {
         DungeonMap dungeonMap = commandSystem.DungeonMap;

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
            commandSystem.MoveMonster( monster, path.StepForward() );
         }
         catch ( NoMoreStepsException )
         {
            Game.Messages.Add( $"{monster.Name} cowers in fear" );
         }
         return true;
      }
   }
}
