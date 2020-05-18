using System.Text;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Items;

namespace RogueSharpRLNetSamples.Systems
{
   public class CommandSystem
   {
      public bool IsPlayerTurn { get; set; }

      public bool MovePlayer( Direction direction )
      {
         int x;
         int y;

         switch ( direction )
         {
            case Direction.Up:
            {
               x = Game.Player.X;
               y = Game.Player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = Game.Player.X;
               y = Game.Player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = Game.Player.X - 1;
               y = Game.Player.Y;
               break;
            }
            case Direction.Right:
            {
               x = Game.Player.X + 1;
               y = Game.Player.Y;
               break;
            }
            default:
            {
               return false;
            }
         }

         if ( Game.DungeonMap.SetActorPosition( Game.Player, x, y ) )
         {
            return true;
         }

         Monster monster = Game.DungeonMap.GetMonsterAt( x, y );

         if ( monster != null )
         {
            Attack( Game.Player, monster );
            return true;
         }

         return false;
      }

      public void ActivateMonsters()
      {
         IScheduleable scheduleable = Game.SchedulingSystem.Get();
         if ( scheduleable is Player )
         {
            IsPlayerTurn = true;
            Game.SchedulingSystem.Add( Game.Player );
         }
         else
         {
            Monster monster = scheduleable as Monster;

            if ( monster != null )
            {
               monster.PerformAction( this );
               Game.SchedulingSystem.Add( monster );
            }

            ActivateMonsters();
         }
      }

      public void MoveMonster( Monster monster, ICell cell )
      {
         if ( !Game.DungeonMap.SetActorPosition( monster, cell.X, cell.Y ) )
         {
            if ( Game.Player.X == cell.X && Game.Player.Y == cell.Y )
            {
               Attack( monster, Game.Player );
            }
         }
      }

      public void Attack( Actor attacker, Actor defender )
      {
         StringBuilder attackMessage = new StringBuilder();
         StringBuilder defenseMessage = new StringBuilder();

         int hits = ResolveAttack( attacker, defender, attackMessage );

         int blocks = ResolveDefense( defender, hits, attackMessage, defenseMessage );

         Game.MessageLog.Add( attackMessage.ToString() );
         if ( !string.IsNullOrWhiteSpace( defenseMessage.ToString() ) )
         {
            Game.MessageLog.Add( defenseMessage.ToString() );
         }

         int damage = hits - blocks;

         ResolveDamage( defender, damage );
      }

      private static int ResolveAttack( Actor attacker, Actor defender, StringBuilder attackMessage )
      {
         int hits = 0;

         attackMessage.AppendFormat( "{0} attacks {1} and rolls: ", attacker.Name, defender.Name );
         DiceExpression attackDice = new DiceExpression().Dice( attacker.Attack, 100 );

         DiceResult attackResult = attackDice.Roll();
         foreach ( TermResult termResult in attackResult.Results )
         {
            attackMessage.Append( termResult.Value + ", " );
            if ( termResult.Value >= 100 - attacker.AttackChance )
            {
               hits++;
            }
         }

         return hits;
      }

      private static int ResolveDefense( Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage )
      {
         int blocks = 0;

         if ( hits > 0 )
         {
            attackMessage.AppendFormat( "scoring {0} hits.", hits );
            defenseMessage.AppendFormat( "  {0} defends and rolls: ", defender.Name );
            DiceExpression defenseDice = new DiceExpression().Dice( defender.Defense, 100 );

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

         return blocks;
      }

      private static void ResolveDamage( Actor defender, int damage )
      {
         if ( damage > 0 )
         {
            defender.Health = defender.Health - damage;

            Game.MessageLog.Add( $"  {defender.Name} was hit for {damage} damage" );

            if ( defender.Health <= 0 )
            {
               ResolveDeath( defender );
            }
         }
         else
         {
            Game.MessageLog.Add( $"  {defender.Name} blocked all damage" );
         }
      }

      private static void ResolveDeath( Actor defender )
      {
         if ( defender is Player )
         {
            Game.MessageLog.Add( $"  {defender.Name} was killed, GAME OVER MAN!" );
         }
         else if ( defender is Monster )
         {
            if ( defender.Head != null && defender.Head != HeadEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Head );
            }
            if ( defender.Body != null && defender.Body != BodyEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Body );
            }
            if ( defender.Hand != null && defender.Hand != HandEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Hand );
            }
            if ( defender.Feet != null && defender.Feet != FeetEquipment.None() )
            {
               Game.DungeonMap.AddTreasure( defender.X, defender.Y, defender.Feet );
            }
            Game.DungeonMap.AddGold( defender.X, defender.Y, defender.Gold );
            Game.DungeonMap.RemoveMonster( (Monster) defender );

            Game.MessageLog.Add( $"  {defender.Name} died and dropped {defender.Gold} gold" );
         }
      }

      public bool HandleKey( RLKey key )
      {
         if ( key == RLKey.Q )
         {
            return Game.Player.QAbility.Perform();
         }
         if ( key == RLKey.W )
         {
            return Game.Player.WAbility.Perform();
         }
         if ( key == RLKey.E )
         {
            return Game.Player.EAbility.Perform();
         }
         if ( key == RLKey.R )
         {
            return Game.Player.RAbility.Perform();
         }


         bool didUseItem = false;
         if ( key == RLKey.Number1 )
         {
            didUseItem = Game.Player.Item1.Use();
         }
         else if ( key == RLKey.Number2 )
         {
            didUseItem = Game.Player.Item2.Use();
         }
         else if ( key == RLKey.Number3 )
         {
            didUseItem = Game.Player.Item3.Use();
         }
         else if ( key == RLKey.Number4 )
         {
            didUseItem = Game.Player.Item4.Use();
         }

         if ( didUseItem )
         {
            RemoveItemsWithNoRemainingUses();
         }

         return didUseItem;
      }

      private static void RemoveItemsWithNoRemainingUses()
      {
         if ( Game.Player.Item1.RemainingUses <= 0 )
         {
            Game.Player.Item1 = new NoItem();
         }
         if ( Game.Player.Item2.RemainingUses <= 0 )
         {
            Game.Player.Item2 = new NoItem();
         }
         if ( Game.Player.Item3.RemainingUses <= 0 )
         {
            Game.Player.Item3 = new NoItem();
         }
         if ( Game.Player.Item4.RemainingUses <= 0 )
         {
            Game.Player.Item4 = new NoItem();
         }
      }

      public void EndPlayerTurn()
      {
         IsPlayerTurn = false;
         Game.Player.Tick();
      }
   }
}
