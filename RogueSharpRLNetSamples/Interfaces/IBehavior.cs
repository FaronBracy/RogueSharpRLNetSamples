using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behavior
{
   public interface IBehavior
   {
      bool Act( Monster monster, CommandService commandService );
   }
}
