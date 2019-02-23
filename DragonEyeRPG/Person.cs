using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonEyeRPG
{
    public class Person
    {
        public List<Item> Inventory { get; } = new List<Item>();
        public int Level { get; set; }
        public int FullHealth { get; set; }
        public int Health { get; set; }
        public string Race { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }
        public Item PrimaryWeapon { get; set; }
        public bool WillHeal { get; set; } = false;
        public int Attack(Person enemy)
        {
            Random rnd = new Random();
            double modifier = (this.Damage >= enemy.Defense)
                ? (1 + (this.Damage - enemy.Defense) * 0.05)
                : (1 / (1 + (enemy.Defense - this.Damage) * 0.05));

            int damage = (int)(rnd.Next((int)(this.PrimaryWeapon.Damage * 0.95), (int)(this.PrimaryWeapon.Damage * 1.05)) * modifier);
            enemy.Health -= damage;
            return damage;
        }
        public bool HasFood => this.Inventory.Any((item) => item.ItemType == ItemType.Food);
        public IEnumerable<Item> Food => this.Inventory.Where((item) => item.ItemType == ItemType.Food);
        public IEnumerable<Item> Weapon => this.Inventory.Where((item) => item.ItemType == ItemType.Weapon);
        public int Heal(Item item)
        {
            Health = Math.Min(Health + item.Damage, FullHealth);
            Inventory.Remove(item);
            WillHeal = false;
            return item.Damage;
        }
    }
    public class Enemy : Person
    {
    }

    public class Player : Person
    {
        private int _xp;
        public string Name { get; set; }
        public int GP { get; set; }
        public int XP
        {
            get { return _xp; }
            set
            {
                Random rnd = new Random();
                _xp = value;
                if (_xp > Level * 500)
                {
                    _xp = 0;
                    Damage += rnd.Next(5, 11);
                    Defense += rnd.Next(5, 11);
                    FullHealth += rnd.Next(20, 41);
                    Console.WriteLine($"{Name} leveled up!");
                    Level++;
                }
            }
        }
    }

    public class PersonFabric
    {
        static string[] enemyRace = { "Orc", "Bandit", "Giant", "Dragon", "Elf", "Dwarf" };
        public static Player CreatePlayer()
        {
            Console.WriteLine("---Create your character---");
            Console.Write("Name: ");
            string plrName = Console.ReadLine();
            Console.Write("Race(Dwarf, Human, Giant, etc...): ");
            string plrRace = Console.ReadLine();
            Player player = new Player()
            {
                Level = 1,
                FullHealth = 100,
                Health = 100,
                Damage = 5,
                Defense = 5,
                XP = 0,
                GP = 50,
                Race = plrRace,
                Name = plrName
            };
            Item stick = ItemFabric.CreateItem(1, ItemType.Weapon);
            stick.ItemName = "Stick";
            player.Inventory.Add(stick);
            player.PrimaryWeapon = stick;
            return player;
        }

        public static Enemy CreateEnemy(Person plr)
        {
            Random rnd = new Random();
            int randomRace = rnd.Next(0, enemyRace.Length);
            int health = plr.FullHealth + (plr.FullHealth * rnd.Next(-5, 6) / 100);

            Enemy enemy = new Enemy()
            {
                Race = enemyRace[randomRace],
                Damage = plr.Damage + rnd.Next(-5, 6),
                Defense = plr.Defense + rnd.Next(-5, 6),
                Level = Math.Max(1, plr.Level + rnd.Next(-2, 1)),
                FullHealth = health,
                Health = health,
            };
            enemy.PrimaryWeapon = ItemFabric.CreateItem(enemy.Level, ItemType.Weapon);
            enemy.Inventory.Add(enemy.PrimaryWeapon);
            enemy.Inventory.Add(ItemFabric.CreateItem(enemy.Level, ItemType.Food));
            return enemy;
        }
    }
}
