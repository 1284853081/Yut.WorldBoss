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
    internal class CommandWBJ : IRocketCommand
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
            if(GameStateManager.Instance.State != EState.Preparing)
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate("Cant_Join"));
                return;
            }
            UnturnedPlayer player = caller as UnturnedPlayer;
            ItemPair ticket = GameStateManager.Instance.ModeConfig.StateConfig.Ticket;
            List<InventorySearch> searches = player.Inventory.search(ticket.Id, true, true);
            if(searches.Count < ticket.Count)
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
            for (int i = 0;i < ticket.Count;i++)
            {
                player.Inventory.removeItem(searches[i].page, player.Inventory.getIndex(searches[i].page, searches[i].jar.x, searches[i].jar.y));
            }
            UnturnedChat.Say(caller, Yut.Instance.Translate("Join_Success"));
        }
    }
}
