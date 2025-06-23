using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Units
{
    public class KnightMono : UnitMono
    {
        [Header("Knight Specific")] 
        public float attackDistance = 1f;
        public float attackCooldown = 1f;

        protected override void Start()
        {
            base.Start();
            currentHealth = sceneData.KnightMaxHealth;
            damage = sceneData.KnightDamage;
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.KnightIdle);
            MeleeAttackManagerMono.Instance?.RegisterKnight(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MeleeAttackManagerMono.Instance?.UnregisterKnight(this);
        }
    }
}
