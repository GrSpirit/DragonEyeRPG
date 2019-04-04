using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DragonEyeRPG;

namespace DraygonEyeRPG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Location[] Places = {
                LocationFabric.CreateGriglagg(),
                LocationFabric.CreateRavenwood(),
                LocationFabric.CreateDunwich(),
                LocationFabric.CreateGreenwood()
            };
            //string[] Places = { "Griglagg", "Ravenwood", "Dunwich", "Greenwood" };
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
                    Location place = Places[rnd.Next(0, Places.Length)];
                    Console.WriteLine("You travel to {0}.", place.Name);
                    System.Threading.Thread.Sleep(1000);
                    place.Enter(MainPlayer);
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