using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class ArmorScroll : IItem
   {
      public string Name { get; }
      public int RemainingUses { get; private set; }

      public ArmorScroll()
      {
         Name = "Armor Scroll";
         RemainingUses = 1;
      }

      public bool Use()
      {
         Player player = Game.CommandSystem.DungeonMap.GetPlayer();

         if ( player.Body == BodyEquipment.None() )
         {
            Game.Messages.Add( $"{player.Name} is not wearing any body armor to enhance" );
         }
         else if ( player.Defense >= 8 )
         {
            Game.Messages.Add( $"{player.Name} cannot enhance their {player.Body.Name} any more" );
         }
         else
         {
            Game.Messages.Add( $"{player.Name} uses a {Name} to enhance their {player.Body.Name}" );
            player.Body.Defense += 1;
            RemainingUses--;
         }

         return true;
      }
   }
}

