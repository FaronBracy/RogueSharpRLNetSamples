using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Items;

namespace RogueSharpRLNetSamples.Systems
{
   public static class ItemGenerator
   {
      public static Item CreateItem()
      {
         Pool<Item> itemPool = new Pool<Item>();

         itemPool.Add( new ArmorScroll(), 10 );
         itemPool.Add( new DestructionWand(), 5 );
         itemPool.Add( new HealingPotion(), 20 );
         itemPool.Add( new RevealMapScroll(), 25 );
         itemPool.Add( new TeleportScroll(), 20 );
         itemPool.Add( new Whetstone(), 10 );

         return itemPool.Get();
      }
   }
}
