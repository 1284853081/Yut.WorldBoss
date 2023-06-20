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
    internal class CommandWBE : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "wbe";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if(command.Length != 0)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Error_Syntax"));
                return;
            }
            if (GameStateManager.Instance.State == EState.Fighting)
            {
                UnturnedPlayer player = caller as UnturnedPlayer;
                bool flag = PlayerManager.Instance.Remove(player);
                if (flag)
                    UnturnedChat.Say(caller, Yut.Instance.Translate("Remove_Success"));
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate("Remove_Failed"));
            }
            else
                UnturnedChat.Say(caller, Yut.Instance.Translate("Not_Start_Fighting"));
        }
    }
}
