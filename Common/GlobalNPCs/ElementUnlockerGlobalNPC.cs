using TerraBend.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraBend.Common.GlobalNPCs {
    /// <summary>
    /// GlobalNPC that handles the triggering of element unlocking for avatar players
    /// when specific bosses are unlocked.
    /// </summary>
    //TODO: Add this functionality properly
    [Autoload(false)]
    public class ElementUnlockerGlobalNPC : GlobalNPC {
        public override void OnKill(NPC npc) {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                if (npc.type is NPCID.EyeofCthulhu or NPCID.SkeletronHead or NPCID.WallofFlesh) {
                    Main.LocalPlayer.GetModPlayer<BendingStancePlayer>().UnlockNextElementInCycle();
                }
            }
            else if (Main.netMode == NetmodeID.Server) {
                //TODO: Multiplayer compat.
            }
        }
    }
}