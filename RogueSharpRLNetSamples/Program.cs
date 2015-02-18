using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
      private enum SelectionType
      {
         Radius = 0,
         Area = 1,
         RadiusBorder = 2,
         AreaBorder = 3,
         Column = 4,
         Row = 5,
         ColumnAndRow = 6,
      }

      // The screen height and width are in number of tiles
      private static readonly int _screenWidth = 50;
      private static readonly int _screenHeight = 50;

      private static RLRootConsole _rootConsole;
      private static IMap _map;

      private static SelectionType _currentSelectionType;
      private static bool _highlightWalls = false;
      private static int _selectionSize = 5;

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
         if ( _rootConsole.Mouse.GetLeftClick() )
         {
            _currentSelectionType++;

            if ( (int) _currentSelectionType == 7 )
            {
               _currentSelectionType = SelectionType.Radius;
            }
         }
      }

      // Event handler for RLNET's Render event
      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         _rootConsole.Clear();

         foreach ( var cell in _map.GetAllCells() )
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

         foreach ( var cell in SelectCellsAroundMouse() )
         {
            _rootConsole.SetBackColor( cell.X, cell.Y, RLColor.LightBlue );
         }

         // Tell RLNET to draw the console that we set
         _rootConsole.Draw();
      }

      private static IEnumerable<Cell> SelectCellsAroundMouse()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         IEnumerable<Cell> selectedCells;
         switch ( _currentSelectionType )
         {
            case SelectionType.Radius:
            {
               selectedCells = _map.GetCellsInRadius( x, y, _selectionSize );
               break;
            }
            case SelectionType.Area:
            {
               selectedCells = _map.GetCellsInArea( x, y, _selectionSize );
               break;
            }
            case SelectionType.Row:
            {
               selectedCells = _map.GetCellsInRows( y );
               break;
            }
            case SelectionType.Column:
            {
               selectedCells = _map.GetCellsInColumns( x );
               break;
            }
            default:
            {
               selectedCells = _map.GetCellsInRadius( x, y, _selectionSize );
               break;
            }
         }
         if ( _highlightWalls )
         {
            return selectedCells;
         }
         else
         {
            return FilterWalls( selectedCells );
         }
      }

      private static IEnumerable<Cell> FilterWalls( IEnumerable<Cell> cells )
      {
         return cells.Where( c => c.IsWalkable );
      }
   }
}
