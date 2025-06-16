using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles the application of user settings for each scene data SO (e.g. IsJobsSystemOn for the EcsSceneData scriptable object) 
    /// </summary>
    public class SettingsApplicationManager : MonoBehaviour
    {
        [Header("ECS Settings")]
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle IsJobsSystemOn;
        public Toggle IsObjectPoolingOn;
        public TMP_InputField ECSWizardsInputField;
        public TMP_InputField ECSKnightsInputField;
    
        [Header("Mono Settings")]
        public MonoSceneDataSO MonoSceneDataSO;
        public TMP_InputField MonoWizardsInputField;
        public TMP_InputField MonoKnightsInputField;
    
        private PanelSwitchManager panelSwitchManager;
        private SceneLoader sceneLoader;

        private void Awake()
        {
            panelSwitchManager = GetComponent<PanelSwitchManager>();
            sceneLoader = GetComponent<SceneLoader>();
        }

        public void ApplyECSSettings()
        {
            if (!int.TryParse(ECSWizardsInputField.text, out int wizards) || wizards <= 0)
            {
                string errorMsg = "Number of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(ECSKnightsInputField.text, out int knights) || knights <= 0)
            {
                string errorMsg = "Number of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
            ECSSceneDataSO.IsObjectPoolingOn = IsObjectPoolingOn.isOn;
            ECSSceneDataSO.WizardsAmountToSpawn = wizards;
            ECSSceneDataSO.KnightsAmountToSpawn = knights;

            sceneLoader.LoadEcsScene();
        }
    
        public void ApplyMonoSettings()
        {
            if (!int.TryParse(ECSWizardsInputField.text, out int wizards) || wizards <= 0)
            {
                string errorMsg = "Number of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(ECSKnightsInputField.text, out int knights) || knights <= 0)
            {
                string errorMsg = "Number of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            MonoSceneDataSO.WizardsAmountToSpawn = wizards;
            MonoSceneDataSO.KnightsAmountToSpawn = knights;

            sceneLoader.LoadEcsScene();
        }
    }
}
