using UnityEngine;

[CreateAssetMenu()]
public class AnimationDataSO : ScriptableObject {

    public enum AnimationType {
        None,
        WizardIdle,
        WizardWalk,
        WizardCastFireball,
        KnightIdle,
        KnightWalk,
        KnightAttack,
    }


    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;


    public static bool IsAnimationUninterruptible(AnimationType animationType)
    {
         if(animationType == AnimationType.KnightAttack || animationType == AnimationType.WizardCastFireball)
             return true;
        
         return false;
    }
}