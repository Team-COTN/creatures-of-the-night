using HSM;

namespace Enemies.Thwomp
{
    public class ThwompEnemy : StateMachineMonoBehaviour
    {
        protected override State CreateRootState() => new ThwompRoot(null, this);
    }
}
