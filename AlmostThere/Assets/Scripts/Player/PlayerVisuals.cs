using Framework;

namespace Player
{
    public sealed class PlayerVisuals : Singleton<PlayerVisuals>
    {
        protected override void Awake()
        {
            base.Awake();
            CanDestroyOnLoad = true;
        }
    }
}