using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Services
{
   public static class AbilityCreationService
   {
      public static Pool<Ability> _abilityPool = null;

      public static Ability CreateAbility()
      {
         if ( _abilityPool == null )
         {
            _abilityPool = new Pool<Ability>();
            _abilityPool.Add( new Heal( 10 ), 1 );
            _abilityPool.Add( new MagicMissile(), 1 );
            _abilityPool.Add( new RevealMap( 15 ), 1 );
            _abilityPool.Add( new Whirlwind(), 1 );
         }

         return _abilityPool.Get();
      }
   }
}
