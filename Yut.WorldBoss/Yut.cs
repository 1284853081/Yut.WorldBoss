﻿using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
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
    public class Yut : RocketPlugin<Config>
    {
        private static Yut instance;
        private int bossTable;
        private int minionTable;
        public static Yut Instance => instance;
        public int BossTable => bossTable;
        public int MinionTable => minionTable;
        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList list = new TranslationList();
                list.Add("Invalid_Point", "Unreasonable boss refresh point");
                list.Add("Error_Syntax", "Error syntax");
                list.Add("Cant_Join", "Currently unable to register to participate in World Boss");
                list.Add("No_Tickets", "No tickets to participate in World Boss");
                list.Add("Join_Success", "Successfully registered to participate in the World BOSS");
                list.Add("Not_Start_Fighting", "Currently unable to teleport to World Boss");
                list.Add("Boss_Start", "World BOSS has been activated. Enter wbj to register and participate.Boss will refresh in {0} seconds");
                list.Add("Boss_Fighting", "The world boss has been refreshed");
                list.Add("Repeat_Join", "Do not repeat registration");
                list.Add("Low_Damage", "Due to the damage you caused being less than {0}, you cannot receive rewards");
                list.Add("Get_Reward", "Congratulations on completing the challenge. You can now use wbr to claim rewards");
                list.Add("Not_Start_Rreward", "Currently unable to claim rewards");
                list.Add("No_Rreward", "There are currently no rewards to claim");
                list.Add("Set_Point", "Successfully set Boss refresh point");
                list.Add("Error_Point", "Unable to set refresh point here");
                list.Add("Set_Time", "Successfully set refresh time to {0}:{1}");
                list.Add("Invalid_Parameter", "Invalid parameter");
                list.Add("Challenge_Ends", "The world boss has ended");
                list.Add("Not_Sign", "Unable to use this command without registering to participate in the World BOSS");
                return list;
            }
        }
        protected override void Load()
        {
            instance = this;
            TryAddComponent<BossManager>();
            TryAddComponent<ZombieManager>();
            TryAddComponent<PlayerManager>();
            DamageTool.damageZombieRequested += DamageTool_damageZombieRequested;
        }
        public DateTime InitNextRefreshTime()
        {
            DateTime now = DateTime.Now;
            DateTime freshTime = new DateTime(now.Year, now.Month, now.Day, Configuration.Instance.RefreshHour, Configuration.Instance.RefreshMinute, 0);
            TimeSpan span = now - freshTime;
            if (span.TotalSeconds > 0)
                freshTime += new TimeSpan((int)Math.Ceiling(span.TotalDays), 0, 0, 0);
            return freshTime;
        }
        public bool Register(byte bound)
        {
            if (SDG.Unturned.ZombieManager.regions != null &&
                SDG.Unturned.ZombieManager.regions[bound] != null)
            {
                SDG.Unturned.ZombieManager.regions[bound].onZombieLifeUpdated += OnZombieLifeUpdated;
                return true;
            }
            return false;
        }
        private void UnRegister()
        {
            if (LevelNavigation.tryGetBounds(Configuration.Instance.BossRefreshPoint, out byte bound))
            {
                SDG.Unturned.ZombieManager.regions[bound].onZombieLifeUpdated -= OnZombieLifeUpdated;
            }
        }
        public void LoadCloth()
        {
            bossTable = LevelZombies.tables.FindIndex(x => x.name == Configuration.Instance.Region.BossTable);
            minionTable = LevelZombies.tables.FindIndex(x => x.name == Configuration.Instance.Region.MinionTable);
            if (bossTable < 0)
                bossTable = UnityEngine.Random.Range(0, LevelZombies.tables.Count);
            if(minionTable < 0)
                minionTable = UnityEngine.Random.Range(0, LevelZombies.tables.Count);
        }
        private void DamageTool_damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (BossManager.Instance.State != EState.Fighting)
                return;
            if (parameters.zombie == null)
                return;
            if (parameters.zombie.bound != ZombieManager.Instance.Bound)
                return;
            if (parameters.zombie.id > Configuration.Instance.Region.MaxMinions)
                return;
            float times = parameters.times;
            if (parameters.applyGlobalArmorMultiplier)
            {
                if (parameters.limb == ELimb.SKULL)
                    times *= Provider.modeConfigData.Zombies.Armor_Multiplier;
                else
                    times *= Provider.modeConfigData.Zombies.NonHeadshot_Armor_Multiplier;
            }
            int num = Mathf.FloorToInt(parameters.damage * times);
            if (num == 0)
                return;
            ZombieManager.Instance.Damage(parameters, num);
            shouldAllow = false;
        }
        private void OnZombieLifeUpdated(Zombie zombie)
        {
            if (BossManager.Instance.IsStart)
            {
                if (zombie.isDead)
                {
                    if(zombie.id == 0)
                    {
                        BossManager.Instance.EndFight();
                        return;
                    }    
                    else if (zombie.id < Configuration.Instance.Region.MaxMinions && BossManager.Instance.State < EState.Rewarding)
                        ZombieManager.Instance.SpawnMinion(zombie.id);
                }
                else
                {
                    if (zombie.id > Configuration.Instance.Region.MaxMinions)
                        SDG.Unturned.ZombieManager.sendZombieDead(zombie, zombie.transform.position);
                }
            }
        }
        protected override void Unload()
        {
            DamageTool.damageZombieRequested -= DamageTool_damageZombieRequested;
            UnRegister();
        }
    }
}
