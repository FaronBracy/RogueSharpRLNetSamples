using System;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Whirlwind : IAbility
   {
      public string Name { get; }

      public int TurnsToRefresh { get; }

      public int TurnsUntilRefreshed { get; private set; }

      private readonly CommandService _commandService;
      
      public Whirlwind( CommandService commandService )
      {
         Name = "Whirlwind";
         TurnsToRefresh = 20;
         TurnsUntilRefreshed = 0;
         _commandService = commandService;
      }

      public bool Perform()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            return false;
         }
         Player player = _commandService.DungeonMap.GetPlayer();
         Game.Messages.Add( $"{player.Name} performs a whirlwind attack against all adjacent enemies" );

         TurnsUntilRefreshed = TurnsToRefresh;

         return true;
      }

      public void Tick()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            TurnsUntilRefreshed--;
         }
      }
   }
}
