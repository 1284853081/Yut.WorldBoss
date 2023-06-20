using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API.Extensions;

namespace Yut.WorldBoss
{
    internal class CommandWBTI : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "wbti";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if(command.Length == 0)
            {
                foreach (var time in Yut.Instance.Configuration.Instance.Refreshs)
                    UnturnedChat.Say(caller, time.ToString());
                return;
            }
            else if(command.Length == 4)
            {
                if(command[0] == "add" || command[0] == "a")
                {
                    byte? hour = command.GetByteParameter(1);
                    byte? minute = command.GetByteParameter(2);
                    if (hour.HasValue && minute.HasValue)
                    {
                        Refresh time = new Refresh(hour.Value, minute.Value, command[3]);
                        bool flag = false;
                        for(int i = 0; i< Yut.Instance.Configuration.Instance.Refreshs.Count;i++)
                        {
                            if(time < Yut.Instance.Configuration.Instance.Refreshs[i])
                            {
                                Yut.Instance.Configuration.Instance.Refreshs.Insert(i, time);
                                flag = true;
                                break;
                            }
                        }
                        if(!flag)
                            Yut.Instance.Configuration.Instance.Refreshs.Add(time);
                        Yut.Instance.Configuration.Save();
                        GameStateManager.Instance.InitNextRefreshTime();
                        UnturnedChat.Say(caller, Yut.Instance.Translate("Set_Time", hour.Value, minute.Value));
                    }
                    else
                        UnturnedChat.Say(caller, Yut.Instance.Translate("Invalid_Parameter"));
                }
                else if(command[0] == "remove" || command[0] == "r")
                {
                    byte? hour = command.GetByteParameter(1);
                    byte? minute = command.GetByteParameter(2);
                    if (hour.HasValue && minute.HasValue)
                    {
                        Refresh refresh = Yut.Instance.Configuration.Instance.Refreshs.Find(x => x.Hour == hour.Value && x.Minute == minute.Value);
                        if (refresh != null)
                        {
                            Yut.Instance.Configuration.Instance.Refreshs.Remove(refresh);
                            Yut.Instance.Configuration.Save();
                            GameStateManager.Instance.InitNextRefreshTime();
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Remove_Time", hour.Value, minute.Value));
                        }
                        else
                            UnturnedChat.Say(caller, Yut.Instance.Translate("Time_Not_Find"));
                    }
                    else
                        UnturnedChat.Say(caller, Yut.Instance.Translate("Invalid_Parameter"));
                }
                else
                {
                    UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                    return;
                }
            }
            else
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                return;
            }
        }
    }
}
