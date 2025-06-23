using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Navigation
{
    public class FlowFieldManagerMono : MonoBehaviour
    {
        public static FlowFieldManagerMono Instance { get; private set; }
        public FlowFieldGridMono grid;
        private Dictionary<Vector2Int, FlowFieldGridMono> cachedFields = new();

        private void Awake()
        {
            Instance = this;
            grid.Initialize();
        }

        public FlowFieldGridMono RequestFlowField(Vector3 targetWorldPos)
        {
            Vector2Int targetGrid = new Vector2Int(
                Mathf.FloorToInt(targetWorldPos.x / grid.nodeSize),
                Mathf.FloorToInt(targetWorldPos.z / grid.nodeSize)
            );
            if (!cachedFields.TryGetValue(targetGrid, out var field))
            {
                grid.CalculateFlowField(targetGrid);
                cachedFields[targetGrid] = grid;
                field = grid;
            }
            return field;
        }
    }
}