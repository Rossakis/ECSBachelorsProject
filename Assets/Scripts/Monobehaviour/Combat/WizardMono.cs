using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class WizardMono : UnitMono
    {
        [Header("Wizard Specific")]
        public GameObject fireballPrefab;
        public bool IsFiringAlready = false;


        protected override void Start()
        {
            base.Start();
            currentHealth = sceneData.WizardMaxHealth;
            damage = sceneData.WizardDamage;
            AnimationController.RequestAnimation(AnimationDataSO.AnimationType.WizardIdle);
            FireballManagerMono.Instance.RegisterWizard(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            FireballManagerMono.Instance.UnregisterWizard(this);
        }
    }
}
