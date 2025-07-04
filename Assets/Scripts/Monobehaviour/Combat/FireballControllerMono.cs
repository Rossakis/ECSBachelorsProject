using Assets.Scripts.Monobehaviour.Units;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class FireballControllerMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;
        public float speed = 40f;
        public UnitMono target;
        public WizardMono ownerWizard;
        private int damage;

        private void Start()
        {
            damage = sceneData.WizardDamage;
        }

        private void Update()
        {
            if (target != null)
            {
                transform.position += (target.transform.position - transform.position) * speed * Time.deltaTime;

                //Reached target
                if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
                {
                    target.TakeDamage(damage);
                    DespawnOrDestroy();
                }
            }
            else // Someone already destroyed the target
            {
                DespawnOrDestroy();
            }
        }

        private void DespawnOrDestroy()
        {
            ownerWizard.IsFiringAlready = false;

            if (sceneData != null && sceneData.IsObjectPoolingOn)
            {
                FireballManagerMono.Instance?.ReturnFireballToPool(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}