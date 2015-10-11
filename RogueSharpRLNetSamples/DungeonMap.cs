using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      private readonly List<Monster> _monsters;
      private Player _player;

      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();

         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public void AddMonster( Monster monster )
      {
         _monsters.Add( monster );
         SetIsWalkable( monster.X, monster.Y, false );
      }

      public void AddPlayer( Player player )
      {
         _player = player;
         SetIsWalkable( _player.X, _player.Y, false );
         UpdatePlayerFieldOfView();
      }

      public void MovePlayer( Direction direction )
      {
         int x;
         int y;

         switch ( direction )
         {
            case Direction.Up:
            {
               x = _player.X;
               y = _player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = _player.X;
               y = _player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = _player.X - 1;
               y = _player.Y;
               break;
            }
            case Direction.Right:
            {
               x = _player.X + 1;
               y = _player.Y;
               break;
            }
            default:
            {
               return;
            }
         }

         if ( GetCell( x, y ).IsWalkable )
         {
            SetIsWalkable( _player.X, _player.Y, true );
            _player.X = x;
            _player.Y = y;
            SetIsWalkable( _player.X, _player.Y, false );
            OpenDoor( x, y );
            UpdatePlayerFieldOfView();
         }
      }

      public void UpdatePlayerFieldOfView()
      {
         ComputeFov( _player.X, _player.Y, 20, true );
         foreach ( Cell cell in GetAllCells() )
         {
            if ( IsInFov( cell.X, cell.Y ) )
            {
               SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
      }

      public Door GetDoor( int x, int y )
      {
         return Doors.SingleOrDefault( d => d.X == x && d.Y == y );
      }

      public void Draw( RLConsole mapConsole, RLConsole statConsole )
      {
         mapConsole.Clear();
         foreach ( Cell cell in GetAllCells() )
         {
            SetConsoleSymbolForCell( mapConsole, cell );
         }

         foreach ( Door door in Doors )
         {
            door.Draw( mapConsole, this );
         }

         StairsUp.Draw( mapConsole, this );
         StairsDown.Draw( mapConsole, this );

         foreach ( Monster monster in _monsters )
         {
            monster.Draw( mapConsole, this );
         }

         _player.Draw( mapConsole );
         _player.DrawStats( statConsole ); 
      }

      private void SetIsWalkable( int x, int y, bool isWalkable )
      {
         Cell cell = GetCell( x, y );
         SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
      }

      private void OpenDoor( int x, int y )
      {
         Door door = GetDoor( x, y );
         if ( door != null )
         {
            door.IsOpen = true;
            SetCellProperties( x, y, true, true, true );
            Game.Messages.Add( "Opened a door" );
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