using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DraygonEyeRPG
{
    enum ItemType
    {
        Weapon = 0,
        Food = 1
    }
    class Item
    {
        public ItemType ItemType;
        public string ItemName;
        public int ItemLevel;
        public int Damage;
        public int Value { get; set; }
    }

    class Person
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

            int damage = (int)(rnd.Next((int)(this.PrimaryWeapon.Damage*0.95), (int)(this.PrimaryWeapon.Damage * 1.05)) * modifier );
            enemy.Health -= damage;
            return damage;
        }
        public bool HasFood => this.Inventory.Exists((item) => item.ItemType == ItemType.Food);
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
    class Enemy : Person
    {
    }

    class Player : Person
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

    class PersonFabric
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

    class ItemFabric
    {
        private static readonly Dictionary<ItemType, string[]> itemList = new Dictionary<ItemType, string[]>() {
            [ItemType.Weapon] = new string[] {"Greatsword", "Axe", "Sword", "Knife", "Dagger"},
            [ItemType.Food] = new string[] {"Potatoe", "Carrot", "Potion", "Spider Eye", "Rope", "Cake", "Jerkey", "somtin special ;)"},
        };
        public static Item CreateItem(int level, ItemType? itemType = null)
        {
            var rnd = new Random();
            ItemType type = itemType ?? (ItemType)rnd.Next(0, 2);
            int mult = (type == ItemType.Food) ? 2 : 1;
            return new Item()
            {
                ItemType = type,
                ItemName = itemList[type][rnd.Next(0, itemList[type].Length)],
                Damage = rnd.Next(5 + level * 5, 15 + level * 5) * mult,
                Value = 0,
                ItemLevel = level
            };

        }
    }

    class Program
    {
        
    public static void Main(string[] args)
        {
            string[] Places = { "Griglagg", "Ravenwood", "Dunwich", "Greenwood" };
            Console.WriteLine("Welcome to Dragon Eye, a terminal RPG!");
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("This game was created for the AI competition on repl.it");
            System.Threading.Thread.Sleep(2000);
            Player MainPlayer = PersonFabric.CreatePlayer();
            RenderStats(MainPlayer);
            RenderInventory(MainPlayer);
            Random rnd = new Random();
            Console.WriteLine("Your adventure begins..");
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                int travel = rnd.Next(0, 5);
                if (travel == 1)
                {
                    string place = Places[rnd.Next(0, Places.Length)];
                    Console.WriteLine("You travel to {0}.", place);
                }
                else
                {
                    int action = rnd.Next(0, 4); // 0 combat, 1 quest, 2 someone asks for gold, 3 find something
                    switch (action)
                    {
                        case 0:
                            Combat(MainPlayer);
                            break;
                        case 1:
                            Console.WriteLine("A fellow {0} asks {1} for 10 Gold Pieces, do you accept?", MainPlayer.Race, MainPlayer.Name);
                            Console.WriteLine("1. Yes\n2. No");
                            string Answer = Console.ReadLine();
                            switch (Answer)
                            {
                                case "1":
                                    if (MainPlayer.GP < 10)
                                    {
                                        Console.WriteLine("You have only {0} Gold Pieces", MainPlayer.GP);
                                    }
                                    else
                                    {
                                        MainPlayer.GP -= 10;
                                        Console.WriteLine("You gave the {0} 10 Gold Pieces.", MainPlayer.Race);
                                        int giftSomething = rnd.Next(0, 2);
                                        if (giftSomething == 1)
                                        {
                                            Item giftItem = ItemFabric.CreateItem(MainPlayer.Level);
                                            MainPlayer.Inventory.Add(giftItem);
                                            Console.WriteLine("{0} gifts you a new {1}!", MainPlayer.Race, giftItem.ItemName);
                                        }
                                    }
                                    break;
                                case "2":
                                    Console.WriteLine("You refused to give the {0} Gold Pieces.", MainPlayer.Race);
                                    break;
                                default:
                                    Console.WriteLine("You refused to give the {0} Gold Pieces.", MainPlayer.Race);
                                    break;
                            }
                            break;
                        case 2:
                            Console.WriteLine("Someone walks up to {0}, and asks him/her to slay a monster.", MainPlayer.Name);
                            System.Threading.Thread.Sleep(500);
                            if (rnd.Next(0, 2) == 1)
                            {
                                Console.WriteLine("{0} accepted the offer.", MainPlayer.Name);
                                Combat(MainPlayer);
                            }
                            else
                            {
                                Console.WriteLine("{0} declined the offer.", MainPlayer.Name);
                            }
                            break;
                        case 3:
                            Item newItem = ItemFabric.CreateItem(MainPlayer.Level);
                            MainPlayer.Inventory.Add(newItem);
                            Console.WriteLine("You found a {0}.", newItem.ItemName);
                            break;
                    }
                }
            }
        }
        public static void Combat(Player player)
        {
            Random rnd = new Random();
            Enemy enemy = PersonFabric.CreateEnemy(player);
            Console.WriteLine("A level {0} {1} started a fight.", enemy.Level, enemy.Race);
            bool Fighting = true;
            bool Success = false;
            while (Fighting)
            {
                if (enemy.Health <= 0)
                {
                    Fighting = false;
                    Win(player, enemy);
                }
                else if (player.Health <= 0)
                {
                    Fighting = false;
                    Lose(player);
                }
                else
                {
                    RenderInventory(player);
                    int DefenseVal = 0;
                    System.Threading.Thread.Sleep(100);
                    Console.WriteLine("Its your turn, you can:\n1. Attack\n2. Defend\n3. Open inventory.");
                    Console.WriteLine("{0}'s Health: {1}", player.Name, player.Health);
                    Console.WriteLine("{0}'s Health: {1}", enemy.Race, enemy.Health);
                    string plrInput = Console.ReadLine();
                    switch (plrInput)
                    {
                        case "1":
                            if (player.PrimaryWeapon != null)
                            {
                                int damage = player.Attack(enemy);
                                Console.WriteLine("{0} attacked enemy {1} with a {2}, the {1} lost {3} health.", player.Name, enemy.Race, player.PrimaryWeapon.ItemName, damage);
                                Success = true;
                            }

                            break;
                        case "2":
                            Success = true;
                            DefenseVal = player.Defense;
                            break;
                        case "3":
                            if (!OpenInventory(player)) continue;
                            Success = true;
                            break;
                    }
                    System.Threading.Thread.Sleep(200);
                    RenderStats(player);
                    System.Threading.Thread.Sleep(100);
                    if (Success)
                    {
                        Success = false;
                        if (enemy.Health <= 0) continue;
                    }
                    else if (Success == false)
                    {
                        Console.WriteLine("Incorrect input, turn skipped.");
                        continue;
                    }
                    if (enemy.Health < enemy.FullHealth / 4)
                    {
                        enemy.WillHeal = rnd.Next(0, 3) == 1 && enemy.HasFood;
                    }
                    if (enemy.WillHeal)
                    {
                        Item food = enemy.Food.First();
                        enemy.Heal(food);
                        Console.WriteLine("{0} healed {1}.", enemy.Race, food.Damage);
                    }
                    else if (DefenseVal > 0)
                    {
                        Console.WriteLine("{0} attacks.", enemy.Race);
                        System.Threading.Thread.Sleep(100);
                        Console.WriteLine("{0} defends.", player.Name);
                        if (enemy.Damage - DefenseVal > 0)
                            player.Health -= enemy.Damage - DefenseVal;
                        DefenseVal = 0;
                    }
                    else
                    {
                        Item enemyWeapon = enemy.Inventory[0];
                        int damage = enemy.Attack(player);
                        Console.WriteLine("{0} attacks, {1} loses {2} health.", enemy.Race, player.Name, damage);
                    }
                }
            }
        }
        public static bool OpenInventory(Player player)
        {
            bool success = false;
            Console.WriteLine("You look into your inventory:");
            RenderInventory(player, "\n");
            while (!success)
            {
                Console.WriteLine("You can:\n1. Select a primary weapon\n2. Use an item\n3. Close inventory");
                string choise = Console.ReadLine();
                switch (choise)
                {
                    case "1":
                        Console.WriteLine("Which weapon?");
                        string weapon = Console.ReadLine();
                        System.Threading.Thread.Sleep(200);
                        int weaponId;
                        if (int.TryParse(weapon, out weaponId))
                        {
                            if (weaponId > 0 && weaponId <= player.Inventory.Count && player.Inventory[weaponId - 1].ItemType == ItemType.Weapon)
                            {
                                player.PrimaryWeapon = player.Inventory[weaponId - 1];
                                Console.WriteLine($"{player.Name} takes {player.PrimaryWeapon.ItemName} in his/her hands");
                                System.Threading.Thread.Sleep(200);
                                return false;
                            }
                        }
                        break;
                    case "2":
                        Console.WriteLine("Which item?");
                        string food = Console.ReadLine();
                        System.Threading.Thread.Sleep(200);
                        int foodId;
                        if (int.TryParse(food, out foodId))
                        {
                            if (foodId > 0 && foodId <= player.Inventory.Count && player.Inventory[foodId - 1].ItemType == ItemType.Food)
                            {
                                Item item = player.Inventory[foodId - 1];
                                player.Heal(item);
                                Console.WriteLine($"{player.Name} healed {item.Damage} with a {item.ItemName}.");
                                System.Threading.Thread.Sleep(200);
                                return true;
                            }
                        }
                        break;
                    case "3":
                        Console.WriteLine("You close inventory");
                        return false;
                }
                Console.WriteLine("Incorrect input");
            }
            return false;
        }
        public static void RenderStats(Player mainPlr)
        {
            Console.WriteLine("[Damage: {0}][Level: {1}][Defense: {2}][XP {3}][Health: {4}/100][GP: {5}]", mainPlr.Damage, mainPlr.Level, mainPlr.Defense, mainPlr.XP, mainPlr.Health, mainPlr.GP);
        }
        public static void RenderInventory(Player newPlr, string separator = "")
        {
            StringBuilder logString = new StringBuilder();
            for(int i = 0; i< newPlr.Inventory.Count; i++)
            {
                Item item = newPlr.Inventory[i];
                logString.AppendFormat("<{0}-{1}({2})>{3}", i + 1, item.ItemName, item.Damage, separator);
            }
            Console.WriteLine("---Inventory---");
            Console.WriteLine(logString.ToString());
            Console.WriteLine("------");
        }
        public static void Win(Player player, Enemy enemy)
        {
            Console.WriteLine("{0} won the fight!", player.Name);
            Random rnd = new Random();
            if (rnd.Next(0, 5) == 0)
            {
                Console.WriteLine($"{enemy.Race} dropped {enemy.PrimaryWeapon.ItemName} and {player.Name} picked it up");
                player.Inventory.Add(enemy.PrimaryWeapon);
            }
            int xpReward = rnd.Next(1, 1000);
            xpReward += (int)(xpReward * player.Level * 0.1);  // 10% Level increase
            int goldReward = rnd.Next(5, 16);
            goldReward += (int)(goldReward * player.Level * 0.2); // 20% Level increase
            Console.WriteLine("Rewards: {0} xp, {1} Gold Peices.", xpReward, goldReward);
            player.XP += xpReward;
            player.GP += goldReward;
            player.Health = player.FullHealth;
        }
        public static void Lose(Player player)
        {
            Random rnd = new Random();
            Console.WriteLine("{0} lost the fight.", player.Name);
            if (rnd.Next(0, 5) == 0)
            {
                int gold = rnd.Next((int)(player.GP * 0.05), (int)(player.GP * 0.15));
                Console.WriteLine($"{player.Name} lost {gold} Gold Pieces.");
                player.GP -= gold;
            }
            player.Health = player.FullHealth;
        }
    }
}