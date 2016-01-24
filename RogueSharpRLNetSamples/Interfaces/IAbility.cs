using System.Security.Cryptography.X509Certificates;

namespace RogueSharpRLNetSamples.Interfaces
{
   public interface IAbility
   {
      string Name { get; }
      int TurnsToRefresh { get; }
      int TurnsUntilRefreshed { get; }

      bool Perform();
      void Tick();
   }
}
