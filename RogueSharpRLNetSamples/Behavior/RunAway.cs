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
         Cell cell = dungeonMap.GetCell( monster.X, monster.Y );
         dungeonMap.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, true, cell.IsExplored );  
         Player player = dungeonMap.GetPlayer();
         GoalMap goalMap = new GoalMap( dungeonMap );
         goalMap.AddGoal( player.X, player.Y, 0 );
         Path path = goalMap.FindPathAvoidingGoals( monster.X, monster.Y );
         dungeonMap.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, false, cell.IsExplored );
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
