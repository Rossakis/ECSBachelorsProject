using UnityEngine;

[CreateAssetMenu()]
public class EcsSceneDataSO : ScriptableObject
{
    [Header("Performance Settings")]
    public bool IsJobSystemOn = true;
    public bool IsObjectPoolingOn = true;

    [Header("Gameplay Settings")] 
    public int WizardsAmountToSpawn = 10;
    public int KnightsAmountToSpawn = 100;
}
