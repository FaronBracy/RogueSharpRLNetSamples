using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
   public class DungeonCell : Cell
   {
      public bool IsExplored
      {
         get;
         set;
      }
   }

   public class DungeonMap : Map<DungeonCell>
   {
      private readonly List<Monster> _monsters;
      private readonly List<TreasurePile> _treasurePiles;
      private readonly FieldOfView<DungeonCell> _fieldOfView;

      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();
         _treasurePiles = new List<TreasurePile>();
         _fieldOfView = new FieldOfView<DungeonCell>( this );
         Game.SchedulingSystem.Clear();

         Rooms = new List<Rectangle>();
         Doors = new List<Door>();
      }

      public void AddMonster( Monster monster )
      {
         _monsters.Add( monster );
         SetIsWalkable( monster.X, monster.Y, false );
         Game.SchedulingSystem.Add( monster );
      }

      public void RemoveMonster( Monster monster )
      {
         _monsters.Remove( monster );
         SetIsWalkable( monster.X, monster.Y, true );
         Game.SchedulingSystem.Remove( monster );
      }

      public Monster GetMonsterAt( int x, int y )
      {
         // BUG: This should be single except sometiems monsters occupy the same space.
         return _monsters.FirstOrDefault( m => m.X == x && m.Y == y );
      }

      public IEnumerable<Point> GetMonsterLocations()
      {
         return _monsters.Select( m => new Point
         {
            X = m.X,
            Y = m.Y
         } );
      }

      public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
      {
         return _monsters.Where( monster => IsInFov( monster.X, monster.Y ) )
            .Select( m => new Point { X = m.X, Y = m.Y } );
      }

      public void AddTreasure( int x, int y, ITreasure treasure )
      {
         _treasurePiles.Add( new TreasurePile( x, y, treasure ) );
      }

      public void AddPlayer( Player player )
      {
         Game.Player = player;
         SetIsWalkable( player.X, player.Y, false );
         UpdatePlayerFieldOfView();
         Game.SchedulingSystem.Add( player );
      }

      public void UpdatePlayerFieldOfView()
      {
         Player player = Game.Player;
         ComputeFov( player.X, player.Y, player.Awareness, true );
         foreach ( Cell cell in GetAllCells() )
         {
            if ( IsInFov( cell.X, cell.Y ) )
            {
               SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }
      }

      public bool IsExplored( int x, int y )
      {
         return this[x, y].IsExplored;
      }

      public void SetCellProperties( int x, int y, bool isTransparent, bool isWalkable, bool isExplored )
      {
         this[x, y].IsTransparent = isTransparent;
         this[x, y].IsWalkable = isWalkable;
         this[x, y].IsExplored = isExplored;
      }

      public bool IsInFov( int x, int y )
      {
         return _fieldOfView.IsInFov( x, y );
      }

      public ReadOnlyCollection<DungeonCell> ComputeFov( int xOrigin, int yOrigin, int radius, bool lightWalls )
      {
         return _fieldOfView.ComputeFov( xOrigin, yOrigin, radius, lightWalls );
      }

      public bool SetActorPosition( Actor actor, int x, int y )
      {
         if ( GetCell( x, y ).IsWalkable )
         {
            PickUpTreasure( actor, x, y );
            SetIsWalkable( actor.X, actor.Y, true );
            actor.X = x;
            actor.Y = y;
            SetIsWalkable( actor.X, actor.Y, false );
            OpenDoor( actor, x, y );
            if ( actor is Player )
            {
               UpdatePlayerFieldOfView();
            }
            return true;
         }
         return false;
      }

      public Door GetDoor( int x, int y )
      {
         return Doors.SingleOrDefault( d => d.X == x && d.Y == y );
      }

      private void OpenDoor( Actor actor, int x, int y )
      {
         Door door = GetDoor( x, y );
         if ( door != null && !door.IsOpen )
         {
            door.IsOpen = true;
            var cell = GetCell( x, y );
            SetCellProperties( x, y, true, true, cell.IsExplored );

            Game.MessageLog.Add( $"{actor.Name} opened a door" );
         }
      }

      public void AddGold( int x, int y, int amount )
      {
         if ( amount > 0 )
         {
            AddTreasure( x, y, new Gold( amount ) );
         }
      }

      private void PickUpTreasure( Actor actor, int x, int y )
      {
         List<TreasurePile> treasureAtLocation = _treasurePiles.Where( g => g.X == x && g.Y == y ).ToList();
         foreach ( TreasurePile treasurePile in treasureAtLocation )
         {
            if ( treasurePile.Treasure.PickUp( actor ) )
            {
               _treasurePiles.Remove( treasurePile );
            }
         }
      }

      public bool CanMoveDownToNextLevel()
      {
         Player player = Game.Player;

         return StairsDown.X == player.X && StairsDown.Y == player.Y;
      }

      public void SetIsWalkable( int x, int y, bool isWalkable )
      {
         DungeonCell cell = GetCell( x, y );
         SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
      }

      public Point GetRandomLocation()
      {
         int roomNumber = Game.Random.Next( 0, Rooms.Count - 1 );
         Rectangle randomRoom = Rooms[roomNumber];

         if ( !DoesRoomHaveWalkableSpace( randomRoom ) )
         {
            GetRandomLocation();
         }

         return GetRandomLocationInRoom( randomRoom );
      }

      public Point GetRandomLocationInRoom( Rectangle room )
      {
         int x = Game.Random.Next( 1, room.Width - 2 ) + room.X;
         int y = Game.Random.Next( 1, room.Height - 2 ) + room.Y;
         if ( !IsWalkable( x, y ) )
         {
            GetRandomLocationInRoom( room );
         }
         return new Point( x, y );
      }

      public bool DoesRoomHaveWalkableSpace( Rectangle room )
      {
         for ( int x = 1; x <= room.Width - 2; x++ )
         {
            for ( int y = 1; y <= room.Height - 2; y++ )
            {
               if ( IsWalkable( x + room.X, y + room.Y ) )
               {
                  return true;
               }
            }
         }
         return false;
      }

      public void Draw( RLConsole mapConsole, RLConsole statConsole, RLConsole inventoryConsole )
      {
         mapConsole.Clear();
         foreach ( DungeonCell cell in GetAllCells() )
         {
            SetConsoleSymbolForCell( mapConsole, cell );
         }

         foreach ( Door door in Doors )
         {
            door.Draw( mapConsole, this );
         }

         StairsUp.Draw( mapConsole, this );
         StairsDown.Draw( mapConsole, this );

         foreach ( TreasurePile treasurePile in _treasurePiles )
         {
            IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
            drawableTreasure?.Draw( mapConsole, this );
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

         Player player = Game.Player;

         player.Draw( mapConsole, this );
         player.DrawStats( statConsole );
         player.DrawInventory( inventoryConsole );
      }

      private void SetConsoleSymbolForCell( RLConsole console, DungeonCell cell )
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