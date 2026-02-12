# tMod-EveryoneAsOne

from mod description:


Ever seen those minecraft mods where everyone works together to control one singular player? Yeah, this isn't that.

It would be theoretically possible to hook into the controls, remove each other player's character, and focus on one main character. This doesn't exactly work well, though. How would you divide up the controls? Placing and attacking are the same button, and movement is already hard enough.

So what I've done is made it so that one main player gets movement control, and each other player is free to mess around with inventory and placing and attacking.

All other players' positions, velocities, inventories, armor, accessories, dyes, loadouts, hp/mana values, and consumed life crystals/fruit and mana crystals are synced to the main player's. Additionally, there is an optional config to sync buffs as well.

WARNING: Since it syncs inventories, items WILL be replaced. I recommend starting a new character.

Only the main player has collision. Only the main player can pickup items.

Due to how syncing works, the following items are disabled for non-main players:
- Life Crystals, Life Fruits, Mana Crystals
- Potions (items that give potion sickness)
- Buffs (if buff sync is enabled)

Currently, the main player swaps every minute (3600 ticks divided by 60). The order is dependent on ordinal compareTo (basically, earlier in the alphabet goes first). The one exception is the last player to join, who gets placed last in the order. When players join or leave, the main player/player swapping is synced.

Here's some more detail on the creative process:

I first started syncing which player is the main player. I had to make it deterministic for performance reasons (you would not like it if it synced every frame), so it only gets set when players join/leave. The sorting is just C#'s ordinal compareTo for each player's name. It just has to be consistent between each client.
Then I messed around with position/velocity, and control disabling. It was relatively simple. Since these things are automatically synced by terraria by default, I just had to set them to the main player's.
Here's where my issues started. I wanted to find a way to mirror without completely replacing your inventory. I went with hooking into the Draw method, which would replicate the main player's inventory, and hooking into the item slot onclick methods. I had to do some weird dancing between client and server and main focus client to send client clicks to the main client's computer to simulate those same clicks, otherwise it wouldn't sync properly. It was a whole mess.
Later I tested out setting the inventory to the main player's inventory and found that it worked flawlessly, so I gave up with this and simply used that method instead.

What I could do to solve the inventory replacement issue is to save a snapshot upon joining the world, then loading it upon leaving the world. That's for future me to implement, if I ever do.
