using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yut.WorldBoss
{
    public class PlayerManager : MonoBehaviour
    {
        internal struct DamageStus
        {
            public byte Rank;
            public CSteamID SteamID;
            public uint Damage;
            public DamageStus(byte rank,CSteamID steamID,uint damage)
            {
                Rank = rank;
                SteamID = steamID;
                Damage = damage;
            }
        }
        private static PlayerManager instance;
        private float frame = 0;
        public static PlayerManager Instance => instance;
        private static readonly Dictionary<CSteamID, uint> damages = new Dictionary<CSteamID,uint>();
        private static readonly Dictionary<CSteamID, Items> rewards = new Dictionary<CSteamID, Items>();
        public bool Sign(UnturnedPlayer player)
        {
            if(!damages.ContainsKey(player.CSteamID))
            {
                damages.Add(player.CSteamID, 0);
                return true;
            }
            return false;
        }
        public bool HasSign(UnturnedPlayer player)
            => damages.ContainsKey(player.CSteamID);
        public void AddDamage(CSteamID id,uint damage)
        {
            if (damages.ContainsKey(id))
                damages[id] += damage;
        }
        internal List<DamageStus> GetPlayerStus(CSteamID id)
        {
            List<KeyValuePair<CSteamID, uint>> list = damages.OrderByDescending(x => x.Value).ToList();
            List<DamageStus> stus = new List<DamageStus>();
            int ind = -1;
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Key == id)
                {
                    ind = i;
                    break;
                }
            }
            if (ind == -1)
                return stus;
            byte start = DataModule.Math.RangeToByte(ind - 2);
            byte end = DataModule.Math.RangeToByte(ind + 3, 0, (byte)list.Count);
            for (byte i = start; i < end; i++)
                stus.Add(new DamageStus((byte)(i + 1), list[i].Key, list[i].Value));
            return stus;
        }
        public void OpenUI(CSteamID id)
        {
            ushort uikey = Yut.Instance.Configuration.Instance.UIKey;
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
            if (player == null)
                return;
            ITransportConnection con = Provider.findTransportConnection(id);
            if (con == null)
                return;
            uint max = Yut.Instance.Configuration.Instance.Region.BossHealth;
            EffectManager.sendUIEffect(uikey, (short)uikey, con, true);
            EffectManager.sendUIEffectImageURL((short)uikey, con, true, "头像", Yut.Instance.Configuration.Instance.BossIcon);
            EffectManager.sendUIEffectText((short)uikey,con,true,"Boss名字",Yut.Instance.Configuration.Instance.BossName);
            EffectManager.sendUIEffectText((short)uikey, con, true, "血量", $"{max}/{max}");
        }
        public void CloseUI()
        {
            foreach(var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(id);
                if (con == null)
                    continue;
                EffectManager.askEffectClearByID(Yut.Instance.Configuration.Instance.UIKey, con);
                UnturnedChat.Say("3");
            }
        }
        public void UpdatePlayerStusUI()
        {
            short key = (short)Yut.Instance.Configuration.Instance.UIKey;
            foreach (var kvp in damages)
            {
                List<DamageStus> stus = GetPlayerStus(kvp.Key);
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(kvp.Key);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(kvp.Key);
                if (con == null)
                    continue;
                for (int i = 0; i < stus.Count; i++)
                {
                    UnturnedPlayer player1 = UnturnedPlayer.FromCSteamID(stus[i].SteamID);
                    if (player1 == null)
                        continue;
                    EffectManager.sendUIEffectText(key, con, true, $"排名{i}", stus[i].Rank.ToString());
                    EffectManager.sendUIEffectText(key, con, true, $"名字{i}", player1.DisplayName);
                    EffectManager.sendUIEffectText(key, con, true, $"分数{i}", stus[i].Damage.ToString());
                    //UnturnedChat.Say(player, $"排名:{stus[i].Rank},名字:{player1.DisplayName},伤害:{stus[i].Damage}");
                }
            }
        }
        public void UpdateBossHealthUI(uint bossHealth)
        {
            short key = (short)Yut.Instance.Configuration.Instance.UIKey;
            uint max = Yut.Instance.Configuration.Instance.Region.BossHealth;
            ushort num = DataModule.Math.RangeToUInt16(706f * bossHealth / max);
            string str = GetHealthBarStr(num);
            foreach (var csteamid in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(csteamid);
                if(player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(csteamid);
                if (con == null)
                    continue;
                EffectManager.sendUIEffectText(key, con, true, "血条数值", str);
                EffectManager.sendUIEffectText(key, con, true, "血量", $"{bossHealth}/{max}");
                //UnturnedChat.Say(player, $"Boss剩余血量{bossHealth}");
            }
        }
        public void Reward()
        {
            var orders = damages.OrderByDescending(x => x.Value);
            foreach (var s in orders)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(s.Key);
                if (player == null)
                    continue;
                if (s.Value < Yut.Instance.Configuration.Instance.MinRewardDamage)
                {
                    UnturnedChat.Say(player, Yut.Instance.Translate("Low_Damage", Yut.Instance.Configuration.Instance.MinRewardDamage));
                    continue;
                }
                Items items = new Items(PlayerInventory.STORAGE);
                items.loadSize(10, 20);
                byte rank = 0;
                RewardInterval interval = Yut.Instance.Configuration.Instance.Rewards.Find(x => x.InInterval(++rank));
                if (interval != null)
                {
                    for (int i = 0; i < interval.Rewards.Count; i++)
                        items.tryAddItem(new Item(interval.Rewards[i].Id, interval.Rewards[i].Count, 100));
                    rewards.Add(s.Key, items);
                }
                UnturnedChat.Say(player, Yut.Instance.Translate("Get_Reward"));
            }
        }
        public void Clear()
        {
            damages.Clear();
            rewards.Clear();
        }
        public bool HasReward(CSteamID id)
            => rewards.ContainsKey(id) && rewards[id].getItemCount() > 0;
        public void SendRewardStorage(UnturnedPlayer player)
        {
            Items item = rewards[player.CSteamID];
            player.Inventory.updateItems(PlayerInventory.STORAGE, item);
            player.Inventory.sendStorage();
        }
        private void Awake()
        {
            instance = this;
        }
        private void Update()
        {
            if(BossManager.Instance.State == EState.Fighting)
            {
                frame += Time.deltaTime;
                if (frame < DataModule.Math.Range(Yut.Instance.Configuration.Instance.LeaderboardRefreshSeconds,1))
                    return;
                frame = 0;
                UpdatePlayerStusUI();
            }
        }
        private string GetHealthBarStr(ushort num)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            for(int i = 0; i < num; i++)
                sb.Append(" ");
            return sb.ToString();
        }
    }
}
