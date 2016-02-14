namespace RogueSharpRLNetSamples.Equipment
{
   public class FeetEquipment : Core.Equipment
   {
      public static FeetEquipment None()
      {
         return new FeetEquipment { Name = "None" };
      }

      public static FeetEquipment Leather()
      {
         return new FeetEquipment() {
            Defense = 1,
            DefenseChance = 5,
            Name = "Leather"
         };
      }

      public static FeetEquipment Chain()
      {
         return new FeetEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Chain"
         };
      }

      public static FeetEquipment Plate()
      {
         return new FeetEquipment() {
            Defense = 1,
            DefenseChance = 15,
            Name = "Plate"
         };
      }
   }
}