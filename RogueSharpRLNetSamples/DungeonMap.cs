using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      private readonly List<Monster> _monsters;
      private Player _player;
      private ActorSchedule _actorSchedule;

      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();
         _actorSchedule = new ActorSchedule();

         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public void AddMonster( Monster monster )
      {
         _monsters.Add( monster );
         SetIsWalkable( monster.X, monster.Y, false );
         _actorSchedule.Add( monster );
      }

      public void RemoveMonster( Monster monster )
      {
         _monsters.Remove( monster );
         SetIsWalkable( monster.X, monster.Y, true );
         _actorSchedule.Remove( monster );
      }

      public void AddPlayer( Player player )
      {
         _player = player;
         SetIsWalkable( _player.X, _player.Y, false );
         UpdatePlayerFieldOfView();
         _actorSchedule.Add( player );
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
               int damage = Dice.Roll( "1D10" );
               Game.Messages.Add( string.Format( "{0} was hit for {1} damage", monster.Name, damage ) );
               monster.Health = monster.Health - damage;
               if ( monster.Health <= 0 )
               {
                  RemoveMonster( monster );
                  Game.Messages.Add( string.Format( "{0} died", monster.Name ) );
               }
            }
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
            int damage = Dice.Roll( "1D4" );
            Game.Messages.Add( string.Format( "Player was hit for {0} damage", damage ) );
            _player.Health = _player.Health - damage;
            if ( _player.Health <= 0 )
            {
               Game.Messages.Add( "Player was killed, GAME OVER MAN!" );
            }
         }
      }

      public void UpdatePlayerFieldOfView()
      {
         ComputeFov( _player.X, _player.Y, 20, true );
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

      public void Draw( RLConsole mapConsole, RLConsole statConsole )
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
         if ( door != null )
         {
            door.IsOpen = true;
            SetCellProperties( x, y, true, true, true );
            Game.Messages.Add( "Opened a door" );
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

      public void ActivateNextActor()
      {
         IActor actor = _actorSchedule.Get();
         if ( actor is Player )
         {
            Game.IsPlayerTurn = true;
            _actorSchedule.Add( _player );
         }
         else
         {
            Monster monster = actor as Monster;
            PerformAction( monster );
            _actorSchedule.Add( monster );
         }
      }

      private void PerformAction( Monster monster )
      {
         FieldOfView monsterFov = new FieldOfView( this );
         monsterFov.ComputeFov( monster.X, monster.Y, 15, true );
         if ( monsterFov.IsInFov( _player.X, _player.Y ) )
         {
            PathFinder pathFinder = new PathFinder( this );
            Path path = pathFinder.ShortestPath( GetCell( monster.X, monster.Y ), GetCell( _player.X, _player.Y ) );
            MoveMonster( monster, path.StepForward() );
         }
      }
   }
}