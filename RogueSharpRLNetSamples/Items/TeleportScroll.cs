using RogueSharp;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class TeleportScroll : IItem
   {
      public string Name { get; }
      public int RemainingUses { get; private set; }

      public TeleportScroll()
      {
         Name = "Teleport Scroll";
         RemainingUses = 1;
      }

      public bool Use()
      {
         DungeonMap map = Game.CommandSystem.DungeonMap;
         Player player = Game.CommandSystem.DungeonMap.GetPlayer();

         Game.Messages.Add( $"{player.Name} uses a {Name} and reappears in another place" );

         Point point = map.GetRandomLocation( new DotNetRandom() );
         
         map.SetActorPosition( player, point.X, point.Y );
         
         RemainingUses--;

         return true;
      }
   }
}

