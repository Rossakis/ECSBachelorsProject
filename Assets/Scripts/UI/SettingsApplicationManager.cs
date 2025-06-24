using Assets.Scripts.ScriptableObjects.Scene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Handles the application of user settings for each scene data SO (e.g. IsJobsSystemOn for the EcsSceneData scriptable object) 
    /// </summary>
    public class SettingsApplicationManager : MonoBehaviour
    {
        [Header("ECS Settings")]
        public EcsSceneDataSO ECSSceneDataSO;
        public Toggle ECSIsJobsSystemOn;
        public Toggle ECSIsObjectPoolingOn;
        
        [Header("ECS Wizards-Settings")]
        public TMP_InputField ECSWizardsAmountInputField;
        public TMP_InputField ECSWizardsHPInputField;
        public TMP_InputField ECSWizardsDamageInputField;
        
        [Header("ECS Knights-Settings")]
        public Toggle ECSIsInfiniteKnightSpawnOn;
        public TMP_InputField ECSKnightsInputField;
        public TMP_InputField ECSKnightsHPInputField;
        public TMP_InputField ECSKnightsDamageInputField;
    
        [Header("Mono Settings")]
        public MonoSceneDataSO MonoSceneDataSO;
        public Toggle MonoIsObjectPoolingOn;
        
        [Header("Mono Wizards-Settings")]
        public TMP_InputField MonoAmountInputField;
        public TMP_InputField MonoHPInputField;
        public TMP_InputField MonoDamageInputField;
        
        [Header("Mono Knights-Settings")]
        public Toggle MonoIsInfiniteKnightSpawnOn;
        public TMP_InputField MonoKnightsInputField;
        public TMP_InputField MonoKnightsHPInputField;
        public TMP_InputField MonoKnightsDamageInputField;
    
        private PanelSwitchManager panelSwitchManager;
        private SceneLoader sceneLoader;

        private void Awake()
        {
            panelSwitchManager = GetComponent<PanelSwitchManager>();
            sceneLoader = GetComponent<SceneLoader>();
            Application.targetFrameRate = -1; // uncapped framerate
            QualitySettings.vSyncCount = 0; // Disable VSync
        }

        #region ECS
        public void ApplyECSSettings(bool isBenchMarkMode)
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

            ECSSceneDataSO.IsBenchMarkMode = isBenchMarkMode;

            ECSSceneDataSO.IsJobSystemOn = ECSIsJobsSystemOn.isOn;
            ECSSceneDataSO.IsObjectPoolingOn = ECSIsObjectPoolingOn.isOn;
            
            ECSSceneDataSO.WizardsAmountToSpawn = wizardAmount;
            ECSSceneDataSO.WizardMaxHealth = wizardHP;
            ECSSceneDataSO.WizardDamage = knightsAmount;
            
            ECSSceneDataSO.IsKnightSpawnInfinite = ECSIsInfiniteKnightSpawnOn.isOn;
            ECSSceneDataSO.KnightsAmountToSpawn = knightsAmount;
            ECSSceneDataSO.KnightMaxHealth = knightsHP;
            ECSSceneDataSO.KnightDamage = knightsDamage;
            
            sceneLoader.LoadEcsScene();
        }
        
        public void LoadDefaultECSSettings()
        {
            //UI changes
            ECSIsJobsSystemOn.isOn = true;
            ECSIsObjectPoolingOn.isOn = true;
            
            ECSWizardsAmountInputField.text = "100";
            ECSWizardsHPInputField.text = "25";
            ECSWizardsDamageInputField.text = "20";

            ECSIsInfiniteKnightSpawnOn.isOn = false;
            ECSKnightsInputField.text = "300";
            ECSKnightsHPInputField.text = "100";
            ECSKnightsDamageInputField.text = "5";
            
            //SceneData changes
            ECSSceneDataSO.IsJobSystemOn = true;
            ECSSceneDataSO.IsObjectPoolingOn = true;
            
            ECSSceneDataSO.WizardsAmountToSpawn = 100;
            ECSSceneDataSO.WizardMaxHealth = 25;
            ECSSceneDataSO.WizardDamage = 20;
            
            ECSSceneDataSO.IsKnightSpawnInfinite = false;
            ECSSceneDataSO.KnightsAmountToSpawn = 300;
            ECSSceneDataSO.KnightMaxHealth = 100;
            ECSSceneDataSO.KnightDamage = 5;
        }
        #endregion

        #region Monobehaviour Settings
        public void ApplyMonoSettings(bool isBenchMarkMode)
        {
            // Wizards
            if (!int.TryParse(MonoAmountInputField.text, out int wizardAmount) || wizardAmount <= 0)
            {
                string errorMsg = "Number of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(MonoHPInputField.text, out int wizardHP) || wizardHP <= 0)
            {
                string errorMsg = "Health of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(MonoDamageInputField.text, out int wizardDamage) || wizardDamage <= 0)
            {
                string errorMsg = "Damage of Wizards must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            // Knights
            if (!int.TryParse(MonoKnightsInputField.text, out int knightAmount) || knightAmount <= 0)
            {
                string errorMsg = "Number of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(MonoKnightsHPInputField.text, out int knightHP) || knightHP <= 0)
            {
                string errorMsg = "Health of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            if (!int.TryParse(MonoKnightsDamageInputField.text, out int knightDamage) || knightDamage <= 0)
            {
                string errorMsg = "Damage of Knights must be greater than zero.";
                panelSwitchManager.SwitchToErrorPanel(errorMsg);
                return;
            }

            MonoSceneDataSO.IsBenchMarkMode = isBenchMarkMode;

            MonoSceneDataSO.IsObjectPoolingOn = MonoIsObjectPoolingOn.isOn;

            MonoSceneDataSO.WizardsAmountToSpawn = wizardAmount;
            MonoSceneDataSO.WizardMaxHealth = wizardHP;
            MonoSceneDataSO.WizardDamage = wizardDamage;

            MonoSceneDataSO.IsKnightSpawnInfinite = MonoIsInfiniteKnightSpawnOn.isOn;
            MonoSceneDataSO.KnightsAmountToSpawn = knightAmount;
            MonoSceneDataSO.KnightMaxHealth = knightHP;
            MonoSceneDataSO.KnightDamage = knightDamage;

            sceneLoader.LoadMonoScene();
        }

        public void LoadDefaultMonoSettings()
        {
            // UI changes
            MonoIsObjectPoolingOn.isOn = true;

            MonoAmountInputField.text = "100";
            MonoHPInputField.text = "25";
            MonoDamageInputField.text = "20";

            MonoIsInfiniteKnightSpawnOn.isOn = false;
            MonoKnightsInputField.text = "300";
            MonoKnightsHPInputField.text = "100";
            MonoKnightsDamageInputField.text = "5";

            // SceneData changes
            MonoSceneDataSO.IsObjectPoolingOn = true;

            MonoSceneDataSO.WizardsAmountToSpawn = 100;
            MonoSceneDataSO.WizardMaxHealth = 25;
            MonoSceneDataSO.WizardDamage = 20;

            MonoSceneDataSO.IsKnightSpawnInfinite = false;
            MonoSceneDataSO.KnightsAmountToSpawn = 300;
            MonoSceneDataSO.KnightMaxHealth = 100;
            MonoSceneDataSO.KnightDamage = 5;
        }

        #endregion
    }
}
