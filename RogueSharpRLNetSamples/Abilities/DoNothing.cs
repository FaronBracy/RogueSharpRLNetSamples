using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Abilities
{
   public class DoNothing : IAbility
   {
      public string Name { get; }

      public int TurnsToRefresh { get; }

      public int TurnsUntilRefreshed { get; }

      public DoNothing()
      {
         Name = "None";
         TurnsToRefresh = 0;
         TurnsUntilRefreshed = 0;
      }

      public bool Perform()
      {
         Game.Messages.Add( "No ability in that slot" );  
         return false;
      }

      public void Tick()
      {
      }
   }
}
