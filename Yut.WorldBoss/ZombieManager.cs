using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Yut.DataModule;
using Rocket.Unturned.Player;
using System.Reflection;
using Steamworks;

namespace Yut.WorldBoss
{
    public delegate void DamageZombieHandler(Zombie zombie, uint damaage,bool isWorldBoss);
    public class ZombieManager : MonoBehaviour
    {
        private bool init = false;
        private class ZombieState
        {
            public ushort id;
            public uint health;
            public ZombieState(ushort id, uint health)
            {
                this.id = id;
                this.health = health;
            }
        }
        private static ZombieManager instance;
        private Zombie boss;
        private byte bossLastBloodPercent;
        private byte bound = 0;
        private float lastAlert;
        private readonly List<ZombieState> states = new List<ZombieState>();
        internal byte Bound => bound;
        public static ZombieManager Instance => instance;
        public static event DamageZombieHandler OnDamageZombie;
        internal void Damage(DamageZombieParameters parameters, uint damage,UnturnedPlayer player)
        {
            ZombieState state = states.Find(t => t.id == parameters.zombie.id);
            if (state == null)
                return;
            if (parameters.zombie.id == 0 && BossSkillManager.Instance.IsInvincible)
            {
                if (player != null)
                    player.Player.life.askDamage(GameStateManager.Instance.ModeConfig.SkillConfig.ShieldDamage, player.Position,
                        EDeathCause.ZOMBIE, ELimb.SKULL, CSteamID.Nil, out EPlayerKill _);
                return;
            }
            if (state.health > damage)
                state.health -= damage;
            else
            {
                damage = state.health;
                state.health = 0;
            }
            if (parameters.zombie.id == 0)
            {
                if (player != null)
                    PlayerManager.Instance.AddDamage(player.CSteamID, damage);
                PlayerManager.Instance.UpdateBossHealthUI(state.health, ref bossLastBloodPercent);
            }
            OnDamageZombie?.Invoke(parameters.zombie, damage, parameters.zombie.id == 0);
            if (state.health <= 0)
            {
                states.Remove(state);
                SDG.Unturned.ZombieManager.sendZombieDead(parameters.zombie, parameters.zombie.transform.position);
            }
        }
        internal void Run()
        {
            SpawnBoss();
            SpawnMinions();
        }
        internal void KillAll()
        {
            if (!init)
                return;
            for (int i = 0; i < SDG.Unturned.ZombieManager.regions[bound].zombies.Count; i++)
            {
                Zombie zombie = SDG.Unturned.ZombieManager.regions[bound].zombies[i];
                if (zombie != null && !zombie.isDead)
                    SDG.Unturned.ZombieManager.sendZombieDead(zombie, Vector3.zero);
            }
        }
        internal void HealBoss(uint amount)
        {
            if (boss == null)
                return;
            states[0].health = DataModule.Math.RangeToUInt32(states[0].health + amount, 0, GameStateManager.Instance.ModeConfig.Region.BossHealth);
            PlayerManager.Instance.UpdateBossHealthUI(states[0].health, ref bossLastBloodPercent);
        }
        internal void TeleportZombie(Zombie zombie, EZombieSpeciality speciality, Vector3 point)
        {
            byte type = DataModule.Math.RangeToByte(GameStateManager.Instance.MinionTable);
            byte suit = RandomSuit(LevelZombies.tables[type]);
            SDG.Unturned.ZombieManager.sendZombieAlive(zombie, type, (byte)speciality,
                suit, suit, suit, suit, point, (byte)UnityEngine.Random.Range(0, 180));
        }
        internal Zombie GetMinion(byte index)
        {
            if (index <= 0 || index >= SDG.Unturned.ZombieManager.regions[bound].zombies.Count)
                return null;
            return SDG.Unturned.ZombieManager.regions[bound].zombies[index];
        }
        internal void TeleportBoss(EZombieSpeciality speciality,Vector3 point)
        {
            if (boss == null)
                return;
            SDG.Unturned.ZombieManager.sendZombieAlive(boss, boss.type, (byte)speciality,
                boss.shirt, boss.pants, boss.hat, boss.gear, point, (byte)UnityEngine.Random.Range(0, 180));
        }
        private void SpawnBoss()
        {
            LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out bound);
            boss = SDG.Unturned.ZombieManager.regions[bound].zombies[0];
            byte type = DataModule.Math.RangeToByte(GameStateManager.Instance.BossTable);
            byte suit = RandomSuit(LevelZombies.tables[type]);
            ZombieType.CheckValid(GameStateManager.Instance.ModeConfig.Region.BossType, out byte speciality);
            SDG.Unturned.ZombieManager.sendZombieAlive(boss, type, speciality, suit, suit, suit, suit,
                Yut.Instance.Configuration.Instance.BossRefreshPoint, (byte)UnityEngine.Random.Range(0, 180));
            states.Add(new ZombieState(0, GameStateManager.Instance.ModeConfig.Region.BossHealth));
            bossLastBloodPercent = 100;
            BossSkillManager.Instance.Init(boss);
            UnturnedPlayer player = PlayerManager.Instance.MinDistPlayerInRange(boss.transform.position, 200);
            if (player != null && player.Player != null)
                boss.alert(player.Player);
            lastAlert = Time.time;
        }
        private byte RandomSuit(ZombieTable table)
        {
            int min = int.MaxValue;
            for (int i = 0; i < 4; i++)
                min = System.Math.Min(min, table.slots[i].table.Count);
            return DataModule.Math.RangeToByte(UnityEngine.Random.Range(0, min));
        }
        private void SpawnMinions()
        {
            var region = GameStateManager.Instance.ModeConfig.Region;
            byte minions = (byte)Mathf.Min(SDG.Unturned.ZombieManager.regions[bound].zombies.Count, region.MaxMinions);
            for (byte i = 1; i <= minions; i++)
                SpawnMinion(i);
        }
        internal void SpawnMinion(ushort id)
        {
            var region = GameStateManager.Instance.ModeConfig.Region;
            byte type = DataModule.Math.RangeToByte(GameStateManager.Instance.MinionTable);
            var specialityStr = region.Minions[UnityEngine.Random.Range(0, region.Minions.Count)].type;
            ZombieType.CheckValid(specialityStr, out byte speciality);
            byte suit = RandomSuit(LevelZombies.tables[type]);
            List<ZombieSpawnpoint> spawnpoints = LevelZombies.zombies[bound];
            Vector3 point = spawnpoints[UnityEngine.Random.Range(0, spawnpoints.Count)].point + Vector3.up;
            uint health = region.Minions.Find(x => x.type == specialityStr).Health;
            SDG.Unturned.ZombieManager.sendZombieAlive(SDG.Unturned.ZombieManager.regions[bound].zombies[id],
                type, speciality, suit, suit, suit, suit, point, (byte)UnityEngine.Random.Range(0, 180));
            states.Add(new ZombieState(id, health));
        }
        internal void Clear()
        {
            states.Clear();
            boss = null;
        }
        private void Awake()
        {
            instance = this;
        }
        private void Update()
        {
            if (!init)
            {
                if (LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out bound))
                {
                    if (Yut.Instance.Register(bound))
                        init = true;
                }
            }
            else
            {
                if (GameStateManager.Instance.IsStart)
                {
                    for (int i = GameStateManager.Instance.ModeConfig.Region.MaxMinions + 1; i < SDG.Unturned.ZombieManager.regions[bound].zombies.Count; i++)
                    {
                        Zombie zombie = SDG.Unturned.ZombieManager.regions[bound].zombies[i];
                        if (!zombie.isDead)
                            SDG.Unturned.ZombieManager.sendZombieDead(zombie, Vector3.zero);
                    }
                }
                if(boss != null && Time.time - lastAlert > 1)
                {
                    Type type = boss.GetType();
                    FieldInfo info = type.GetField("player", BindingFlags.Instance | BindingFlags.NonPublic);
                    if(info.GetValue(boss) == null)
                    {
                        UnturnedPlayer player = PlayerManager.Instance.MinDistPlayerInRange(boss.transform.position, 200);
                        if (player != null && player.Player != null)
                            boss.alert(player.Player);
                    }
                    lastAlert = Time.time;
                }
            }
        }
        //private void FixedUpdate()
        //{
        //    if (BossManager.Instance.State != EState.Fighting || boss is null)
        //        return;
        //    if (!BossSkillManager.Instance.IsReleaseSkill && Time.time - lastPhantom >= DataModule.Math.Range(Yut.Instance.Configuration.Instance.PhantomSeconds, 1f))
        //    {
        //        string typeStr = Yut.Instance.Configuration.Instance.PhantomType[UnityEngine.Random.Range(0,
        //            Yut.Instance.Configuration.Instance.PhantomType.Count)];
        //        if (ZombieType.CheckValid(typeStr, out byte typeb))
        //            SDG.Unturned.ZombieManager.sendZombieSpeciality(boss, (EZombieSpeciality)typeb);
        //        lastPhantom = Time.time;
        //    }
        //}
    }
}