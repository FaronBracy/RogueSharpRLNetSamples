using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      private readonly List<Monster> _monsters;
      private readonly List<Gold> _goldPiles;
      private Player _player;
      private ScheduleService _scheduleService;

      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();
         _goldPiles = new List<Gold>();
         _scheduleService = new ScheduleService();

         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public void AddMonster( Monster monster )
      {
         _monsters.Add( monster );
         SetIsWalkable( monster.X, monster.Y, false );
         _scheduleService.Add( monster );
      }

      public void RemoveMonster( Monster monster )
      {
         _monsters.Remove( monster );
         SetIsWalkable( monster.X, monster.Y, true );
         _scheduleService.Remove( monster );
      }

      public void AddPlayer( Player player )
      {
         _player = player;
         SetIsWalkable( _player.X, _player.Y, false );
         UpdatePlayerFieldOfView();
         _scheduleService.Add( player );
      }

      public Player GetPlayer()
      {
         return _player;
      }

      public void AddGold( int x, int y, int amount )
      {
         _goldPiles.Add( new Gold( x, y, amount ) );
      }

      public void MovePlayer( Direction direction )
      {
         int x;
         int y;

         switch ( direction )
         {
            case Direction.Up:
            {
               x = _player.X;
               y = _player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = _player.X;
               y = _player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = _player.X - 1;
               y = _player.Y;
               break;
            }
            case Direction.Right:
            {
               x = _player.X + 1;
               y = _player.Y;
               break;
            }
            default:
            {
               return;
            }
         }

         if ( GetCell( x, y ).IsWalkable )
         {
            PickUpGold( x, y );
            SetIsWalkable( _player.X, _player.Y, true );
            _player.X = x;
            _player.Y = y;
            SetIsWalkable( _player.X, _player.Y, false );
            OpenDoor( x, y );
            UpdatePlayerFieldOfView();
         }
         else
         {
            Monster monster = MonsterAt( x, y );

            if ( monster != null )
            {
               Game.CommandService.Attack( _player, monster );
            }
         }
      }

      public bool CanMoveDownToNextLevel()
      {
         return StairsDown.X == _player.X && StairsDown.Y == _player.Y;
      }

      private void PickUpGold( int x, int y )
      {
         List<Gold> goldAtLocation = _goldPiles.Where( g => g.X == x && g.Y == y ).ToList();
         foreach ( Gold gold in goldAtLocation )
         {
            _player.Gold += gold.Amount;
            Game.Messages.Add( string.Format( "{0} picked up {1} gold", _player.Name, gold.Amount ) );
            _goldPiles.Remove( gold );
         }
      }

      public void MoveMonster( Monster monster, Cell cell )
      {
         Cell realCell = this.GetCell( cell.X, cell.Y );
         if ( realCell.IsWalkable )
         {
            SetIsWalkable( monster.X, monster.Y, true );
            monster.X = realCell.X;
            monster.Y = realCell.Y;
            SetIsWalkable( monster.X, monster.Y, false );
         }
         else if ( realCell.X == _player.X && realCell.Y == _player.Y )
         {
            Game.CommandService.Attack( monster, _player );
         }
      }

      public void UpdatePlayerFieldOfView()
      {
         ComputeFov( _player.X, _player.Y, _player.Awareness, true );
         foreach ( Cell cell in GetAllCells() )
         {
            if ( IsInFov( cell.X, cell.Y ) )
            {
               SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
      }

      public Door GetDoor( int x, int y )
      {
         return Doors.SingleOrDefault( d => d.X == x && d.Y == y );
      }

      public void Draw( RLConsole mapConsole, RLConsole statConsole, RLConsole inventoryConsole )
      {
         mapConsole.Clear();
         foreach ( Cell cell in GetAllCells() )
         {
            SetConsoleSymbolForCell( mapConsole, cell );
         }

         foreach ( Door door in Doors )
         {
            door.Draw( mapConsole, this );
         }

         StairsUp.Draw( mapConsole, this );
         StairsDown.Draw( mapConsole, this );

         foreach ( Gold gold in _goldPiles )
         {
            gold.Draw( mapConsole );
         }

         statConsole.Clear();
         int i = 0;
         foreach ( Monster monster in _monsters )
         {
            monster.Draw( mapConsole, this );
            if ( IsInFov( monster.X, monster.Y ) )
            {
               monster.DrawStats( statConsole, i );
               i++;
            }
         }

         _player.Draw( mapConsole );
         _player.DrawStats( statConsole );
         _player.DrawInventory( inventoryConsole );
      }

      private Monster MonsterAt( int x, int y )
      {
         return _monsters.SingleOrDefault( m => m.X == x && m.Y == y );
      }

      private void SetIsWalkable( int x, int y, bool isWalkable )
      {
         Cell cell = GetCell( x, y );
         SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
      }

      private void OpenDoor( int x, int y )
      {
         Door door = GetDoor( x, y );
         if ( door != null && !door.IsOpen )
         {
            door.IsOpen = true;
            SetCellProperties( x, y, true, true, true );
            Game.Messages.Add( string.Format( "{0} opened a door", _player.Name ) );
         }
      }

      private void SetConsoleSymbolForCell( RLConsole console, Cell cell )
      {
         if ( !cell.IsExplored )
         {
            return;
         }

         if ( IsInFov( cell.X, cell.Y ) )
         {
            if ( cell.IsWalkable )
            {
               console.Set( cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.' );
            }
            else
            {
               console.Set( cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#' );
            }
         }
         else
         {
            if ( cell.IsWalkable )
            {
               console.Set( cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.' );
            }
            else
            {
               console.Set( cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#' );
            }
         }
      }

      public void ActivateMonsters()
      {
         IScheduleable scheduleable = _scheduleService.Get();
         if ( scheduleable is Player )
         {
            Game.IsPlayerTurn = true;
            _scheduleService.Add( _player );
         }
         else
         {
            Monster monster = scheduleable as Monster;
            PerformAction( monster );
            _scheduleService.Add( monster );
            ActivateMonsters();
         }
      }

      private void PerformAction( Monster monster )
      {
         FieldOfView monsterFov = new FieldOfView( this );
         monsterFov.ComputeFov( monster.X, monster.Y, monster.Awareness, true );
         if ( monsterFov.IsInFov( _player.X, _player.Y ) )
         {
            PathFinder pathFinder = new PathFinder( this );
            Path path = pathFinder.ShortestPath( GetCell( monster.X, monster.Y ), GetCell( _player.X, _player.Y ) );
            try
            {
               MoveMonster( monster, path.StepForward() );
            }
            catch ( NoMoreStepsException )
            {
               Game.Messages.Add( string.Format( "{0} waits for a turn", monster.Name ) );
            }
         }
      }
   }
}