using Unity.Entities;

public struct MoveStateData : IComponentData
{
    public enum MoveState
    {
        Arrived,
        Moving
    }
    
    public MoveState State;
}