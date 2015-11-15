using System.Collections.Generic;
using System.Linq;
using RLNET;

namespace RogueSharpRLNetSamples
{
   public class ActorSchedule
   {
      private int _time;
      private readonly SortedDictionary<int, List<IActor>> _actors;

      public ActorSchedule()
      {
         _time = 0;
         _actors = new SortedDictionary<int, List<IActor>>();
      }

      public void Add( IActor actor )
      {
         int key = _time + actor.Speed;
         if ( !_actors.ContainsKey( key ) )
         {
            _actors.Add( key, new List<IActor>() );
         }
         _actors[key].Add( actor );
      }

      public void Remove( IActor actor )
      {
         KeyValuePair<int, List<IActor>> actorListFound = new KeyValuePair<int, List<IActor>>( -1, null );

         foreach ( var actorList in _actors )
         {
            if ( actorList.Value.Contains( actor ) )
            {
               actorListFound = actorList;
               break;
            }
         }
         if ( actorListFound.Value != null )
         {
            actorListFound.Value.Remove( actor );
            if ( actorListFound.Value.Count <= 0 )
            {
               _actors.Remove( actorListFound.Key );
            }
         }
      }

      public IActor Get()
      {
         var firstActorGroup = _actors.First();
         var firstActor = firstActorGroup.Value.First();
         Remove( firstActor );
         _time = firstActorGroup.Key;
         return firstActor;
      }

      public int GetTime()
      {
         return _time;
      }
   }
}
