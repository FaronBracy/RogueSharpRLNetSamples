using System;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Actors
{
   public class Monster : Actor
   {
      public void DrawStats( RLConsole statConsole, int position )
      {
         int yPosition = 13 + ( position * 2 );
         statConsole.Print( 1, yPosition, Symbol.ToString(), Color );
         int width = Convert.ToInt32( ( (double) Health / (double) MaxHealth ) * 16.0 );
         int remainingWidth = 16 - width;
         statConsole.SetBackColor( 3, yPosition, width, 1, Swatch.Primary );
         statConsole.SetBackColor( 3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );  
         statConsole.Print( 2, yPosition, string.Format( ": {0}", Name ), RLColor.White );
      }

      public void Draw( RLConsole mapConsole, IMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            mapConsole.Set( X, Y, Color, null, Symbol );
         }
         else
         {
            mapConsole.Set( X, Y, Colors.Floor, Colors.FloorBackground, '.' );
         }
      }

      public virtual void PerformAction( CommandService commandService )
      {
         DungeonMap dungeonMap = commandService.DungeonMap;
         Player player = dungeonMap.GetPlayer();
         FieldOfView monsterFov = new FieldOfView( dungeonMap );
         monsterFov.ComputeFov( X, Y, Awareness, true );
         if ( monsterFov.IsInFov( player.X, player.Y ) )
         {
            PathFinder pathFinder = new PathFinder( dungeonMap );
            Path path = pathFinder.ShortestPath( dungeonMap.GetCell( X, Y ), dungeonMap.GetCell( player.X, player.Y ) );
            try
            {
               commandService.MoveMonster( this, path.StepForward() );
            }
            catch ( NoMoreStepsException )
            {
               Game.Messages.Add( string.Format( "{0} waits for a turn", Name ) );
            }
         }
      }
   }
}