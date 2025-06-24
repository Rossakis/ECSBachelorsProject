using Assets.Scripts.Monobehaviour.Animation;
using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.Monobehaviour.Movement;
using Assets.Scripts.Monobehaviour.Navigation;
using Assets.Scripts.ScriptableObjects.Animation;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Units
{
    public abstract class UnitMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;

        [Header("Unit Stats")]
        public FactionType FactionType = FactionType.Wizard;
        [HideInInspector] public int damage;
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
        public GameObject healthBar;
        public Transform healthBarFill;

        [Header("UI")]
        public GameObject selectionRing;

        // Navigation
        public bool IsFollowingFlowField { get; private set; }
        public Vector3 FlowFieldTarget { get; private set; }

        public bool IsMoveOverrideActive { get; private set; }
        private Vector3 moveOverrideTarget = Vector3.zero;

        public enum HealthState
        {
            Alive,
            Injured,
            Dead
        }

        protected virtual void Awake()
        {
            AnimationController = GetComponent<AnimationControllerMono>();
        }

        protected virtual void Start()
        {
            UnitMoveManagerMono.Instance?.RegisterUnit(this);
            TargetManagerMono.Instance?.RegisterUnit(this);
            HealthManagerMono.Instance?.RegisterUnit(this);
        }

        protected virtual void OnDestroy()
        {
            UnitMoveManagerMono.Instance?.UnregisterUnit(this);
            TargetManagerMono.Instance?.UnregisterUnit(this);
            HealthManagerMono.Instance?.UnregisterUnit(this);
        }

        public bool HasTarget => !float.IsNaN(targetPos.x);

        public virtual void SetTargetPosition(Vector3 pos)
        {
            targetPos = pos;
        }

        private bool IsMovingAlready = false;
        public virtual void Move(AnimationDataSO.AnimationType moveAnimation, AnimationDataSO.AnimationType idleAnimation)
        {
            if (IsMoveOverrideActive)
            {
                Vector3 direction = moveOverrideTarget - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude < 0.01f)
                {
                    // Arrived at override target
                    ClearMoveOverride();
                    targetPos = NoTarget;
                    AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                    IsMovingAlready = false;
                    return;
                }

                i f (!IsMovingAlready)
                {
                    AnimationManagerMono.Instance.PlayAnimationForUnits(this, moveAnimation);
                    IsMovingAlready = true;
                }
                Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
                transform.position += move;

                // Only rotate if direction is not zero
                if (direction.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
                return;
            }

            // If following flow field, move along the flow field
            if (IsFollowingFlowField)
            {
                var flowField = FlowFieldManagerMono.Instance.grid;
                Vector2Int gridPos = flowField.GetGridPosition(transform.position);

                if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= flowField.width || gridPos.y >= flowField.height)
                {
                    StopFollowingFlowField();
                    AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                    return;
                }

                Vector2 dir = flowField.flowField[gridPos.x, gridPos.y];
                if (dir == Vector2.zero)
                {
                    StopFollowingFlowField();
                    AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                    return;
                }

                int nextX = Mathf.Clamp(gridPos.x + Mathf.RoundToInt(dir.x), 0, flowField.width - 1);
                int nextY = Mathf.Clamp(gridPos.y + Mathf.RoundToInt(dir.y), 0, flowField.height - 1);

                Vector3 moveTarget = flowField.GetWorldCenterPosition(nextX, nextY);
                Vector3 direction = moveTarget - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude < 0.01f)
                {
                    // Check if close enough to the final flow field target
                    if (Vector3.Distance(transform.position, FlowFieldTarget) < flowField.nodeSize / 2f)
                    {
                        StopFollowingFlowField();
                        targetPos = NoTarget;
                        AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                        IsMovingAlready = false;
                        return;
                    }
                    else
                    {
                        // Snap to next cell and continue
                        transform.position = moveTarget;
                        return;
                    }
                }

                if (!IsMovingAlready)
                {
                    AnimationManagerMono.Instance.PlayAnimationForUnits(this, moveAnimation);
                    IsMovingAlready = true;
                }

                Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
                transform.position += move;

                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                return;
            }

            // If NOT following flow field (direct movement)
            Vector3 directDirection = targetPos - transform.position;
            directDirection.y = 0f;

            if (directDirection.sqrMagnitude < 0.01f)
            {
                targetPos = NoTarget;
                UnitMoveManagerMono.Instance.UnregisterUnit(this);
                AnimationManagerMono.Instance.PlayAnimationForUnits(this, idleAnimation);
                IsMovingAlready = false;
                return;
            }

            Vector3 directMove = directDirection.normalized * moveSpeed * Time.deltaTime;
            transform.position += directMove;

            // Only rotate if direction is not zero
            if (directDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion directTargetRot = Quaternion.LookRotation(directDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, directTargetRot, rotationSpeed * Time.deltaTime);
            }
        }

        protected virtual void Die()
        {
            Destroy(gameObject);
        }

        public virtual void Attack(UnitMono target)
        {
            currentTarget = target;
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

        public void StartFollowingFlowField(Vector3 target)
        {
            IsFollowingFlowField = true;
            FlowFieldTarget = target;
        }

        public void StopFollowingFlowField()
        {
            IsFollowingFlowField = false;
        }

        public void SetMoveOverride(Vector3 position)
        {
            IsMoveOverrideActive = true;
            moveOverrideTarget = position;
            SetTargetPosition(position);
            StartFollowingFlowField(position);
        }

        public void ClearMoveOverride()
        {
            IsMoveOverrideActive = false;
        }

        /// <summary>
        /// If unit movement is blocked by a wall, return true 
        /// </summary>
        /// <returns></returns>
        public bool NeedsFlowField()
        {
            Vector3 dir = targetPos - transform.position;
            if (Physics.Raycast(transform.position, dir.normalized, dir.magnitude, FlowFieldManagerMono.Instance.grid.wallMask))
                return true;

            return false;
        }
        public virtual void SetSelected(bool selected)
        {
            if (selectionRing != null)
                selectionRing.SetActive(selected);
        }
    }
}
