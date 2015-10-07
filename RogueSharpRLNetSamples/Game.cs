using System;
using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharp.MapCreation;
using RogueSharp.Random;

namespace RogueSharpRLNetSamples
{
   public static class Game
   {
      private static readonly int _screenWidth = 100;
      private static readonly int _screenHeight = 50;
      private static readonly int _mapWidth = 80;
      private static readonly int _mapHeight = 45;
      private static readonly int _messageHeight = 5;
      private static readonly int _messageWidth = 80;
      private static readonly int _statWidth = 20;
      private static readonly int _statHeight = 50;
      private static RLRootConsole _rootConsole;
      private static RLConsole _mapConsole;
      private static RLConsole _messageConsole;
      private static RLConsole _statConsole;
      private static DungeonMap _map;
      private static Player _player;
      private static Messages _messages;

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
         DungeonMapCreationStrategy mapCreationStrategy = new DungeonMapCreationStrategy( _mapWidth, _mapHeight, 20, 13, 7, Singleton.DefaultRandom );
         _map = mapCreationStrategy.CreateMap();
         _messages = new Messages();
         _player = new Player {
            X = _map.Rooms[0].Center.X,
            Y = _map.Rooms[0].Center.Y
         };
         UpdatePlayerFieldOfView();
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _mapConsole = new RLConsole( _mapWidth, _mapHeight );
         _messageConsole = new RLConsole( _messageWidth, _messageHeight );
         _statConsole = new RLConsole( _statWidth, _statHeight );
         _messages.Add( "The rogue arrives on level 1" );
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
               MovePlayer( _player.X, _player.Y - 1 );
            }
            else if ( keyPress.Key == RLKey.Down )
            {
               MovePlayer( _player.X, _player.Y + 1 );
            }
            else if ( keyPress.Key == RLKey.Left )
            {
               MovePlayer( _player.X - 1, _player.Y );
            }
            else if ( keyPress.Key == RLKey.Right )
            {
               MovePlayer( _player.X + 1, _player.Y );
            }
            else if ( keyPress.Key == RLKey.Escape )
            {
               _rootConsole.Close();
            }
         }
      }

      private static void MovePlayer( int x, int y )
      {
         if ( _map.GetCell( x, y ).IsWalkable )
         {
            _player.X = x;
            _player.Y = y;
            OpenDoor( x, y );
            UpdatePlayerFieldOfView();
         }
      }

      public static void OpenDoor( int x, int y )
      {
         Door door = _map.GetDoor( x, y );
         if ( door != null )
         {
            door.IsOpen = true;
            _map.SetCellProperties( x, y, true, true, true );
            _messages.Add( "Opened a door" );
         }
      }

      private static void UpdatePlayerFieldOfView()
      {
         _map.ComputeFov( _player.X, _player.Y, 20, true );
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
         _mapConsole.Clear();
         _map.Draw( _mapConsole );
         _player.Draw( _mapConsole );
         _player.DrawStats( _statConsole );  
         _messages.Draw( _messageConsole );
         RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, 0 );
         RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0 );  
         RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight );
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

         MakeStairs();

         MakeMonsters();

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

      private void MakeStairs()
      {
         _map.StairsUp = new Stairs {
            X = _map.Rooms.First().Center.X + 1,
            Y = _map.Rooms.First().Center.Y,
            IsUp = true
         };
         _map.StairsDown = new Stairs {
            X = _map.Rooms.Last().Center.X,
            Y = _map.Rooms.Last().Center.Y,
            IsUp = false
         };
      }

      private void MakeMonsters()
      {
         // Place encounters that are well thought out in rooms

         // Place random monsters anywhere on floor tiles

         // Add a few more monsters to rooms
         foreach ( var room in _map.Rooms )
         {
            if ( Dice.Roll( "1D10" ) < 7 )
            {
               _map.Monsters.Add( MakeGoblin( GetRandomLocationInRoom( room ) ) );
            }
         }
      }

      private Monster MakeGoblin( Point location )
      {
         return new Monster 
         {
            Armor = 3,
            Attack = 3,
            Health = 10,
            MaxHealth = 10,
            Name = "Goblin",
            Symbol = 'g',
            Color = RLColor.Green,
            X = location.X,
            Y = location.Y
         };
      }

      private Point GetRandomLocationInRoom( Rectangle room )
      {
         int x = _random.Next( room.Width - 1 ) + room.X;
         int y = _random.Next( room.Height - 1 ) + room.Y;
         return new Point( x, y );
      }
   }

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
            monster.Draw( console );
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

   public class Door
   {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsOpen { get; set; }

      public void Draw( RLConsole console, IMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            if ( IsOpen )
            {
               console.Set( X, Y, Colors.DoorFov, Colors.DoorBackgroundFov, '-' );
            }
            else
            {
               console.Set( X, Y, Colors.DoorFov, Colors.DoorBackgroundFov, '+' );
            }
         }
         else
         {
            if ( IsOpen )
            {
               console.Set( X, Y, Colors.Door, Colors.DoorBackground, '-' );
            }
            else
            {
               console.Set( X, Y, Colors.Door, Colors.DoorBackground, '+' );
            }
         }
      }
   }

   public class Player
   {
      public int X { get; set; }
      public int Y { get; set; }

      public int Gold { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public int Armor { get; set; }
      public int Attack { get; set; }

      public void DrawStats( RLConsole console )
      {
         console.Clear();
         console.Print( 1, 1, string.Format( "Health: {0}/{1}", Health, MaxHealth ), RLColor.White );
         console.Print( 1, 3, string.Format( "Attack:  {0}", Attack ), RLColor.White );
         console.Print( 1, 5, string.Format( "Armor:   {0}", Armor ), RLColor.White );
         console.Print( 1, 7, string.Format( "Gold:    {0}", Armor ), RLColor.Yellow );
      }

      public void Draw( RLConsole console )
      {
         console.Set( X, Y, Colors.Player, null, '@' );
      }
   }

   public class Monster
   {
      public int X { get; set; }
      public int Y { get; set; }

      public char Symbol { get; set; }
      public RLColor Color { get; set; }

      public string Name { get; set; }
      public int Health { get; set; }
      public int MaxHealth { get; set; }
      public int Armor { get; set; }
      public int Attack { get; set; }

      public void Draw( RLConsole console )
      {
         console.Set( X, Y, Color, null, Symbol );
      }
   }

   public class Stairs
   {
      public int X { get; set; }
      public int Y { get; set; }
      public bool IsUp { get; set; }

      public void Draw( RLConsole console, IMap map )
      {
         if ( !map.GetCell( X, Y ).IsExplored )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            if ( IsUp )
            {
               console.Set( X, Y, Colors.Player, null, '<' );
            }
            else
            {
               console.Set( X, Y, Colors.Player, null, '>' );
            }
         }
         else
         {
            if ( IsUp )
            {
               console.Set( X, Y, Colors.Floor, null, '<' );
            }
            else
            {
               console.Set( X, Y, Colors.Floor, null, '>' );
            }
         }
      }
   }

   public class Messages
   {
      private readonly Queue<string> _lines;

      public Messages()
      {
         _lines = new Queue<string>();
      }

      public void Add( string message )
      {
         _lines.Enqueue( message );
         if ( _lines.Count > 5 )
         {
            _lines.Dequeue();
         }
      }

      public void Draw( RLConsole console )
      {
         console.Clear();
         string[] lines = _lines.ToArray();
         for ( int i = 0; i < lines.Count(); i++ )
         {
            console.Print( 1, i, lines[i], RLColor.White );
         }
      }
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
