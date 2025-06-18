using UnityEngine;

public class GameAssets : MonoBehaviour {
    
    public const int DEFAULT_LAYER = 0;
    public const int UNITS_LAYER = 6;
    public const int PATHFINDING_WALLS = 7;
    public const int PATHFINDING_HEAVY = 8;
    
    public static GameAssets Instance { get; private set; }

    
    private void Awake() {
        Instance = this;
    }
}