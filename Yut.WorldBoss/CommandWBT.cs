using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    public class CommandWBT : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "wbt";
        public string Help => "";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 0)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                return;
            }
            if (BossManager.Instance.State != EState.Fighting && BossManager.Instance.State != EState.Preparing)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Not_Start_Fighting"));
                return;
            }
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (!PlayerManager.Instance.HasSign(player))
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Not_Sign"));
                return;
            }
            player.Teleport(Yut.Instance.Configuration.Instance.BossRefreshPoint + new UnityEngine.Vector3(30, 0, 0), 0);
        }
    }
}
