using RogueSharp;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class StandardMoveAndAttack : IBehavior
   {
      public bool Act( Monster monster, CommandSystem commandSystem )
      {
         DungeonMap dungeonMap = Game.DungeonMap;
         Player player = Game.Player;
         FieldOfView<DungeonCell> monsterFov = new FieldOfView<DungeonCell>( dungeonMap );
         if ( !monster.TurnsAlerted.HasValue )
         {
            monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
            if ( monsterFov.IsInFov( player.X, player.Y ) )
            {
               Game.MessageLog.Add( $"{monster.Name} is eager to fight {player.Name}" );
               monster.TurnsAlerted = 1;
            }
         }
         if ( monster.TurnsAlerted.HasValue )
         {
            dungeonMap.SetIsWalkable( monster.X, monster.Y, true );
            dungeonMap.SetIsWalkable( player.X, player.Y, true );

            PathFinder<DungeonCell> pathFinder = new PathFinder<DungeonCell>( dungeonMap );
            Path path = null;

            try
            {
               path = pathFinder.ShortestPath( dungeonMap.GetCell( monster.X, monster.Y ), dungeonMap.GetCell( player.X, player.Y ) );
            }
            catch ( PathNotFoundException )
            {
               Game.MessageLog.Add( $"{monster.Name} waits for a turn" );
            }

            dungeonMap.SetIsWalkable( monster.X, monster.Y, false );
            dungeonMap.SetIsWalkable( player.X, player.Y, false );

            if ( path != null )
            {
               try
               {
                  commandSystem.MoveMonster( monster, path.StepForward() );
               }
               catch ( NoMoreStepsException )
               {
                  Game.MessageLog.Add( $"{monster.Name} waits for a turn" );
               }
            }

            monster.TurnsAlerted++;

            // Lose alerted status every 15 turns. As long as the player is still in FoV the monster will be realerted otherwise the monster will quit chasing the player.
            if ( monster.TurnsAlerted > 15 )
            {
               monster.TurnsAlerted = null;
            }
         }
         return true;
      }
   }
}
