using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Units;
using UnityEngine;
using Assets.Scripts.ScriptableObjects.Animation;
using Assets.Scripts.ScriptableObjects.Scene;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class FireballManagerMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;
        public GameObject FireballPrefab;
        [Header("Fireball Settings")]
        public float fireballCooldown = 0.5f;
        public float fireballRange = 25f;
        public static FireballManagerMono Instance { get; private set; }

        private readonly HashSet<WizardMono> wizards = new();
        private readonly List<WizardMono> wizardBuffer = new();
        private readonly Queue<GameObject> fireballPool = new();
        private readonly Dictionary<WizardMono, float> cooldowns = new();

        private bool useObjectPooling = false;

        private void Awake()
        {
            Instance = this;
            useObjectPooling = sceneData.IsObjectPoolingOn;
        }

        private void Start()
        {
            if (useObjectPooling)
            {
                for (int i = 0; i < sceneData.WizardsAmountToSpawn; i++)
                {
                    var fireball = Instantiate(FireballPrefab, Vector3.zero, Quaternion.identity);
                    fireball.SetActive(false);
                    fireballPool.Enqueue(fireball);
                }
            }
        }


        private void Update()
        {
            wizardBuffer.Clear();
            wizardBuffer.AddRange(wizards);

            foreach (var wizard in wizardBuffer)
            {
                if (wizard == null || wizard.healthState == UnitMono.HealthState.Dead)
                    continue;

                if (wizard.currentTarget == null)
                {
                    wizard.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.WizardIdle);
                    continue;
                }

                // Cooldown management
                if (!cooldowns.ContainsKey(wizard))
                    cooldowns[wizard] = 0f;

                cooldowns[wizard] -= Time.deltaTime;
                if (cooldowns[wizard] > 0f)
                    continue;

                // Check if target is in range and alive
                float dist = Vector3.Distance(wizard.transform.position, wizard.currentTarget.transform.position);
                if (dist > fireballRange || wizard.currentTarget.healthState == UnitMono.HealthState.Dead)
                    continue;

                Vector3 dir = (wizard.currentTarget.transform.position - wizard.transform.position).normalized;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    wizard.transform.rotation = Quaternion.Slerp(wizard.transform.rotation, lookRot, wizard.rotationSpeed * Time.deltaTime);
                }

                if(!wizard.IsFiringAlready)
                {
                    wizard.AnimationController?.RequestAnimation(AnimationDataSO.AnimationType.WizardCastFireball);
                    wizard.IsFiringAlready = true;
                }

                if (wizard.fireballPrefab != null)
                {
                    Vector3 spawnPos = wizard.transform.position + Vector3.up * 1.2f;
                    Quaternion spawnRot = Quaternion.LookRotation(dir);

                    GameObject fireball = null;
                    if (useObjectPooling && fireballPool.Count > 0)
                    {
                        fireball = fireballPool.Dequeue();
                        fireball.transform.position = spawnPos;
                        fireball.transform.rotation = spawnRot;
                        fireball.SetActive(true);
                    }
                    else
                    {
                        fireball = Instantiate(wizard.fireballPrefab, spawnPos, spawnRot);
                    }

                    var fireballController = fireball.GetComponent<FireballControllerMono>();
                    if (fireballController != null)
                    {
                        fireballController.target = wizard.currentTarget;
                        fireballController.ownerWizard = wizard;
                    }
                }

                cooldowns[wizard] = fireballCooldown;
            }
        }

        public void RegisterWizard(WizardMono wizard)
        {
            wizards.Add(wizard);
        }

        public void UnregisterWizard(WizardMono wizard)
        {
            wizards.Remove(wizard);
            cooldowns.Remove(wizard);
        }

        /// <summary>
        /// Return to pool instead of destroying when Object Pooling is enabled.
        /// </summary>
        public void ReturnFireballToPool(GameObject fireball)
        {
            fireball.SetActive(false);
            fireball.transform.position = Vector3.zero;
            fireballPool.Enqueue(fireball);
        }
    }
}