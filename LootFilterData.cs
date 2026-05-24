using System.Collections.Generic;

namespace SpiritMod
{
    public class LootFilterData
    {
        public bool SellFilterEnabled { get; set; }
        public bool[] SellRarityEnabled { get; set; }

        public Dictionary<string, LootFilterEntry> Items { get; set; }

        public bool StatFilterEnabled { get; set; }
        public List<StatFilterRule> MainStatRules { get; set; }
        public List<StatFilterRule> SecondaryStatRules { get; set; }

        public int SecondaryMinMatch { get; set; }

        public LootFilterData()
        {
            SellRarityEnabled = new[] { true, true, false, false };
            Items = new Dictionary<string, LootFilterEntry>();
            MainStatRules = new List<StatFilterRule>();
            SecondaryStatRules = new List<StatFilterRule>();
            SecondaryMinMatch = 1;
        }
    }
}