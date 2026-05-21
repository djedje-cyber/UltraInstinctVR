using UnityEngine;

public class ControllerSelector : MonoBehaviour
{
    [Header("Controller Settings")]
    public Transform controllerTransform;     // Drag your VR controller here
    public float rayDistance = 10f;
    public LayerMask selectableLayers;        // Set which objects can be selected

    [Header("Visual Feedback")]
    public Material highlightMaterial;        // Material when hovering
    public Material selectedMaterial;         // Material when selected

    private GameObject hoveredObject;
    private GameObject selectedObject;
    private Material originalMaterial;

    void Update()
    {
        RaycastForObject();
    }

    void RaycastForObject()
    {
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit;

        // Draw ray in editor for debugging
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, rayDistance, selectableLayers))
        {
            GameObject target = hit.collider.gameObject;

            // New object being hovered
            if (target != hoveredObject)
            {
                ClearHover();
                hoveredObject = target;
                SetMaterial(hoveredObject, highlightMaterial);
            }
        }
        else
        {
            ClearHover();
        }
    }

    // Called by BERT when "Put that" is recognized
    public void SelectHoveredObject()
    {
        if (hoveredObject != null)
        {
            // Deselect previous
            if (selectedObject != null)
                ClearSelection();

            selectedObject = hoveredObject;
            SetMaterial(selectedObject, selectedMaterial);

            Debug.Log("Selected: " + selectedObject.name);

            // Notify other systems
            GameObjectSelected(selectedObject);
        }
        else
        {
            Debug.Log("No object being pointed at!");
        }
    }

    public GameObject GetSelectedObject() => selectedObject;

    void GameObjectSelected(GameObject obj)
    {
        // You can add more logic here:
        // obj.GetComponent<Rigidbody>().isKinematic = true;
        // obj.tag = "Selected";
    }

    void ClearHover()
    {
        if (hoveredObject != null && hoveredObject != selectedObject)
            SetMaterial(hoveredObject, originalMaterial);
        hoveredObject = null;
    }

    public void ClearSelection()
    {
        if (selectedObject != null)
            SetMaterial(selectedObject, originalMaterial);
        selectedObject = null;
    }

    void SetMaterial(GameObject obj, Material mat)
    {
        if (mat == null) return;
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            renderer.material = mat;
        }
    }
}