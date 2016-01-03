using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Inventory;

namespace RogueSharpRLNetSamples.Services
{
   public static class EquipmentCreationService
   {
      public static Equipment CreateEquipment( int level )
      {
         int result = Dice.Roll( "1D100" );
         if ( level <= 3 )
         {
            if ( result > 80 )
            {
               return BodyEquipment.Leather();
            }
            if ( result > 60 )
            {
               return HeadEquipment.Leather();
            }
            if ( result > 40 )
            {
               return FeetEquipment.Leather();
            }
            if ( result > 15 )
            {
               return HandEquipment.Dagger();
            }
            if ( result > 10 )
            {
               return HandEquipment.Sword();
            }
            if ( result > 5 )
            {
               return HeadEquipment.Chain();
            }
            return BodyEquipment.Chain();
         }
         if ( level <= 6 )
         {
            if ( result > 80 )
            {
               return BodyEquipment.Chain();
            }
            if ( result > 60 )
            {
               return HeadEquipment.Chain();
            }
            if ( result > 40 )
            {
               return FeetEquipment.Chain();
            }
            if ( result > 15 )
            {
               return HandEquipment.Sword();
            }
            if ( result > 10 )
            {
               return HandEquipment.Axe();
            }
            if ( result > 5 )
            {
               return HeadEquipment.Plate();
            }
            return BodyEquipment.Plate();
         }
         if ( result > 75 )
         {
            return BodyEquipment.Plate();
         }
         if ( result > 50 )
         {
            return HeadEquipment.Plate();
         }
         if ( result > 25 )
         {
            return FeetEquipment.Plate();
         }
         return HandEquipment.TwoHandedSword();
      }
   }
}
