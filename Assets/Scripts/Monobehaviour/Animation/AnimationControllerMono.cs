using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Animation
{
    public class AnimationControllerMono : MonoBehaviour
    {
        [Header("Shared Animation Data")]
        public AnimationDataListSO animationDataListSO;

        [Header("Mesh Output")]
        public MeshFilter meshFilter;

        private AnimationDataSO currentDataSO;
        private AnimationDataSO.AnimationType currentType = AnimationDataSO.AnimationType.None;
        private AnimationDataSO.AnimationType nextType = AnimationDataSO.AnimationType.None;

        private int currentFrame = 0;
        private float frameTimer = 0f;

        private bool hasPendingQueued => nextType != AnimationDataSO.AnimationType.None;

        private void OnEnable()
        {
            AnimationManagerMono.Instance.Register(this);
        }

        private void OnDisable()
        {
            AnimationManagerMono.Instance.Unregister(this);
        }

        public bool CanInterruptAnimation()
        {
            return CurrentAnimation != AnimationDataSO.AnimationType.KnightAttack &&
                   CurrentAnimation != AnimationDataSO.AnimationType.WizardCastFireball;
        }

        public void RequestAnimation(AnimationDataSO.AnimationType type)
        {
            if (currentType == type)
                return;

            // If currently playing an uninterruptible animation, queue it
            if (currentDataSO != null && !CanInterruptAnimation())
            {
                nextType = type;
                return;
            }

            SetAnimation(type);
        }

        private void SetAnimation(AnimationDataSO.AnimationType type)
        {
            AnimationDataSO newDataSO = animationDataListSO.GetAnimationDataSO(type);

            if (newDataSO == null || newDataSO.meshArray.Length == 0)
            {
                UnityEngine.Debug.LogWarning($"[AnimationControllerMono] Animation data for {type} is null or empty.");
                return;
            }

            currentType = type;
            currentDataSO = newDataSO;
            currentFrame = 0;
            frameTimer = 0f;
            meshFilter.mesh = currentDataSO.meshArray[currentFrame];
        }

        public void Tick(float deltaTime)
        {
            if (currentDataSO == null || currentDataSO.meshArray.Length == 0)
                return;

            frameTimer += deltaTime;

            if (frameTimer >= currentDataSO.frameTimerMax)
            {
                frameTimer -= currentDataSO.frameTimerMax;
                currentFrame = (currentFrame + 1) % currentDataSO.meshArray.Length;
                meshFilter.mesh = currentDataSO.meshArray[currentFrame];

                if (currentFrame == 0 && hasPendingQueued)
                {
                    SetAnimation(nextType);
                    nextType = AnimationDataSO.AnimationType.None;
                }
            }
        }

        public AnimationDataSO.AnimationType CurrentAnimation => currentType;
    }
}
