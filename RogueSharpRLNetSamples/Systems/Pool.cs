using System;
using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharpRLNetSamples.Systems
{
   public class Pool<T>
   {
      private readonly List<PoolItem<T>> _poolItems;
      private static readonly IRandom _random = new DotNetRandom();
      private int _totalWeight;

      public Pool()
      {
         _poolItems = new List<PoolItem<T>>();
      }

      public T Get()
      {
         int runningWeight = 0;
         int roll = _random.Next( 1, _totalWeight );
         foreach ( var poolItem in _poolItems )
         {
            runningWeight += poolItem.Weight;
            if ( roll <= runningWeight )
            {
               Remove( poolItem );  
               return poolItem.Item;
            }
         }

         throw new InvalidOperationException( "Could not get an item from the pool" );
      }

      public void Add( T item, int weight )
      {
         _poolItems.Add( new PoolItem<T> { Item = item, Weight = weight } );
         _totalWeight += weight;
      }

      public void Remove( PoolItem<T> poolItem )
      {
         _poolItems.Remove( poolItem );
         _totalWeight -= poolItem.Weight;
      }
   }

   public class PoolItem<T>
   {
      public int Weight { get; set; }
      public T Item { get; set; }
   }
}
