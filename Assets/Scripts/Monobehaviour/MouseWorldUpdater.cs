using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MouseWorldUpdater : MonoBehaviour
{
    private Camera _gameCamera;
    private EntityManager _entityManager;
    private Entity _mouseEntity;

    void Start()
    {
        _gameCamera = Camera.main;
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _mouseEntity = _entityManager.CreateEntity(typeof(MouseWorldPosition));
    }

    void Update()
    {
        if(!Input.GetMouseButtonDown(0))
            return;
        
        Vector3 mouseScreen = Input.mousePosition;
        Ray ray = _gameCamera.ScreenPointToRay(mouseScreen);
        
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            float3 worldPos = new float3(hit.x, hit.y, hit.z); // ignore y-axis
            
            if (_entityManager.Exists(_mouseEntity))
            {
                _entityManager.SetComponentData(_mouseEntity, new MouseWorldPosition { Value = worldPos });
            }
        }
    }
}