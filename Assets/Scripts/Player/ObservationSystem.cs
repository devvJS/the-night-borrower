using UnityEngine;

public class ObservationSystem : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask interactableLayer;

    private InteractableObject currentObserved;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        HandleObservation();
    }

    private void HandleObservation()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, GameConstants.ObservationRange, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null && interactable.IsImportant)
            {
                if (interactable != currentObserved)
                {
                    if (currentObserved != null)
                    {
                        currentObserved.ObservationUnhighlight();
                    }
                    currentObserved = interactable;
                    currentObserved.ObservationHighlight();
                    GameEvents.ObjectObserved(currentObserved.ObjectId);
                }
                return;
            }
        }

        if (currentObserved != null)
        {
            currentObserved.ObservationUnhighlight();
            currentObserved = null;
        }
    }
}
