
using Assets.Scripts.ScriptableObjects.Animation;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class KnightMono : UnitMono
    {
        void Start()
        {
            currentHealth = sceneData.KnightMaxHealth;
            damage = sceneData.KnightDamage;
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.KnightIdle);
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
