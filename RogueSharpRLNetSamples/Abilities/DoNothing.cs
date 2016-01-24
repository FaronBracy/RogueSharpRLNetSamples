﻿using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class DoNothing : Ability
   {
      public DoNothing()
      {
         Name = "None";
         TurnsToRefresh = 0;
         TurnsUntilRefreshed = 0;
      }

      protected override bool PerformAbility()
      {
         Game.Messages.Add( "No ability in that slot" );
         return false;
      }
   }
}
