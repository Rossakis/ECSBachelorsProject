using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ECSDebugMenuManager : MonoBehaviour
    {
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle IsJobsSystemOn;
        public Toggle IsInfiniteKnightSpawnOn;

        private void Start()
        {
            ECSDebugValuesChanged();
        }
        
        public void ECSDebugValuesChanged()
        {
            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
            ECSSceneDataSO.IsKnightSpawnInfinite = IsInfiniteKnightSpawnOn.isOn;
        }
    }
}