using RLNET;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
      private static readonly int _screenWidth = 80;
      private static readonly int _screenHeight = 50;
      private static readonly int _mapWidth = 80;
      private static readonly int _mapHeight = 45;
      private static int _playerX = 25;
      private static int _playerY = 25;
      private static RLRootConsole _rootConsole;
      private static RLConsole _mapConsole;

      public static void Main()
      {
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
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
               _playerY--;
            }
            else if ( keyPress.Key == RLKey.Down )
            {
               _playerY++;
            }
            else if ( keyPress.Key == RLKey.Left )
            {
               _playerX--;
            }
            else if ( keyPress.Key == RLKey.Right )
            {
               _playerX++;
            }
            else if ( keyPress.Key == RLKey.Escape )
            {
               _rootConsole.Close();
            }
         }
      }

      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         _rootConsole.Clear();
         _rootConsole.Set( _playerX, _playerY, RLColor.White, null, '@' );
         _rootConsole.Draw();
      }
   }

   public class Tile
   {
      public bool BlocksMovement { get; set; }
      public bool BlocksSight { get; set; }

      public Tile( bool blocksMovement, bool blocksSight )
      {
         BlocksMovement = blocksMovement;
         BlocksSight = blocksSight;
      }

      public Tile( bool blocksMovement )
      {
         BlocksMovement = blocksMovement;
         BlocksSight = blocksMovement;
      }
   }

   public class Entity
   {
      
   }
}
