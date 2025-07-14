using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IsekaiTextRPG
{
    internal class Skill
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Damage { get; set; }
        public string Description { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }
        public Skill(string name, string description, int manaCost, int cooldown,int id,int damage)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            Cooldown = cooldown;
            Id = id;
            Damage = damage;
        }
        public void skillUse()
        {
            

        }
    }


}
