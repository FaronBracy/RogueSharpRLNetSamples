using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Interfaces
{
   public interface IBehavior
   {
      bool Act( Monster monster, CommandSystem commandSystem );
   }
}
