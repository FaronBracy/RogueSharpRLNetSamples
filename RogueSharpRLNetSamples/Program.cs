using System;
using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharp.Random;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
      private static readonly int _screenWidth = 80;
      private static readonly int _screenHeight = 50;
      private static readonly int _mapWidth = 80;
      private static readonly int _mapHeight = 45;
      private static int _playerX;
      private static int _playerY;
      private static RLRootConsole _rootConsole;
      private static RLConsole _mapConsole;
      private static DungeonMap _map;

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
         DungeonMapCreationStrategy mapCreationStrategy = new DungeonMapCreationStrategy( _mapWidth, _mapHeight, 20, 13, 7, Singleton.DefaultRandom );
         _map = mapCreationStrategy.CreateMap();
         _playerX = _map.Rooms[0].Center.X;
         _playerY = _map.Rooms[0].Center.Y;
         UpdateFov();
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _rootConsole.Update += OnRootConsoleUpdate;
         _rootConsole.Render += OnRootConsoleRender;
         _rootConsole.Run();
      }

      private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e )
      {
         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
         if ( keyPress != null )
         {
            if ( keyPress.Key == RLKey.Up )
            {
               if ( _map.GetCell( _playerX, _playerY - 1 ).IsWalkable )
               {
                  _playerY--;
                  OpenDoor( _playerX, _playerY );
                  UpdateFov();
               }
            }
            else if ( keyPress.Key == RLKey.Down )
            {
               if ( _map.GetCell( _playerX, _playerY + 1 ).IsWalkable )
               {
                  _playerY++;
                  OpenDoor( _playerX, _playerY );
                  UpdateFov();
               }
            }
            else if ( keyPress.Key == RLKey.Left )
            {
               if ( _map.GetCell( _playerX - 1, _playerY ).IsWalkable )
               {
                  _playerX--;
                  OpenDoor( _playerX, _playerY );
                  UpdateFov();
               }
            }
            else if ( keyPress.Key == RLKey.Right )
            {
               if ( _map.GetCell( _playerX + 1, _playerY ).IsWalkable )
               {
                  _playerX++;
                  OpenDoor( _playerX, _playerY );
                  UpdateFov();
               }
            }
            else if ( keyPress.Key == RLKey.Escape )
            {
               _rootConsole.Close();
            }
         }
      }

      private static void OpenDoor( int x, int y )
      {
         Door door = _map.GetDoor( x, y );
         if ( door != null )
         {
            door.IsOpen = true;
            _map.SetCellProperties( x, y, true, true, true );
         }
      }

      private static void UpdateFov()
      {
         _map.ComputeFov( _playerX, _playerY, 20, true );
         foreach ( Cell cell in _map.GetAllCells() )
         {
            if ( _map.IsInFov( cell.X, cell.Y ) )
            {
               _map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
      }

      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         _rootConsole.Clear();
         foreach ( Cell cell in _map.GetAllCells() )
         {
            if ( !cell.IsExplored )
            {
               continue;
            }
            if ( _map.IsInFov( cell.X, cell.Y ) )
            {
               if ( cell.IsWalkable )
               {
                  _rootConsole.Set( cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.' );
               }
               else
               {
                  _rootConsole.Set( cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#' );
               }
            }
            else
            {
               if ( cell.IsWalkable )
               {
                  _rootConsole.Set( cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.' );
               }
               else
               {
                  _rootConsole.Set( cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#' );
               }
            }
         }
         foreach ( Door door in _map.Doors )
         {
            if ( !_map.GetCell( door.X, door.Y ).IsExplored )
            {
               continue;
            }
            if ( _map.IsInFov( door.X, door.Y ) )
            {
               if ( door.IsOpen )
               {
                  _rootConsole.Set( door.X, door.Y, Colors.DoorFov, Colors.DoorBackgroundFov, '-' );
               }
               else
               {
                  _rootConsole.Set( door.X, door.Y, Colors.DoorFov, Colors.DoorBackgroundFov, '+' );
               }
            }
            else
            {
               if ( door.IsOpen )
               {
                  _rootConsole.Set( door.X, door.Y, Colors.Door, Colors.DoorBackground, '-' );
               }
               else
               {
                  _rootConsole.Set( door.X, door.Y, Colors.Door, Colors.DoorBackground, '+' );
               }
            }

         }
         _rootConsole.Set( _playerX, _playerY, Colors.Player, null, '@' );
         _rootConsole.Draw();
      }
   }

   public class DungeonMapCreationStrategy : IMapCreationStrategy<DungeonMap>
   {
      private readonly IRandom _random;
      private readonly int _height;
      private readonly int _maxRooms;
      private readonly int _roomMaxSize;
      private readonly int _roomMinSize;
      private readonly int _width;
      private readonly DungeonMap _map;

      public DungeonMapCreationStrategy( int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, IRandom random )
      {
         _width = width;
         _height = height;
         _maxRooms = maxRooms;
         _roomMaxSize = roomMaxSize;
         _roomMinSize = roomMinSize;
         _random = random;
         _map = new DungeonMap();
      }

      public DungeonMap CreateMap()
      {
         _map.Initialize( _width, _height );

         for ( int r = 0; r < _maxRooms; r++ )
         {
            int roomWidth = _random.Next( _roomMinSize, _roomMaxSize );
            int roomHeight = _random.Next( _roomMinSize, _roomMaxSize );
            int roomXPosition = _random.Next( 0, _width - roomWidth - 1 );
            int roomYPosition = _random.Next( 0, _height - roomHeight - 1 );

            var newRoom = new Rectangle( roomXPosition, roomYPosition, roomWidth, roomHeight );
            bool newRoomIntersects = _map.Rooms.Any( room => newRoom.Intersects( room ) );
            if ( !newRoomIntersects )
            {
               _map.Rooms.Add( newRoom );
            }
         }

         foreach ( Rectangle room in _map.Rooms )
         {
            MakeRoom( room );
         }

         for ( int r = 0; r < _map.Rooms.Count; r++ )
         {
            if ( r == 0 )
            {
               continue;
            }

            int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
            int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
            int currentRoomCenterX = _map.Rooms[r].Center.X;
            int currentRoomCenterY = _map.Rooms[r].Center.Y;

            if ( _random.Next( 0, 2 ) == 0 )
            {
               MakeHorizontalTunnel( previousRoomCenterX, currentRoomCenterX, previousRoomCenterY );
               MakeVerticalTunnel( previousRoomCenterY, currentRoomCenterY, currentRoomCenterX );
            }
            else
            {
               MakeVerticalTunnel( previousRoomCenterY, currentRoomCenterY, previousRoomCenterX );
               MakeHorizontalTunnel( previousRoomCenterX, currentRoomCenterX, currentRoomCenterY );
            }
         }

         foreach ( Rectangle room in _map.Rooms )
         {
            MakeDoors( room );
         }

         return _map;
      }

      private void MakeRoom( Rectangle room )
      {
         for ( int x = room.Left + 1; x < room.Right; x++ )
         {
            for ( int y = room.Top + 1; y < room.Bottom; y++ )
            {
               _map.SetCellProperties( x, y, true, true );
            }
         }
      }

      private void MakeHorizontalTunnel( int xStart, int xEnd, int yPosition )
      {
         for ( int x = Math.Min( xStart, xEnd ); x <= Math.Max( xStart, xEnd ); x++ )
         {
            _map.SetCellProperties( x, yPosition, true, true );
         }
      }

      private void MakeVerticalTunnel( int yStart, int yEnd, int xPosition )
      {
         for ( int y = Math.Min( yStart, yEnd ); y <= Math.Max( yStart, yEnd ); y++ )
         {
            _map.SetCellProperties( xPosition, y, true, true );
         }
      }

      private void MakeDoors( Rectangle room )
      {
         int xMin = room.Left;
         int xMax = room.Right;
         int yMin = room.Top;
         int yMax = room.Bottom;

         List<Cell> borderCells = _map.GetCellsAlongLine( xMin, yMin, xMax, yMin ).ToList();
         borderCells.AddRange( _map.GetCellsAlongLine( xMin, yMin, xMin, yMax ) );
         borderCells.AddRange( _map.GetCellsAlongLine( xMin, yMax, xMax, yMax ) );
         borderCells.AddRange( _map.GetCellsAlongLine( xMax, yMin, xMax, yMax ) );

         foreach ( Cell cell in borderCells )
         {
            if ( IsPotentialDoor( cell ) )
            {
               _map.SetCellProperties( cell.X, cell.Y, false, true );
               _map.Doors.Add( new Door {
                  X = cell.X,
                  Y = cell.Y,
                  IsOpen = false
               } );
            }
         }
      }

      private bool IsPotentialDoor( Cell cell )
      {
         if ( !cell.IsWalkable )
         {
            return false;
         }

         Cell right = _map.GetCell( cell.X + 1, cell.Y );
         Cell left = _map.GetCell( cell.X - 1, cell.Y );
         Cell top = _map.GetCell( cell.X, cell.Y - 1 );
         Cell bottom = _map.GetCell( cell.X, cell.Y + 1 );

         if ( _map.GetDoor( cell.X, cell.Y ) != null ||
            _map.GetDoor( right.X, right.Y ) != null ||
            _map.GetDoor( left.X, left.Y ) != null ||
            _map.GetDoor( top.X, top.Y ) != null ||
            _map.GetDoor( bottom.X, bottom.Y ) != null )
         {
            return false;
         }

         if ( right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable )
         {
            return true;
         }
         if ( !right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable )
         {
            return true;
         }
         return false;
      }
   }

   public class DungeonMap : Map
   {
      public List<Rectangle> Rooms;
      public List<Door> Doors;

      public DungeonMap()
      {
         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public Door GetDoor( int x, int y )
      {
         return Doors.SingleOrDefault( d => d.X == x && d.Y == y );
      }
   }

   public class Door
   {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsOpen { get; set; }
   }

   public static class Colors
   {
      public static RLColor DoorBackground = Swatch.ComplimentDarkest;
      public static RLColor Door = Swatch.ComplimentLighter;
      public static RLColor DoorBackgroundFov = Swatch.ComplimentDarker;
      public static RLColor DoorFov = Swatch.ComplimentLightest;
      public static RLColor FloorBackground = RLColor.Black;
      public static RLColor Floor = Swatch.AlternateDarkest;
      public static RLColor FloorBackgroundFov = RLColor.Black;
      public static RLColor FloorFov = Swatch.Alternate;
      public static RLColor WallBackground = Swatch.SecondaryDarkest;
      public static RLColor Wall = Swatch.Secondary;
      public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
      public static RLColor WallFov = Swatch.SecondaryLighter;
      public static RLColor Player = RLColor.White;
   }

   public static class Swatch
   {
      // http://paletton.com/#uid=73d0u0k5qgb2NnT41jT74c8bJ8X

      public static RLColor PrimaryLightest = new RLColor( 110, 121, 119 );
      public static RLColor PrimaryLighter = new RLColor( 88, 100, 98 );
      public static RLColor Primary = new RLColor( 68, 82, 79 );
      public static RLColor PrimaryDarker = new RLColor( 48, 61, 59 );
      public static RLColor PrimaryDarkest = new RLColor( 29, 45, 42 );

      public static RLColor SecondaryLightest = new RLColor( 116, 120, 126 );
      public static RLColor SecondaryLighter = new RLColor( 93, 97, 105 );
      public static RLColor Secondary = new RLColor( 72, 77, 85 );
      public static RLColor SecondaryDarker = new RLColor( 51, 56, 64 );
      public static RLColor SecondaryDarkest = new RLColor( 31, 38, 47 );

      public static RLColor AlternateLightest = new RLColor( 190, 184, 174 );
      public static RLColor AlternateLighter = new RLColor( 158, 151, 138 );
      public static RLColor Alternate = new RLColor( 129, 121, 107 );
      public static RLColor AlternateDarker = new RLColor( 97, 89, 75 );
      public static RLColor AlternateDarkest = new RLColor( 71, 62, 45 );

      public static RLColor ComplimentLightest = new RLColor( 190, 180, 174 );
      public static RLColor ComplimentLighter = new RLColor( 158, 147, 138 );
      public static RLColor Compliment = new RLColor( 129, 116, 107 );
      public static RLColor ComplimentDarker = new RLColor( 97, 84, 75 );
      public static RLColor ComplimentDarkest = new RLColor( 71, 56, 45 );
   }
}
