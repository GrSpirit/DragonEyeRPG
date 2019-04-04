using System;
using System.Collections.Generic;
using System.Text;

namespace DragonEyeRPG
{
    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ActionList Actions { get; } = new ActionList();
        public void Enter(Player player)
        {
            RenderLocation(player);
            Actions.Add(new Action()
            {
                Name = "Go to another location",
                IsReturn = true,
            });
            Actions.Render();
        }

        private void RenderLocation(Player player)
        {
            Console.WriteLine($"{player.Name} came into {Name}.");
            Console.WriteLine(Description);
        }
    }

    public class LocationFabric
    {
        public static Location CreateGriglagg()
        {
            Location place = new Location()
            {
                Name = "Griglagg",
                Description = "You came to Griglagg"
            };
            return place;
        }
        public static Location CreateRavenwood()
        {
            Location place = new Location()
            {
                Name = "Ravenwood",
                Description = "You came to Ravenwood"
            };
            return place;
        }
        public static Location CreateDunwich()
        {
            Location place = new Location()
            {
                Name = "Dunwich",
                Description = "You came to Dunwich"
            };
            return place;
        }
        public static Location CreateGreenwood()
        {
            Location place = new Location()
            {
                Name = "Greenwood",
                Description = "You came to Greenwood"
            };
            return place;
        }
    }
}
