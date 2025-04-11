using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

public class PlaceOnPlane : PressInputBase
{
    // Prefab to instantiate
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;

    // Newly instantiated object
    GameObject spawnedObject;

    // Is there any touch input?
    bool isPressed;

    // Raycast manager
    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    protected override void Awake()
    {
        base.Awake();
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is any pointer device connected to the system
        // or if there is existing touch input
        if (Pointer.current == null || isPressed == false)
            return;



        // Check if there's a valid touch
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
            return;

        var touch = Touchscreen.current.primaryTouch;

        // This is the fix: use the touch's deviceId as the pointerId
        int pointerId = touch.touchId.ReadValue();

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId))
            return;




        // Store the current touch position
        var touchPosition = Pointer.current.position.ReadValue();

        // Check if the raycast hit any trackables
        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            // Check if there is already spawned object. If there is none, instantiated the prefab
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                // Change the spawned object position and rotation to the touch position
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }

            // Make the spawned object always look at the camera
            Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
            lookPos.y = 0;
            spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel() => isPressed = false;
}
