using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yut.WorldBoss
{
    public delegate void ChallengeSuccessHandler(UnturnedPlayer player);
    public class PlayerManager : MonoBehaviour
    {
        private struct DamageStus
        {
            public byte Rank;
            public CSteamID SteamID;
            public uint Damage;
            public DamageStus(byte rank, CSteamID steamID, uint damage)
            {
                Rank = rank;
                SteamID = steamID;
                Damage = damage;
            }
        }
        private static PlayerManager instance;
        private float frame = Time.time;
        public static PlayerManager Instance => instance;
        private static readonly Dictionary<CSteamID, uint> damages = new Dictionary<CSteamID, uint>();
        private static readonly Dictionary<CSteamID, Items> rewards = new Dictionary<CSteamID, Items>();
        public static event ChallengeSuccessHandler OnChallengeSuccess;
        internal bool Sign(UnturnedPlayer player)
        {
            if (!damages.ContainsKey(player.CSteamID) && damages.Count < GameStateManager.Instance.ModeConfig.StateConfig.MaxPlayers)
            {
                damages.Add(player.CSteamID, 0);
                return true;
            }
            return false;
        }
        internal bool HasSign(UnturnedPlayer player)
        {
            if (player != null && player.Player != null)
                return damages.ContainsKey(player.CSteamID);
            return false;
        }
        internal bool Remove(UnturnedPlayer player)
            => damages.Remove(player.CSteamID);
        internal void AddDamage(CSteamID id, uint damage)
        {
            if (damages.ContainsKey(id))
                damages[id] += damage;
        }
        internal void OpenUI()
        {
            ushort uikey = Yut.Instance.Configuration.Instance.HKey;
            foreach (var id in damages.Keys)
                OpenUI(uikey, id);
        }
        internal void OpenUI(ushort uikey, CSteamID id)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
            if (player == null)
                return;
            ITransportConnection con = Provider.findTransportConnection(id);
            if (con == null)
                return;
            uint max = GameStateManager.Instance.ModeConfig.Region.BossHealth;
            EffectManager.sendUIEffect(uikey, (short)uikey, con, true);
            //EffectManager.sendUIEffectImageURL((short)uikey, con, true, "头像", Yut.Instance.Configuration.Instance.GameConfig.BossIcon);
            EffectManager.sendUIEffectText((short)uikey, con, true, "Boss名字", GameStateManager.Instance.ModeConfig.StateConfig.BossName);
            EffectManager.sendUIEffectText((short)uikey, con, true, "血量", $"{max}/{max}");
        }
        internal void CloseUI()
        {
            foreach (var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(id);
                if (con == null)
                    continue;
                EffectManager.askEffectClearByID(Yut.Instance.Configuration.Instance.HKey, con);
            }
        }
        internal void UpdatePlayerStusUI()
        {
            short key = (short)Yut.Instance.Configuration.Instance.HKey;
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
                    if (player1 == null || player1.Player == null)
                        continue;
                    EffectManager.sendUIEffectText(key, con, true, $"排名{i}", stus[i].Rank.ToString());
                    EffectManager.sendUIEffectText(key, con, true, $"名字{i}", player1.DisplayName);
                    EffectManager.sendUIEffectText(key, con, true, $"分数{i}", stus[i].Damage.ToString());
                }
            }
        }
        internal void OnPlayerConnected(UnturnedPlayer player)
        {
            if (player == null || player.Player == null)
                return;
            if (damages.ContainsKey(player.CSteamID))
            {
                ushort uikey = Yut.Instance.Configuration.Instance.HKey;
                OpenUI(uikey, player.CSteamID);
                Teleport(player);
            }
        }
        internal void OnPlayerRevive(UnturnedPlayer player)
        {
            if (player == null || player.Player == null)
                return;
            if (damages.ContainsKey(player.CSteamID))
                Teleport(player);
        }
        internal void UpdateBossHealthUI(uint bossHealth, ref byte lastPercent)
        {
            short key = (short)Yut.Instance.Configuration.Instance.HKey;
            uint max = GameStateManager.Instance.ModeConfig.Region.BossHealth;
            byte num = DataModule.Math.RangeToByte(100f * bossHealth / max);
            foreach (var csteamid in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(csteamid);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(csteamid);
                if (con == null)
                    continue;
                if (num < lastPercent)
                {
                    for (int i = num; i < lastPercent; i++)
                        EffectManager.sendUIEffectVisibility(key, con, true, $"b ({i})", false);
                }
                else if (lastPercent < num)
                {
                    for (int i = lastPercent; i < num; i++)
                        EffectManager.sendUIEffectVisibility(key, con, true, $"b ({i})", true);
                }
                EffectManager.sendUIEffectText(key, con, true, "血量", $"{bossHealth}/{max}");
            }
            lastPercent = num;
        }
        internal void OpenSkillUI(string text)
        {
            ushort key = Yut.Instance.Configuration.Instance.NKey;
            foreach (var csteamid in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(csteamid);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(csteamid);
                if (con == null)
                    continue;
                EffectManager.sendUIEffect(key, (short)key, con, true);
                EffectManager.sendUIEffectText((short)key, con, true, "提示文本", text);
            }
        }
        internal void SendMessageToPlayers(string message, Color color)
        {
            foreach (var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player != null && player.Player != null)
                    UnturnedChat.Say(player, message, color);
            }
        }
        internal void UpdateShieldState(bool state)
        {
            short key = (short)Yut.Instance.Configuration.Instance.HKey;
            foreach (var csteamid in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(csteamid);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(csteamid);
                if (con == null)
                    continue;
                EffectManager.sendUIEffectVisibility(key, con, true, "反弹", state);
            }
        }
        internal void UpdateTime(int seconds)
        {
            short key = (short)Yut.Instance.Configuration.Instance.HKey;
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            string time = span.ToString(seconds >= 3600 ? @"hh\:mm\:ss" : @"mm\:ss");
            foreach (var csteamid in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(csteamid);
                if (player == null)
                    continue;
                ITransportConnection con = Provider.findTransportConnection(csteamid);
                if (con == null)
                    continue;
                EffectManager.sendUIEffectText(key, con, true, "倒计时", time);
            }
        }
        internal void SendMessageToPlayers(string message)
        {
            foreach (var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player != null && player.Player != null)
                    UnturnedChat.Say(player, message);
            }
        }
        internal void TeleportPlayers()
        {
            List<ZombieSpawnpoint> spawnpoints = LevelZombies.zombies[ZombieManager.Instance.Bound];
            foreach (var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player != null && player.Player != null)
                {
                    Vector3 point = spawnpoints[UnityEngine.Random.Range(0, spawnpoints.Count)].point + new Vector3(0, 1, 0);
                    player.Teleport(point + Vector3.up, UnityEngine.Random.Range(0, 360f));
                }
            }
        }
        internal void Reward()
        {
            var orders = damages.OrderByDescending(x => x.Value);
            foreach (var s in orders)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(s.Key);
                //if (player == null || player.Player == null)
                //    continue;
                if (s.Value < GameStateManager.Instance.ModeConfig.StateConfig.MinRewardDamage)
                {
                    if (player != null && player.Player != null)
                        UnturnedChat.Say(player, Yut.Instance.Translate("Low_Damage", GameStateManager.Instance.ModeConfig.StateConfig.MinRewardDamage));
                    continue;
                }
                Items items = new Items(PlayerInventory.STORAGE);
                items.loadSize(10, 20);
                byte rank = 0;
                RewardInterval interval = GameStateManager.Instance.ModeConfig.StateConfig.Rewards.Find(x => x.InInterval(++rank));
                if (interval != null)
                {
                    for (int i = 0; i < interval.Rewards.Count; i++)
                    {
                        ItemPair item = interval.Rewards[i];
                        byte a = (byte)UnityEngine.Random.Range(0, 100);
                        if (a < interval.Rewards[i].Chance)
                        {
                            var count = UnityEngine.Random.Range(1, item.Count + 1);
                            for(var j = 0;j < count;j++)
                            {
                                items.tryAddItem(new Item(item.Id,EItemOrigin.ADMIN, 100));
                            }
                        }
                    }
                    rewards.Add(s.Key, items);
                }
                if (player != null && player.Player != null)
                {
                    UnturnedChat.Say(player, Yut.Instance.Translate("Get_Reward"));
                    OnChallengeSuccess?.Invoke(player);
                }
            }
        }
        internal void Clear()
        {
            damages.Clear();
            rewards.Clear();
        }
        internal bool HasReward(CSteamID id)
            => rewards.ContainsKey(id) && rewards[id].getItemCount() > 0;
        internal void SendRewardStorage(UnturnedPlayer player)
        {
            Items item = rewards[player.CSteamID];
            player.Inventory.updateItems(PlayerInventory.STORAGE, item);
            player.Inventory.sendStorage();
        }
        internal IEnumerable<UnturnedPlayer> PlayersInRange(Vector3 vector, float r)
        {
            foreach (var id in damages.Keys)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(id);
                if (player is null || player.Player is null)
                    continue;
                float r1 = Vector3.Distance(vector, player.Position);
                if (r1 < r)
                    yield return player;
            }
        }
        internal UnturnedPlayer RandomPlayerInRange(Vector3 vector, float r)
        {
            List<UnturnedPlayer> players = PlayersInRange(vector, r).ToList();
            if (players.Count == 0)
                return null;
            return players[UnityEngine.Random.Range(0, players.Count)];
        }
        internal UnturnedPlayer MinDistPlayerInRange(Vector3 vector, float r)
        {
            UnturnedPlayer result = null;
            float min = float.MaxValue;
            foreach (var player in PlayersInRange(vector, r))
            {
                float dist = Vector3.Distance(vector, player.Position);
                if (dist < min)
                {
                    min = dist;
                    result = player;
                }
            }
            return result;
        }
        internal List<UnturnedPlayer> RandomPlayersInRange(Vector3 vector, float r, byte num)
        {
            List<UnturnedPlayer> players = PlayersInRange(vector, r).ToList();
            List<UnturnedPlayer> result = new List<UnturnedPlayer>();
            if (players.Count == 0)
                return result;
            for (int i = 0; i < num; i++)
            {
                int ind = UnityEngine.Random.Range(0, players.Count);
                result.Add(players[ind]);
                players.RemoveAt(ind);
                if (players.Count == 0)
                    break;
            }
            return result;
        }
        internal void Teleport(UnturnedPlayer player)
        {
            List<ZombieSpawnpoint> spawnpoints = LevelZombies.zombies[ZombieManager.Instance.Bound];
            Vector3 point = spawnpoints[UnityEngine.Random.Range(0, spawnpoints.Count)].point + new Vector3(0, 1, 0);
            player.Teleport(point + Vector3.up, UnityEngine.Random.Range(0, 360f));
        }
        private List<DamageStus> GetPlayerStus(CSteamID id)
        {
            List<KeyValuePair<CSteamID, uint>> list = damages.OrderByDescending(x => x.Value).ToList();
            List<DamageStus> stus = new List<DamageStus>();
            int ind = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key == id)
                {
                    ind = i;
                    break;
                }
            }
            if (ind == -1)
                return stus;
            byte start = 0;
            byte end = DataModule.Math.RangeToByte(list.Count);
            if (ind == 0)
                end = DataModule.Math.RangeToByte(ind + 5, 0, (byte)list.Count);
            else if (ind == 1)
                end = DataModule.Math.RangeToByte(ind + 4, 0, (byte)list.Count);
            else if (ind == list.Count - 1 || ind == list.Count - 2)
                start = DataModule.Math.RangeToByte(list.Count - 5);
            else
            {
                start = DataModule.Math.RangeToByte(ind - 2);
                end = DataModule.Math.RangeToByte(ind + 3, 0, (byte)list.Count);
            }
            for (byte i = start; i < end; i++)
                stus.Add(new DamageStus((byte)(i + 1), list[i].Key, list[i].Value));
            return stus;
        }
        private void Awake()
        {
            instance = this;
        }
        private void Update()
        {
            if (GameStateManager.Instance.State == EState.Fighting)
            {
                if (Time.time - frame < DataModule.Math.Range(Yut.Instance.Configuration.Instance.LeaderboardRefreshSeconds, 1))
                    return;
                frame = Time.time;
                UpdatePlayerStusUI();
            }
        }
    }
}