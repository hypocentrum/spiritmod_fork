using System;
using System.Collections.Generic;

namespace SpiritMod
{
    public class LootFilterData
    {
        // Token: 0x17000017 RID: 23
        // (get) Token: 0x060000B0 RID: 176 RVA: 0x0000A568 File Offset: 0x00008768
        // (set) Token: 0x060000B1 RID: 177 RVA: 0x0000A570 File Offset: 0x00008770
        public bool FilterEnabled { get; set; }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x060000B2 RID: 178 RVA: 0x0000A579 File Offset: 0x00008779
        // (set) Token: 0x060000B3 RID: 179 RVA: 0x0000A581 File Offset: 0x00008781
        public bool[] RarityEnabled { get; set; }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x060000B4 RID: 180 RVA: 0x0000A58A File Offset: 0x0000878A
        // (set) Token: 0x060000B5 RID: 181 RVA: 0x0000A592 File Offset: 0x00008792
        public bool SellFilterEnabled { get; set; }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x060000B6 RID: 182 RVA: 0x0000A59B File Offset: 0x0000879B
        // (set) Token: 0x060000B7 RID: 183 RVA: 0x0000A5A3 File Offset: 0x000087A3
        public bool[] SellRarityEnabled { get; set; }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x060000B8 RID: 184 RVA: 0x0000A5AC File Offset: 0x000087AC
        // (set) Token: 0x060000B9 RID: 185 RVA: 0x0000A5B4 File Offset: 0x000087B4
        public Dictionary<string, LootFilterEntry> Items { get; set; }

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x060000BA RID: 186 RVA: 0x0000A5BD File Offset: 0x000087BD
        // (set) Token: 0x060000BB RID: 187 RVA: 0x0000A5C5 File Offset: 0x000087C5
        public bool StatFilterEnabled { get; set; }

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x060000BC RID: 188 RVA: 0x0000A5CE File Offset: 0x000087CE
        // (set) Token: 0x060000BD RID: 189 RVA: 0x0000A5D6 File Offset: 0x000087D6
        public List<StatFilterRule> MainStatRules { get; set; }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x060000BE RID: 190 RVA: 0x0000A5DF File Offset: 0x000087DF
        // (set) Token: 0x060000BF RID: 191 RVA: 0x0000A5E7 File Offset: 0x000087E7
        public List<StatFilterRule> SecondaryStatRules { get; set; }

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x060000C0 RID: 192 RVA: 0x0000A5F0 File Offset: 0x000087F0
        // (set) Token: 0x060000C1 RID: 193 RVA: 0x0000A5F8 File Offset: 0x000087F8
        public int SecondaryMinMatch { get; set; }

        // Token: 0x060000C2 RID: 194 RVA: 0x0000A604 File Offset: 0x00008804
        public LootFilterData()
        {
            bool[] array = new bool[4];
            array[0] = true;
            array[1] = true;
            this.SellRarityEnabled = array;
            this.Items = new Dictionary<string, LootFilterEntry>();
            this.MainStatRules = new List<StatFilterRule>();
            this.SecondaryStatRules = new List<StatFilterRule>();
            this.SecondaryMinMatch = 1;
        }
    }
}
