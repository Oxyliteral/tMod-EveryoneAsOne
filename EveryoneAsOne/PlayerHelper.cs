using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EveryoneAsOne;

public class PlayerHelper
{

    public static List<int> Players = new List<int>();

    public static int PlayerIndex;

    public static ModPlayer CurrentPlayer;

    public static Dictionary<int, Item> PlayerToItem = new Dictionary<int, Item>();
    
    public static ModPlayer MainPlayer
    {
        get
        {
            return CurrentPlayer;
        }
    }

    public static void SwapMouseItem(int player)
    {
        Item item;
        if (PlayerToItem.TryGetValue(player, out item))
        {
            var item2 = Main.mouseItem;
            PlayerToItem[player] = item2;
            Main.mouseItem = item;
        }
    }

    public static void SyncPlayers()
    {
        ResetPlayers();
        UpdatePlayer();
    }

    public static void SyncPlayers(int playerLast)
    {
        ResetPlayers();
        if (Players.Contains(playerLast))
        {
            Players.Remove(playerLast);
            Players.Add(playerLast);
        }
        UpdatePlayer();
    }

    public static void ResetPlayers()
    {
        Players.Clear();
        List<Player> players = new List<Player>();
        foreach (var player in Main.ActivePlayers)
            players.Add(player);
        if (players.Count <= 1)
        {
            return;
        }
        players.Sort((x, y) => String.CompareOrdinal(x.name, y.name));
        foreach (var player in players)
        {
            Players.Add(player.whoAmI);
        }
        PlayerToItem.Clear();
        foreach (var player in Players)
        {
            PlayerToItem.Add(player, new Item());
        }
        PlayerIndex = 0;
    }
    
    public static void SwitchPlayer()
    {
        PlayerIndex += 1;
        PlayerIndex %= Players.Count;
        UpdatePlayer();
    }

    public static void UpdatePlayer()
    {
        if (PlayerIndex >= Players.Count)
        {
            CurrentPlayer = null;
            return;
        }
        CurrentPlayer = Main.player[Players[PlayerIndex]].GetModPlayer<ModPlayer_EveryoneAsOne>();
    }
}