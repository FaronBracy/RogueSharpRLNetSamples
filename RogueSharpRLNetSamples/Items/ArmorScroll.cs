using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Equipment;

namespace RogueSharpRLNetSamples.Items
{
   public class ArmorScroll : Item
   {
      public ArmorScroll()
      {
         Name = "Armor Scroll";
         RemainingUses = 1;
      }

      protected override bool UseItem()
      {
         Player player = Game.Player;

         if ( player.Body == BodyEquipment.None() )
         {
            Game.MessageLog.Add( $"{player.Name} is not wearing any body armor to enhance" );
         }
         else if ( player.Defense >= 8 )
         {
            Game.MessageLog.Add( $"{player.Name} cannot enhance their {player.Body.Name} any more" );
         }
         else
         {
            Game.MessageLog.Add( $"{player.Name} uses a {Name} to enhance their {player.Body.Name}" );
            player.Body.Defense += 1;
            RemainingUses--;
         }

         return true;
      }
   }
}

