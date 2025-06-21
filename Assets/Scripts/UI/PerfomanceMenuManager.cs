using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PerfomanceMenuManager : MonoBehaviour
    {
        public bool IsECSScene = true;

        [Header("ECS Settings")]
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle IsJobsSystemOn;

        [Header("General Settings")]
        public Toggle IsVSyncOn;


        private void Start()
        {
            if (IsECSScene)
            {
                IsJobsSystemOn.isOn = ECSSceneDataSO.IsJobSystemOn;
            }


            IsVSyncOn.isOn = QualitySettings.vSyncCount > 0;
        }

        public void ECSDebugValuesChanged()
        {
            if(!IsECSScene) 
                return;

            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
        }
    }
}