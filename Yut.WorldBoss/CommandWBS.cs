using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Rocket.API.Extensions;
using Rocket.Unturned.Player;

namespace Yut.WorldBoss
{
    internal class CommandWBS : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "wbs";
        public string Help => "";
        public string Syntax => "wbs <mode> <tag> <value> [<value>]|wbs <point|p>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            ModeConfig config;
            if (command.Length > 0)
            {
                string tag = command[0].ToLower();
                if(command.Length == 1 && (tag == "point" || tag == "p"))
                {
                    UnturnedPlayer player = caller as UnturnedPlayer;
                    if (LevelNavigation.tryGetBounds(player.Position, out byte bound))
                    {
                        Yut.Instance.Configuration.Instance.BossRefreshPoint = player.Position;
                        Yut.Instance.Configuration.Save();
                        Yut.Instance.Register(bound);
                        UnturnedChat.Say(player, Yut.Instance.Translate("Set_Point"));
                    }
                    else
                        UnturnedChat.Say(player, Yut.Instance.Translate("Error_Point"));
                    return;
                }
                config = Yut.Instance.Configuration.Instance.ModeConfigs.Find(x => x.Mode.ToLower() == command[0].ToLower());
                if(config == null)
                {
                    UnturnedChat.Say(caller, Yut.Instance.Translate("Mode_Not_Found"));
                    return;
                }
            }
            else
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                return;
            }
            if (command.Length == 3)
            {
                string tag = command[1].ToLower();
                switch(tag)
                {
                    case "bosshealth":
                    case "bh":
                        if (uint.TryParse(command[2], out uint health))
                        {
                            config.Region.BossHealth = health;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Health", health));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "maxminions":
                    case "mm":
                        if (byte.TryParse(command[2], out byte maxMinions))
                        {
                            config.Region.MaxMinions = maxMinions;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Max_Minions", maxMinions));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "bosstable":
                    case "bt":
                        config.Region.BossTable = command[2];
                        Yut.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Table", command[2]));
                        break;
                    case "miniontable":
                    case "mt":
                        config.Region.MinionTable = command[2];
                        Yut.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Minion_Table", command[2]));
                        break;
                    case "bossname":
                    case "bn":
                        config.StateConfig.BossName = command[2];
                        Yut.Instance.Configuration.Save();
                        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Name", command[2]));
                        break;
                    case "prepareseconds":
                    case "ps":
                        if (ushort.TryParse(command[2], out ushort ps))
                        {
                            config.StateConfig.PrepareSeconds = ps;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Prepare_Seconds", ps));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "fightingseconds":
                    case "fs":
                        if (ushort.TryParse(command[2], out ushort fs))
                        {
                            config.StateConfig.FightingSeconds = fs;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fighting_Seconds", fs));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "rewardseconds":
                    case "rs":
                        if (ushort.TryParse(command[2], out ushort rs))
                        {
                            config.StateConfig.RewardSeconds = rs;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Reward_Seconds", rs));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "minrewarddamage":
                    case "mrd":
                        if (uint.TryParse(command[2], out uint mrd))
                        {
                            config.StateConfig.MinRewardDamage = mrd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Min_Reward_Damage", mrd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "skillrefreshseconds":
                    case "srs":
                        if (float.TryParse(command[2], out float srs))
                        {
                            config.StateConfig.SkillRefreshSeconds = srs;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Skill_Refresh_Seconds", srs));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "maxplayers":
                    case "mp":
                        if (byte.TryParse(command[2], out byte mp))
                        {
                            config.StateConfig.MaxPlayers = mp;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Max_Players", mp));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "shieldseconds":
                    case "ss":
                        if (ushort.TryParse(command[2], out ushort ss))
                        {
                            config.SkillConfig.ShieldSeconds = ss;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Shield_Seconds", ss));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "firedamage":
                    case "fd":
                        if (byte.TryParse(command[2], out byte fd))
                        {
                            config.SkillConfig.FireDamage = fd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fire_Damage", fd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "shielddamage":
                    case "sd":
                        if (byte.TryParse(command[2], out byte sd))
                        {
                            config.SkillConfig.ShieldDamage = sd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Shield_Damage", sd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "virusdamage":
                    case "vd":
                        if (byte.TryParse(command[2], out byte vd))
                        {
                            config.SkillConfig.VirusDamage = vd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Virus_Damage", vd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "flydamage":
                    case "flyd":
                        if (byte.TryParse(command[2], out byte flyd))
                        {
                            config.SkillConfig.FlyDamage = flyd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fly_Damage", flyd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "explosiondamage":
                    case "ed":
                        if (ushort.TryParse(command[2], out ushort ed))
                        {
                            config.SkillConfig.ExplosionDamage = ed;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Explosion_Damage", ed));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "baptismdamage":
                    case "bd":
                        if (byte.TryParse(command[2], out byte bd))
                        {
                            config.SkillConfig.BaptismDamage = bd;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Baptism_Damage", bd));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    case "healamount":
                    case "ha":
                        if (uint.TryParse(command[2], out uint ha))
                        {
                            config.SkillConfig.HealAmount = ha;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Heal_Amount", ha));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    default:
                        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                }
                //if(tag == "bosshealth" || tag == "bh")
                //{
                //    if (uint.TryParse(command[2], out uint health))
                //    {
                //        config.Region.BossHealth = health;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Health", health));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if(tag == "maxminions" || tag == "mm")
                //{
                //    if (byte.TryParse(command[2], out byte maxMinions))
                //    {
                //        config.Region.MaxMinions = maxMinions;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Max_Minions", maxMinions));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if(tag == "bosstable" || tag == "bt")
                //{
                //    config.Region.BossTable = command[2];
                //    Yut.Instance.Configuration.Save();
                //    UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Table", command[2]));
                //}
                //else if (tag == "miniontable" || tag == "mt")
                //{
                //    config.Region.MinionTable = command[2];
                //    Yut.Instance.Configuration.Save();
                //    UnturnedChat.Say(caller, Yut.Instance.Translate("New_Minion_Table", command[2]));
                //}
                //else if(tag == "bossname" || tag == "bn")
                //{
                //    config.StateConfig.BossName = command[2];
                //    Yut.Instance.Configuration.Save();
                //    UnturnedChat.Say(caller, Yut.Instance.Translate("New_Boss_Name", command[2]));
                //}
                //else if (tag == "prepareseconds" || tag == "ps")
                //{
                //    if (ushort.TryParse(command[2], out ushort ps))
                //    {
                //        config.StateConfig.PrepareSeconds = ps;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Prepare_Seconds", ps));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "fightingseconds" || tag == "fs")
                //{
                //    if (ushort.TryParse(command[2], out ushort fs))
                //    {
                //        config.StateConfig.FightingSeconds = fs;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fighting_Seconds", fs));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "rewardseconds" || tag == "rs")
                //{
                //    if (ushort.TryParse(command[2], out ushort rs))
                //    {
                //        config.StateConfig.RewardSeconds = rs;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Reward_Seconds", rs));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "minrewarddamage" || tag == "mrd")
                //{
                //    if (uint.TryParse(command[2], out uint mrd))
                //    {
                //        config.StateConfig.MinRewardDamage = mrd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Min_Reward_Damage", mrd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "skillrefreshseconds" || tag == "srs")
                //{
                //    if (float.TryParse(command[2], out float srs))
                //    {
                //        config.StateConfig.SkillRefreshSeconds = srs;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Skill_Refresh_Seconds", srs));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "maxplayers" || tag == "mp")
                //{
                //    if (byte.TryParse(command[2], out byte mp))
                //    {
                //        config.StateConfig.MaxPlayers = mp;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Max_Players", mp));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "shieldseconds" || tag == "ss")
                //{
                //    if (ushort.TryParse(command[2], out ushort ss))
                //    {
                //        config.SkillConfig.ShieldSeconds = ss;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Shield_Seconds", ss));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "firedamage" || tag == "fd")
                //{
                //    if (byte.TryParse(command[2], out byte fd))
                //    {
                //        config.SkillConfig.FireDamage = fd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fire_Damage", fd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "shielddamage" || tag == "sd")
                //{
                //    if (byte.TryParse(command[2], out byte sd))
                //    {
                //        config.SkillConfig.ShieldDamage = sd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Shield_Damage", sd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "virusdamage" || tag == "vd")
                //{
                //    if (byte.TryParse(command[2], out byte vd))
                //    {
                //        config.SkillConfig.VirusDamage = vd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Virus_Damage", vd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "flydamage" || tag == "flyd")
                //{
                //    if (byte.TryParse(command[2], out byte flyd))
                //    {
                //        config.SkillConfig.FlyDamage = flyd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Fly_Damage", flyd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "explosiondamage" || tag == "ed")
                //{
                //    if (ushort.TryParse(command[2], out ushort ed))
                //    {
                //        config.SkillConfig.ExplosionDamage = ed;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Explosion_Damage", ed));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "baptismdamage" || tag == "bd")
                //{
                //    if (byte.TryParse(command[2], out byte bd))
                //    {
                //        config.SkillConfig.BaptismDamage = bd;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Baptism_Damage", bd));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else if (tag == "healamount" || tag == "ha")
                //{
                //    if (uint.TryParse(command[2], out uint ha))
                //    {
                //        config.SkillConfig.HealAmount = ha;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Heal_Amount", ha));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else
                //    UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
            }
            else if(command.Length == 4)
            {
                string tag = command[1].ToLower();
                switch (tag)
                {
                    case "skill":
                    case "s":
                        SkillPair pair = config.SkillConfig.SkillPairs.Find(x => x.Key.ToLower() == command[2]);
                        if (pair == null)
                        {
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Skill_Not_Found"));
                            return;
                        }
                        if (float.TryParse(command[3], out float range))
                        {
                            pair.SkillRange = range;
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Skill_Range", command[2], command[3]));
                        }
                        break;
                    case "ticket":
                    case "t":
                        ushort? id = command.GetUInt16Parameter(2);
                        byte? count = command.GetByteParameter(3);
                        if (id.HasValue && count.HasValue)
                        {
                            config.StateConfig.Ticket = new ItemPair(id.Value, count.Value, 100);
                            Yut.Instance.Configuration.Save();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("New_Ticket", id.Value, count.Value));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                    default:
                        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                        break;
                }
                //if (tag == "skill" || tag == "s")
                //{
                //    SkillPair pair = config.SkillConfig.SkillPairs.Find(x => x.Key.ToLower() == command[2]);
                //    if(pair == null)
                //    {
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Skill_Not_Found"));
                //        return;
                //    }
                //    if(float.TryParse(command[3], out float range))
                //    {
                //        pair.SkillRange = range;
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Skill_Range", command[2], command[3]));
                //    }
                //}
                //else if (tag == "ticket" || tag == "t")
                //{
                //    ushort? id = command.GetUInt16Parameter(2);
                //    byte? count = command.GetByteParameter(3);
                //    if (id.HasValue && count.HasValue)
                //    {
                //        config.StateConfig.Ticket = new ItemPair(id.Value, count.Value, 100);
                //        Yut.Instance.Configuration.Save();
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("New_Ticket", id.Value, count.Value));
                //    }
                //    else
                //        UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                //}
                //else
                //    UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
            }
            else
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
        }
    }
}
