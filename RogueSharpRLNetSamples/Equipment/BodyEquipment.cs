namespace RogueSharpRLNetSamples.Equipment
{
   public class BodyEquipment : Core.Equipment
   {
      public static BodyEquipment None()
      {
         return new BodyEquipment { Name = "None" };
      }

      public static BodyEquipment Leather()
      {
         return new BodyEquipment() {
            Defense = 1,
            DefenseChance = 10,
            Name = "Leather"
         };
      }

      public static BodyEquipment Chain()
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 5,
            Name = "Chain"
         };
      }

      public static BodyEquipment Plate()
      {
         return new BodyEquipment() {
            Defense = 2,
            DefenseChance = 10,
            Name = "Plate"
         };
      }
   }
}