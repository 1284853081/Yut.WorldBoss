using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Yut.WorldBoss
{
    public class Yut : RocketPlugin<Config>
    {
        private static Yut instance;
        public static Yut Instance => instance;
        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList list = new TranslationList();
                list.Add("Invalid_Point", "非法点位");
                list.Add("Error_Syntax", "指令使用错误");
                list.Add("Cant_Join", "现在不能报名世界BOSS");
                list.Add("No_Tickets", "没有报名世界BOSS的门票");
                list.Add("Join_Success", "成功报名世界BOSS");
                list.Add("Boss_Start", "世界BOSS已经开启,输入/wbj即可报名参加,报名时间还剩{0}秒");
                list.Add("Boss_Fighting", "世界BOSS进入战斗阶段");
                list.Add("Repeat_Join", "请勿重复报名");
                list.Add("Low_Damage", "由于你造成的伤害少于{0}你无法获得奖励");
                list.Add("Get_Reward", "恭喜挑战成功.你现在可以使用/wbr来领取奖励");
                list.Add("Not_Start_Rreward", "当前不可领取奖励");
                list.Add("No_Rreward", "你没有任何奖励可以领取");
                list.Add("Set_Point", "成功设置世界BOSS刷新点");
                list.Add("Error_Point", "无法在此处设置世界BOSS刷新点");
                list.Add("Mode_Not_Found", "未找到该模式");
                list.Add("New_Boss_Health", "成功将该模式下的BOSS血量设置为{0}");
                list.Add("New_Max_Minions", "成功将该模式下的小怪数量设置为{0}");
                list.Add("New_Boss_Table", "成功将该模式下的BOSS服装表名设置为{0}");
                list.Add("New_Minion_Table", "成功将该模式下的小怪服装表名设置为{0}");
                list.Add("New_Boss_Name", "成功将该模式下的世界BOSS名字设置为{0}");
                list.Add("New_Prepare_Seconds", "成功将该模式下的准备时间设置为{0}");
                list.Add("New_Fighting_Seconds", "成功将该模式下的挑战时间设置为{0}");
                list.Add("New_Reward_Seconds", "成功将该模式下的奖励领取时间设置为{0}");
                list.Add("New_Min_Reward_Damage", "成功将该模式下的最低奖励伤害设置为{0}");
                list.Add("New_Skill_Refresh_Seconds", "成功将该模式下的世界BOSS技能间隔设置为{0}");
                list.Add("New_Max_Players", "成功将该模式下的最大参与人数设置为{0}");
                list.Add("New_Shield_Seconds", "成功将该模式下的护盾时间设置为{0}");
                list.Add("New_Fire_Damage", "成功将该模式下的灼烧伤害设置为{0}");
                list.Add("New_Shield_Damage", "成功将该模式下的护盾反弹伤害设置为{0}");
                list.Add("New_Fly_Damage", "成功将该模式下的击飞伤害设置为{0}");
                list.Add("New_Explosion_Damage", "成功将该模式下的爆炸伤害设置为{0}");
                list.Add("New_Baptism_Damage", "成功将该模式下的洗礼伤害设置为{0}");
                list.Add("New_Heal_Amount", "成功将该模式下的回血量设置为{0}");
                list.Add("Set_Time", "成功将刷新世界设置为{0}:{1}");
                list.Add("Remove_Time", "成功删除刷新时间{0}:{1}");
                list.Add("Invalid_Parameter", "非法参数");
                list.Add("Challenge_Ends", "世界BOSS结束");
                list.Add("Boss_Skill", "世界BOSS释放了技能:{0}");
                list.Add("Shield_Open", "世界BOSS开启了护盾保护，当前无敌并反弹伤害");
                list.Add("Shield_Close", "世界BOSS关闭了护盾");
                list.Add("Time_Not_Find", "未找到该刷新时间");
                list.Add("Not_Disturb", "请勿打扰正常的挑战进程");
                list.Add("Skill_Not_Found", "未找到指定技能");
                list.Add("New_Skill_Range", "成功将该模式下的技能{0}的范围设置为{1}");
                list.Add("New_Ticket", "成功将该模式下的门票设置为{0}({1})");
                list.Add("Remove_Success", "成功退出挑战");
                list.Add("Remove_Failed", "退出失败");
                list.Add("Not_Start_Fighting", "当前不在挑战时间，退出无效");
                list.Add("Challenge_Ends", "世界BOSS结束");
                return list;
            }
        }
        protected override void Load()
        {
            instance = this;
            TryAddComponent<GameStateManager>();
            TryAddComponent<ZombieManager>();
            TryAddComponent<PlayerManager>();
            TryAddComponent<BossSkillManager>();
            DamageTool.damageZombieRequested += DamageTool_damageZombieRequested;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive += UnturnedPlayerEvents_OnPlayerRevive;
            OrderRefreshTime();
            Logger.Log("* * * * * * * * * * * * * * *");
            Logger.Log("*      出品方：Yuthung      *");
            Logger.Log("*         作者：月鸿        *");
            Logger.Log("*    插件：Yut.WorldBoss    *");
            Logger.Log("*       状态：已加载        *");
            Logger.Log("* * * * * * * * * * * * * * *\n");
        }
        private void UnturnedPlayerEvents_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
            => PlayerManager.Instance.OnPlayerRevive(player);
        private void Events_OnPlayerConnected(UnturnedPlayer player)
            => PlayerManager.Instance.OnPlayerConnected(player);
        internal Refresh InitNextRefreshTime()
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now.TimeOfDay;
            if (Configuration.Instance.Refreshs.Count == 0)
                return Refresh.Zero;
            else if (Configuration.Instance.Refreshs.Count == 1)
                return Configuration.Instance.Refreshs[0];
            for (int i = 0;i< Configuration.Instance.Refreshs.Count-1;i++)
            {
                Refresh a = Configuration.Instance.Refreshs[i];
                if(span < a)
                    return a;
                Refresh b = Configuration.Instance.Refreshs[i+1];
                if (span < b)
                    return b;
            }
            return Configuration.Instance.Refreshs[0];
        }
        internal void OrderRefreshTime()
        {
            Configuration.Instance.Refreshs = Configuration.Instance.Refreshs.OrderBy(x => x.Hour * 60 + x.Minute).ToList();
            Configuration.Save();
        }
        internal bool Register(byte bound)
        {
            if (SDG.Unturned.ZombieManager.regions != null &&
                SDG.Unturned.ZombieManager.regions[bound] != null)
            {
                SDG.Unturned.ZombieManager.regions[bound].onZombieLifeUpdated += OnZombieLifeUpdated;
                return true;
            }
            return false;
        }
        internal bool CheckMinion(string mode,string type)
        {
            ModeConfig modeConfig = Configuration.Instance.ModeConfigs.Find(x => x.Mode.ToLower() == mode.ToLower());
            if (modeConfig == null)
                return false;
            Minion minion = modeConfig.Region.Minions.Find(x => x.type.ToLower() == type.ToLower());
            return minion != null;
        }
        private void UnRegister()
        {
            if (LevelNavigation.tryGetBounds(Configuration.Instance.BossRefreshPoint, out byte bound))
            {
                SDG.Unturned.ZombieManager.regions[bound].onZombieLifeUpdated -= OnZombieLifeUpdated;
            }
        }
        internal ModeConfig GetModeConfig(string mode)
            => Configuration.Instance.ModeConfigs.Find(x => x.Mode.ToLower() == mode.ToLower());
        private void DamageTool_damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (GameStateManager.Instance.State != EState.Fighting ||
                parameters.zombie == null ||
                parameters.zombie.bound != ZombieManager.Instance.Bound ||
                parameters.zombie.id > GameStateManager.Instance.ModeConfig.Region.MaxMinions)
                return;
            shouldAllow = false;
            float times = parameters.times;
            if (parameters.applyGlobalArmorMultiplier)
            {
                if (parameters.limb == ELimb.SKULL)
                    times *= Provider.modeConfigData.Zombies.Armor_Multiplier;
                else
                    times *= Provider.modeConfigData.Zombies.NonHeadshot_Armor_Multiplier;
            }
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(parameters.instigator as Player);
            if(player != null && player.Player != null)
            {
                if (!PlayerManager.Instance.HasSign(player))
                {
                    player.Player.teleportToRandomSpawnPoint();
                    UnturnedChat.Say(player, Translate("Not_Disturb"), Color.red);
                    return;
                }
                if(parameters.limb == ELimb.SKULL || parameters.limb == ELimb.SPINE)
                {
                    if (player.Player.equipment.asset is ItemGunAsset gunAsset)
                        parameters.damage /= parameters.limb == ELimb.SKULL ? gunAsset.zombieDamageMultiplier.skull : gunAsset.zombieDamageMultiplier.spine;
                    else if (player.Player.equipment.asset is ItemMeleeAsset meleeAsset)
                        parameters.damage /= parameters.limb == ELimb.SKULL ? meleeAsset.zombieDamageMultiplier.skull : meleeAsset.zombieDamageMultiplier.spine;
                }
                uint num = (uint)Mathf.FloorToInt(parameters.damage * times);
                if (num == 0)
                    return;
                ZombieManager.Instance.Damage(parameters, num, player);
            }
        }
        private void OnZombieLifeUpdated(Zombie zombie)
        {
            if (GameStateManager.Instance.IsStart)
            {
                if (zombie.isDead)
                {
                    if(zombie.id == 0)
                    {
                        GameStateManager.Instance.EndFight();
                        return;
                    }    
                    else if (zombie.id < GameStateManager.Instance.ModeConfig.Region.MaxMinions && GameStateManager.Instance.State < EState.Rewarding)
                        ZombieManager.Instance.SpawnMinion(zombie.id);
                }
                else
                {
                    if (zombie.id > GameStateManager.Instance.ModeConfig.Region.MaxMinions)
                        SDG.Unturned.ZombieManager.sendZombieDead(zombie, zombie.transform.position);
                }
            }
        }
        protected override void Unload()
        {
            DamageTool.damageZombieRequested -= DamageTool_damageZombieRequested;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive -= UnturnedPlayerEvents_OnPlayerRevive;
            UnRegister();
            Logger.Log("* * * * * * * * * * * * * * *");
            Logger.Log("*      出品方：Yuthung      *");
            Logger.Log("*         作者：月鸿        *");
            Logger.Log("*    插件：Yut.WorldBoss    *");
            Logger.Log("*       状态：已卸载        *");
            Logger.Log("* * * * * * * * * * * * * * *\n");
        }
    }
}