using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace TerraBend.Common.Configs {
    /// <summary>
    /// Config that handles personalization and other client-side stuff.
    /// </summary>
    [Label("$Mods.TerraBend.Configs.ClientSide.ConfigName")]
    public class ClientConfig : ModConfig {
        [Header("$Mods.TerraBend.Configs.ClientSide.PreferenceSettingsHeader")]
        [Label("$Mods.TerraBend.Configs.ClientSide.PositiveJingColorLabel")]
        [Tooltip("$Mods.TerraBend.Configs.ClientSide.PositiveJingColorTooltip")]
        [DefaultValue(typeof(Color), "255, 0, 0, 255")]
        public Color positiveJingColor;

        [Label("$Mods.TerraBend.Configs.ClientSide.NeutralJingColorLabel")]
        [Tooltip("$Mods.TerraBend.Configs.ClientSide.NeutralJingColorTooltip")]
        [DefaultValue(typeof(Color), "255, 0, 255, 255")]
        public Color neutralJingColor;

        [Label("$Mods.TerraBend.Configs.ClientSide.NegativeJingColorLabel")]
        [Tooltip("$Mods.TerraBend.Configs.ClientSide.NegativeJingColorTooltip")]
        [DefaultValue(typeof(Color), "0, 0, 255, 255")]
        public Color negativeJingColor;

        [Label("$Mods.TerraBend.Configs.ClientSide.UnalignedJingColorLabel")]
        [Tooltip("$Mods.TerraBend.Configs.ClientSide.UnalignedJingColorTooltip")]
        [DefaultValue(typeof(Color), "255, 255, 255, 255")]
        public Color unalignedJingColor;

        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}