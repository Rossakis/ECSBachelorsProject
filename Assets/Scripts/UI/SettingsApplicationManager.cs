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
        public Toggle IsVsyncOn;
        
        [Header("ECS Wizards-Settings")]
        public TMP_InputField ECSWizardsAmountInputField;
        public TMP_InputField ECSWizardsHPInputField;
        public TMP_InputField ECSWizardsDamageInputField;
        
        [Header("ECS Knights-Settings")]
        public Toggle InfiniteKnightSpawnOn;
        public TMP_InputField ECSKnightsInputField;
        public TMP_InputField ECSKnightsHPInputField;
        public TMP_InputField ECSKnightsDamageInputField;
    
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
            Application.targetFrameRate = -1; // uncapped framerate
        }

        #region ECS
        public void ApplyECSSettings()
        {
            // Wizards
            if (!int.TryParse(ECSWizardsAmountInputField.text, out int wizardAmount) || wizardAmount <= 0)
            {
                string errorMsg = "Number of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }
            
            if (!int.TryParse(ECSWizardsHPInputField.text, out int wizardHP) || wizardHP <= 0)
            {
                string errorMsg = "Health of  Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }
            
            if (!int.TryParse(ECSWizardsDamageInputField.text, out int wizardDamage) || wizardDamage <= 0)
            {
                string errorMsg = "Damage of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            // Knights
            if (!int.TryParse(ECSKnightsInputField.text, out int knightsAmount) || knightsAmount <= 0)
            {
                string errorMsg = "Number of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }
            
            if (!int.TryParse(ECSKnightsHPInputField.text, out int knightsHP) || knightsHP <= 0)
            {
                string errorMsg = "Health of  Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }
            
            if (!int.TryParse(ECSKnightsDamageInputField.text, out int knightsDamage) || knightsDamage <= 0)
            {
                string errorMsg = "Damage of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            ECSSceneDataSO.IsJobSystemOn = IsJobsSystemOn.isOn;
            ECSSceneDataSO.IsObjectPoolingOn = IsObjectPoolingOn.isOn;
            
            ECSSceneDataSO.WizardsAmountToSpawn = wizardAmount;
            ECSSceneDataSO.WizardMaxHealth = wizardHP;
            ECSSceneDataSO.WizardDamage = knightsAmount;
            
            ECSSceneDataSO.IsKnightSpawnInfinite = InfiniteKnightSpawnOn.isOn;
            ECSSceneDataSO.KnightsAmountToSpawn = knightsAmount;
            ECSSceneDataSO.KnightMaxHealth = knightsHP;
            ECSSceneDataSO.KnightDamage = knightsDamage;
            
            // VSync Off/On
            if (IsVsyncOn.isOn)
            {
                QualitySettings.vSyncCount = 1; // Enable 
            }
            else
            {
                QualitySettings.vSyncCount = 0; // Disable
            }

            
            sceneLoader.LoadEcsScene();
        }
        
        public void LoadDefaultECSSettings()
        {
            //Ui changes
            IsJobsSystemOn.isOn = true;
            IsObjectPoolingOn.isOn = true;
            
            ECSWizardsAmountInputField.text = "100";
            ECSWizardsHPInputField.text = "25";
            ECSWizardsDamageInputField.text = "20";

            InfiniteKnightSpawnOn.isOn = false;
            ECSKnightsInputField.text = "300";
            ECSKnightsHPInputField.text = "100";
            ECSKnightsDamageInputField.text = "5";
            
            //SceneData changes
            ECSSceneDataSO.IsJobSystemOn = true;
            ECSSceneDataSO.IsObjectPoolingOn = true;
            
            ECSSceneDataSO.WizardsAmountToSpawn = 100;
            ECSSceneDataSO.WizardMaxHealth = 25;
            ECSSceneDataSO.WizardDamage = 20;
            
            ECSSceneDataSO.IsKnightSpawnInfinite = InfiniteKnightSpawnOn.isOn;
            ECSSceneDataSO.KnightsAmountToSpawn = 300;
            ECSSceneDataSO.KnightMaxHealth = 100;
            ECSSceneDataSO.KnightDamage = 5;
        }
        

        #endregion

        
    
        public void ApplyMonoSettings()
        {
            if (!int.TryParse(ECSWizardsAmountInputField.text, out int wizards) || wizards <= 0)
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
