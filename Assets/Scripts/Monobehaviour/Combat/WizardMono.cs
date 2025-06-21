using Assets.Scripts.Monobehaviour.Combat.Assets.Scripts.Monobehaviour.Combat;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class WizardMono : UnitMono
    {
        public GameObject fireballPrefab;

        void Start()
        {
        
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
