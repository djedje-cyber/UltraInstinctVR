using UnityEngine;

public class ObjectTeleporter : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private Rigidbody rb;

    private void Awake()
    {
        if (targetObject != null)
            rb = targetObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Teleport to a Vector3 position.
    /// </summary>
    public void Teleport(Vector3 destination)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("ObjectTeleporter: No target object assigned.");
            return;
        }

        ApplyTeleport(destination, targetObject.transform.rotation);
    }

    /// <summary>
    /// Teleport to a Vector3 position with a specific rotation.
    /// </summary>
    public void Teleport(Vector3 destination, Quaternion rotation)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("ObjectTeleporter: No target object assigned.");
            return;
        }

        ApplyTeleport(destination, rotation);
    }

    /// <summary>
    /// Teleport to another Transform's position and rotation.
    /// </summary>
    public void TeleportToTransform(Transform destination)
    {
        if (targetObject == null || destination == null)
        {
            Debug.LogWarning("ObjectTeleporter: Missing target or destination.");
            return;
        }

        ApplyTeleport(destination.position, destination.rotation);
    }

    /// <summary>
    /// Teleport a specific GameObject to a destination (without using the serialized target).
    /// </summary>
    public void TeleportObject(GameObject obj, Vector3 destination)
    {
        if (obj == null)
        {
            Debug.LogWarning("ObjectTeleporter: No object provided.");
            return;
        }

        Rigidbody objRb = obj.GetComponent<Rigidbody>();

        if (objRb != null)
        {
            objRb.MovePosition(destination);
            objRb.linearVelocity = Vector3.zero;
            objRb.angularVelocity = Vector3.zero;
        }
        else
        {
            obj.transform.position = destination;
        }
    }

    /// <summary>
    /// Core teleport logic — handles Rigidbody or Transform.
    /// </summary>
    private void ApplyTeleport(Vector3 destination, Quaternion rotation)
    {
        if (rb != null)
        {
            rb.MovePosition(destination);
            rb.MoveRotation(rotation);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            targetObject.transform.position = destination;
            targetObject.transform.rotation = rotation;
        }

        Debug.Log($"ObjectTeleporter: {targetObject.name} teleported to {destination}");
    }
}