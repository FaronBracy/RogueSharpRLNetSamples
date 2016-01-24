using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class Ability : IAbility
   {
      public string Name { get; protected set; }

      public int TurnsToRefresh { get; protected set; }

      public int TurnsUntilRefreshed { get; protected set; }

      public bool Perform()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            return false;
         }

         TurnsUntilRefreshed = TurnsToRefresh;

         return PerformAbility();
      }

      protected virtual bool PerformAbility()
      {
         return false;
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
