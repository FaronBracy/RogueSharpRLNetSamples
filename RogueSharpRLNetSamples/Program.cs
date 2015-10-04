using System.Collections.Generic;
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
      private static IGoalMap _goalMap;
      private static List<Point> _goals;
      private static List<Point> _obstacles;
      private static Point _start;
      private static Path _path;
      private static IReadOnlyCollection<Path> _allPaths;
      private static bool _avoidGoals;

      public static void Main()
      {
         // Use RogueSharp to create a new cave map the same size as the screen.
         _map = Map.Create( new CaveMapCreationStrategy<Map>( _screenWidth, _screenHeight, 45, 4, 3 ) );
         // Use RogueSharp to create a new GoalMap for this Map
         _goalMap = new GoalMap( _map );
         // Initialize the List of Points that are selected to be Goals for the GoalMap calculations
         _goals = new List<Point>();
         // Initialize the List of Points that are selected to be Obstacles for the GoalMap calculations
         _obstacles = new List<Point>();
         // Starting location to begin calculating GoalMaps from. This will be selected with the left mouse button
         _start = null;
         // This will hold the chosen "best" path after the GoalMap calculation has been completed
         _path = null;
         // This will hold one more paths that all determined to have the shortest length after the GoalMap calculation completes
         _allPaths = null;

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
         // Check for the left mouse button and use that to set the start location for GoalMap calculation
         if ( _rootConsole.Mouse.GetLeftClick() )
         {
            SetStartLocation();
         }
         // Check for the right mouse button and use that to set a Goal at the mouse cursor position
         if ( _rootConsole.Mouse.GetRightClick() )
         {
            ToggleGoalAtMouseLocation();
         }

         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
         if ( keyPress != null )
         {
            // Check for the "O" key to set an obstacle at the mouse cursor position
            if ( keyPress.Key == RLKey.O )
            {
               ToggleObstacleAtMouseLocation();
            }
            // Check for the "A" key and toggle between avoiding and seeking Goals
            else if ( keyPress.Key == RLKey.A )
            {
               ToggleAvoidance();
            }
            // Check for the "C" key to clear all Goals, Paths, and Obstacles
            else if ( keyPress.Key == RLKey.C )
            {
               _start = null;
               _goalMap.ClearGoals();
               _goals = new List<Point>();
               _obstacles = new List<Point>();
               _path = null;
               _allPaths = null;
            }
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

         // Go through each Goal and set the background color to Yellow
         foreach ( Point point in _goals )
         {
            _rootConsole.SetBackColor( point.X, point.Y, RLColor.Yellow );
         }

         // Set the background color of the start location for the GoalMap calculation background Green
         if ( _start != null )
         {
            _rootConsole.SetBackColor( _start.X, _start.Y, RLColor.Green );
         }

         // Set the background color of all minimum distance paths to LightBlue
         if ( _allPaths != null )
         {
            foreach ( Path path in _allPaths )
            {
               foreach ( Cell cell in path.Steps )
               {
                  if ( cell == path.Start || cell == path.End )
                  {
                     continue;
                  }
                  _rootConsole.SetBackColor( cell.X, cell.Y, RLColor.LightBlue );
               }
            }
         }

         // Set the background color of the first minimum distance path to Blue
         if ( _path != null )
         {
            foreach ( Cell cell in _path.Steps )
            {
               if ( cell == _path.Start || cell == _path.End )
               {
                  continue;
               }
               _rootConsole.SetBackColor( cell.X, cell.Y, RLColor.Blue );
            }
         }

         // Set the background color of each obstacle to LightRed
         foreach ( Point point in _obstacles )
         {
            _rootConsole.SetBackColor( point.X, point.Y, RLColor.LightRed );
         }

         // Tell RLNET to draw the console that we set
         _rootConsole.Draw();
      }

      private static void ToggleGoalAtMouseLocation()
      {
         var mouseLocataion = GetMouseLocation();
         if ( mouseLocataion == null )
         {
            return;
         }

         // Check to see if we already have the Goal so we know to toggle it on or off
         if ( _goals.Contains( mouseLocataion ) )
         {
            _goals.Remove( mouseLocataion );

            // Remove the Goal at the mouse location from our GoalMap
            _goalMap.RemoveGoal( mouseLocataion.X, mouseLocataion.Y );
         }
         else
         {
            _goals.Add( mouseLocataion );

            // Add a Goal at the mouse location to our GoalMap
            // Set the weight to 0 which is the highest priority Goal
            _goalMap.AddGoal( mouseLocataion.X, mouseLocataion.Y, 0 );
         }
         if ( _start != null )
         {
            UpdateGoalMapPaths();
         }
      }

      private static void SetStartLocation()
      {
         var mouseLocataion = GetMouseLocation();
         if ( mouseLocataion == null )
         {
            return;
         }

         // Set the starting location for the GoalMap calculation to the current mouse location
         _start = mouseLocataion;
         UpdateGoalMapPaths();
      }

      private static void ToggleObstacleAtMouseLocation()
      {
         var mouseLocataion = GetMouseLocation();
         if ( mouseLocataion == null )
         {
            return;
         }

         // Check to see if we already have the obstacle so we know to toggle it on or off
         if ( _obstacles.Contains( mouseLocataion ) )
         {
            _obstacles.Remove( mouseLocataion );
         }
         else
         {
            _obstacles.Add( mouseLocataion );
         }
         UpdateGoalMapPaths();
      }

      // Determine the current mouse location (in tiles) from RLNet's _rootConsole
      private static Point GetMouseLocation()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            // When the mouse cursor is not on the screen return null.
            return null;
         }
         Point mouseLocataion = new Point( x, y );
         return mouseLocataion;
      }

      private static void UpdateGoalMapPaths()
      {
         // Remove all previous obstacles from the GoalMap
         _goalMap.ClearObstacles();

         // Add obstacles back to the GoalMap based on the obstacles that were selected with the mouse
         _goalMap.AddObstacles( _obstacles );

         // Check to see if we are avoiding Goals or seeking them
         if ( _avoidGoals )
         {
            // Get the best path avoiding the Goals specified
            _path = _goalMap.FindPathAvoidingGoals( _start.X, _start.Y );
         }
         else
         {
            // Get the best path seeking the Goals specified
            _path = _goalMap.FindPath( _start.X, _start.Y );
         }

         // Sometimes there is more than one path that has an equally short length
         // FindPaths will return them all so that we can decide which one to take
         _allPaths = _goalMap.FindPaths( _start.X, _start.Y );
      }

      // Switch between seeking Goals and avoiding them
      private static void ToggleAvoidance()
      {
         _avoidGoals = !_avoidGoals;
         UpdateGoalMapPaths();
      }
   }
}