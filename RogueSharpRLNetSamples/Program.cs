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
         Cross = 7
      }

      private static readonly int _screenWidth = 50;
      private static readonly int _screenHeight = 50;

      private static RLRootConsole _rootConsole;
      private static IMap _map;

      private static SelectionType _currentSelectionType;
      private static bool _highlightWalls;
      private static int _selectionSize = 5;

      public static void Main()
      {
         _map = Map.Create( new CaveMapCreationStrategy<Map>( _screenWidth, _screenHeight, 45, 4, 3 ) );
         string fontFileName = "terminal8x8.png";
         string consoleTitle = "RougeSharp RLNet Tutorial";
         _rootConsole = new RLRootConsole( fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle );
         _rootConsole.Update += OnRootConsoleUpdate;
         _rootConsole.Render += OnRootConsoleRender;
         _rootConsole.Run();
      }

      private static void OnRootConsoleUpdate( object sender, UpdateEventArgs e )
      {
         if ( _rootConsole.Mouse.GetLeftClick() )
         {
            _currentSelectionType++;

            if ( (int) _currentSelectionType == 8 )
            {
               _currentSelectionType = SelectionType.Radius;
            }
         }

         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
         if ( keyPress != null )
         {
            if ( keyPress.Key == RLKey.W )
            {
               _highlightWalls = !_highlightWalls;
            }
            else if ( keyPress.Key == RLKey.Q )
            {
               _selectionSize--;
               if ( _selectionSize == 0 )
               {
                  _selectionSize = 1;
               }
            }
            else if ( keyPress.Key == RLKey.E )
            {
               _selectionSize++;
               if ( _selectionSize == 51 )
               {
                  _selectionSize = 50;
               }
            }
         }
      }

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
            _rootConsole.SetBackColor( cell.X, cell.Y, RLColor.Yellow );
         }

         _rootConsole.Draw();
      }

      private static IEnumerable<Cell> SelectCellsAroundMouse()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            return new List<Cell>();
         }
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
            case SelectionType.RadiusBorder:
            {
               selectedCells = _map.GetBorderCellsInRadius( x, y, _selectionSize );
               break;
            }
            case SelectionType.AreaBorder:
            {
               selectedCells = _map.GetBorderCellsInArea( x, y, _selectionSize );
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
            case SelectionType.ColumnAndRow:
            {
               List<Cell> rowCells = _map.GetCellsInRows( y ).ToList();
               rowCells.AddRange( _map.GetCellsInColumns( x ) );
               selectedCells = rowCells;
               break;
            }
            case SelectionType.Cross:
            {
               if ( x < 1 || x >= _screenWidth -1 || y < 1 || y >= _screenHeight - 1 )
               {
                  return new List<Cell>();
               }
               List<Cell> rowCells = _map.GetCellsInRows( y + 1, y - 1 ).ToList();
               rowCells.AddRange( _map.GetCellsInColumns( x + 1, x - 1 ) );
               selectedCells = rowCells;
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
         return FilterWalls( selectedCells );
      }

      private static IEnumerable<Cell> FilterWalls( IEnumerable<Cell> cells )
      {
         return cells.Where( c => c.IsWalkable );
      }
   }
}
