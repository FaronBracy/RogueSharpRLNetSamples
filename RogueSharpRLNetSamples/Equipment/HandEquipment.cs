namespace RogueSharpRLNetSamples.Equipment
{
   public class HandEquipment : Equipment
   {
      public static HandEquipment None()
      {
         return new HandEquipment { Name = "None" };
      }
   }
}