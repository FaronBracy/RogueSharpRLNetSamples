using System.Text;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpRLNetSamples.Actors;
using RogueSharpRLNetSamples.Equipment;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Items;

namespace RogueSharpRLNetSamples.Services
{
   public class CommandService
   {
      public bool IsPlayerTurn { get; set; }

      public DungeonMap DungeonMap
      {
         get;
      }

      public CommandService( DungeonMap dungeonMap )
      {
         DungeonMap = dungeonMap;
      }

      public bool MovePlayer( Direction direction )
      {
         Player player = DungeonMap.GetPlayer();
         int x;
         int y;

         switch ( direction )
         {
            case Direction.Up:
            {
               x = player.X;
               y = player.Y - 1;
               break;
            }
            case Direction.Down:
            {
               x = player.X;
               y = player.Y + 1;
               break;
            }
            case Direction.Left:
            {
               x = player.X - 1;
               y = player.Y;
               break;
            }
            case Direction.Right:
            {
               x = player.X + 1;
               y = player.Y;
               break;
            }
            default:
            {
               return false;
            }
         }

         if ( DungeonMap.SetActorPosition( player, x, y ) )
         {
            return true;
         }

         Monster monster = DungeonMap.GetMonsterAt( x, y );

         if ( monster != null )
         {
            Attack( player, monster );
            return true;
         }

         return false;
      }

      public void ActivateMonsters()
      {
         IScheduleable scheduleable = Game.ScheduleService.Get();
         if ( scheduleable is Player )
         {
            IsPlayerTurn = true;
            Game.ScheduleService.Add( DungeonMap.GetPlayer() );
         }
         else
         {
            Monster monster = scheduleable as Monster;

            if ( monster != null )
            {
               monster.PerformAction( this );
               Game.ScheduleService.Add( monster );
            }

            ActivateMonsters();
         }
      }

      public void MoveMonster( Monster monster, Cell cell )
      {
         if ( !DungeonMap.SetActorPosition( monster, cell.X, cell.Y ) )
         {
            Player player = DungeonMap.GetPlayer();
            if ( player.X == cell.X && player.Y == cell.Y )
            {
               Attack( monster, player );
            }
         }
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

            Game.Messages.Add( $"  {defender.Name} was hit for {damage} damage" );

            if ( defender.Health <= 0 )
            {
               if ( defender is Player )
               {
                  Game.Messages.Add( $"  {defender.Name} was killed, GAME OVER MAN!" );
               }
               else if ( defender is Monster )
               {
                  if ( defender.Head != null && defender.Head != HeadEquipment.None() )
                  {
                     DungeonMap.AddEquipment( defender.X, defender.Y, defender.Head );
                  }
                  if ( defender.Body != null && defender.Body != BodyEquipment.None() )
                  {
                     DungeonMap.AddEquipment( defender.X, defender.Y, defender.Body );
                  }
                  if ( defender.Hand != null && defender.Hand != HandEquipment.None() )
                  {
                     DungeonMap.AddEquipment( defender.X, defender.Y, defender.Hand );
                  }
                  if ( defender.Feet != null && defender.Feet != FeetEquipment.None() )
                  {
                     DungeonMap.AddEquipment( defender.X, defender.Y, defender.Feet );
                  }
                  DungeonMap.AddGold( defender.X, defender.Y, defender.Gold );
                  DungeonMap.RemoveMonster( (Monster) defender );

                  Game.Messages.Add( $"  {defender.Name} died and dropped {defender.Gold} gold" );
               }
            }
         }
         else
         {
            Game.Messages.Add( $"  {defender.Name} blocked all damage" );
         }
      }

      public bool HandleKey( RLKey key )
      {
         Player player = DungeonMap.GetPlayer();

         if ( key == RLKey.Q )
         {
            return player.QAbility.Perform();
         }
         if ( key == RLKey.W )
         {
            return player.WAbility.Perform();
         }
         if ( key == RLKey.E )
         {
            return player.EAbility.Perform();
         }
         if ( key == RLKey.R )
         {
            return player.RAbility.Perform();
         }


         bool didUseItem = false;
         if ( key == RLKey.Number1 )
         {
            didUseItem = player.Item1.Use();
         }
         else if ( key == RLKey.Number2 )
         {
            didUseItem = player.Item2.Use();
         }
         else if ( key == RLKey.Number3 )
         {
            didUseItem = player.Item3.Use();
         }
         else if ( key == RLKey.Number4 )
         {
            didUseItem = player.Item4.Use();
         }

         if ( didUseItem )
         {
            if ( player.Item1.RemainingUses <= 0 )
            {
               player.Item1 = new NoItem();
            }
            if ( player.Item2.RemainingUses <= 0 )
            {
               player.Item2 = new NoItem();
            }
            if ( player.Item3.RemainingUses <= 0 )
            {
               player.Item3 = new NoItem();
            }
            if ( player.Item4.RemainingUses <= 0 )
            {
               player.Item4 = new NoItem();
            }
         }

         return didUseItem;
      }

      public void EndPlayerTurn()
      {
         IsPlayerTurn = false;
         DungeonMap.GetPlayer().Tick();
      }
   }
}
