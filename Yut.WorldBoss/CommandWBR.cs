using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Yut.WorldBoss
{
    internal class CommandWBR : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "wbr";

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
            if (GameStateManager.Instance.State != EState.Rewarding)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Not_Start_Rreward"));
                return;
            }
            UnturnedPlayer player = caller as UnturnedPlayer;
            if (!PlayerManager.Instance.HasReward(player.CSteamID))
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("No_Rreward"));
                return;
            }
            PlayerManager.Instance.SendRewardStorage(player);
        }
    }
}
