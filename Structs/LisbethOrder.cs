﻿
namespace Generate
{
    public class LisbethOrder
    {
        public int Id;
        public int Group;
        public int Item;
        public int Amount;
        public bool Enabled = true;
        public string Type;

        public bool Hq = false;
        public LisbethOrder(int id, int group, int item, int amount, string type)
        {
            Id = id;
            Group = group;
            Item = item;
            Amount = amount;
            Type = type;
        }

        public LisbethOrder(int id, int group, int item, int amount, string type, bool hq)
        {
            Id = id;
            Group = group;
            Item = item;
            Amount = amount;
            Type = type;
            Hq = hq;
        }

        public override string ToString()
        {
            return $"{Id}, {Group}, {Item}, {Amount}, {Enabled}, {Type}";
        }
    }
}