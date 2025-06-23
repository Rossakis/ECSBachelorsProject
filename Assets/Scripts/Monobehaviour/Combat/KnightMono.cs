
using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class KnightMono : UnitMono
    {
        [Header("Knight Specific")] 
        public float attackDistance = 1f;
        public float attackCooldown = 1f;

        protected override void Awake()
        {
            base.Awake();
            MeleeAttackManagerMono.Instance?.RegisterKnight(this);
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.KnightIdle);
        }

        protected override void Start()
        {
            base.Awake();
            currentHealth = sceneData.KnightMaxHealth;
            damage = sceneData.KnightDamage;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MeleeAttackManagerMono.Instance?.UnregisterKnight(this);
        }
    }
}
