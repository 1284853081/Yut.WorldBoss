using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.RewardModule
{
    public static class PlayerRewards
    {
        public static void SendRewards(PlayerInventory inventory,List<ItemPair> items)
        {
            Items a = new Items(PlayerInventory.STORAGE);
            a.loadSize(10, 20);
            for(int i = 0; i < items.Count; i++)
            {
                a.tryAddItem(new Item(items[i].Id, items[i].Count, 100));
            }
            inventory.updateItems(PlayerInventory.STORAGE, a);
            inventory.sendStorage();
        }
    }
}
