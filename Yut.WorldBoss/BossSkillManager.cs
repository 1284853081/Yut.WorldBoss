using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yut.WorldBoss
{
    public class BossSkillManager : MonoBehaviour
    {
        private Zombie boss;
        private static BossSkillManager instance;
        private readonly List<MethodInfo> skills = new List<MethodInfo>();
        private readonly List<UnturnedPlayer> virusPlayers = new List<UnturnedPlayer>();
        private SkillPair fireSkill;
        private float lastFire;
        private float lastSkill;
        private float lastShield;
        private bool isInvincible;
        private float lastVirus;
        private byte virusCount;
        private float lastBaptism;
        private float lastBaptism2;
        private UnturnedPlayer baptismPlayer;
        private List<UnturnedPlayer> baptismPlayer2;
        internal bool IsInvincible => isInvincible;
        public static BossSkillManager Instance => instance;
        internal void Init(Zombie zombie)
        {
            boss = zombie;
            lastSkill = Time.time;
            lastFire = Time.time;
            fireSkill = GameStateManager.Instance.GetSkill("burn");
            isInvincible = false;
            baptismPlayer2 = new List<UnturnedPlayer>();
        }
        internal void Destroy()
        {
            boss = null;
        }
        private void LoadSkills()
        {
            MethodInfo[] infos = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < infos.Length; i++)
            {
                if (Attribute.IsDefined(infos[i], typeof(SkillAttribute)))
                    skills.Add(infos[i]);
            }
        }
        [Skill]
        private void Thud()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("thud");
            SDG.Unturned.ZombieManager.sendZombieSpeciality(boss, EZombieSpeciality.BOSS_WIND);
            SDG.Unturned.ZombieManager.sendZombieStomp(boss);
            List<UnturnedPlayer> players = PlayerManager.Instance.PlayersInRange(boss.transform.position, skill.SkillRange).ToList();
            float num = 360f / players.Count;
            for(int i = 0; i < players.Count; i++)
            {
                if (IsBaptismTarget(players[i]))
                    continue;
                Quaternion quaternion = Quaternion.AngleAxis(num * i, Vector3.up);
                players[i].Teleport(boss.transform.position + quaternion * Vector3.forward, UnityEngine.Random.Range(0, 360));
            }
        }
        [Skill]
        private void Shield ()
        {
            if (isInvincible)
                return;
            isInvincible = true;
            lastShield = Time.time;
        }
        [Skill]
        private void Stomp()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("stomp");
            UnturnedPlayer player = PlayerManager.Instance.RandomPlayerInRange(boss.transform.position, skill.SkillRange);
            if (player != null && player.Player != null)
            {
                ZombieManager.Instance.TeleportBoss(EZombieSpeciality.BOSS_WIND, player.Position + Vector3.up * 2);
                SDG.Unturned.ZombieManager.sendZombieStomp(boss);
            }
        }
        [Skill]
        private void Explosion2()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("explosion2");
            Vector3 point = boss.transform.position;
            EffectAsset effectAsset = Assets.find(EAssetType.EFFECT, 119) as EffectAsset;
            for(int i = 1; i<= 3;i++)
            {
                int num = 6 + i * 2;
                float angle = 360f / num;
                for(int j = 0; j < num; j++)
                {
                    Quaternion quaternion = Quaternion.AngleAxis(angle * j, Vector3.up);
                    Vector3 position = point + quaternion * Vector3.forward * skill.SkillRange * i;
                    if (effectAsset != null)
                    {
                        TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                        parameters.position = position;
                        parameters.relevantDistance = EffectManager.MEDIUM;
                        EffectManager.triggerEffect(parameters);
                    }
                    DamageTool.explode(position + new Vector3(0f, 0.25f, 0f), 4f, EDeathCause.ZOMBIE, CSteamID.Nil, GameStateManager.Instance.ModeConfig.SkillConfig.ExplosionDamage, 0f, 0f, 0f, 0f, 0f, 0f, 0f, out List<EPlayerKill> _, EExplosionDamageType.ZOMBIE_FIRE, 4f, playImpactEffect: true, penetrateBuildables: false, EDamageOrigin.Flamable_Zombie_Explosion);
                }
            }
        }
        [Skill]
        private void Explosion()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("explosion");
            EffectAsset effectAsset = Assets.find(EAssetType.EFFECT, 119) as EffectAsset;
            foreach (var player in PlayerManager.Instance.PlayersInRange(boss.transform.position, skill.SkillRange))
            {
                if (effectAsset != null)
                {
                    TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                    parameters.position = player.Position;
                    parameters.relevantDistance = EffectManager.MEDIUM;
                    EffectManager.triggerEffect(parameters);
                }
                DamageTool.explode(player.Position + new Vector3(0f, 0.25f, 0f), 4f, EDeathCause.ZOMBIE, CSteamID.Nil, GameStateManager.Instance.ModeConfig.SkillConfig.ExplosionDamage, 0f, 0f, 0f, 0f, 0f, 0f, 0f, out List<EPlayerKill> _, EExplosionDamageType.ZOMBIE_FIRE, 4f, playImpactEffect: true, penetrateBuildables: false, EDamageOrigin.Flamable_Zombie_Explosion);
            }
        }
        [Skill]
        private void Breath()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("breath");
            float a = 2f;
            foreach(var player in PlayerManager.Instance.PlayersInRange(boss.transform.position, skill.SkillRange))
            {
                if (IsBaptismTarget(player))
                    continue;
                player.Teleport(boss.transform.position + boss.transform.forward * a, UnityEngine.Random.Range(0, 360));
                a += 0.5f;
            }
            SDG.Unturned.ZombieManager.sendZombieSpeciality(boss, EZombieSpeciality.BOSS_FIRE);
            SDG.Unturned.ZombieManager.sendZombieBreath(boss);
        }
        [Skill]
        private void Virus()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("virus");
            EffectAsset effectAsset = Assets.find(EAssetType.EFFECT, 133) as EffectAsset;
            foreach (var player in PlayerManager.Instance.PlayersInRange(boss.transform.position, skill.SkillRange))
            {
                virusPlayers.Add(player);
                if (effectAsset != null)
                {
                    TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                    parameters.SetRelevantPlayer(player.SteamPlayer());
                    parameters.position = player.Position;
                    parameters.scale = Vector3.one * 2f;
                    EffectManager.triggerEffect(parameters);
                }
            }
            lastVirus = Time.time;
            virusCount = 0;
        }
        [Skill]
        private void Fly()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("fly");
            EffectAsset effectAsset = Assets.find(EAssetType.EFFECT, 128) as EffectAsset;
            foreach (var player in PlayerManager.Instance.PlayersInRange(boss.transform.position, skill.SkillRange))
            {
                if (IsBaptismTarget(player))
                    continue;
                if (effectAsset != null)
                {
                    TriggerEffectParameters parameters = new TriggerEffectParameters();
                    parameters.reliable = true;
                    parameters.position = player.Position;
                    EffectManager.triggerEffect(parameters);
                }
                player.Teleport(player.Position + Vector3.up * 10f, player.Rotation);
                player.Player.life.askDamage(GameStateManager.Instance.ModeConfig.SkillConfig.FlyDamage, player.Position, EDeathCause.ZOMBIE, ELimb.SKULL, CSteamID.Nil, out EPlayerKill _);
            }
        }
        [Skill]
        private void Acid()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("acid");
            Vector3 point = boss.transform.position;
            byte num = DataModule.Math.RangeToByte(skill.SkillRange * Mathf.PI / 5);
            float angle = 360f / num;
            for(int i = 0; i < num; i++)
            {
                Quaternion quaternion = Quaternion.AngleAxis(angle * i, Vector3.up);
                Vector3 vector = point + quaternion * Vector3.forward * skill.SkillRange + Vector3.up * 15f;
                SDG.Unturned.ZombieManager.sendZombieAcid(boss, vector, Vector3.down);
            }
        }
        [Skill]
        private void Acid2()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("acid2");
            Vector3 point = boss.transform.position;
            for(int i = 0; i < 6;i++)
            {
                Quaternion quaternion = Quaternion.AngleAxis(60 * i, Vector3.up);
                int num = (int)skill.SkillRange / 10;
                for(int j = 1; j <= num; j++)
                {
                    Vector3 vector = point + quaternion * Vector3.forward * 10 * j + Vector3.up * 15f;
                    SDG.Unturned.ZombieManager.sendZombieAcid(boss, vector, Vector3.down);
                }
            }
        }
        [Skill]
        private void Flash()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("acid2");
            Vector3 point = boss.transform.position;
            EffectAsset effectAsset = Assets.find(EAssetType.EFFECT,166) as EffectAsset;
            if (effectAsset != null)
            {
                TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                parameters.reliable = true;
                parameters.position = point + Vector3.up * 10f;
                EffectManager.triggerEffect(parameters);
            }
            for(int i = 0; i<3;i++)
            {
                Quaternion quaternion = Quaternion.AngleAxis(120 * i, Vector3.up);
                TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                parameters.reliable = true;
                parameters.position = point + quaternion * Vector3.forward * skill.SkillRange + Vector3.up * 10;
                EffectManager.triggerEffect(parameters);
            }
        }
        [Skill]
        private void Heal()
        {
            ZombieManager.Instance.HealBoss(GameStateManager.Instance.ModeConfig.SkillConfig.HealAmount);
            Vector3 point = boss.transform.position;
            if (Assets.find(EAssetType.EFFECT, 141) is EffectAsset effectAsset)
            {
                TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                parameters.reliable = true;
                parameters.position = point;
                EffectManager.triggerEffect(parameters);
            }
        }
        [Skill]
        private void Boulder()
        {
            SkillPair skill = GameStateManager.Instance.GetSkill("boulder");
            Vector3 point = boss.transform.position;
            for(int i = 0; i< 3;i++)
            {
                int num = 10 + 4 * i;
                float angle = 360 / num;
                for(int j = 0; j < num; j++)
                {
                    Quaternion quaternion = Quaternion.AngleAxis(angle * j, Vector3.up);
                    SDG.Unturned.ZombieManager.sendZombieBoulder(boss, point + quaternion * Vector3.forward * skill.SkillRange * (i + 1) + Vector3.up * 20f, Vector3.down);
                }
            }
        }
        [Skill]
        private void Boulder2()
        {
            Vector3 point = boss.transform.position;
            for (int i = 0; i < 6; i++)
            {
                Quaternion quaternion = Quaternion.AngleAxis(60 * i, Vector3.up);
                Vector3 direction = quaternion * Vector3.forward;
                SDG.Unturned.ZombieManager.sendZombieBoulder(boss, point + direction * 3f + Vector3.up * 3f, direction);
            }
        }   
        [Skill]
        private void Baptism()
        {
            if (baptismPlayer != null)
                return;
            SkillPair skill = GameStateManager.Instance.GetSkill("baptism");
            UnturnedPlayer player = PlayerManager.Instance.RandomPlayerInRange(boss.transform.position, skill.SkillRange);
            if (player == null || player.Player == null || IsBaptismTarget(player))
                return;
            if (Assets.find(EAssetType.EFFECT, 166) is EffectAsset effectAsset)
            {
                player.Player.movement.sendPluginGravityMultiplier(0);
                player.Player.movement.sendPluginSpeedMultiplier(0);
                player.Teleport(player.Position + Vector3.up * 10f, player.Rotation);
                TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                parameters.reliable = true;
                parameters.position = player.Position + Vector3.down * 2;
                EffectManager.triggerEffect(parameters);
                lastBaptism = Time.time;
                baptismPlayer = player;
            }
        }
        [Skill]
        private void Baptism2()
        {
            if (baptismPlayer2.Count > 0)
                return;
            SkillPair skill = GameStateManager.Instance.GetSkill("baptism2");
            List<UnturnedPlayer> players = PlayerManager.Instance.RandomPlayersInRange(boss.transform.position, skill.SkillRange, 4);
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null || players[i].Player == null || IsBaptismTarget(players[i]))
                    continue;
                if (Assets.find(EAssetType.EFFECT, 166) is EffectAsset effectAsset)
                {
                    players[i].Player.movement.sendPluginGravityMultiplier(0);
                    players[i].Player.movement.sendPluginSpeedMultiplier(0);
                    players[i].Teleport(players[i].Position + Vector3.up * 10f, players[i].Rotation);
                    TriggerEffectParameters parameters = new TriggerEffectParameters(effectAsset);
                    parameters.reliable = true;
                    parameters.position = players[i].Position + Vector3.down * 2;
                    EffectManager.triggerEffect(parameters);
                }
            }
            lastBaptism2 = Time.time;
            baptismPlayer2 = players;
        }
        private bool IsBaptismTarget(UnturnedPlayer player)
        {
            if (player == null || player.Player == null ||
                baptismPlayer == null || baptismPlayer.Player == null)
                return false;
            if (baptismPlayer.CSteamID == player.CSteamID)
                return true;
            for(int i = 0; i < baptismPlayer2.Count; i++)
            {
                if (baptismPlayer2[i] == null || baptismPlayer2[i].Player == null)
                    continue;
                if (baptismPlayer2[i].CSteamID == player.CSteamID)
                    return true;
            }
            return false;
        }
        private void Awake()
        {
            instance = this;
            LoadSkills();
        }
        private void Update()
        {
            if(boss == null)
                return;
            if(Time.time - lastSkill > GameStateManager.Instance.ModeConfig.StateConfig.SkillRefreshSeconds)
            {
                MethodInfo skill = skills[UnityEngine.Random.Range(0, skills.Count)];
                skill.Invoke(this, null);
                if (skill.Name == "Shield")
                {
                    PlayerManager.Instance.SendMessageToPlayers(Yut.Instance.Translate("Shield_Open"),Yut.Instance.Configuration.Instance.SkillNoticeColor);
                    PlayerManager.Instance.UpdateShieldState(true);
                }
                PlayerManager.Instance.OpenSkillUI(Yut.Instance.Translate("Boss_Skill", GameStateManager.Instance.GetSkill(skill.Name).SkillName));
                lastSkill = Time.time;
            }
            if(Time.time - lastFire > 1)
            {
                foreach(var player in PlayerManager.Instance.PlayersInRange(boss.transform.position, fireSkill.SkillRange))
                {
                    if (player != null)
                        player.Player.life.askDamage(GameStateManager.Instance.ModeConfig.SkillConfig.FireDamage, player.Position, EDeathCause.ZOMBIE,
                            ELimb.SKULL, CSteamID.Nil, out EPlayerKill _);
                }
                lastFire = Time.time;
            }
            if (isInvincible && Time.time - lastShield > GameStateManager.Instance.ModeConfig.SkillConfig.ShieldSeconds)
            {
                isInvincible = false;
                PlayerManager.Instance.UpdateShieldState(false);
                PlayerManager.Instance.SendMessageToPlayers(Yut.Instance.Translate("Shield_Close"), Yut.Instance.Configuration.Instance.SkillNoticeColor);
            }
            if(virusPlayers.Count > 0)
            {
                if(Time.time - lastVirus > 1)
                {
                    for (int i = 0; i < virusPlayers.Count; i++)
                    {
                        if (virusPlayers[i].Player != null)
                            virusPlayers[i].Player.life.askInfect(GameStateManager.Instance.ModeConfig.SkillConfig.VirusDamage);
                    }
                    lastVirus = Time.time;
                    virusCount++;
                    if (virusCount > 10)
                        virusPlayers.Clear();
                }
            }
            if (baptismPlayer != null && baptismPlayer.Player != null && Time.time - lastBaptism > 5.2f)
            {
                baptismPlayer.Player.life.askDamage(GameStateManager.Instance.ModeConfig.SkillConfig.BaptismDamage, baptismPlayer.Position, EDeathCause.ZOMBIE, ELimb.SKULL, CSteamID.Nil, out EPlayerKill _);
                baptismPlayer.Player.movement.sendPluginGravityMultiplier(1);
                baptismPlayer.Player.movement.sendPluginSpeedMultiplier(1);
                baptismPlayer = null;
            }
            if (baptismPlayer2.Count > 0 && Time.time - lastBaptism2 > 5.2f)
            {
                for (int i = 0; i < baptismPlayer2.Count; i++)
                {
                    if (baptismPlayer2[i] != null && baptismPlayer2[i].Player != null)
                    {
                        baptismPlayer2[i].Player.life.askDamage(GameStateManager.Instance.ModeConfig.SkillConfig.BaptismDamage, baptismPlayer2[i].Position, EDeathCause.ZOMBIE, ELimb.SKULL, CSteamID.Nil, out EPlayerKill _);
                        baptismPlayer2[i].Player.movement.sendPluginGravityMultiplier(1);
                        baptismPlayer2[i].Player.movement.sendPluginSpeedMultiplier(1);
                    }
                }
                baptismPlayer2.Clear();
            }
        }
    }
}