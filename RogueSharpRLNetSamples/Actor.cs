using RLNET;
using RogueSharpRLNetSamples.Equipment;

namespace RogueSharpRLNetSamples
{
   public interface IHasStats
   {
      int Attack { get; set; }
      int AttackChance { get; set; }
      int Awareness { get; set; }
      int Defense { get; set; }
      int DefenseChance { get; set; }
      int Gold { get; set; }
      int Health { get; set; }
      int MaxHealth { get; set; }
      string Name { get; set; }
      int Speed { get; set; }
   }

   public interface IDrawable
   {
      RLColor Color { get; set; }
      char Symbol { get; set; }
      int X { get; set; }
      int Y { get; set; }
   }

   public interface IScheduleable
   {
      int Time { get; }
   }

   public class Actor : IHasStats, IDrawable, IScheduleable
   {
      public Actor()
      {
         Head = HeadEquipment.None();
         Body = BodyEquipment.None();
         Hand = HandEquipment.None();
         Feet = FeetEquipment.None();
      }

      public HeadEquipment Head { get; set; }
      public BodyEquipment Body { get; set; }
      public HandEquipment Hand { get; set; }
      public FeetEquipment Feet { get; set; }

      // IHasStats
      private int _attack;
      private int _attackChance;
      private int _awareness;
      private int _defense;
      private int _defenseChance;
      private int _gold;
      private int _health;
      private int _maxHealth;
      private string _name;
      private int _speed;
      
      public int Attack
      {
         get
         {
            return _attack + Head.Attack + Body.Attack + Hand.Attack + Feet.Attack;
         }
         set
         {
            _attack = value;
         }
      }

      public int AttackChance
      {
         get
         {
            return _attackChance + Head.AttackChance + Body.AttackChance + Hand.AttackChance + Feet.AttackChance;
         }
         set
         {
            _attackChance = value;
         }
      }

      public int Awareness
      {
         get
         {
            return _awareness + Head.Awareness + Body.Awareness + Hand.Awareness + Feet.Awareness;
         }
         set
         {
            _awareness = value;
         }
      }

      public int Defense
      {
         get
         {
            return _defense + Head.Defense + Body.Defense + Hand.Defense + Feet.Defense;
         }
         set
         {
            _defense = value;
         }
      }

      public int DefenseChance
      {
         get
         {
            return _defenseChance + Head.DefenseChance + Body.DefenseChance + Hand.DefenseChance + Feet.DefenseChance;
         }
         set
         {
            _defenseChance = value;
         }
      }

      public int Gold
      {
         get
         {
            return _gold + Head.Gold + Body.Gold + Hand.Gold + Feet.Gold;
         }
         set
         {
            _gold = value;
         }
      }

      public int Health
      {
         get
         {
            return _health;
         }
         set
         {
            _health = value;
         }
      }

      public int MaxHealth
      {
         get
         {
            return _maxHealth + Head.MaxHealth + Body.MaxHealth + Hand.MaxHealth + Feet.MaxHealth;
         }
         set
         {
            _maxHealth = value;
         }
      }

      public string Name
      {
         get
         {
            return _name;
         }
         set
         {
            _name = value;
         }
      }

      public int Speed
      {
         get
         {
            return _speed + Head.Speed + Body.Speed + Hand.Speed + Feet.Speed;
         }
         set
         {
            _speed = value;
         }
      }

      // IDrawable
      public RLColor Color { get; set; }
      public char Symbol { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      // IScheduleable
      public  int Time { get
      {
         return Speed;
      } }
   }
}