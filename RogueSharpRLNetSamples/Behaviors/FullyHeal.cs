using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Behaviors
{
   public class FullyHeal : IBehavior
   {
      public bool Act( Monster monster, CommandSystem commandSystem )
      {
         if ( monster.Health < monster.MaxHealth )
         {
            int healthToRecover = monster.MaxHealth - monster.Health;
            monster.Health = monster.MaxHealth;
            Game.MessageLog.Add( $"{monster.Name} catches his breath and recovers {healthToRecover} health" );    
            return true;
         }
         return false;
      }
   }
}
