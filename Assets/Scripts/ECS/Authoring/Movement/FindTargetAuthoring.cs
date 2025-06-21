using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Movement
{
    public class FindTargetAuthoring : MonoBehaviour {



        public float range;
        public FactionType targetFaction;
        public float timerMax;


        public class Baker : Baker<FindTargetAuthoring> {


            public override void Bake(FindTargetAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget {
                    range = authoring.range,
                    targetFaction = authoring.targetFaction,
                    timerMax = authoring.timerMax,
                });
            }


        }

    }



    public struct FindTarget : IComponentData {

    
        public float range;
        public FactionType targetFaction;
        public float timer;
        public float timerMax;


    }
}