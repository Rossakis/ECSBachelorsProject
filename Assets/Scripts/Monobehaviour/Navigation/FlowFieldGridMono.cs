using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Navigation
{
    public class FlowFieldGridMono : MonoBehaviour
    {
        public int width = 40;
        public int height = 40;
        public float nodeSize = 5f;
        public LayerMask wallMask;
        public LayerMask heavyMask;
        public int wallCost = int.MaxValue;
        public int heavyCost = 50;
        public GameObject floorObject;
        public Vector3 gridOrigin;  

        [HideInInspector] public int[,] costMap;
        [HideInInspector] public Vector2[,] flowField;
        [HideInInspector] public int[,] bestCost;
        [HideInInspector] public Vector2Int targetGridPos;

        [Header("Debug")]
        public bool debugDrawNodes = true;

        public void Initialize()
        {
            // If using Unity's default plane, its mesh is 10x10 units, so scale.x = 1 means 10 units wide
            if (floorObject != null)
            {
                Vector3 floorScale = floorObject.transform.localScale;
                Vector3 floorPosition = floorObject.transform.position;

                // For Unity's default plane, size is 10 units per scale
                float floorWidth = 10f * floorScale.x;
                float floorHeight = 10f * floorScale.z;

                // Set grid size based on floor size and nodeSize
                width = Mathf.RoundToInt(floorWidth / nodeSize);
                height = Mathf.RoundToInt(floorHeight / nodeSize);

                // Store the grid's world origin (bottom-left corner)
                gridOrigin = new Vector3(
                    floorPosition.x - floorWidth / 2f,
                    floorPosition.y,
                    floorPosition.z - floorHeight / 2f
                );
            }

            costMap = new int[width, height];
            flowField = new Vector2[width, height];
            bestCost = new int[width, height];
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && debugDrawNodes)
            {
                DebugDrawNodes(Color.yellow);
            }
        }

        public void CalculateFlowField(Vector2Int target)
        {
            target.x = Mathf.Clamp(target.x, 0, width - 1);
            target.y = Mathf.Clamp(target.y, 0, height - 1);

            targetGridPos = target;
            // Fill cost map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = GetWorldCenterPosition(x, y);
                    if (Physics.CheckSphere(worldPos, nodeSize * 0.5f, wallMask))
                    {
                        costMap[x, y] = wallCost;
                    }
                    else if (Physics.CheckSphere(worldPos, nodeSize * 0.5f, heavyMask))
                    {
                        costMap[x, y] = heavyCost;
                    }
                    else
                    {
                        costMap[x, y] = 1;
                    }
                    bestCost[x, y] = int.MaxValue;
                    flowField[x, y] = Vector2.zero;
                }
            }

            // BFS for best cost
            Queue<Vector2Int> open = new Queue<Vector2Int>();
            bestCost[target.x, target.y] = 0;
            open.Enqueue(target);

            int[,] directions = {
                { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 },
                { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 }
            };

            while (open.Count > 0)
            {
                Vector2Int current = open.Dequeue();
                int curCost = bestCost[current.x, current.y];

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int nx = current.x + directions[i, 0];
                    int ny = current.y + directions[i, 1];
                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;
                    if (costMap[nx, ny] == wallCost)
                        continue;

                    int newCost = curCost + costMap[nx, ny];
                    if (newCost < bestCost[nx, ny])
                    {
                        bestCost[nx, ny] = newCost;
                        open.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }

            // 3. Set flow field vectors
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (costMap[x, y] == wallCost)
                    {
                        flowField[x, y] = Vector2.zero;
                        continue;
                    }

                    Vector2Int bestNeighbor = new Vector2Int(x, y);
                    int minCost = bestCost[x, y];

                    for (int i = 0; i < directions.GetLength(0); i++)
                    {
                        int nx = x + directions[i, 0];
                        int ny = y + directions[i, 1];
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                            continue;
                        if (bestCost[nx, ny] < minCost)
                        {
                            minCost = bestCost[nx, ny];
                            bestNeighbor = new Vector2Int(nx, ny);
                        }
                    }

                    Vector2 dir = new Vector2(bestNeighbor.x - x, bestNeighbor.y - y).normalized;
                    flowField[x, y] = dir;
                }
            }
        }

        public Vector2 GetFlowDirection(Vector3 worldPos)
        {
            Vector2Int gridPos = GetGridPosition(worldPos);
            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= height) return Vector2.zero;
            return flowField[gridPos.x, gridPos.y];
        }

        public Vector3 GetWorldCenterPosition(int x, int y)
        {
            return gridOrigin + new Vector3(x * nodeSize + nodeSize * 0.5f, 0f, y * nodeSize + nodeSize * 0.5f);
        }

        public Vector2Int GetGridPosition(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - gridOrigin;
            int x = Mathf.FloorToInt(localPos.x / nodeSize);
            int y = Mathf.FloorToInt(localPos.z / nodeSize);
            return new Vector2Int(x, y);
        }

        public void DebugDrawNodes(Color color, float duration = 0f)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 center = GetWorldCenterPosition(x, y);
                    Debug.DrawLine(center + Vector3.left * nodeSize * 0.4f, center + Vector3.right * nodeSize * 0.4f, color, duration);
                    Debug.DrawLine(center + Vector3.forward * nodeSize * 0.4f, center + Vector3.back * nodeSize * 0.4f, color, duration);
                }
            }
        }
    }
}