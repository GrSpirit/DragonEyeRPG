using System;
using System.Collections.Generic;
using System.Text;

namespace DragonEyeRPG
{
    public class Action
    {
        public delegate void Runable();
        public string Name { get; set; }
        public bool IsReturn { get; set; } = false;
        public Runable Run { get; set; }
    }

    public class ActionList : List<Action>
    {
        public void Render()
        {
            bool success = false;
            bool finish = false;
            while (!finish)
            {
                Console.WriteLine("You can:");
                for (int i = 0; i < this.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {this[i].Name}");
                }
                while (!success)
                {
                    string action = Console.ReadLine();
                    if (int.TryParse(action, out int actionId) && actionId > 0 && actionId <= this.Count)
                    {
                        success = true;
                        if (this[actionId - 1].IsReturn) finish = true;
                        else this[actionId - 1].Run();
                    }
                }
            }
        }
    }
}
