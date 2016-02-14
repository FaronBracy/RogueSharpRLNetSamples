﻿using RogueSharp;
using RogueSharp.Random;
using RogueSharpRLNetSamples.Actors;

namespace RogueSharpRLNetSamples.Items
{
   public class TeleportScroll : Item
   {
      public TeleportScroll()
      {
         Name = "Teleport Scroll";
         RemainingUses = 1;
      }

      protected override bool UseItem()
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

