using System;
using RLNET;
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
      private static bool _renderRequired = true;

      public static bool IsPlayerTurn = false;
      public static Messages Messages;

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
         int seed = (int) DateTime.UtcNow.Ticks;
         DungeonMapCreationStrategy mapCreationStrategy = new DungeonMapCreationStrategy( _mapWidth, _mapHeight, 20, 13, 7, new DotNetRandom( seed ) );
         _map = mapCreationStrategy.CreateMap();
         Messages = new Messages();
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _mapConsole = new RLConsole( _mapWidth, _mapHeight );
         _messageConsole = new RLConsole( _messageWidth, _messageHeight );
         _statConsole = new RLConsole( _statWidth, _statHeight );
         Messages.Add( "The rogue arrives on level 1" );
         Messages.Add( string.Format( "Level created with seed '{0}'", seed ) );
         _rootConsole.Update += OnRootConsoleUpdate;
         _rootConsole.Render += OnRootConsoleRender;
         _rootConsole.Run();
      }

      private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e )
      {
         if ( IsPlayerTurn )
         {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if ( keyPress != null )
            {
               _renderRequired = true;
               if ( keyPress.Key == RLKey.Up )
               {
                  _map.MovePlayer( Direction.Up );
               }
               else if ( keyPress.Key == RLKey.Down )
               {
                  _map.MovePlayer( Direction.Down );
               }
               else if ( keyPress.Key == RLKey.Left )
               {
                  _map.MovePlayer( Direction.Left );
               }
               else if ( keyPress.Key == RLKey.Right )
               {
                  _map.MovePlayer( Direction.Right );
               }
               else if ( keyPress.Key == RLKey.Escape )
               {
                  _rootConsole.Close();
               }
               IsPlayerTurn = false;
               _map.ActivateMonsters();
            }
         }
         else
         {
            _map.ActivateMonsters();
         }
      }

      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         if ( _renderRequired )
         {
            _mapConsole.Clear();
            _map.Draw( _mapConsole, _statConsole );
            Messages.Draw( _messageConsole );
            RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, 0 );
            RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0 );
            RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight );
            _rootConsole.Draw();

            _renderRequired = false;
         }
      }
   }
}
