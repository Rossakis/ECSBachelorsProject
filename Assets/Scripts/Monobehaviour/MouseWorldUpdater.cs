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
        _mouseEntity = _entityManager.CreateEntity(typeof(MouseWorldPositionData));
    }

    void Update()
    {
        if(!Input.GetMouseButtonDown(0))
            return;
        
        Ray ray = _gameCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float distance))
            _entityManager.SetComponentData(_mouseEntity, new MouseWorldPositionData { Value = ray.GetPoint(distance) });
        else
            _entityManager.SetComponentData(_mouseEntity, new MouseWorldPositionData { Value = Vector3.zero });
    }
}