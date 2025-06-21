using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Scene
{
    [CreateAssetMenu()]
    public class EcsSceneDataSO : SceneDataSO
    {
        [Header("ECS Performance Settings")]
        public bool IsJobSystemOn = true;
    }
}
