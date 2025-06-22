using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class WizardMono : UnitMono
    {
        [Header("Wizard Specific")]
        public GameObject fireballPrefab;
        public GameObject selectionRing;
        void Start()
        {
            currentHealth = sceneData.WizardMaxHealth;
            damage = sceneData.WizardDamage;
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.WizardIdle);
            FireballManagerMono.Instance.RegisterWizard(this);
        }

        void OnDisable()
        {
            FireballManagerMono.Instance.UnregisterWizard(this);
        }

        void Update()
        {
        
        }

        public override void Attack(UnitMono target)
        {
            //TODO:
            if (fireballPrefab != null && target != null)
            {
               
            }
        }
        public virtual void SetSelected(bool selected)
        {
            if (selectionRing != null)
                selectionRing.SetActive(selected);
        }
    }
}
