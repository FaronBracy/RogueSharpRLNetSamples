using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Services
{
   public class TargetingService
   {
      public bool IsPlayerTargeting { get; private set; }

      private Point _cursorPosition;
      private List<Point> _selectableTargets = new List<Point>();
      private int _currentTargetIndex;
      private ITargetable _targetable;
      private int _area;

      public bool SelectMonster( ITargetable targetable )
      {
         Initialize();
         DungeonMap map = Game.CommandService.DungeonMap;
         _selectableTargets = map.GetMonsterLocationsInFieldOfView().ToList();
         _targetable = targetable;
         _cursorPosition = _selectableTargets.FirstOrDefault();
         if ( _cursorPosition == null )
         {
            StopTargeting();
            return false;
         }

         IsPlayerTargeting = true;
         return true;
      }

      public bool SelectLocation( ITargetable targetable, int area = 0 )
      {
         Initialize();
         Player player = Game.CommandService.DungeonMap.GetPlayer();
         _cursorPosition = new Point { X = player.X, Y = player.Y };
         _targetable = targetable;
         _area = area;

         IsPlayerTargeting = true;
         return true;
      }

      private void StopTargeting()
      {
         IsPlayerTargeting = false;
         Initialize();
      }

      private void Initialize()
      {
         _cursorPosition = null;
         _selectableTargets = new List<Point>();
         _currentTargetIndex = 0;
         _area = 0;
         _targetable = null;
      }

      public bool HandleKey( RLKey key )
      {
         if ( _selectableTargets.Any() )
         {
            HandleSelectableTargeting( key );
         }
         else
         {
            HandleLocationTargeting( key );
         }

         if ( key == RLKey.Enter )
         {
            _targetable.SelectTarget( _cursorPosition );
            StopTargeting();
            return true;
         }

         return false;
      }

      private void HandleSelectableTargeting( RLKey key )
      {
         if ( key == RLKey.Right || key == RLKey.Down )
         {
            _currentTargetIndex++;
            if ( _currentTargetIndex >= _selectableTargets.Count )
            {
               _currentTargetIndex = 0;
            }
            _cursorPosition = _selectableTargets[_currentTargetIndex];
         }
         else if ( key == RLKey.Left || key == RLKey.Up )
         {
            _currentTargetIndex--;
            if ( _currentTargetIndex < 0 )
            {
               _currentTargetIndex = _selectableTargets.Count - 1;
            }
            _cursorPosition = _selectableTargets[_currentTargetIndex];
         }
      }

      private void HandleLocationTargeting( RLKey key )
      {
         int x = _cursorPosition.X;
         int y = _cursorPosition.Y;
         DungeonMap map = Game.CommandService.DungeonMap;

         if ( key == RLKey.Right )
         {
            x++;
         }
         else if ( key == RLKey.Left )
         {
            x--;
         }
         else if ( key == RLKey.Up )
         {
            y--;
         }
         else if ( key == RLKey.Down )
         {
            y++;
         }

         if ( map.IsInFov( x, y ) )
         {
            _cursorPosition.X = x;
            _cursorPosition.Y = y;
         }
      }

      public void Draw( RLConsole mapConsole )
      {
         if ( IsPlayerTargeting )
         {
            foreach ( Cell cell in Game.CommandService.DungeonMap.GetCellsInArea( _cursorPosition.X, _cursorPosition.Y, _area ) )
            {
               mapConsole.SetBackColor( cell.X, cell.Y, Swatch.DbSun );
            }
            mapConsole.SetBackColor( _cursorPosition.X, _cursorPosition.Y, Swatch.DbLight );
         }
      }
   }
}
