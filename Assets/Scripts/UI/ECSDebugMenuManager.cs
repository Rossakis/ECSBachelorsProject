using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ECSDebugMenuManager : MonoBehaviour
    {
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle IsJobsSystemOn;
        public Toggle IsVSyncOn;

        private void Start()
        {
            IsJobsSystemOn.isOn = ECSSceneDataSO.IsJobSystemOn;
            IsVSyncOn.isOn = QualitySettings.vSyncCount > 0;
        }
        
        public void ECSDebugValuesChanged()
        {
            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
        }
    }
}