using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharp.MapCreation;
using RogueSharp.Random;

namespace RogueSharpRLNetSamples
{
   public class DungeonMapCreationStrategy : IMapCreationStrategy<DungeonMap>
   {
      private readonly IRandom _random;
      private readonly int _height;
      private readonly int _maxRooms;
      private readonly int _roomMaxSize;
      private readonly int _roomMinSize;
      private readonly int _width;
      private readonly DungeonMap _map;

      public DungeonMapCreationStrategy( int width, int height, int maxRooms, int roomMaxSize, int roomMinSize, IRandom random )
      {
         _width = width;
         _height = height;
         _maxRooms = maxRooms;
         _roomMaxSize = roomMaxSize;
         _roomMinSize = roomMinSize;
         _random = random;
         _map = new DungeonMap();
      }

      public DungeonMap CreateMap()
      {
         _map.Initialize( _width, _height );

         for ( int r = 0; r < _maxRooms; r++ )
         {
            int roomWidth = _random.Next( _roomMinSize, _roomMaxSize );
            int roomHeight = _random.Next( _roomMinSize, _roomMaxSize );
            int roomXPosition = _random.Next( 0, _width - roomWidth - 1 );
            int roomYPosition = _random.Next( 0, _height - roomHeight - 1 );

            var newRoom = new Rectangle( roomXPosition, roomYPosition, roomWidth, roomHeight );
            bool newRoomIntersects = _map.Rooms.Any( room => newRoom.Intersects( room ) );
            if ( !newRoomIntersects )
            {
               _map.Rooms.Add( newRoom );
            }
         }

         foreach ( Rectangle room in _map.Rooms )
         {
            MakeRoom( room );
         }

         for ( int r = 0; r < _map.Rooms.Count; r++ )
         {
            if ( r == 0 )
            {
               continue;
            }

            int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
            int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
            int currentRoomCenterX = _map.Rooms[r].Center.X;
            int currentRoomCenterY = _map.Rooms[r].Center.Y;

            if ( _random.Next( 0, 2 ) == 0 )
            {
               MakeHorizontalTunnel( previousRoomCenterX, currentRoomCenterX, previousRoomCenterY );
               MakeVerticalTunnel( previousRoomCenterY, currentRoomCenterY, currentRoomCenterX );
            }
            else
            {
               MakeVerticalTunnel( previousRoomCenterY, currentRoomCenterY, previousRoomCenterX );
               MakeHorizontalTunnel( previousRoomCenterX, currentRoomCenterX, currentRoomCenterY );
            }
         }

         foreach ( Rectangle room in _map.Rooms )
         {
            MakeDoors( room );
         }

         MakeStairs();

         MakeMonsters();

         MakePlayer();

         return _map;
      }

      private void MakePlayer()
      {
         _map.AddPlayer( new Player {
            Armor = 10,
            Attack = 10,
            Awareness = 15,
            Color = Colors.Player,
            Gold = 0,
            Health = 100,
            MaxHealth = 100,
            Speed = 10,
            Symbol = '@',
            X = _map.Rooms[0].Center.X,
            Y = _map.Rooms[0].Center.Y
         } );
      }

      private void MakeRoom( Rectangle room )
      {
         for ( int x = room.Left + 1; x < room.Right; x++ )
         {
            for ( int y = room.Top + 1; y < room.Bottom; y++ )
            {
               _map.SetCellProperties( x, y, true, true );
            }
         }
      }

      private void MakeHorizontalTunnel( int xStart, int xEnd, int yPosition )
      {
         for ( int x = Math.Min( xStart, xEnd ); x <= Math.Max( xStart, xEnd ); x++ )
         {
            _map.SetCellProperties( x, yPosition, true, true );
         }
      }

      private void MakeVerticalTunnel( int yStart, int yEnd, int xPosition )
      {
         for ( int y = Math.Min( yStart, yEnd ); y <= Math.Max( yStart, yEnd ); y++ )
         {
            _map.SetCellProperties( xPosition, y, true, true );
         }
      }

      private void MakeDoors( Rectangle room )
      {
         int xMin = room.Left;
         int xMax = room.Right;
         int yMin = room.Top;
         int yMax = room.Bottom;

         List<Cell> borderCells = _map.GetCellsAlongLine( xMin, yMin, xMax, yMin ).ToList();
         borderCells.AddRange( _map.GetCellsAlongLine( xMin, yMin, xMin, yMax ) );
         borderCells.AddRange( _map.GetCellsAlongLine( xMin, yMax, xMax, yMax ) );
         borderCells.AddRange( _map.GetCellsAlongLine( xMax, yMin, xMax, yMax ) );

         foreach ( Cell cell in borderCells )
         {
            if ( IsPotentialDoor( cell ) )
            {
               _map.SetCellProperties( cell.X, cell.Y, false, true );
               _map.Doors.Add( new Door {
                  X = cell.X,
                  Y = cell.Y,
                  IsOpen = false
               } );
            }
         }
      }

      private bool IsPotentialDoor( Cell cell )
      {
         if ( !cell.IsWalkable )
         {
            return false;
         }

         Cell right = _map.GetCell( cell.X + 1, cell.Y );
         Cell left = _map.GetCell( cell.X - 1, cell.Y );
         Cell top = _map.GetCell( cell.X, cell.Y - 1 );
         Cell bottom = _map.GetCell( cell.X, cell.Y + 1 );

         if ( _map.GetDoor( cell.X, cell.Y ) != null ||
              _map.GetDoor( right.X, right.Y ) != null ||
              _map.GetDoor( left.X, left.Y ) != null ||
              _map.GetDoor( top.X, top.Y ) != null ||
              _map.GetDoor( bottom.X, bottom.Y ) != null )
         {
            return false;
         }

         if ( right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable )
         {
            return true;
         }
         if ( !right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable )
         {
            return true;
         }
         return false;
      }

      private void MakeStairs()
      {
         _map.StairsUp = new Stairs {
            X = _map.Rooms.First().Center.X + 1,
            Y = _map.Rooms.First().Center.Y,
            IsUp = true
         };
         _map.StairsDown = new Stairs {
            X = _map.Rooms.Last().Center.X,
            Y = _map.Rooms.Last().Center.Y,
            IsUp = false
         };
      }

      private void MakeMonsters()
      {
         // Place encounters that are well thought out in rooms

         // Place random monsters anywhere on floor tiles

         // Add a few more monsters to rooms
         foreach ( var room in _map.Rooms )
         {
            if ( Dice.Roll( "1D10" ) < 7 )
            {
               var numberOfMonsters = Dice.Roll( "1D4" );
               for ( int i = 0; i < numberOfMonsters; i++ )
               {
                  if ( DoesRoomHaveWalkableSpace( room ) )
                  {
                     Point randomRoomLocation = GetRandomLocationInRoom( room );
                     if ( randomRoomLocation != null )
                     {
                        _map.AddMonster( MakeGoblin( GetRandomLocationInRoom( room ) ) );
                     }
                  }
               }
            }
         }
      }

      private Monster MakeGoblin( Point location )
      {
         return new Monster 
         {
            Armor = 3,
            Attack = 3,
            Awareness = 10,
            Color = Colors.GoblinColor,
            Gold = Dice.Roll( "1D20" ),  
            Health = 10,
            MaxHealth = 10,
            Name = "Goblin",
            Speed = 12,
            Symbol = 'g',
            X = location.X,
            Y = location.Y
         };
      }

      private Point GetRandomLocationInRoom( Rectangle room )
      {
         int x = _random.Next( 1, room.Width - 2 ) + room.X;
         int y = _random.Next( 1, room.Height - 2 ) + room.Y;
         if ( !_map.IsWalkable( x, y ) )
         {
            GetRandomLocationInRoom( room );
         }
         return new Point( x, y );
      }

      private bool DoesRoomHaveWalkableSpace( Rectangle room )
      {
         for ( int x = 1; x <= room.Width - 2; x++ )
         {
            for ( int y = 1; y <= room.Height - 2; y++ )
            {
               if ( _map.IsWalkable( x + room.X, y + room.Y ) )
               {
                  return true;
               }
            }
         }
         return false;
      }
   }
}