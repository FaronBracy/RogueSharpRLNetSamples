﻿using System;
using RLNET;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples
{
   public static class Game
   {
      private static readonly int _screenWidth = 100;
      private static readonly int _screenHeight = 70;
      private static readonly int _mapWidth = 80;
      private static readonly int _mapHeight = 48;
      private static readonly int _messageWidth = 80;
      private static readonly int _messageHeight = 11;
      private static readonly int _statWidth = 20;
      private static readonly int _statHeight = 70;
      private static readonly int _inventoryWidth = 80;
      private static readonly int _inventoryHeight = 11;
      private static RLRootConsole _rootConsole;
      private static RLConsole _mapConsole;
      private static RLConsole _messageConsole;
      private static RLConsole _statConsole;
      private static RLConsole _inventoryConsole;
      private static DungeonMap _map;
      private static bool _renderRequired = true;

      public static bool IsPlayerTurn = false;
      public static Messages Messages;
      public static CommandService CommandService;
      public static ScheduleService ScheduleService;

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
         int seed = (int) DateTime.UtcNow.Ticks;
         MapCreationService mapCreationService = new MapCreationService( _mapWidth, _mapHeight, 20, 13, 7, 1, new DotNetRandom( seed ) );
         _map = mapCreationService.CreateMap();
         Messages = new Messages();
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _mapConsole = new RLConsole( _mapWidth, _mapHeight );
         _messageConsole = new RLConsole( _messageWidth, _messageHeight );
         _statConsole = new RLConsole( _statWidth, _statHeight );
         _inventoryConsole = new RLConsole( _inventoryWidth, _inventoryHeight );
         Messages.Add( "The rogue arrives on level 1" );
         Messages.Add( string.Format( "Level created with seed '{0}'", seed ) );
         CommandService = new CommandService( _map );
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
                  CommandService.MovePlayer( Direction.Up );
               }
               else if ( keyPress.Key == RLKey.Down )
               {
                  CommandService.MovePlayer( Direction.Down );
               }
               else if ( keyPress.Key == RLKey.Left )
               {
                  CommandService.MovePlayer( Direction.Left );
               }
               else if ( keyPress.Key == RLKey.Right )
               {
                  CommandService.MovePlayer( Direction.Right );
               }
               else if ( keyPress.Key == RLKey.Escape )
               {
                  _rootConsole.Close();
               }
               else if ( keyPress.Key == RLKey.Period )
               {
                  if ( _map.CanMoveDownToNextLevel() )
                  {
                     MapCreationService mapCreationService = new MapCreationService( _mapWidth, _mapHeight, 20, 13, 7, 1, new DotNetRandom(), _map.GetPlayer() );
                     _map = mapCreationService.CreateMap();
                     Messages = new Messages();
                     CommandService = new CommandService( _map );
                  }
               }
               IsPlayerTurn = false;
               CommandService.ActivateMonsters();
            }
         }
         else
         {
            CommandService.ActivateMonsters();
         }
      }

      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         if ( _renderRequired )
         {
            _mapConsole.Clear();
            _map.Draw( _mapConsole, _statConsole, _inventoryConsole );
            Messages.Draw( _messageConsole );
            RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight );
            RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0 );
            RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _screenHeight - _messageHeight );
            RLConsole.Blit( _inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0 );
            _rootConsole.Draw();

            _renderRequired = false;
         }
      }
   }
}
