using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace EveryoneAsOne
{
    public class ModConfig_EveryoneAsOne : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static ModConfig_EveryoneAsOne Self;

        public static ModConfig_EveryoneAsOne Instance
        {
            get
            {
                if (Self == null)
                {
                    Self = ModContent.GetInstance<ModConfig_EveryoneAsOne>();
                }
                return Self;
            }
        }
        
        // The things in brackets are known as "Attributes".

        [Header("Swap")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
        // [Label("$Some.Key")] // A label is the text displayed next to the option. This should usually be a short description of what it does. By default all ModConfig fields and properties have an automatic label translation key, but modders can specify a specific translation key.
        // [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.
        [DefaultValue(3600)] // This sets the configs default value.
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public int ticksPerSwap; // To see the implementation of this option, see ExampleWings.cs

        [DefaultValue(true)]
        [ReloadRequired] 
        public bool copyBuffs;
    }
}