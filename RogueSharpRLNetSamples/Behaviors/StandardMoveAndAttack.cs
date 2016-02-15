﻿using RogueSharp;
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
         FieldOfView monsterFov = new FieldOfView( dungeonMap );
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
            PathFinder pathFinder = new PathFinder( dungeonMap );
            Path path = pathFinder.ShortestPath( dungeonMap.GetCell( monster.X, monster.Y ), dungeonMap.GetCell( player.X, player.Y ) );
            try
            {
               commandSystem.MoveMonster( monster, path.StepForward() );
            }
            catch ( NoMoreStepsException )
            {
               Game.MessageLog.Add( $"{monster.Name} waits for a turn" );
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
