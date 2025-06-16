using UnityEngine;

[CreateAssetMenu()]
public class MonoSceneDataSO : ScriptableObject
{
    [Header("Gameplay Settings")] 
    public int WizardsAmountToSpawn;
    public int KnightsAmountToSpawn;
}
