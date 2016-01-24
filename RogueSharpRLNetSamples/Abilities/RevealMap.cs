using RogueSharp;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Services;

namespace RogueSharpRLNetSamples.Abilities
{
   public class RevealMap : Ability
   {
      private readonly CommandService _commandService;
      private readonly int _revealDistance;

      public RevealMap( CommandService commandService, int revealDistance )
      {
         Name = "Reveal Map";
         TurnsToRefresh = 100;
         TurnsUntilRefreshed = 0;
         _commandService = commandService;
         _revealDistance = revealDistance;
      }

      protected override bool PerformAbility()
      {
         DungeonMap map = _commandService.DungeonMap;
         Player player = map.GetPlayer();

         foreach ( Cell cell in map.GetCellsInArea( player.X, player.Y, _revealDistance ) )
         {
            if ( cell.IsWalkable )
            {
               map.SetCellProperties( cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true );
            }
         }

         return true;
      }
   }
}
