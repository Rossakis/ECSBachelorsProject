using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class WizardMono : UnitMono
    {
        public GameObject fireballPrefab;

        void Start()
        {
            currentHealth = sceneData.WizardMaxHealth;
            damage = sceneData.WizardDamage;
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.WizardIdle);
        }

        void Update()
        {
        
        }

        public override void Attack(UnitMono target)
        {
            // Cast fireball at target
            if (fireballPrefab != null && target != null)
            {
                // Instantiate fireball, set its target, etc.
            }
        }
    }
}
