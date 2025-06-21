using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Debug
{
    public class GridSystemDebug : MonoBehaviour {


        public static GridSystemDebug Instance { get; private set; }


        [SerializeField] private Transform debugPrefab;
        [SerializeField] private Sprite circleSprite;
        [SerializeField] private Sprite arrowSprite;


        private bool isInit;
        private GridSystemDebugSingle[,] gridSystemDebugSingleArray;


        private void Awake() {
            Instance = this;
        }

        public void InitializeGrid(ECS.Systems.Navigation.GridSystem.GridSystemData gridSystemData) {
            if (isInit) {
                return;
            }
            isInit = true;

            gridSystemDebugSingleArray = new GridSystemDebugSingle[gridSystemData.width, gridSystemData.height];
            for (int x = 0; x < gridSystemData.width; x++) {
                for (int y = 0; y < gridSystemData.height; y++) {
                    Transform debugTransform = Instantiate(debugPrefab);
                    GridSystemDebugSingle gridSystemDebugSingle = debugTransform.GetComponent<GridSystemDebugSingle>();
                    gridSystemDebugSingle.Setup(x, y, gridSystemData.gridNodeSize);

                    gridSystemDebugSingleArray[x, y] = gridSystemDebugSingle;
                }
            }
        }

        public void UpdateGrid(ECS.Systems.Navigation.GridSystem.GridSystemData gridSystemData) {
            for (int x = 0; x < gridSystemData.width; x++) {
                for (int y = 0; y < gridSystemData.height; y++) {
                    GridSystemDebugSingle gridSystemDebugSingle = gridSystemDebugSingleArray[x, y];

                    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    int index = ECS.Systems.Navigation.GridSystem.CalculateIndex(x, y, gridSystemData.width);
                    int gridIndex = gridSystemData.nextGridIndex - 1;
                    if (gridIndex < 0) {
                        gridIndex = 0;
                    }
                    Entity gridNodeEntity = gridSystemData.gridMapArray[gridIndex].gridEntityArray[index];
                    ECS.Systems.Navigation.GridSystem.GridNode gridNode = entityManager.GetComponentData<ECS.Systems.Navigation.GridSystem.GridNode>(gridNodeEntity);

                    if (gridNode.cost == 0) {
                        // This is the target
                        gridSystemDebugSingle.SetSprite(circleSprite);
                        gridSystemDebugSingle.SetColor(Color.green);
                    } else {
                        if (gridNode.cost == ECS.Systems.Navigation.GridSystem.WALL_COST) {
                            gridSystemDebugSingle.SetSprite(circleSprite);
                            gridSystemDebugSingle.SetColor(Color.black);
                        } else {
                            gridSystemDebugSingle.SetSprite(arrowSprite);
                            gridSystemDebugSingle.SetColor(Color.white);
                            gridSystemDebugSingle.SetSpriteRotation(
                                Quaternion.LookRotation(new float3(gridNode.vector.x, 0, gridNode.vector.y), Vector3.up));
                        }
                    }
                }
            }
        }

    }
}