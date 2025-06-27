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

        // Default Values 
        private const bool defaultJobsSystemOn = true;
        private const bool defaultObjectPoolingOn = true;
        private const int defaultWizardsAmount = 100;
        private const int defaultWizardsHP = 25;
        private const int defaultWizardsDamage = 20;
        private const bool defaultInfiniteKnightSpawn = true;
        private const int defaultKnightsAmount = 300;
        private const int defaultKnightsHP = 100;
        private const int defaultKnightsDamage = 5;

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
            // UI changes
            ECSIsJobsSystemOn.isOn = defaultJobsSystemOn;
            ECSIsObjectPoolingOn.isOn = defaultObjectPoolingOn;

            ECSWizardsAmountInputField.text = defaultWizardsAmount.ToString();
            ECSWizardsHPInputField.text = defaultWizardsHP.ToString();
            ECSWizardsDamageInputField.text = defaultWizardsDamage.ToString();

            ECSIsInfiniteKnightSpawnOn.isOn = defaultInfiniteKnightSpawn;
            ECSKnightsInputField.text = defaultKnightsAmount.ToString();
            ECSKnightsHPInputField.text = defaultKnightsHP.ToString();
            ECSKnightsDamageInputField.text = defaultKnightsDamage.ToString();

            // SceneData changes
            ECSSceneDataSO.IsJobSystemOn = defaultJobsSystemOn;
            ECSSceneDataSO.IsObjectPoolingOn = defaultObjectPoolingOn;

            ECSSceneDataSO.WizardsAmountToSpawn = defaultWizardsAmount;
            ECSSceneDataSO.WizardMaxHealth = defaultWizardsHP;
            ECSSceneDataSO.WizardDamage = defaultWizardsDamage;

            ECSSceneDataSO.IsKnightSpawnInfinite = defaultInfiniteKnightSpawn;
            ECSSceneDataSO.KnightsAmountToSpawn = defaultKnightsAmount;
            ECSSceneDataSO.KnightMaxHealth = defaultKnightsHP;
            ECSSceneDataSO.KnightDamage = defaultKnightsDamage;
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
            MonoIsObjectPoolingOn.isOn = defaultObjectPoolingOn;

            MonoAmountInputField.text = defaultWizardsAmount.ToString();
            MonoHPInputField.text = defaultWizardsHP.ToString();
            MonoDamageInputField.text = defaultWizardsDamage.ToString();

            MonoIsInfiniteKnightSpawnOn.isOn = defaultInfiniteKnightSpawn;
            MonoKnightsInputField.text = defaultKnightsAmount.ToString();
            MonoKnightsHPInputField.text = defaultKnightsHP.ToString();
            MonoKnightsDamageInputField.text = defaultKnightsDamage.ToString();

            // SceneData changes
            MonoSceneDataSO.IsObjectPoolingOn = defaultObjectPoolingOn;

            MonoSceneDataSO.WizardsAmountToSpawn = defaultWizardsAmount;
            MonoSceneDataSO.WizardMaxHealth = defaultWizardsHP;
            MonoSceneDataSO.WizardDamage = defaultWizardsDamage;

            MonoSceneDataSO.IsKnightSpawnInfinite = defaultInfiniteKnightSpawn;
            MonoSceneDataSO.KnightsAmountToSpawn = defaultKnightsAmount;
            MonoSceneDataSO.KnightMaxHealth = defaultKnightsHP;
            MonoSceneDataSO.KnightDamage = defaultKnightsDamage;
        }

        #endregion
    }
}
