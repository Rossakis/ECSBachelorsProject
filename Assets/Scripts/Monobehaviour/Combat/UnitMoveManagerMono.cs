using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Combat.Assets.Scripts.Monobehaviour.Combat;
using UnityEngine;

public class UnitMoveManagerMono : MonoBehaviour
{
    // Singleton for easy access (optional)
    public static UnitMoveManagerMono Instance { get; private set; }

    // List of all active units
    private readonly List<UnitMono> units = new List<UnitMono>();

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3? targetPosition;

    public bool IsMoving => targetPosition.HasValue;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Fast for-loop for best performance
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Move();
        }
    }

    // Called by units on enable/disable
    public void RegisterUnit(UnitMono unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);
    }

    public void UnregisterUnit(UnitMono unit)
    {
        units.Remove(unit);
    }
}