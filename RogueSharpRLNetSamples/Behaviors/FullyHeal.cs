using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Behavior
{
   public class FullyHeal : IBehavior
   {
      public bool Act( Monster monster, CommandService commandService )
      {
         if ( monster.Health < monster.MaxHealth )
         {
            int healthToRecover = monster.MaxHealth - monster.Health;
            monster.Health = monster.MaxHealth;
            Game.Messages.Add( string.Format( "{0} catches his breath and recovers {1} health", monster.Name, healthToRecover ) );    
            return true;
         }
         return false;
      }
   }
}
