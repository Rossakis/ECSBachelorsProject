using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ScriptableObjects.Animation;
using Assets.Scripts.Monobehaviour.Movement;
using Assets.Scripts.Monobehaviour.Units;
using Assets.Scripts.ScriptableObjects.Scene;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class MeleeAttackManagerMono : MonoBehaviour
    {
        public static MeleeAttackManagerMono Instance { get; private set; }

        private readonly HashSet<KnightMono> knights = new();
        private readonly List<KnightMono> knightBuffer = new();
        private readonly Dictionary<KnightMono, float> cooldowns = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            knightBuffer.Clear();
            knightBuffer.AddRange(knights);

            foreach (var knight in knightBuffer)
            {
                if (knight == null || knight.healthState == UnitMono.HealthState.Dead)
                    continue;

                var target = knight.currentTarget;
                if (target == null || target.healthState == UnitMono.HealthState.Dead)
                {
                    knight.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.KnightIdle);
                    continue;
                }

                // Cooldown management
                if (!cooldowns.ContainsKey(knight))
                    cooldowns[knight] = 0f;

                cooldowns[knight] -= Time.deltaTime;
                if (cooldowns[knight] < 0f)
                    cooldowns[knight] = 0f;

                // Check distance to target
                float distSqr = (target.transform.position - knight.transform.position).sqrMagnitude;
                float attackDistSqr = knight.attackDistance * knight.attackDistance;

                if (distSqr > attackDistSqr)
                {
                    // Move towards target
                    UnitMoveManagerMono.Instance.MoveUnitToPosition(knight, target.transform.position);
                    knight.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.KnightWalk);
                }
                else
                {
                    // Stop moving
                    UnitMoveManagerMono.Instance.UnregisterUnit(knight);

                    // Attack if cooldown is ready
                    if (cooldowns[knight] <= 0f)
                    {
                        // Face the target
                        Vector3 dir = (target.transform.position - knight.transform.position).normalized;
                        dir.y = 0f;
                        if (dir.sqrMagnitude > 0.01f)
                        {
                            Quaternion lookRot = Quaternion.LookRotation(dir);
                            knight.transform.rotation = Quaternion.Slerp(knight.transform.rotation, lookRot, knight.rotationSpeed * Time.deltaTime);
                        }

                        // Play attack animation
                        knight.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.KnightAttack);

                        // Deal damage
                        target.TakeDamage(knight.damage);

                        // Reset cooldown
                        cooldowns[knight] = knight.attackCooldown;
                    }
                    else
                    {
                        knight.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.KnightIdle);
                    }
                }
            }
        }

        public void RegisterKnight(KnightMono knight)
        {
            knights.Add(knight);
        }

        public void UnregisterKnight(KnightMono knight)
        {
            knights.Remove(knight);
            cooldowns.Remove(knight);
        }
    }
}