using System;
using RLNET;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Items;
using RogueSharpRLNetSamples.Systems;

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

      private static int _mapLevel = 1;
      private static bool _renderRequired = true;

      public static Player Player { get; set; }
      public static DungeonMap DungeonMap { get; private set; }
      public static MessageLog MessageLog { get; private set; }
      public static CommandSystem CommandSystem { get; private set; }
      public static SchedulingSystem SchedulingSystem { get; private set; }
      public static TargetingSystem TargetingSystem { get; private set; }
      public static IRandom Random { get; private set; }

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial - Level 1";
         int seed = (int) DateTime.UtcNow.Ticks;
         Random = new DotNetRandom( seed );

         MessageLog = new MessageLog();
         MessageLog.Add( "The rogue arrives on level 1" );
         MessageLog.Add( $"Level created with seed '{seed}'" );

         Player = new Player();
         SchedulingSystem = new SchedulingSystem();

         MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 20, 13, 7, _mapLevel );
         DungeonMap = mapGenerator.CreateMap();

         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _mapConsole = new RLConsole( _mapWidth, _mapHeight );
         _messageConsole = new RLConsole( _messageWidth, _messageHeight );
         _statConsole = new RLConsole( _statWidth, _statHeight );
         _inventoryConsole = new RLConsole( _inventoryWidth, _inventoryHeight );

         CommandSystem = new CommandSystem();
         TargetingSystem = new TargetingSystem();

         Player.Item1 = new RevealMapScroll();
         Player.Item2 = new RevealMapScroll();

         _rootConsole.Update += OnRootConsoleUpdate;
         _rootConsole.Render += OnRootConsoleRender;
         _rootConsole.Run();
      }

      private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e )
      {
         bool didPlayerAct = false;
         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

         if ( TargetingSystem.IsPlayerTargeting )
         {
            if ( keyPress != null )
            {
               _renderRequired = true;
               TargetingSystem.HandleKey( keyPress.Key );
            }
         }
         else if ( CommandSystem.IsPlayerTurn )
         {
            if ( keyPress != null )
            {
               if ( keyPress.Key == RLKey.Up )
               {
                  didPlayerAct = CommandSystem.MovePlayer( Direction.Up );
               }
               else if ( keyPress.Key == RLKey.Down )
               {
                  didPlayerAct = CommandSystem.MovePlayer( Direction.Down );
               }
               else if ( keyPress.Key == RLKey.Left )
               {
                  didPlayerAct = CommandSystem.MovePlayer( Direction.Left );
               }
               else if ( keyPress.Key == RLKey.Right )
               {
                  didPlayerAct = CommandSystem.MovePlayer( Direction.Right );
               }
               else if ( keyPress.Key == RLKey.Escape )
               {
                  _rootConsole.Close();
               }
               else if ( keyPress.Key == RLKey.Period )
               {
                  if ( DungeonMap.CanMoveDownToNextLevel() )
                  {
                     MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel );
                     DungeonMap = mapGenerator.CreateMap();
                     MessageLog = new MessageLog();
                     CommandSystem = new CommandSystem();
                     _rootConsole.Title = $"RougeSharp RLNet Tutorial - Level {_mapLevel}";
                     didPlayerAct = true;
                  }
               }
               else
               {
                  didPlayerAct = CommandSystem.HandleKey( keyPress.Key );
               }

               if ( didPlayerAct )
               {
                  _renderRequired = true;
                  CommandSystem.EndPlayerTurn();
               }
            }
         }
         else
         {
            CommandSystem.ActivateMonsters();
            _renderRequired = true;
         }
      }

      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         if ( _renderRequired )
         {
            _mapConsole.Clear();
            _messageConsole.Clear();
            _statConsole.Clear();
            _inventoryConsole.Clear();
            DungeonMap.Draw( _mapConsole, _statConsole, _inventoryConsole );
            MessageLog.Draw( _messageConsole );
            TargetingSystem.Draw( _mapConsole );
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
