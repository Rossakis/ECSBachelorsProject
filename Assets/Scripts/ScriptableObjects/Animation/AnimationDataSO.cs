using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Animation
{
    [CreateAssetMenu()]
    public class AnimationDataSO : ScriptableObject {

        public enum AnimationType {
            None,
            WizardIdle,
            WizardWalk,
            WizardCastFireball,
            WizardDeath,
            KnightIdle,
            KnightWalk,
            KnightAttack,
            KnightDeath,
        }


        public AnimationType animationType;
        public Mesh[] meshArray;
        public float frameTimerMax;


        public static bool IsAnimationUninterruptible(AnimationType animationType)
        {
            if(animationType == AnimationType.KnightAttack || animationType == AnimationType.WizardCastFireball) // || animationType == AnimationType.WizardDeath || animationType == AnimationType.KnightDeath)
                return true;
        
            return false;
        }
    }
}