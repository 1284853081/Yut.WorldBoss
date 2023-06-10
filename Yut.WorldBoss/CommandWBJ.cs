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
    public class CommandWBJ : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "wbj";

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
            if(BossManager.Instance.State != EState.Preparing)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Cant_Join"));
                return;
            }
            UnturnedPlayer player = caller as UnturnedPlayer;
            List<InventorySearch> searches = player.Inventory.search(Yut.Instance.Configuration.Instance.Ticket.Id, true, true);
            if(searches.Count < Yut.Instance.Configuration.Instance.Ticket.Count)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("No_Tickets"));
                return;
            }
            bool flag = PlayerManager.Instance.Sign(player);
            if(!flag)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Repeat_Join"));
                return;
            }
            for (int i = 0;i < Yut.Instance.Configuration.Instance.Ticket.Count;i++)
            {
                player.Inventory.removeItem(searches[i].page, player.Inventory.getIndex(searches[i].page, searches[i].jar.x, searches[i].jar.y));
            }
            PlayerManager.Instance.OpenUI(player.CSteamID);
            UnturnedChat.Say(caller, Yut.Instance.Translate("Join_Success"));
        }
    }
}
