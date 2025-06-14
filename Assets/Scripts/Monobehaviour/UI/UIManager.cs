using TMPro;
using UnityEngine;
using Unity.Entities;

public class UIManager : MonoBehaviour
{
    //Global values 
    public static bool isJobEnabled;
    
    [SerializeField] private TMP_InputField UnitInputField;
    [SerializeField] private TMP_Text JobButtonText;
    [SerializeField] private float Radius = 100f;

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
