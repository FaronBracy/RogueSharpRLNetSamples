using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      private readonly List<Monster> _monsters;
      private readonly List<Gold> _goldPiles;

      public Player Player;
      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();
         _goldPiles = new List<Gold>();
         Game.ScheduleService = new ScheduleService();

         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public void AddMonster( Monster monster )
      {
         _monsters.Add( monster );
         SetIsWalkable( monster.X, monster.Y, false );
         Game.ScheduleService.Add( monster );
      }

      public void RemoveMonster( Monster monster )
      {
         _monsters.Remove( monster );
         SetIsWalkable( monster.X, monster.Y, true );
         Game.ScheduleService.Remove( monster );
      }

      public void AddPlayer( Player player )
      {
         Player = player;
         SetIsWalkable( Player.X, Player.Y, false );
         UpdatePlayerFieldOfView();
         Game.ScheduleService.Add( player );
      }

      public Player GetPlayer()
      {
         return Player;
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
               x = Player.X;
               y = Player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = Player.X;
               y = Player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = Player.X - 1;
               y = Player.Y;
               break;
            }
            case Direction.Right:
            {
               x = Player.X + 1;
               y = Player.Y;
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
            SetIsWalkable( Player.X, Player.Y, true );
            Player.X = x;
            Player.Y = y;
            SetIsWalkable( Player.X, Player.Y, false );
            OpenDoor( x, y );
            UpdatePlayerFieldOfView();
         }
         else
         {
            Monster monster = MonsterAt( x, y );

            if ( monster != null )
            {
               Game.CommandService.Attack( Player, monster );
            }
         }
      }

      public bool CanMoveDownToNextLevel()
      {
         return StairsDown.X == Player.X && StairsDown.Y == Player.Y;
      }

      private void PickUpGold( int x, int y )
      {
         List<Gold> goldAtLocation = _goldPiles.Where( g => g.X == x && g.Y == y ).ToList();
         foreach ( Gold gold in goldAtLocation )
         {
            Player.Gold += gold.Amount;
            Game.Messages.Add( string.Format( "{0} picked up {1} gold", Player.Name, gold.Amount ) );
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
         else if ( realCell.X == Player.X && realCell.Y == Player.Y )
         {
            Game.CommandService.Attack( monster, Player );
         }
      }

      public void UpdatePlayerFieldOfView()
      {
         ComputeFov( Player.X, Player.Y, Player.Awareness, true );
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

         Player.Draw( mapConsole );
         Player.DrawStats( statConsole );
         Player.DrawInventory( inventoryConsole );
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
            Game.Messages.Add( string.Format( "{0} opened a door", Player.Name ) );
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
   }
}