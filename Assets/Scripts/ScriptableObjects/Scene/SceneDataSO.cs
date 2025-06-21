using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Scene
{
    public class SceneDataSO : ScriptableObject
    {
        [Header("Performance Settings")]
        public bool IsJobSystemOn = true;
        public bool IsObjectPoolingOn = true;

        [Header("Wizard Settings")] 
        public int WizardsAmountToSpawn = 10;
        public int WizardMaxHealth = 25;
        public int WizardDamage = 20;

        [Header("Knight Settings")] 
        public bool IsKnightSpawnInfinite = false;
        public int KnightsAmountToSpawn = 100;
        public int KnightMaxHealth = 100;
        public int KnightDamage = 5;
    }
}