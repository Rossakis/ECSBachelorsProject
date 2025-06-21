using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PanelSwitchManager : MonoBehaviour
    {
        public GameObject mainMenuPanel;
        public GameObject ecsPanel;
        public GameObject monoPanel;
        
        //Error
        public GameObject errorPanel;
        public TMP_Text errorMessage;
        
        private GameObject currentPanel;

        private void Awake()
        {
            currentPanel = mainMenuPanel;
            ecsPanel.SetActive(false);
            monoPanel.SetActive(false);
            errorPanel.SetActive(false);
        }

        public void SwitchToEcsPanel()
        {
            SetActivePanel(ecsPanel);
        }
    
        public void SwitchToMonoPanel()
        {
            SetActivePanel(monoPanel);
        }
        
        public void SwitchToErrorPanel(string message)
        {
            errorPanel.SetActive(true);
            errorMessage.text = message;
        }
        
        public void SwitchToMainMenu()
        {
            SetActivePanel(mainMenuPanel);
        }

        private void SetActivePanel(GameObject panel)
        {
            if(errorPanel.activeSelf)
                errorPanel.SetActive(false);
            
            currentPanel.SetActive(false);
            currentPanel = panel;
            currentPanel.SetActive(true);
        }
        
        
    }
}