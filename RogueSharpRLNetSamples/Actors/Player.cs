using System;
using System.Data.Odbc;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Abilities;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Actors
{
   public class Player : Actor
   {
      public IAbility QAbility { get; set; }
      public IAbility WAbility { get; set; }
      public IAbility EAbility { get; set; }
      public IAbility RAbility { get; set; }

      public Player()
      {
         QAbility = new DoNothing();
         WAbility = new DoNothing();
         EAbility = new DoNothing();
         RAbility = new DoNothing();
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
         inventoryConsole.Print( 1, 1, "Equipment", RLColor.White );
         inventoryConsole.Print( 1, 3, $"Head: {Head.Name}", RLColor.LightGray );
         inventoryConsole.Print( 1, 5, $"Body: {Body.Name}", RLColor.LightGray );
         inventoryConsole.Print( 1, 7, $"Hand: {Hand.Name}", RLColor.LightGray );
         inventoryConsole.Print( 1, 9, $"Feet: {Feet.Name}", RLColor.LightGray );

         inventoryConsole.Print( 28, 1, "Abilities", RLColor.White );
         inventoryConsole.Print( 28, 3, $"Q - {QAbility.Name}", RLColor.LightGray );
         inventoryConsole.Print( 28, 5, $"W - {WAbility.Name}", RLColor.LightGray );
         inventoryConsole.Print( 28, 7, $"E - {EAbility.Name}", RLColor.LightGray );
         inventoryConsole.Print( 28, 9, $"R - {RAbility.Name}", RLColor.LightGray );

         inventoryConsole.Print( 55, 1, "Items", RLColor.White );
         inventoryConsole.Print( 55, 3, "1 - Health Potion", RLColor.LightGray );
         inventoryConsole.Print( 55, 5, "2 - Mana Potion", RLColor.LightGray );
         inventoryConsole.Print( 55, 7, "3 - Scroll", RLColor.LightGray );
         inventoryConsole.Print( 55, 9, "4 - Wand", RLColor.LightGray );

         DrawAbility( QAbility, inventoryConsole, 0 );
         DrawAbility( WAbility, inventoryConsole, 1 );
         DrawAbility( EAbility, inventoryConsole, 2 );
         DrawAbility( RAbility, inventoryConsole, 3 );
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

         RLColor highlightTextColor = RLColor.LightGray;
         if ( !( ability is DoNothing ) )
         {
            if ( ability.TurnsUntilRefreshed == 0 && !( ability is DoNothing ) )
            {
               highlightTextColor = Swatch.PrimaryLightest;
            }
            else
            {
               highlightTextColor = Swatch.SecondaryLightest;
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
            inventoryConsole.SetBackColor( xHighlightPosition, yPosition, width, 1, Swatch.SecondaryDarkest );
            inventoryConsole.SetBackColor( xHighlightPosition + width, yPosition, remainingWidth, 1, RLColor.Black );
         }
      }

      public void Tick()
      {
         QAbility?.Tick();
         WAbility?.Tick();
         EAbility?.Tick();
         RAbility?.Tick();
      }

      public void Draw( RLConsole mapConsole )
      {
         mapConsole.Set( X, Y, Color, null, Symbol );
      }
   }
}