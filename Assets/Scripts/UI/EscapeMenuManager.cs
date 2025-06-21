using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Enables/disables the Escape ui panel
    /// </summary>
    public class EscapeMenuManager : MonoBehaviour
    {
        public GameObject EscapeMenu;
        public GameObject EscapeMenuInstructions;

        private void Start()
        {
            EscapeMenu.SetActive(false);
            EscapeMenuInstructions.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeMenu.SetActive(!EscapeMenu.activeSelf);
                EscapeMenuInstructions.SetActive(!EscapeMenuInstructions.activeSelf);
            }
        }
    }
}