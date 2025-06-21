using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        //Global values 
        public static bool isJobEnabled;
    
        [SerializeField] private TMP_InputField UnitInputField;
        [SerializeField] private TMP_Text JobButtonText;

        private EntityManager entityManager;
        private Entity prefabSingletonEntity;
    
        void Start()
        {
        }

        public void SpawnUnitsOnClick()
        {
       
        }
    
        public void SwitchToJobMultithreading()
        {
        }


    }
}
