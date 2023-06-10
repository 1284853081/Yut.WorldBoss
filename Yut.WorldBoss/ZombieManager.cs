﻿using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Yut.ZombieModule;
using Yut.DataModule;
using Rocket.Unturned.Player;

namespace Yut.WorldBoss
{
    public class ZombieManager : MonoBehaviour
    {
        private bool init = false;
        private class ZombieState
        {
            public ushort id;
            public uint health;
            public ZombieState( ushort id, uint health)
            {
                this.id = id;
                this.health = health;
            }
        }
        private static ZombieManager instance;
        public static ZombieManager Instance => instance;
        private byte bound = 0;
        private static readonly List<ZombieState> states = new List<ZombieState>();
        public byte Bound => bound;
        public void Damage(DamageZombieParameters parameters,int damage)
        {
            ZombieState state = states.Find(t => t.id == parameters.zombie.id);
            if (state == null)
                return;
            uint x = (uint)damage;
            if (state.health > x)
                state.health -= x;
            else
            {
                x = state.health;
                state.health = 0;
            }
            if (parameters.zombie.id == 0)
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(parameters.instigator as Player);
                if (player != null)
                    PlayerManager.Instance.AddDamage(player.CSteamID, x);
                PlayerManager.Instance.UpdateBossHealthUI(state.health);
            }
            if(state.health <= 0)
            {
                states.Remove(state);
                SDG.Unturned.ZombieManager.sendZombieDead(parameters.zombie, parameters.zombie.transform.position);
            }
        }
        public void Run()
        {
            SpawnBoss();
            SpawnMinions();
        }
        public void KillAll()
        {
            if (!init)
                return;
            for (int i = 0; i < SDG.Unturned.ZombieManager.regions[bound].zombies.Count; i++)
            {
                Zombie zombie = SDG.Unturned.ZombieManager.regions[bound].zombies[i];
                if(zombie != null && !zombie.isDead)
                    SDG.Unturned.ZombieManager.sendZombieDead(zombie, Vector3.zero);
            }
        }
        private void SpawnBoss()
        {
            LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out bound);
            Zombie boss = SDG.Unturned.ZombieManager.regions[bound].zombies[0];
            byte type = (byte)(Yut.Instance.TableInd == -1 ? 0 : Yut.Instance.TableInd);
            ZombieType.CheckValid(Yut.Instance.Configuration.Instance.regions[0].BossType, out byte speciality);
            SDG.Unturned.ZombieManager.sendZombieAlive(boss, type, speciality, 0, 0, 0, 0,
                Yut.Instance.Configuration.Instance.BossRefreshPoint, (byte)UnityEngine.Random.Range(0, 180));
            states.Add(new ZombieState(0, Yut.Instance.Configuration.Instance.regions[0].BossHealth));
        }
        private void SpawnMinions()
        {
            var region = Yut.Instance.Configuration.Instance.regions[0];
            byte minions = region.MaxMinions;
            for (byte i = 1; i <= minions; i++)
                SpawnMinion(i);
        }
        public void SpawnMinion(ushort id)
        {
            var region = Yut.Instance.Configuration.Instance.regions[0];
            byte type = (byte)(Yut.Instance.TableInd == -1 ? 0 : Yut.Instance.TableInd);
            var specialityStr = region.Minions[UnityEngine.Random.Range(0, region.Minions.Count)].type;
            ZombieType.CheckValid(specialityStr, out byte speciality);
            byte suitInd = (byte)(UnityEngine.Random.Range(0, region.MinionCloths.Count) + 1);
            List<ZombieSpawnpoint> spawnpoints = LevelZombies.zombies[bound];
            Vector3 point = spawnpoints[UnityEngine.Random.Range(0, spawnpoints.Count)].point + new Vector3(0,1,0);
            uint health = Yut.Instance.Configuration.Instance.regions[0].Minions.Find(x => x.type == specialityStr).Health;
            SDG.Unturned.ZombieManager.sendZombieAlive(SDG.Unturned.ZombieManager.regions[bound].zombies[id],
                type, speciality, suitInd, suitInd, suitInd, suitInd, point, (byte)UnityEngine.Random.Range(0, 180));
            states.Add(new ZombieState(id, health));
        }
        public void Clear()
            => states.Clear();
        private void Awake()
        {
            instance = this;
        }
        private void Update()
        {
            if(!init)
            {
                if(LevelNavigation.tryGetBounds(Yut.Instance.Configuration.Instance.BossRefreshPoint, out bound))
                {
                    if(Yut.Instance.Register(bound))
                    {
                        init = true;
                    }
                }
            }
            else
            {
                if (BossManager.Instance.IsStart)
                {
                    for (int i = Yut.Instance.Configuration.Instance.regions[0].MaxMinions + 1; i < SDG.Unturned.ZombieManager.regions[bound].zombies.Count; i++)
                    {
                        Zombie zombie = SDG.Unturned.ZombieManager.regions[bound].zombies[i];
                        if (!zombie.isDead)
                            SDG.Unturned.ZombieManager.sendZombieDead(zombie, Vector3.zero);
                    }
                }
            }
        }
    }
}