using System;

namespace SpiritMod
{
    public class LootFilterEntry
    {
        public string Name { get; set; } = "";

        public int Rarity { get; set; }

        public int Override { get; set; }

        public int SellOverride { get; set; }

        public int TimesSeen { get; set; }
    }
}
