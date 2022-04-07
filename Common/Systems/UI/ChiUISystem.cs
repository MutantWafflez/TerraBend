using TerraBend.Content.UI.States;

namespace TerraBend.Common.Systems.UI {
    /// <summary>
    /// UISystem that handles the Chi Resource Bar UI.
    /// </summary>
    public class ChiUISystem : UISystem<ChiUIState> {
        public override string InternalInterfaceName => "Chi Resource Bar";

        public override void Load() {
            base.Load();
            //Since this is a resource bar, it is always visible, so post load we make sure to set the
            //state immediately.
            correspondingInterface.SetState(correspondingUIState);
        }
    }
}