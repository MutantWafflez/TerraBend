using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TerraBend.Custom.Enums;
using Terraria.ModLoader;

namespace TerraBend.Common.Players {
    /// <summary>
    /// ModPlayer that handles the "Jing" resource, which is essentially the resource that determines critical
    /// strikes and other rare circumstances with bending moves.
    /// </summary>
    /// <remarks>
    /// "Jing" ("power"/"energy" in Chinese), as explained in the actual ATLA universe, is the "methodology" of
    /// how certain benders generally act and "channel their energy" in combat, which we can translate to gameplay.
    ///
    /// <para></para>
    /// Firstly, "Positive Jing", attributed commonly to Firebenders, is an offensive and agressive expression of Chi,
    /// which favors overwhelming an opponent with force. Secondly, "Negative Jing", commonly attributed to Airbenders,
    /// is a more evasive and mobile expression of Chi; attacking only in defense. Finally (for the purposes of gameplay)
    /// there is "Neutral Jing", which is (almost) exclusively attributed to Earthbenders; it is a heavily passive form
    /// of Chi expression, where the bender "waits and listens", letting their opponent come to them and striking at the
    /// key time. There is also Unaligned Jing, which can't be generated, and is not canon within the actual ATLA universe.
    ///
    /// <para></para>
    /// Quick Note: No Jing is purely attributed to one type of bending; usually it will be COMMONLY attributed
    /// to a certain bending type, but they are not mutually exclusive.
    /// 
    /// <para>Technical/Verbose Gameplay Explanation from here down:</para>
    /// As for gameplay, this is where it gets a bit more technical, so read carefully. Jing, as a "resource", is a system
    /// based upon a ratio of four types; Positive (Attacking), Neutral (Parrying/Waiting), Negative (Evading), and Unaligned
    /// (Doing Nothing). By default, all the Jing values must add up to equal the "Max Jing" value, which (as of now) is
    /// equivalent to 300% (3x) of the player's BASE max chi value (<seealso cref="ChiPlayer.baseMaxChi"/>).
    ///
    /// <para></para>
    /// Under default circumstances (default circumstances being defined as the player has just spawned in, and has not
    /// attacked/moved/done ANYTHING at all), the player has 100% Unaligned Jing. Depending on the bending move used, Jing will change;
    /// if the move generates Positive Jing (it will say so on the bending move's tooltip: what Jing it produces and how much), however
    /// much Positive Jing is generated will be taken from the Unaligned Jing and replaced with said Positive Jing (for example, if a move
    /// generates 25 Positive Jing, 25 Unaligned Jing will be eaten and replaced). This functions the exact same way with Negative/Neutral Jing
    /// (i.e move generates 25 Negative/Neutral Jing, 25 Unaligned Jing will be eaten and replaced). However, if a move generates Neutral Jing, and
    /// there is no Unaligned Jing, the other Jing type with the highest value will be consumed and replaced (i.e if the current Jing status
    /// is 100/200 Positive, 50/200 Neutral, and 50/200 Negative, and the move generates 25 Neutral Jing, 100 Positive is the greatest value,
    /// thus 25 Positive Jing will be consumed and replaced with Neutral Jing, making it 75/200 Positive, 75/200 Neutral, 50/200 Negative).
    /// If a move generates Positive Jing and there is no Unaligned Jing left over, Neutral Jing is prioritized for consumption, and if there is
    /// no Neutral Jing, it consumes Negative Jing. Works the exact same way vice versa for Negative Jing; if no Unaligned Jing, consume Neutral,
    /// if no Neutral, consume Positive.
    ///
    /// <para></para>
    /// Now, at this point, one is likely to ask "Okay, so what is the point of all this?" Basically; it all depends on the current majority
    /// Jing type. At any point, the player is capable of a "Jing Purge", which will consume all of the majority Jing type and replace it with
    /// Unaligned Jing, and have a certain effect depending on what that Jing type is. For a Positive Jing Purge, all moves that generate positive
    /// Jing will have that move's potency amplified; it can differ based on the move, but generally "more potent" means guaranteed critical
    /// hits for a short time. For a Negative Jing Purge, the player will gain damage reduction equivalent to how dominant Negative Jing is.
    /// For a Neutral Jing Purge, it depends entirely on the next Neutral Jing move used. Finally, there is a True Jing Purge, which requires
    /// an equal ratio of Positive, Neutral, and Negative Jing; it will trigger all the effects of each Jing Purge for twice the duration,
    /// and ALL Jing will be consumed and converted into Unaligned Jing.
    ///
    /// <para></para>
    /// Finally, there are some moves that have conditions based on Jing to do things. For example, to Lightningbend, the closer one is to balance
    /// of Positive and Negative Jing, the less chance there is for the bending to backfire. 
    /// </remarks>
    public class JingPlayer : ModPlayer {
        /// <summary>
        /// Gets and returns the current Positive Jing value.
        /// </summary>
        public int PositiveJing => _positiveJing;

        /// <summary>
        /// Gets and returns the current Neutral Jing value.
        /// </summary>
        public int NeutralJing => _neutralJing;

        /// <summary>
        /// Gets and returns the current Negative Jing value.
        /// </summary>
        public int NegativeJing => _negativeJing;

        /// <summary>
        /// The "amount" of Unaligned Jing current within the player's system. Remember that
        /// Unaligned Jing has no effects whatsoever.
        /// </summary>
        public int UnalignedJing => MaxJing - _positiveJing - _neutralJing - _negativeJing;

        /// <summary>
        /// The "Max" amount of Jing; the absolute cap of any Jing type.
        /// </summary>
        public int MaxJing => Player.GetModPlayer<ChiPlayer>().baseMaxChi * 3;

        /// <summary>
        /// The amount of Positive Jing currently within the player's system.
        /// </summary>
        private int _positiveJing;

        /// <summary>
        /// The amount of Neutral Jing currently within the player's system.
        /// </summary>
        private int _neutralJing;

        /// <summary>
        /// The amount of Negative Jing currently within the player's system.
        /// </summary>
        private int _negativeJing;

        /// <summary>
        /// Adds the change value to Positive Jing. If the change value is negative, simply removes that much
        /// from the Positive Jing value. If positive however, first it tries to take
        /// from Unaligned Jing, then Neutral, then Negative, and if none of that works,
        /// nothing changes.
        /// </summary>
        /// <param name="changeValue"> The value to add to Positive Jing. </param>
        public void ChangePositiveJing(int changeValue) {
            if (changeValue <= 0) {
                _positiveJing += changeValue;
            }
            else if (UnalignedJing > 0) {
                _positiveJing += changeValue;
            }
            else if (_neutralJing > 0 || _negativeJing > 0) {
                ref int backupJing = ref _neutralJing > 0 ? ref _neutralJing : ref _negativeJing;
                _positiveJing += changeValue;
                backupJing -= changeValue;
            }

            _positiveJing = (int)MathHelper.Clamp(_positiveJing, 0f, MaxJing);
        }

        /// <summary>
        /// Adds the change value to Neutral Jing. If the change value is negative, simply removes that much
        /// from the Neutral Jing value. If positive however, first it tries to take
        /// from Unaligned Jing, then either Positive or Negative, depending on which is bigger.
        /// If none of that applies, nothing happens.
        /// </summary>
        /// <param name="changeValue"> The value to add to Neutral Jing. </param>
        public void ChangeNeutralJing(int changeValue) {
            if (changeValue <= 0) {
                _neutralJing += changeValue;
            }
            else if (UnalignedJing > 0) {
                _neutralJing += changeValue;
            }
            else if (_positiveJing > 0 || _negativeJing > 0) {
                ref int backupJing = ref _positiveJing >= _negativeJing ? ref _positiveJing : ref _negativeJing;
                _neutralJing += changeValue;
                backupJing -= changeValue;
            }

            _neutralJing = (int)MathHelper.Clamp(_neutralJing, 0f, MaxJing);
        }

        /// <summary>
        /// Adds the change value to Negative Jing. If the change value is negative, simply removes that much
        /// from the Negative Jing value. If positive however, first it tries to take
        /// from Unaligned Jing, then Neutral, then Positive, and if none of that works,
        /// nothing changes.
        /// </summary>
        /// <param name="changeValue"> The value to add to Negative Jing. </param>
        public void ChangeNegativeJing(int changeValue) {
            if (changeValue <= 0) {
                _negativeJing += changeValue;
            }
            else if (UnalignedJing > 0) {
                _negativeJing += changeValue;
            }
            else if (_neutralJing > 0 || _positiveJing > 0) {
                ref int backupJing = ref _neutralJing > 0 ? ref _neutralJing : ref _positiveJing;
                _negativeJing += changeValue;
                backupJing -= changeValue;
            }

            _negativeJing = (int)MathHelper.Clamp(_negativeJing, 0f, MaxJing);
        }

        /// <summary>
        /// Calculates and returns the current "Majority Type" of Jing.
        /// </summary>
        public JingType GetMajorityJing() {
            if (_positiveJing == _neutralJing && _positiveJing == _negativeJing && UnalignedJing == 0) {
                return JingType.Balanced;
            }
            else if (UnalignedJing > _positiveJing && UnalignedJing > _neutralJing && UnalignedJing > _negativeJing) {
                return JingType.Unaligned;
            }
            else {
                return new Tuple<JingType, int>[] {
                        new Tuple<JingType, int>(JingType.Positive, _positiveJing),
                        new Tuple<JingType, int>(JingType.Neutral, _neutralJing),
                        new Tuple<JingType, int>(JingType.Negative, _negativeJing)
                    }.OrderByDescending(tuple => tuple.Item2)
                     .First()
                     .Item1;
            }
        }

        public void AttemptJingPurge() {
            //TODO: Write Jing Purge
        }
    }
}