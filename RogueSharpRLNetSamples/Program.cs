using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
      // The screen height and width are in number of tiles
      private static readonly int _screenWidth = 50;
      private static readonly int _screenHeight = 50;
      // The starting position for the player
      private static int _playerX = 25;
      private static int _playerY = 25;
      private static RLRootConsole _rootConsole;
      private static IMap _map;

      public static void Main()
      {
         // Use RogueSharp to create a new cave map the same size as the screen.
         _map = Map.Create( new CaveMapCreationStrategy<Map>( _screenWidth, _screenHeight, 45, 4, 3 ) );
         // This must be the exact name of the bitmap font file we are using or it will error.
         string fontFileName = "terminal8x8.png";
         // The title will appear at the top of the console window
         string consoleTitle = "RougeSharp RLNet Tutorial";
         // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         // Set up a handler for RLNET's Update event
         _rootConsole.Update += OnRootConsoleUpdate;
         // Set up a handler for RLNET's Render event
         _rootConsole.Render += OnRootConsoleRender;
         // Begin RLNET's game loop
         _rootConsole.Run();
      }

      // Event handler for RLNET's Update event
      private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e )
      {
         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
         if ( keyPress != null )
         {
            // Check for the Up key press
            if ( keyPress.Key == RLKey.Up )
            {
               // Check the RogueSharp map to make sure the Cell is walkable before moving
               if ( _map.GetCell( _playerX, _playerY - 1 ).IsWalkable )
               {
                  // Update the player position
                  _playerY--;
               }
            }
            // Repeat for the other directions
            else if ( keyPress.Key == RLKey.Down )
            {
               if ( _map.GetCell( _playerX, _playerY + 1 ).IsWalkable )
               {
                  _playerY++;
               }
            }
            else if ( keyPress.Key == RLKey.Left )
            {
               if ( _map.GetCell( _playerX - 1, _playerY ).IsWalkable )
               {
                  _playerX--;
               }
            }
            else if ( keyPress.Key == RLKey.Right )
            {
               if ( _map.GetCell( _playerX + 1, _playerY ).IsWalkable )
               {
                  _playerX++;
               }
            }
         }
      }

      // Event handler for RLNET's Render event
      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         _rootConsole.Clear();

         // Use RogueSharp to calculate the current field-of-view for the player
         _map.ComputeFov( _playerX, _playerY, 50, true );

         foreach ( var cell in _map.GetAllCells() )
         {
            // When a Cell is in the field-of-view set it to a brighter color
            if ( cell.IsInFov )
            {
               _map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
               if ( cell.IsWalkable )
               {
                  _rootConsole.Set( cell.X, cell.Y, RLColor.Gray, null, '.' );
               }
               else
               {
                  _rootConsole.Set( cell.X, cell.Y, RLColor.LightGray, null, '#' );
               }
            }
            // If the Cell is not in the field-of-view but has been explored set it darker
            else if ( cell.IsExplored )
            {
               if ( cell.IsWalkable )
               {
                  _rootConsole.Set( cell.X, cell.Y, new RLColor( 30, 30, 30 ), null, '.' );
               }
               else
               {
                  _rootConsole.Set( cell.X, cell.Y, RLColor.Gray, null, '#' );
               }
            }
         }

         // Set the player's symbol after the map symbol to make sure it is draw
         _rootConsole.Set( _playerX, _playerY, RLColor.LightGreen, null, '@' );

         // Tell RLNET to draw the console that we set
         _rootConsole.Draw();
      }
   }
}
