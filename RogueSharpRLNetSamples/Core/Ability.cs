using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
   public class Ability : IAbility, ITreasure, IDrawable
   {
      public Ability()
      {
         Symbol = '*';
         Color = RLColor.Yellow;
      }

      public string Name { get; protected set; }

      public int TurnsToRefresh { get; protected set; }

      public int TurnsUntilRefreshed { get; protected set; }

      public bool Perform()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            return false;
         }

         TurnsUntilRefreshed = TurnsToRefresh;

         return PerformAbility();
      }

      protected virtual bool PerformAbility()
      {
         return false;
      }


      public void Tick()
      {
         if ( TurnsUntilRefreshed > 0 )
         {
            TurnsUntilRefreshed--;
         }
      }

      public bool PickUp( IActor actor )
      {
         Player player = actor as Player;

         if ( player != null )
         {
            if ( player.AddAbility( this ) )
            {
               Game.MessageLog.Add( $"{actor.Name} learned the {Name} ability" );
               return true;
            }
         }

         return false;
      }

      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }
      public void Draw( RLConsole console, DungeonMap map )
      {
         if ( !map.IsExplored( X, Y ) )
         {
            return;
         }

         if ( map.IsInFov( X, Y ) )
         {
            console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol );
         }
         else
         {
            console.Set( X, Y, RLColor.Blend( Color, RLColor.Gray, 0.5f ), Colors.FloorBackground, Symbol );
         }
      }
   }
}
