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
    public class CommandWBTI : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "wbti";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 2)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                return;
            }
            byte? hour = command.GetByteParameter(0);
            byte? minute = command.GetByteParameter(1);
            if(hour.HasValue && minute.HasValue)
            {
                Yut.Instance.Configuration.Instance.RefreshHour = hour.Value;
                Yut.Instance.Configuration.Instance.RefreshMinute = minute.Value;
                Yut.Instance.Configuration.Save();
                BossManager.Instance.InitNextRefreshTime();
                UnturnedChat.Say(caller, Yut.Instance.Translate("Set_Time", hour.Value, minute.Value));
            }
            else
                UnturnedChat.Say(caller, Yut.Instance.Translate("Invalid_Parameter"));
        }
    }
}
