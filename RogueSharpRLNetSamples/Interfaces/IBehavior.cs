using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Interfaces
{
   public interface IBehavior
   {
      bool Act( Monster monster, CommandService commandService );
   }
}
