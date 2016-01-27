using RogueSharpRLNetSamples.Abilities;

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
            _abilityPool.Add( new MagicMissile( 2, 80 ), 1 );
            _abilityPool.Add( new RevealMap( 15 ), 1 );
            _abilityPool.Add( new Whirlwind(), 1 );
            _abilityPool.Add( new Fireball( 4, 60, 2 ), 1 );
            _abilityPool.Add( new LightningBolt( 6, 40 ), 1 );
         }

         return _abilityPool.Get();
      }
   }
}
