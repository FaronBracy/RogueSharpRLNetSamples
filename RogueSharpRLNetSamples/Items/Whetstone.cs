﻿using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Items
{
   public class Whetstone : IItem
   {
      public string Name { get; }
      public int RemainingUses { get; private set; }

      public Whetstone()
      {
         Name = "Whetstone";
         RemainingUses = 5;
      }

      public bool Use()
      {
         Player player = Game.CommandSystem.DungeonMap.GetPlayer();

         if ( player.Hand == HandEquipment.None() )
         {
            Game.Messages.Add( $"{player.Name} is not holding anything they can sharpen" );
         }
         else if ( player.AttackChance >= 80 )
         {
            Game.Messages.Add( $"{player.Name} cannot make their {player.Hand.Name} any sharper" );
         }
         else
         {
            Game.Messages.Add( $"{player.Name} uses a {Name} to sharper their {player.Hand.Name}" );
            player.Hand.AttackChance += Dice.Roll( "1D3" );
            RemainingUses--;
         }

         return true;
      }
   }
}
