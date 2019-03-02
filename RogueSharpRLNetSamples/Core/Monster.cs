using System;
using RLNET;
using RogueSharpRLNetSamples.Behaviors;
using RogueSharpRLNetSamples.Monsters;
using RogueSharpRLNetSamples.Systems;

namespace RogueSharpRLNetSamples.Core
{
   public class Monster : Actor
   {
      public int? TurnsAlerted { get; set; }

      public void DrawStats( RLConsole statConsole, int position )
      {
         int yPosition = 13 + ( position * 2 );
         statConsole.Print( 1, yPosition, Symbol.ToString(), Color );
         int width = Convert.ToInt32( ( (double) Health / (double) MaxHealth ) * 16.0 );
         int remainingWidth = 16 - width;
         statConsole.SetBackColor( 3, yPosition, width, 1, Swatch.Primary );
         statConsole.SetBackColor( 3 + width, yPosition, remainingWidth, 1, Swatch.PrimaryDarkest );  
         statConsole.Print( 2, yPosition, $": {Name}", RLColor.White );
      }



      public static Monster Clone( Monster anotherMonster )
      {
         return new Ooze {
            Attack = anotherMonster.Attack,
            AttackChance = anotherMonster.AttackChance,
            Awareness = anotherMonster.Awareness,
            Color = anotherMonster.Color,
            Defense = anotherMonster.Defense,
            DefenseChance = anotherMonster.DefenseChance,
            Gold = anotherMonster.Gold,
            Health = anotherMonster.Health,
            MaxHealth = anotherMonster.MaxHealth,
            Name = anotherMonster.Name,
            Speed = anotherMonster.Speed,
            Symbol = anotherMonster.Symbol
         };
      }

      public virtual void PerformAction( CommandSystem commandSystem )
      {
         var behavior = new StandardMoveAndAttack();
         behavior.Act( this, commandSystem );
      }
   }
}