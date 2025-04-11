using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

/*
 * Required by this script:
 * - AR Plane Manager in XR Origin
 * - AR Raycast Manager in XR Origin
 * - PressInputBase script
 * - Attach this script to XR Origin
 * - Add the prefab to spawn
 * - Create a new input system called TouchControls with <Pointer>/press as the binding
 */

// Sanity check
[RequireComponent(typeof(ARRaycastManager))]

public class PlaceMultipleObjectsOnPlaneNewInputSystem : MonoBehaviour
{
    // Prefab to instantiate
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    // Newly instantiated object
    GameObject spawnedObject;

    /// <summary>
    /// The input touch control.
    /// </summary>
    TouchControls controls;

    // Raycast manager
    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();
        
        // If there is touch input being performed. call the OnPress function.
        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                OnPress(device.position.ReadValue());
            }
        };
    }

    private void OnEnable()
    {
        controls.control.Enable();
    }

    private void OnDisable()
    {
        controls.control.Disable();
    }

    void OnPress(Vector3 position)
    {
        // Check if the raycast hit any trackables
        if (arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            // Instantiated the prefab
            spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);

            // To make the spawned object always look at the camera. Delete if not needed
            Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
            lookPos.y = 0;
            spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}
