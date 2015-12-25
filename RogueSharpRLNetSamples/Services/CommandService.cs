using System.Text;
using RogueSharp.DiceNotation;

namespace RogueSharpRLNetSamples.Services
{
   public class CommandService
   {
      private DungeonMap _dungeonMap;

      public CommandService( DungeonMap dungeonMap )
      {
         _dungeonMap = dungeonMap;
      }

      public void Attack( Actor attacker, Actor defender )
      {
         StringBuilder attackMessage = new StringBuilder();
         attackMessage.AppendFormat( "{0} attacks {1} and rolls: ", attacker.Name, defender.Name );
         DiceExpression attackDice = new DiceExpression()
            .Dice( attacker.Attack, 100 );

         int hits = 0;
         int blocks = 0;

         DiceResult attackResult = attackDice.Roll();
         foreach ( TermResult termResult in attackResult.Results )
         {
            attackMessage.Append( termResult.Value + ", " );
            if ( termResult.Value >= 100 - attacker.AttackChance )
            {
               hits++;
            }
         }

         StringBuilder defenseMessage = new StringBuilder();
         if ( hits > 0 )
         {
            attackMessage.AppendFormat( "scoring {0} hits.", hits );
            defenseMessage.AppendFormat( "  {0} defends and rolls: ", defender.Name );
            DiceExpression defenseDice = new DiceExpression()
               .Dice( defender.Defense, 100 );

            DiceResult defenseRoll = defenseDice.Roll();
            foreach ( TermResult termResult in defenseRoll.Results )
            {
               defenseMessage.Append( termResult.Value + ", " );
               if ( termResult.Value >= 100 - defender.DefenseChance )
               {
                  blocks++;
               }
            }
            defenseMessage.AppendFormat( "resulting in {0} blocks.", blocks );
         }
         else
         {
            attackMessage.Append( "and misses completely." );
         }

         Game.Messages.Add( attackMessage.ToString() );
         if ( !string.IsNullOrEmpty( defenseMessage.ToString() ) )
         {
            Game.Messages.Add( defenseMessage.ToString() );
         }

         int damage = hits - blocks;

         if ( damage > 0 )
         {
            defender.Health = defender.Health - damage;

            Game.Messages.Add( string.Format( "  {0} was hit for {1} damage", defender.Name, damage ) );

            if ( defender.Health <= 0 )
            {
               if ( defender is Player )
               {
                  Game.Messages.Add( string.Format( "  {0} was killed, GAME OVER MAN!", defender.Name ) );
               }
               else if ( defender is Monster )
               {
                  _dungeonMap.AddGold( defender.X, defender.Y, defender.Gold );
                  _dungeonMap.RemoveMonster( (Monster) defender );

                  Game.Messages.Add( string.Format( "  {0} died and dropped {1} gold", defender.Name, defender.Gold ) );
               }
            }
         }
         else
         {
            Game.Messages.Add( string.Format( "  {0} blocked all damage", defender.Name, damage ) );
         }
      }
   }
}
