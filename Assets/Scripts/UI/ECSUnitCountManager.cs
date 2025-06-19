using ECS.Authoring.Combat;
using TMPro;
using Unity.Entities;
using UnityEngine;

public class ECSUnitCountManager : MonoBehaviour
{
    public TMP_Text wizardsText;
    public TMP_Text knightsText;
    
    private EntityManager _entityManager;
    private Entity _singletonEntity;
    
    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Find the singleton entity manually by checking for UnitCountData
        var query = _entityManager.CreateEntityQuery(typeof(UnitCount));

        if (query.CalculateEntityCount() == 1)
        {
            _singletonEntity = query.GetSingletonEntity();
        }
        else
        {
            Debug.LogError("UnitCountData singleton not found or multiple exist.");
        }
    }

    void Update()
    {
        if (_singletonEntity == Entity.Null)
        {
            var query = _entityManager.CreateEntityQuery(typeof(UnitCount));

            if (query.CalculateEntityCount() == 1)
            {
                _singletonEntity = query.GetSingletonEntity();
            }
            else
            {
                // Still not created; wait for next frame
                return;
            }
        }

        if (_entityManager.Exists(_singletonEntity))
        {
            UnitCount countData = _entityManager.GetComponentData<UnitCount>(_singletonEntity);
            wizardsText.text = "Wizards: " + countData.WizardCount;
            knightsText.text = "Knights: " + countData.KnightCount;
        }
    }
}
