using System.Collections.Generic;
using System.Linq;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Systems
{
   public class SchedulingSystem
   {
      private int _time;
      private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

      public SchedulingSystem()
      {
         _time = 0;
         _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
      }

      public void Add( IScheduleable actor )
      {
         int key = _time + actor.Time;
         if ( !_scheduleables.ContainsKey( key ) )
         {
            _scheduleables.Add( key, new List<IScheduleable>() );
         }
         _scheduleables[key].Add( actor );
      }

      public void Remove( IScheduleable scheduleable )
      {
         KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>( -1, null );

         foreach ( var scheduleablesList in _scheduleables )
         {
            if ( scheduleablesList.Value.Contains( scheduleable ) )
            {
               scheduleableListFound = scheduleablesList;
               break;
            }
         }
         if ( scheduleableListFound.Value != null )
         {
            scheduleableListFound.Value.Remove( scheduleable );
            if ( scheduleableListFound.Value.Count <= 0 )
            {
               _scheduleables.Remove( scheduleableListFound.Key );
            }
         }
      }

      public IScheduleable Get()
      {
         var firstScheduleableGroup = _scheduleables.First();
         var firstScheduleable = firstScheduleableGroup.Value.First();
         Remove( firstScheduleable );
         _time = firstScheduleableGroup.Key;
         return firstScheduleable;
      }

      public int GetTime()
      {
         return _time;
      }

      public void Clear()
      {
         _time = 0;
         _scheduleables.Clear();
      }
   }
}
