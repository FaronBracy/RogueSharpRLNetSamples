using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public List<Monster> Monsters; 
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
         Monsters = new List<Monster>();
      }

      public Door GetDoor( int x, int y )
      {
         return Doors.SingleOrDefault( d => d.X == x && d.Y == y );
      }

      public void Draw( RLConsole console )
      {
         console.Clear();
         foreach ( Cell cell in GetAllCells() )
         {
            SetConsoleSymbolForCell( console, cell );
         }

         foreach ( Door door in Doors )
         {
            door.Draw( console, this );
         }

         StairsUp.Draw( console, this );
         StairsDown.Draw( console, this );

         foreach ( Monster monster in Monsters )
         {
            monster.Draw( console, this );
         }
      }

      private void SetConsoleSymbolForCell( RLConsole console, Cell cell )
      {
         if ( !cell.IsExplored )
         {
            return;
         }

         if ( IsInFov( cell.X, cell.Y ) )
         {
            if ( cell.IsWalkable )
            {
               console.Set( cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.' );
            }
            else
            {
               console.Set( cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#' );
            }
         }
         else
         {
            if ( cell.IsWalkable )
            {
               console.Set( cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.' );
            }
            else
            {
               console.Set( cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#' );
            }
         }
      }
   }
}