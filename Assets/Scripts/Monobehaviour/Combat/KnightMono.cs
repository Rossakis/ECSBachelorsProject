using Assets.Scripts.Monobehaviour.Combat.Assets.Scripts.Monobehaviour.Combat;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class KnightMono : UnitMono
    {
        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public override void Attack(UnitMono target)
        {
            if (target != null)
            {
                target.TakeDamage(damage);
                // Play melee attack animation, etc.
            }
        }
    }
}
