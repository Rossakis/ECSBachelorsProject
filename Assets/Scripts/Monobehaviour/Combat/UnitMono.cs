using Assets.Scripts.Monobehaviour.Animation;
using Assets.Scripts.Monobehaviour.Movement;
using Assets.Scripts.ScriptableObjects.Animation;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public abstract class UnitMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;

        [Header("Unit Stats")]
        public FactionType FactionType = FactionType.Wizard;
        protected int damage;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        protected static readonly Vector3 NoTarget = new Vector3(float.NaN, float.NaN, float.NaN);
        protected Vector3 targetPos = NoTarget;

        [HideInInspector] 
        public AnimationControllerMono AnimationController;

        [Header("Target Params")]
        public UnitMono currentTarget;
        public UnitMono overrideTarget;
        public float loseTargetDistance = 20f;
        public float findTargetRange = 15f;
        public FactionType targetFaction = FactionType.Knight;
        public float findTargetInterval = 1f;
        [HideInInspector] public float findTargetTimer = 0f;

        [Header("Health")]
        public int currentHealth;
        public int maxHealth;
        public HealthState healthState = HealthState.Alive;

        public enum HealthState
        {
            Alive,
            Injured,
            Dead
        }

        protected virtual void Awake()
        {
            AnimationController = GetComponent<AnimationControllerMono>();
            TargetManagerMono.Instance?.RegisterUnit(this);
        }
        protected virtual void OnDestroy()
        {
            TargetManagerMono.Instance?.UnregisterUnit(this);
        }

        public bool HasTarget => !float.IsNaN(targetPos.x);

        public virtual void SetTargetPosition(Vector3 pos)
        {
            targetPos = pos;
        }

        private bool IsMovingAlready = false;
        public virtual void Move(AnimationDataSO.AnimationType moveAnimation, AnimationDataSO.AnimationType idleAnimation)
        {
            if (!HasTarget) 
                return;

            if (!IsMovingAlready) // Prevent multiple move calls in the same frame
            {
                AnimationManagerMono.Instance.PlayAnimationForUnits(this, moveAnimation);
                IsMovingAlready = true;
            }

            Vector3 direction = targetPos - transform.position;
            direction.y = 0f;

            //Reached target position
            if (direction.sqrMagnitude < 0.01f)
            {
                targetPos = NoTarget;
                UnitMoveManagerMono.Instance.UnregisterUnit(this);
                AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                IsMovingAlready = false;
                return;
            }

            Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;

            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void Attack(UnitMono target)
        {
        }

        public virtual void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(currentHealth - amount, 0);
            if (currentHealth <= 0)
            {
                healthState = HealthState.Dead;
                Die();
            }
            else if (currentHealth < maxHealth)
            {
                healthState = HealthState.Injured;
            }
            else
            {
                healthState = HealthState.Alive;
            }
        }
    }
}
