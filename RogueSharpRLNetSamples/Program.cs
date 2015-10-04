using System.Collections.Generic;
using RLNET;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueSharpRLNetSamples
{
   public class Program
   {
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
      private static bool _avoidGoals = false;

      public static void Main()
      {
         _map = Map.Create( new CaveMapCreationStrategy<Map>( _screenWidth, _screenHeight, 45, 4, 3 ) );
         _goalMap = new GoalMap( _map );
         _goals = new List<Point>();
         _obstacles = new List<Point>();
         _start = null;
         _path = null;
         _allPaths = null;

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
            SetStartLocation();
         }
         if ( _rootConsole.Mouse.GetRightClick() )
         {
            ToggleGoalAtMouseLocation();
         }

         RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
         if ( keyPress != null )
         {
            if ( keyPress.Key == RLKey.O )
            {
               ToggleObstacleAtMouseLocation();
            }
            else if ( keyPress.Key == RLKey.A )
            {
               ToggleAvoidance();
            }
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

         foreach ( Point point in _goals )
         {
            _rootConsole.SetBackColor( point.X, point.Y, RLColor.Yellow );
         }

         if ( _start != null )
         {
            _rootConsole.SetBackColor( _start.X, _start.Y, RLColor.Green );
         }

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

         foreach ( Point point in _obstacles )
         {
            _rootConsole.SetBackColor( point.X, point.Y, RLColor.LightRed );
         }

         _rootConsole.Draw();
      }

      private static void ToggleGoalAtMouseLocation()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            return;
         }
         Point mouseLocataion = new Point( x, y );
         if ( _goals.Contains( mouseLocataion ) )
         {
            _goals.Remove( mouseLocataion );
            _goalMap.RemoveGoal( x, y );
         }
         else
         {
            _goals.Add( mouseLocataion );
            _goalMap.AddGoal( x, y, 0 );
         }
         if ( _start != null )
         {
            UpdateGoalMapPaths();
         }
      }

      private static void SetStartLocation()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            return;
         }
         Point mouseLocataion = new Point( x, y );
         _start = mouseLocataion;
         UpdateGoalMapPaths();
      }

      private static void ToggleObstacleAtMouseLocation()
      {
         int x = _rootConsole.Mouse.X;
         int y = _rootConsole.Mouse.Y;
         if ( x < 0 || x >= _screenWidth || y < 0 || y >= _screenHeight )
         {
            return;
         }
         Point mouseLocataion = new Point( x, y );
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

      private static void UpdateGoalMapPaths()
      {
         _goalMap.ClearObstacles();
         _goalMap.AddObstacles( _obstacles );
         if ( _avoidGoals )
         {
            _path = _goalMap.FindPathAvoidingGoals( _start.X, _start.Y );
         }
         else
         {
            _path = _goalMap.FindPath( _start.X, _start.Y );
         }
         _allPaths = _goalMap.FindPaths( _start.X, _start.Y );
      }

      private static void ToggleAvoidance()
      {
         _avoidGoals = !_avoidGoals;
         UpdateGoalMapPaths();
      }
   }
}