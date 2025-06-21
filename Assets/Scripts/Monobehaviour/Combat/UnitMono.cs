using Assets.Scripts.Monobehaviour.Animation;
using Assets.Scripts.ScriptableObjects.Animation;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public abstract class UnitMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;

        [Header("Unit Stats")]
        protected int damage;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public FactionType FactionType = FactionType.Wizard;

        [Header("Visuals")]
        public GameObject selectionRing;

        [Header("Runtime State")]
        public int currentHealth;

        protected static readonly Vector3 NoTarget = new Vector3(float.NaN, float.NaN, float.NaN);
        protected Vector3 targetPos = NoTarget;

        [HideInInspector] 
        public AnimationControllerMono AnimationController;


        protected virtual void Awake()
        {
            AnimationController = GetComponent<AnimationControllerMono>();
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

        public virtual void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(currentHealth - amount, 0);
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void Attack(UnitMono target)
        {
        }

        public virtual void SetSelected(bool selected)
        {
            if (selectionRing != null)
                selectionRing.SetActive(selected);
        }
    }
}
