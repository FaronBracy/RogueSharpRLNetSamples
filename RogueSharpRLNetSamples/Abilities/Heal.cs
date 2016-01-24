using System;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Heal : Ability
   {
      private readonly CommandService _commandService;
      private readonly int _amountToHeal;

      public Heal( CommandService commandService, int amountToHeal )
      {
         Name = "Heal Self";
         TurnsToRefresh = 60;
         TurnsUntilRefreshed = 0;
         _commandService = commandService;
         _amountToHeal = amountToHeal;
      }

      protected override bool PerformAbility()
      {
         Player player = _commandService.DungeonMap.GetPlayer();

         player.Health = Math.Min( player.MaxHealth, player.Health + _amountToHeal );

         return true;
      }
   }
}
