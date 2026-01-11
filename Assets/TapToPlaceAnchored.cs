using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TapToPlaceAnchored : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private GameObject placeablePrefab;

    private static readonly List<ARRaycastHit> hits = new();

    private void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (!raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            return;

        ARRaycastHit hit = hits[0];

        // Get the plane that was hit
        ARPlane plane = planeManager.GetPlane(hit.trackableId);
        if (plane == null)
        {
            Debug.LogWarning("No plane found for hit.");
            return;
        }

        // Attach anchor to the plane
        ARAnchor anchor = anchorManager.AttachAnchor(plane, hit.pose);
        if (anchor == null)
        {
            Debug.LogWarning("Failed to attach anchor.");
            return;
        }

        // Spawn cube and parent it to the anchor
        GameObject spawned = Instantiate(
            placeablePrefab,
            hit.pose.position,
            hit.pose.rotation
        );

        spawned.transform.SetParent(anchor.transform, worldPositionStays: true);
    }
}
