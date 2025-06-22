
using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class KnightMono : UnitMono
    {
        [Header("Knight Specific")] 
        public float meleeRange = 1f;
        
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
