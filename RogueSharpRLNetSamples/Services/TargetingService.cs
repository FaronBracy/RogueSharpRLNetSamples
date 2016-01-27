using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
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

      public bool SelectMonster( ITargetable targetable )
      {
         DungeonMap map = Game.CommandService.DungeonMap;
         _selectableTargets = map.GetMonsterLocationsInFieldOfView().ToList();
         _targetable = targetable;
         _currentTargetIndex = 0;
         _cursorPosition = _selectableTargets.FirstOrDefault();
         if ( _cursorPosition == null )
         {
            StopTargeting();
            return false;
         }

         IsPlayerTargeting = true;
         return true;
      }

      private void StopTargeting()
      {
         IsPlayerTargeting = false;
         _cursorPosition = null;
         _selectableTargets = new List<Point>();
         _currentTargetIndex = 0;
         _targetable = null;
      }

      public bool HandleKey( RLKey key )
      {
         if ( key == RLKey.N )
         {
            _currentTargetIndex++;
            if ( _currentTargetIndex >= _selectableTargets.Count )
            {
               _currentTargetIndex = 0;
            }
            _cursorPosition = _selectableTargets[_currentTargetIndex];
         }

         if ( key == RLKey.Enter )
         {
            _targetable.SelectTarget( _cursorPosition );
            StopTargeting();
            return true;
         }

         return false;
      }

      public void Draw( RLConsole mapConsole )
      {
         if ( IsPlayerTargeting )
         {
            mapConsole.SetBackColor( _cursorPosition.X, _cursorPosition.Y, Swatch.DbSun );
         }
      }
   }
}
