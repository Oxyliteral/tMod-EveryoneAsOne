using Terraria;
using Terraria.ModLoader;

namespace EveryoneAsOne;

public class GlobalItem_EveryoneAsOne : GlobalItem
{
    protected static ModPlayer MainPlayer
    {
        get
        {
            return PlayerHelper.MainPlayer;
        }
    }

    public override bool CanPickup(Item item, Player player)
    {
        if (MainPlayer != null && MainPlayer.Player != player)
        {
            return false;
        }
        return base.CanPickup(item, player);
    }
}