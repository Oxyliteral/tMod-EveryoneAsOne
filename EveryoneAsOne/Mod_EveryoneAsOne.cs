using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EveryoneAsOne
{
	public class Mod_EveryoneAsOne : Mod
	{
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte type = reader.ReadByte();
			if (Main.netMode == NetmodeID.Server)
			{
				if (type == 0)
				{
					var player = reader.ReadByte();
					ModPacket packet = GetPacket();
					packet.Write((byte)0);
					packet.Write((byte)player);
					packet.Send();
					ModSystem_EveryoneAsOne.ResetSwap();
				}
				else if (type == 1 || type == 2)
				{
					var context = reader.ReadByte();
					var slot = reader.ReadByte();
					var player = reader.ReadByte();
					ModPacket packet = GetPacket();
					packet.Write((byte)type);
					packet.Write((byte)context);
					packet.Write((byte)slot);
					packet.Write((byte)player);
					packet.Send();
				}
			}
			else if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (type == 0)
				{
					var player = reader.ReadByte();
					PlayerHelper.SyncPlayers(player);
				}
				else if (type == 1 || type == 2)
				{
					var context = reader.ReadByte();
					var slot = reader.ReadByte();
					var player = reader.ReadByte();
					if (type == 1)
					{
						ModSystem_EveryoneAsOne.OnLeftClick(context, slot, player);
					}
					else
					{
						ModSystem_EveryoneAsOne.OnRightClick(context, slot, player);
					}
				}
				else if (type == 3)
				{
					if (PlayerHelper.MainPlayer == null)
						return;
					PlayerHelper.SwitchPlayer();
					Main.NewText("Swapping to Player: " + PlayerHelper.CurrentPlayer.Player.name, 0, 255, 0);
				}
			}
		}
	}
}
