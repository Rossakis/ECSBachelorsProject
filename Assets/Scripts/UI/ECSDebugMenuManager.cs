using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ECSDebugMenuManager : MonoBehaviour
    {
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle IsJobsSystemOn;
        public Toggle IsObjectPoolingOn;
        
        public void ECSDebugValuesChanged()
        {
            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
            ECSSceneDataSO.IsObjectPoolingOn = IsObjectPoolingOn.isOn;
        }
    }
}