using UnityEngine;

[CreateAssetMenu()]
public class EcsSceneDataSO : ScriptableObject
{
    [Header("Performance Settings")]
    public bool IsJobSystemOn = true;
    public bool IsObjectPoolingOn = true;

    [Header("Gameplay Settings")] 
    public int WizardsAmountToSpawn = 10;
    public int WizardMaxHealth = 25;
    public int WizardDamage = 20;

    public bool IsKnightSpawnInfinite = false;
    public int KnightsAmountToSpawn = 100;
    public int KnightMaxHealth = 100;
    public int KnightDamage = 5;
}
