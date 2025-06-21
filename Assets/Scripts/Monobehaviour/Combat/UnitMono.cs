using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    namespace Assets.Scripts.Monobehaviour.Combat
    {
        public abstract class UnitMono : MonoBehaviour
        {
            [Header("Unit Stats")]
            public int maxHealth = 100;
            public int damage = 10;
            public float moveSpeed = 5f;
            public float rotationSpeed = 10f;

            [Header("Runtime State")]
            public int currentHealth;
            public Vector3 targetPos;

            public enum Faction
            {
                Wizard,
                Knight
            }

            protected virtual void Awake()
            {
                currentHealth = maxHealth;
                targetPos = Vector3.negativeInfinity;
            }

            public virtual void TakeDamage(int amount)
            {
                currentHealth -= amount;
                if (currentHealth <= 0)
                {
                    Die();
                }
            }

            protected virtual void Die()
            {
                // Play death animation, disable unit, etc.
                Destroy(gameObject);
            }

            public virtual void Move()
            {
                if (targetPos != Vector3.negativeInfinity)
                {
                    Vector3 direction = targetPos - transform.position;
                    direction.y = 0f;

                    if (direction.sqrMagnitude < 0.1f)
                    {
                        targetPos = Vector3.negativeInfinity;
                        return;
                    }

                    Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
                    transform.position += move;

                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }

            public virtual void Attack(UnitMono target)
            {
                // Implement attack logic in derived class or here
            }

            public virtual void SetSelected(bool selected)
            {
                // Implement visual feedback, e.g., enable/disable selection ring
            }

            public void SetTargetPosition(Vector3 pos)
            {
                targetPos = pos;
            }
        }
    }
}
