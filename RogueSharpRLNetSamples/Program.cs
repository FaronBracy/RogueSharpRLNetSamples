using RLNET;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
      // The screen height and width are in number of tiles
      private static readonly int _screenWidth = 50;
      private static readonly int _screenHeight = 50;

      private static RLRootConsole _rootConsole;
      private static IMap _map;
      private static PathFinder _pathFinder;
      private static Point _start;
      private static Point _end;
      private static Path _path;

      public static void Main()
      {
         // Use RogueSharp to create a new cave map the same size as the screen.
         _map = Map.Create( new CaveMapCreationStrategy<Map>( _screenWidth, _screenHeight, 45, 4, 3 ) );
         _pathFinder = new PathFinder( _map );
         _start = Point.Zero;
         _end = Point.Zero;
         _path = null;

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
            SetStartLocation();
         }
         if ( _rootConsole.Mouse.GetRightClick() )
         {
            SetEndLocation();
         }
      }

      // Event handler for RLNET's Render event
      private static void OnRootConsoleRender( object sender, UpdateEventArgs e )
      {
         _rootConsole.Clear();

         // Go through all the Cells in the Map and set walkable ones to the "." character and walls to the "#" character
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

         // Set the background color of the start location background Green
         _rootConsole.SetBackColor( _start.X, _start.Y, RLColor.Green );
         _rootConsole.SetBackColor( _end.X, _end.Y, RLColor.Yellow );

         if ( _path != null )
         {
            foreach ( ICell cell in _path.Steps )
            {
               if ( cell.Equals( _path.Start ) || cell.Equals( _path.End ) )
               {
                  continue;
               }
               _rootConsole.SetBackColor( cell.X, cell.Y, RLColor.Blue );
            }
         }

         // Tell RLNET to draw the console that we set
         _rootConsole.Draw();
      }

      private static void SetStartLocation()
      {
         var mouseLocataion = GetMouseLocation();

         _start = mouseLocataion;
         UpdatePath();
      }

      private static void SetEndLocation()
      {
         var mouseLocataion = GetMouseLocation();

         _end = mouseLocataion;
         UpdatePath();
      }

      // Determine the current mouse location (in tiles) from RLNet's _rootConsole
      private static Point GetMouseLocation()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            // When the mouse cursor is not on the screen return null.
            return Point.Zero;
         }
         Point mouseLocataion = new Point( x, y );
         return mouseLocataion;
      }

      private static void UpdatePath()
      {
         ICell source = _map.GetCell( _start.X, _start.Y );
         ICell destination = _map.GetCell( _end.X, _end.Y );
         _path = _pathFinder.TryFindShortestPath( source, destination );
      }
   }
}