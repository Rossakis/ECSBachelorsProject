using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour {
    
    public class Baker : Baker<HealthAuthoring> {
        
        public override void Bake(HealthAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new Health {
                healthAmount = 1,
                healthAmountMax = 1,
                onHealthChanged = true,
            });
        }
    }
}


public struct Health : IComponentData {
    
    public int healthAmount;
    public int healthAmountMax;
    public bool onHealthChanged;
    public bool onDead;
}