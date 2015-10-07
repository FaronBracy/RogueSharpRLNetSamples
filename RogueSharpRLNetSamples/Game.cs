using RLNET;
using RogueSharp;
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
}
