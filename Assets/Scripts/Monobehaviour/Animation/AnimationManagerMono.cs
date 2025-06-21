using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Animation
{
    public class AnimationManagerMono : MonoBehaviour
    {
        public static AnimationManagerMono Instance { get; private set; }

        private readonly HashSet<AnimationControllerMono> controllers = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void Register(AnimationControllerMono controller)
        {
            controllers.Add(controller);
        }

        public void Unregister(AnimationControllerMono controller)
        {
            controllers.Remove(controller);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            foreach (var controller in controllers)
            {
                controller.Tick(dt);
            }
        }

        public void PlayAnimationForUnits(UnitMono unit, AnimationDataSO.AnimationType type)
        {
            unit.AnimationController.RequestAnimation(type);
        }
    }
}