using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.WorldBoss
{
    public class CommandWBP : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "wbp";

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
        }
    }
}
