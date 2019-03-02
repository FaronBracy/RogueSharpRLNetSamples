using System;
using RLNET;
using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Items;

namespace RogueSharpRLNetSamples.Core
{
   public class Player : Actor
   {
      public IAbility QAbility { get; set; }
      public IAbility WAbility { get; set; }
      public IAbility EAbility { get; set; }
      public IAbility RAbility { get; set; }

      public IItem Item1 { get; set; }
      public IItem Item2 { get; set; }
      public IItem Item3 { get; set; }
      public IItem Item4 { get; set; }

      public Player()
      {
         QAbility = new DoNothing();
         WAbility = new DoNothing();
         EAbility = new DoNothing();
         RAbility = new DoNothing();
         Item1 = new NoItem();
         Item2 = new NoItem();
         Item3 = new NoItem();
         Item4 = new NoItem();
      }

      public bool AddAbility( IAbility ability )
      {
         if ( QAbility is DoNothing )
         {
            QAbility = ability;
         }
         else if ( WAbility is DoNothing )
         {
            WAbility = ability;
         }
         else if ( EAbility is DoNothing )
         {
            EAbility = ability;
         }
         else if ( RAbility is DoNothing )
         {
            RAbility = ability;
         }
         else
         {
            return false;
         }

         return true;
      }

      public bool AddItem( IItem item )
      {
         if ( Item1 is NoItem )
         {
            Item1 = item;
         }
         else if ( Item2 is NoItem )
         {
            Item2 = item;
         }
         else if( Item3 is NoItem )
         {
            Item3 = item;
         }
         else if( Item4 is NoItem )
         {
            Item4 = item;
         }
         else
         {
            return false;
         }

         return true;
      }

      public void DrawStats( RLConsole statConsole )
      {
         statConsole.Print( 1, 1, $"Name:    {Name}", RLColor.White );
         statConsole.Print( 1, 3, $"Health:  {Health}/{MaxHealth}", RLColor.White );
         statConsole.Print( 1, 5, $"Attack:  {Attack} ({AttackChance}%)", RLColor.White );
         statConsole.Print( 1, 7, $"Defense: {Defense} ({DefenseChance}%)", RLColor.White );
         statConsole.Print( 1, 9, $"Gold:    {Gold}", RLColor.Yellow );
      }

      public void DrawInventory( RLConsole inventoryConsole )
      {
         inventoryConsole.Print( 1, 1, "Equipment", Colors.InventoryHeading );
         inventoryConsole.Print( 1, 3, $"Head: {Head.Name}", Head == HeadEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight );
         inventoryConsole.Print( 1, 5, $"Body: {Body.Name}", Body == BodyEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight );
         inventoryConsole.Print( 1, 7, $"Hand: {Hand.Name}", Hand == HandEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight );
         inventoryConsole.Print( 1, 9, $"Feet: {Feet.Name}", Feet == FeetEquipment.None() ? Swatch.DbOldStone : Swatch.DbLight );

         inventoryConsole.Print( 28, 1, "Abilities", Colors.InventoryHeading );
         DrawAbility( QAbility, inventoryConsole, 0 );
         DrawAbility( WAbility, inventoryConsole, 1 );
         DrawAbility( EAbility, inventoryConsole, 2 );
         DrawAbility( RAbility, inventoryConsole, 3 );

         inventoryConsole.Print( 55, 1, "Items", Colors.InventoryHeading );
         DrawItem( Item1, inventoryConsole, 0 );
         DrawItem( Item2, inventoryConsole, 1 );
         DrawItem( Item3, inventoryConsole, 2 );
         DrawItem( Item4, inventoryConsole, 3 );
      }

      private void DrawAbility( IAbility ability, RLConsole inventoryConsole, int position )
      {
         char letter = 'Q';
         if ( position == 0 )
         {
            letter = 'Q';
         }
         else if ( position == 1 )
         {
            letter = 'W';
         }
         else if ( position == 2 )
         {
            letter = 'E';
         }
         else if ( position == 3 )
         {
            letter = 'R';
         }

         RLColor highlightTextColor = Swatch.DbOldStone;
         if ( !( ability is DoNothing ) )
         {
            if ( ability.TurnsUntilRefreshed == 0 )
            {
               highlightTextColor = Swatch.DbLight;
            }
            else
            {
               highlightTextColor = Swatch.DbSkin;
            }
         }

         int xPosition = 28;
         int xHighlightPosition = 28 + 4;
         int yPosition = 3 + ( position * 2 );
         inventoryConsole.Print( xPosition, yPosition, $"{letter} - {ability.Name}", highlightTextColor );

         if ( ability.TurnsToRefresh > 0 )
         {
            int width = Convert.ToInt32( ( (double) ability.TurnsUntilRefreshed / (double) ability.TurnsToRefresh ) * 16.0 );
            int remainingWidth = 20 - width;
            inventoryConsole.SetBackColor( xHighlightPosition, yPosition, width, 1, Swatch.DbOldBlood );
            inventoryConsole.SetBackColor( xHighlightPosition + width, yPosition, remainingWidth, 1, RLColor.Black );
         }
      }

      private void DrawItem( IItem item, RLConsole inventoryConsole, int position )
      {
         int xPosition = 55;
         int yPosition = 3 + ( position * 2 );
         string place = ( position + 1 ).ToString();
         inventoryConsole.Print( xPosition, yPosition, $"{place} - {item.Name}", item is NoItem ? Swatch.DbOldStone : Swatch.DbLight );
      }

      public void Tick()
      {
         QAbility?.Tick();
         WAbility?.Tick();
         EAbility?.Tick();
         RAbility?.Tick();
      }
   }
}