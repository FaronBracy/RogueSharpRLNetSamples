using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Inventory;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples
{
   public class DungeonMap : Map
   {
      private readonly List<Monster> _monsters;
      private readonly List<Treasure> _treasurePiles;
      private Player _player;

      public List<Rectangle> Rooms;
      public List<Door> Doors;
      public Stairs StairsUp;
      public Stairs StairsDown;

      public DungeonMap()
      {
         _monsters = new List<Monster>();
         _treasurePiles = new List<Treasure>();
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

      public Monster GetMonsterAt( int x, int y )
      {
         // TODO: Sometimes this throws an exception because 2 monsters occupy the same space. Not sure how this happens
         return _monsters.SingleOrDefault( m => m.X == x && m.Y == y );
      }

      public IEnumerable<Point> GetMonsterLocations()
      {
         return _monsters.Select( m => new Point {
            X = m.X,
            Y = m.Y
         } );
      }

      public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
      {
         return _monsters.Where( monster => IsInFov( monster.X, monster.Y ) )
            .Select( m => new Point { X = m.X, Y = m.Y } );
      }

      public void AddEquipment( int x, int y, Equipment equipment )
      {
         Treasure treasure = new Treasure( x, y, 0, equipment );
         _treasurePiles.Add( treasure );
      }

      public void AddAbility( int x, int y, Ability ability )
      {
         Treasure treasure = new Treasure( x, y, 0, null, ability );
         _treasurePiles.Add( treasure );
      }

      public void AddPlayer( Player player )
      {
         _player = player;
         SetIsWalkable( _player.X, _player.Y, false );
         UpdatePlayerFieldOfView();
         Game.ScheduleService.Add( player );
      }

      public Player GetPlayer()
      {
         return _player;
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

            Game.Messages.Add( string.Format( "{0} opened a door", actor.Name ) );
         }
      }

      public void AddGold( int x, int y, int amount )
      {
         if ( amount > 0 )
         {
            _treasurePiles.Add( new Treasure( x, y, amount ) );
         }
      }

      private void PickUpTreasure( Actor actor, int x, int y )
      {
         List<Treasure> treasureAtLocation = _treasurePiles.Where( g => g.X == x && g.Y == y ).ToList();
         foreach ( Treasure treasure in treasureAtLocation )
         {
            if ( treasure.Gold > 0 )
            {
               actor.Gold += treasure.Gold;
               Game.Messages.Add( string.Format( "{0} picked up {1} gold", actor.Name, treasure.Gold ) );
            }

            if ( treasure.Equipment != null )
            {
               if ( treasure.Equipment is HeadEquipment )
               {
                  actor.Head = treasure.Equipment as HeadEquipment;
                  Game.Messages.Add( string.Format( "{0} picked up a {1} helmet", actor.Name, treasure.Equipment.Name ) );
               }
               else if ( treasure.Equipment is BodyEquipment )
               {
                  actor.Body = treasure.Equipment as BodyEquipment;
                  Game.Messages.Add( string.Format( "{0} picked up {1} body armor", actor.Name, treasure.Equipment.Name ) );
               }
               else if ( treasure.Equipment is HandEquipment )
               {
                  actor.Hand = treasure.Equipment as HandEquipment;
                  Game.Messages.Add( string.Format( "{0} picked up a {1}", actor.Name, treasure.Equipment.Name ) );
               }
               else if ( treasure.Equipment is FeetEquipment )
               {
                  actor.Feet = treasure.Equipment as FeetEquipment;
                  Game.Messages.Add( string.Format( "{0} picked up {1} boots", actor.Name, treasure.Equipment.Name ) );
               }
            }

            if ( treasure.Ability != null && actor is Player )
            {
               _player.AddAbility( treasure.Ability );
               _treasurePiles.Remove( treasure );
            }
            else if ( treasure.Ability == null )
            {
               _treasurePiles.Remove( treasure );
            }
         }
      }

      public bool CanMoveDownToNextLevel()
      {
         return StairsDown.X == _player.X && StairsDown.Y == _player.Y;
      }

      public void SetIsWalkable( int x, int y, bool isWalkable )
      {
         Cell cell = GetCell( x, y );
         SetCellProperties( cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored );
      }

      public Point GetRandomLocation( IRandom random )
      {
         int roomNumber = random.Next( 0, Rooms.Count - 1 );
         Rectangle randomRoom = Rooms[roomNumber];

         if ( !DoesRoomHaveWalkableSpace( randomRoom ) )
         {
            GetRandomLocation( random );
         }

         return GetRandomLocationInRoom( randomRoom, random );
      }

      public Point GetRandomLocationInRoom( Rectangle room, IRandom random )
      {
         int x = random.Next( 1, room.Width - 2 ) + room.X;
         int y = random.Next( 1, room.Height - 2 ) + room.Y;
         if ( !IsWalkable( x, y ) )
         {
            GetRandomLocationInRoom( room, random );
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

         foreach ( Treasure treasure in _treasurePiles )
         {
            treasure.Draw( mapConsole, this );
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