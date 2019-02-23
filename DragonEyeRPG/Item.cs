using System;
using System.Collections.Generic;
using System.Text;

namespace DragonEyeRPG
{
    public enum ItemType
    {
        Weapon = 0,
        Food = 1
    }
    public class Item
    {
        public ItemType ItemType;
        public string ItemName;
        public int ItemLevel;
        public int Damage;
        public int Value { get; set; }
    }
    public class ItemFabric
    {
        private static readonly Dictionary<ItemType, string[]> itemList = new Dictionary<ItemType, string[]>()
        {
            [ItemType.Weapon] = new string[] { "Greatsword", "Axe", "Sword", "Knife", "Dagger" },
            [ItemType.Food] = new string[] { "Potatoe", "Carrot", "Potion", "Spider Eye", "Rope", "Cake", "Jerkey", "somtin special ;)" },
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

}
