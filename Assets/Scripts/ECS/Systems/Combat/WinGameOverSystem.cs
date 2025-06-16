using Unity.Entities;
using Unity.VisualScripting;

partial struct WinGameOverSystem : ISystem {
    
    private EntityQuery knightUnitsEntityQuery;
    private EntityQuery wizardUnitsEntityQuery;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<GameSceneTag>();

        knightUnitsEntityQuery = state.GetEntityQuery(typeof(Unit), typeof(Knight));
        wizardUnitsEntityQuery = state.GetEntityQuery(typeof(Unit), typeof(Wizard));
    }

    // We can't use [BurstCompile] here because DOTSEventsManager is not an Unhandled type of data
    public void OnUpdate(ref SystemState state) {
        if (knightUnitsEntityQuery.CalculateEntityCount() == 0) {
            DOTSEventsManager.Instance.GameWin();
            return;
        }
        
        if (wizardUnitsEntityQuery.CalculateEntityCount() == 0) {
            DOTSEventsManager.Instance.GameOver();
        }
    }

}