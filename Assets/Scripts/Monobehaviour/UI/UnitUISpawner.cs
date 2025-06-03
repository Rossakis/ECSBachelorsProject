using ECS.Components.Spawn;
using TMPro;
using UnityEngine;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UI;

public class UnitUISpawner : MonoBehaviour
{
    public TMP_InputField UnitInputField;
    public float Radius = 100f;

    private EntityManager entityManager;
    private Entity prefabSingletonEntity;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Query the prefab reference singleton baked at authoring time
        var query = entityManager.CreateEntityQuery(typeof(UnitPrefabReference));
        prefabSingletonEntity = query.GetSingletonEntity(); // This should now exist
    }

    public void SpawnUnitsOnClick()
    {
        // Destroy all existing units (filtered by tag/type)
        var query = entityManager.CreateEntityQuery(typeof(UnitTag));
        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        foreach (var root in entities)
        {
            if (!entityManager.HasComponent<LinkedEntityGroup>(root))
            {
                entityManager.DestroyEntity(root);
                continue;
            }

            var linkedGroup = entityManager.GetBuffer<LinkedEntityGroup>(root);
            for (int i = 0; i < linkedGroup.Length; i++)
            {
                entityManager.DestroyEntity(linkedGroup[i].Value);
            }
        }

        if (!int.TryParse(UnitInputField.text, out int unitsToSpawn) || unitsToSpawn <= 0)
        {
            Debug.LogWarning("Invalid input for unit count.");
            return;
        }

        // Get prefab entity from baked singleton
        var prefabData = entityManager.GetComponentData<UnitPrefabReference>(prefabSingletonEntity);

        // Trigger new spawn
        Entity spawnCommand = entityManager.CreateEntity(typeof(UnitSpawnData));
        entityManager.SetComponentData(spawnCommand, new UnitSpawnData
        {
            Prefab = prefabData.Prefab,
            Count = unitsToSpawn,
            Radius = Radius
        });
    }
}
