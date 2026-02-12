using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace EveryoneAsOne
{
	public class ModSystem_EveryoneAsOne : ModSystem
	{
		public static float updates;
		protected static ModPlayer MainPlayer
		{
			get
			{
				return PlayerHelper.MainPlayer;
			}
		}
		
		public override void Load()
		{
			//Terraria.UI.On_ItemSlot.LeftClick_ItemArray_int_int += LeftClick_ItemArray_int_int;
			//Terraria.UI.On_ItemSlot.RightClick_ItemArray_int_int += RightClick_ItemArray_int_int;
			/*
			Terraria.UI.On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			Terraria.UI.On_ItemSlot.MouseHover_ItemArray_int_int += MouseHover_ItemArray_int_int;
			*/
		}

		public static void ResetSwap()
		{
			updates = 0.0f;
		}

		public override void PreUpdateWorld()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				return;
			}
			updates++;
			var ticksPerSwap = ModConfig_EveryoneAsOne.Instance.ticksPerSwap;
			if (updates >= ticksPerSwap)
			{
				updates -= ticksPerSwap;
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)3);
				packet.Send();
			}
		}

		protected void LeftClick_ItemArray_int_int(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || Main.keyState.IsKeyDown(Main.FavoriteKey))
			{
				return;
			}
			if (!Main.mouseLeft || !Main.mouseLeftRelease)
			{
				return;
			}
			if (MainPlayer != null && MainPlayer.Player.inventory != inv)
			{
				var origPlayer = Main.myPlayer;
				Main.myPlayer = MainPlayer.Player.whoAmI;
				ContextToInv_LeftClick(orig, MainPlayer.Player, Main.player[origPlayer], inv, context, slot);
				Main.myPlayer = origPlayer;
				return;
			}
			orig(inv, context, slot);
		}
		
		private void RightClick_ItemArray_int_int(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || Main.keyState.IsKeyDown(Main.FavoriteKey))
			{
				return;
			}
			if (!Main.mouseRight || !Main.mouseRightRelease)
			{
				return;
			}
			if (MainPlayer != null && MainPlayer.Player.inventory != inv)
			{
				var origPlayer = Main.myPlayer;
				Main.myPlayer = MainPlayer.Player.whoAmI;
				ContextToInv_RightClick(orig, MainPlayer.Player, Main.player[origPlayer], inv, context, slot);
				Main.myPlayer = origPlayer;
				return;
			}
			orig(inv, context, slot);
		}

		public static void OnLeftClick(int context, int slot, int whoAmI)
		{
			if (Main.LocalPlayer == MainPlayer.Player)
			{
				var mouseLeft = Main.mouseLeft;
				var mouseLeftRelease = Main.mouseLeftRelease;
				Main.mouseLeft = true;
				Main.mouseLeftRelease = true;
				PlayerHelper.SwapMouseItem(whoAmI);
				ItemSlot.LeftClick(MainPlayer.Player.inventory, context, slot);
				PlayerHelper.SwapMouseItem(whoAmI);
				Main.mouseLeft = mouseLeft;
				Main.mouseLeftRelease = mouseLeftRelease;
				
			}
		}
		
		public static void OnRightClick(int context, int slot, int whoAmI)
		{
			if (Main.LocalPlayer == MainPlayer.Player)
			{
				var mouseRight = Main.mouseRight;
				var mouseRightRelease = Main.mouseRightRelease;
				Main.mouseRight = true;
				Main.mouseRightRelease = true;
				PlayerHelper.SwapMouseItem(whoAmI);
				ItemSlot.RightClick(MainPlayer.Player.inventory, context, slot);
				PlayerHelper.SwapMouseItem(whoAmI);
				Main.mouseRight = mouseRight;
				Main.mouseRightRelease = mouseRightRelease;
				
			}
		}

		protected void ContextToInv_LeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Player mainPlayer, Player origPlayer, Item[] inv, int context, int slot)
		{
			bool sync;
			bool client;
			Item[] invUse;
			ContextToInv(mainPlayer, origPlayer, inv, context, slot, out sync, out client, out invUse);
			if (sync)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)1);
				packet.Write((byte)context);
				packet.Write((byte)slot);
				packet.Send();
			}
			if (client)
			{
				orig(invUse, context, slot);
			}
		}
		
		protected void ContextToInv_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Player mainPlayer, Player origPlayer, Item[] inv, int context, int slot)
		{
			bool sync;
			bool client;
			Item[] invUse;
			ContextToInv(mainPlayer, origPlayer, inv, context, slot, out sync, out client, out invUse);
			if (sync)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)2);
				packet.Write((byte)context);
				packet.Write((byte)slot);
				packet.Send();
			}
			if (client)
			{
				orig(invUse, context, slot);
			}
		}
		
		protected void ContextToInv(Player mainPlayer, Player origPlayer, Item[] inv, int context, int slot, out bool sync, out bool client, out Item[] invUse)
		{
			sync = true;
			client = false;
			invUse = inv;
			if (context == ItemSlot.Context.InventoryItem)
			{
				invUse = mainPlayer.inventory;
			}
			else if (context == ItemSlot.Context.EquipArmor)
			{
				invUse = mainPlayer.armor;
			}
			else if (context == ItemSlot.Context.EquipArmorVanity)
			{
				invUse = mainPlayer.armor;
			}
			else if (context == ItemSlot.Context.EquipAccessory)
			{
				invUse = mainPlayer.armor;
			}
			else if (context == ItemSlot.Context.EquipAccessoryVanity)
			{
				invUse = mainPlayer.armor;
			}
			else if (context == ItemSlot.Context.EquipDye)
			{
				invUse = mainPlayer.dye;
			}
			else if (context == ItemSlot.Context.EquipMiscDye)
			{
				invUse = mainPlayer.miscDyes;
			}
			else if (context == ItemSlot.Context.InventoryCoin)
			{
				invUse = mainPlayer.inventory;
			}
			else if (context == ItemSlot.Context.InventoryAmmo)
			{
				invUse = mainPlayer.inventory;
			}
			else if (context == ItemSlot.Context.EquipGrapple)
			{
				invUse = mainPlayer.miscEquips;
			}
			else if (context == ItemSlot.Context.EquipMinecart)
			{
				invUse = mainPlayer.miscEquips;
			}
			else if (context == ItemSlot.Context.EquipMount)
			{
				invUse = mainPlayer.miscEquips;
			}
			else if (context == ItemSlot.Context.EquipPet)
			{
				invUse = mainPlayer.miscEquips;
			}
			else if (context == ItemSlot.Context.EquipLight)
			{
				invUse = mainPlayer.miscEquips;
			}
			else if (context == ItemSlot.Context.VoidItem)
			{
				invUse = mainPlayer.bank4.item;
			}
			else if (context == ItemSlot.Context.TrashItem)
			{
				invUse = [mainPlayer.trashItem];
			}
			else if (context == ItemSlot.Context.MouseItem)
			{
				invUse = [Main.mouseItem];
				client = true;
			}
			else if (context == ItemSlot.Context.BankItem)
			{
				if (origPlayer.chest == -2)
				{
					invUse = mainPlayer.bank.item;
				}
				else if (origPlayer.chest == -3)
				{
					invUse = mainPlayer.bank2.item;
				}
				else if (origPlayer.chest == -4)
				{
					invUse = mainPlayer.bank3.item;
				}
			}
			else if (context == ItemSlot.Context.ChestItem)
			{
				invUse = Main.chest[origPlayer.chest].item;
			}
			else
			{
				sync = false;
				client = true;
			}
		}
		
		/*
		 
		protected void MouseHover_ItemArray_int_int(On_ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (MainPlayer != null && MainPlayer.Player.inventory != inv)
			{
				var origPlayer = Main.myPlayer;
				Main.myPlayer = MainPlayer.Player.whoAmI;
				orig(ContextToInv(MainPlayer.Player, Main.player[origPlayer], context), context, slot);
				Main.myPlayer = origPlayer;
				return;
			}
			orig(inv, context, slot);
		}
		
		protected void Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			if (MainPlayer != null && MainPlayer.Player.inventory != inv)
			{
				var origPlayer = Main.myPlayer;
				Main.myPlayer = MainPlayer.Player.whoAmI;
				orig(spriteBatch, ContextToInv(MainPlayer.Player, Main.player[origPlayer], context), context, slot, position, lightColor);
				Main.myPlayer = origPlayer;
				return;
			}
			orig(spriteBatch, inv, context, slot, position, lightColor);
		}
		
		
		*/
	}
}
