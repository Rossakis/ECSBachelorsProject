using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Units;
using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.Monobehaviour.Units;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UnitCountManager : MonoBehaviour
    {
        public static UnitCountManager Instance { get; private set; }
        public TMP_Text wizardsText;
        public TMP_Text knightsText;

        //ECS
        public bool IsECSScene = true;
        private EntityManager _entityManager;
        private Entity _singletonEntity;

        public int ECSWizardsCount { get; private set; }
        public int ECSKnightsCount { get; private set; }
        public int MonoWizardsCount { get; private set; }
        public int MonoKnightsCount { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (IsECSScene)
            {
                _singletonEntity = Entity.Null; // Reset singleton entity
            }
        }

        private void Update()
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

                    ECSWizardsCount = countData.WizardCount;
                    ECSKnightsCount = countData.KnightCount;

                    wizardsText.text = "Wizards: " + countData.WizardCount;
                    knightsText.text = "Knights: " + countData.KnightCount;
                }
            }
            else
            {
                // Find all active WizardMono and KnightMono in the scene
                int wizardCount = Object.FindObjectsByType<WizardMono>(FindObjectsSortMode.None).Length;
                int knightCount = Object.FindObjectsByType<KnightMono>(FindObjectsSortMode.None).Length;

                MonoWizardsCount = wizardCount;
                MonoKnightsCount = knightCount;

                wizardsText.text = "Wizards: " + wizardCount;
                knightsText.text = "Knights: " + knightCount;

            }

        }
    }
}
