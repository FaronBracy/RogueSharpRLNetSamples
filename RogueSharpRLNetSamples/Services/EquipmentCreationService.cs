using RogueSharpRLNetSamples.Inventory;

namespace RogueSharpRLNetSamples.Services
{
   public class EquipmentCreationService
   {
      private readonly Pool<Equipment> _equipmentPool;

      public EquipmentCreationService( int level )
      {
         _equipmentPool = new Pool<Equipment>();

         if ( level <= 3 )
         {
            _equipmentPool.Add( BodyEquipment.Leather(), 20 );
            _equipmentPool.Add( HeadEquipment.Leather(), 20 );
            _equipmentPool.Add( FeetEquipment.Leather(), 20 );
            _equipmentPool.Add( HandEquipment.Dagger(), 25 );
            _equipmentPool.Add( HandEquipment.Sword(), 5 );
            _equipmentPool.Add( HeadEquipment.Chain(), 5 );
            _equipmentPool.Add( BodyEquipment.Chain(), 5 );
         }
         else if ( level <= 6 )
         {
            _equipmentPool.Add( BodyEquipment.Chain(), 20 );
            _equipmentPool.Add( HeadEquipment.Chain(), 20 );
            _equipmentPool.Add( FeetEquipment.Chain(), 20 );
            _equipmentPool.Add( HandEquipment.Sword(), 15 );
            _equipmentPool.Add( HandEquipment.Axe(), 15 );
            _equipmentPool.Add( HeadEquipment.Plate(), 5 );
            _equipmentPool.Add( BodyEquipment.Plate(), 5 );
         }
         else
         {
            _equipmentPool.Add( BodyEquipment.Plate(), 25 );
            _equipmentPool.Add( HeadEquipment.Plate(), 25 );
            _equipmentPool.Add( FeetEquipment.Plate(), 25 );
            _equipmentPool.Add( HandEquipment.TwoHandedSword(), 25 );
         }
      }

      public Equipment CreateEquipment()
      { 
         return _equipmentPool.Get();
      }
   }
}
