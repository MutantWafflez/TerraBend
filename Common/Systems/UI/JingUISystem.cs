using TerraBend.Content.UI.States;

namespace TerraBend.Common.Systems.UI {
    /// <summary>
    /// UISystem that handles the Jing Resource Bar UI.
    /// </summary>
    public class JingUISystem : UISystem<JingUIState> {
        public override string InternalInterfaceName => "Jing Resource Bar";

        public override void Load() {
            base.Load();
            //Since this is a resource bar, it is always visible, so post load we make sure to set the
            //state immediately.
            correspondingInterface.SetState(correspondingUIState);
        }
    }
}