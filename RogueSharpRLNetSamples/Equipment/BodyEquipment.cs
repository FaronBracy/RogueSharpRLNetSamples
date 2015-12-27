namespace RogueSharpRLNetSamples.Equipment
{
   public class BodyEquipment : Equipment
   {
      public static BodyEquipment None()
      {
         return new BodyEquipment { Name = "None" };
      }
   }
}