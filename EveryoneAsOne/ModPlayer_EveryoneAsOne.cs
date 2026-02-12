using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace EveryoneAsOne;

public class ModPlayer_EveryoneAsOne : ModPlayer
{

    protected static ModPlayer MainPlayer
    {
        get
        {
            return PlayerHelper.MainPlayer;
        }
    }

    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        if (MainPlayer != null && MainPlayer != this)
        {
            return true;
        }
        return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }

    public override void PreUpdate()
    {
        if (MainPlayer != null && MainPlayer != this)
        {
            var player = this.Player;
            var mainPlayer = MainPlayer.Player;
            player.direction = mainPlayer.direction;
            player.inventory = mainPlayer.inventory;
            player.armor = mainPlayer.armor;
            player.miscEquips = mainPlayer.miscEquips;
            player.extraAccessory = mainPlayer.extraAccessory;
            player.extraAccessorySlots = mainPlayer.extraAccessorySlots;
            player.dye = mainPlayer.dye;
            player.miscDyes = mainPlayer.miscDyes;
            player.equippedWings = mainPlayer.equippedWings;
            player.Loadouts = mainPlayer.Loadouts;
            player.CurrentLoadoutIndex = mainPlayer.CurrentLoadoutIndex;
            player.statLife = mainPlayer.statLife;
            player.statMana = mainPlayer.statMana;
            player.ConsumedLifeCrystals = mainPlayer.ConsumedLifeCrystals;
            player.ConsumedLifeFruit = mainPlayer.ConsumedLifeFruit;
            player.ConsumedManaCrystals = mainPlayer.ConsumedManaCrystals;
            if (mainPlayer.dead)
            {
                if (mainPlayer.respawnTimer > 60)
                {
                    player.KillMe(PlayerDeathReason.LegacyDefault(), 0.0, 0);
                }
                player.respawnTimer = mainPlayer.respawnTimer;
            }
            if (!ModConfig_EveryoneAsOne.Instance.copyBuffs)
            {
                return;
            }
            for (int i = 0; i < mainPlayer.buffType.Length; i++)
            {
                var type = mainPlayer.buffType[i];
                if (type == 0)
                {
                    continue;
                }
                var time = mainPlayer.buffTime[i];
                if (time == 0)
                {
                    continue;
                }
                var hasBuff = player.HasBuff(type);
                if (hasBuff)
                {
                    continue;
                }
                player.AddBuff(type, time);
            }
        }
    }

    public override bool CanUseItem(Item item)
    {
        if (MainPlayer != null && MainPlayer != this)
        {
            if (item.buffType != 0 && ModConfig_EveryoneAsOne.Instance.copyBuffs)
                return false;
            if (item.potion)
                return false;
            if (item.type == ItemID.LifeCrystal)
                return false;
            if (item.type == ItemID.LifeFruit)
                return false;
            if (item.type == ItemID.ManaCrystal)
                return false; 
        }
        return base.CanUseItem(item);
    }

    public override void SetControls()
    {
        if (MainPlayer != null && MainPlayer != this)
        {
            Player.controlDown = false;
            Player.controlUp = false;
            Player.controlLeft = false;
            Player.controlRight = false;
            Player.controlJump = false;
            Player.controlHook = false;
            Player.controlMount = false;
            Player.controlDownHold = false;
        }
    }

    public override void PreUpdateMovement()
    {
        if (MainPlayer != null && MainPlayer != this)
        {
            Player.position = MainPlayer.Player.position;
            Player.velocity = MainPlayer.Player.velocity;
        }
    }

    public override void PlayerDisconnect()
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        PlayerHelper.SyncPlayers();
    }

    public override void OnEnterWorld()
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)0);
        packet.Write((byte)Main.LocalPlayer.whoAmI);
        packet.Send();
    }
}