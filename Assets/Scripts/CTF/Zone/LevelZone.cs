using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelZone : MonoBehaviour
{
    [Header("Components")]
    public SphereCollider sphereCollider;

    [Header("Settings")]
    public float radius;
    [Range(0, 1)] public int teamIndex = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = teamIndex == 0 ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        UpdateRadius();
    }

    private void Awake()
    {
        UpdateRadius();
    }

    private void OnEnable()
    {
        CTFManager.Instance.RegisterZone(this);
    }

    private void OnDisable()
    {
        if (CTFManager.Instance != null)
            CTFManager.Instance.UnregisterZone(this);
    }

    private void OnTriggerStay(Collider other)
    {
        Character character;
        if (other.gameObject.TryGetComponent(out character))
        {
            OnCharacterStay(character);
        }
    }

    public virtual void OnCharacterStay(Character character)
    {

    }

    private void UpdateRadius()
    {
        sphereCollider.radius = radius;
    }
}
