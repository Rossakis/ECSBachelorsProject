using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.Monobehaviour.Combat;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UnitCountManager : MonoBehaviour
    {
        public TMP_Text wizardsText;
        public TMP_Text knightsText;

        //ECS
        public bool IsECSScene = true;
        private EntityManager _entityManager;
        private Entity _singletonEntity;


        void Start()
        {
            if (IsECSScene)
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
            
        }

        void Update()
        {
            if (IsECSScene)
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
            else
            {
                // Find all active WizardMono and KnightMono in the scene
                int wizardCount = Object.FindObjectsByType<WizardMono>(FindObjectsSortMode.None).Length;
                int knightCount = Object.FindObjectsByType<KnightMono>(FindObjectsSortMode.None).Length;

                wizardsText.text = "Wizards: " + wizardCount;
                knightsText.text = "Knights: " + knightCount;

            }

        }
    }
}
