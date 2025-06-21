using UnityEngine;

namespace Assets.Scripts.Level
{
    /// <summary>
    /// Adjust the position of child tiles in a grid layout automatically (instead of manually finding and setting positions in the scene view).
    /// </summary>
    [ExecuteAlways]
    public class TileAutoGrid : MonoBehaviour
    {
        [Tooltip("Optional extra spacing between tiles.")]
        public Vector2 spacing = new Vector2(0, 0);

        [Tooltip("Number of tiles per row. Set 0 for auto row (single line).")]
        public int tilesPerRow = 0;

        void Start()
        {
            ArrangeTiles();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            ArrangeTiles(); // Auto-update in editor
        }
#endif

        void ArrangeTiles()
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            Vector3 startPos = transform.position;

            float maxTileWidth = 0f;
            float maxTileHeight = 0f;

            // Get max tile size among children
            foreach (Transform child in children)
            {
                if (child == transform) // skip self
                    continue; 

                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Vector3 size = renderer.bounds.size;
                    maxTileWidth = Mathf.Max(maxTileWidth, size.x);
                    maxTileHeight = Mathf.Max(maxTileHeight, size.y);
                }
            }

            int col = 0;
            int row = 0;
            int count = 0;

            // Position tiles in grid
            foreach (Transform child in children)
            {
                if (child == transform) 
                    continue;

                float posX = (maxTileWidth + spacing.x) * col;
                float posY = -(maxTileHeight + spacing.y) * row;

                child.localPosition = new Vector3(posX, posY, 0f);

                count++;
                col++;

                if (tilesPerRow > 0 && col >= tilesPerRow)
                {
                    col = 0;
                    row++;
                }
            }
        }
    }
}