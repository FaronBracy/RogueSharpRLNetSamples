using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behavior
{
   public class SplitOoze : IBehavior
   {
      public bool Act( Monster monster, CommandService commandService )
      {
         // Ooze only splits when wounded
         if ( monster.Health >= monster.MaxHealth )
         {
            return false;
         }

         int halfHealth = monster.MaxHealth / 2;
         if ( halfHealth <= 0 )
         {
            // Health would be too low so bail out
            return false;
         }

         Cell cell = FindClosestUnoccupiedCell( commandService.DungeonMap, monster.X, monster.Y );
         
         if ( cell == null )
         {
            // No empty cells so bail out
            return false;
         }

         // Make a new ooze with half the health of the old one
         Ooze newOoze = Monster.Clone( monster ) as Ooze;
         if ( newOoze != null )
         {
            newOoze.TurnsAlerted = 1;
            newOoze.X = cell.X;
            newOoze.Y = cell.Y;
            newOoze.MaxHealth = halfHealth;
            newOoze.Health = halfHealth;
            commandService.DungeonMap.AddMonster( newOoze );
            Game.Messages.Add( string.Format( "{0} splits itself in two", monster.Name ) );
         }
         else
         {
            // Not an ooze so bail out
            return false;
         }

         // Halve the original ooze's health too
         monster.MaxHealth = halfHealth;
         monster.Health = halfHealth;

         return true;
      }

      private Cell FindClosestUnoccupiedCell( DungeonMap dungeonMap, int x, int y )
      {
         for ( int i = 1; i < 5; i++ )
         {
            foreach ( Cell cell in dungeonMap.GetBorderCellsInArea( x, y, i ) )
            {
               if ( cell.IsWalkable )
               {
                  return cell;
               }
            }
         }

         return null;
      }
   }
}
