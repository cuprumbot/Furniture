using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

// For button interactions
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


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

public class ObjectManager : MonoBehaviour
{
    // We get the placeable prefab from here
    public ModelMenu modelMenu;

    // For button interactions 
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GraphicRaycaster uiRaycaster;

    // DEBUG
    public TextMeshProUGUI statusText;

    // Newly instantiated object
    GameObject spawnedObject;

    TouchControls controls;

    // Raycast manager
    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Object to move
    private GameObject selectedObject = null;

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

        controls = new TouchControls();

        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                Vector2 pos = device.position.ReadValue();

                // Add this check to skip if touching UI
                if (IsTouchOverUI(pos))
                    return;

                OnPress(pos);
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
        if (ModeHandler.mode == Mode.Place)
        {
            placeObject(position);
        } 
        else if (ModeHandler.mode == Mode.Move)
        {
            moveObject(position);
        }
        else if (ModeHandler.mode == Mode.Delete)
        {
            deleteObject(position);
        }
        else if (ModeHandler.mode == Mode.Rotate)
        {
            rotateObjectPlaneless(position);
        }
    }

    private void placeObject (Vector3 position)
    {
        // Check if the raycast hit any trackables
        if (arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            // Instantiated the prefab
            spawnedObject = Instantiate(modelMenu.prefabToPlace, hitPose.position, hitPose.rotation);

            // To make the spawned object always look at the camera. Delete if not needed
            Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
            lookPos.y = 0;
            spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

    private void moveObject(Vector3 position)
    {
        if (selectedObject == null)
        {
            // STEP 1: Select object
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;

            LayerMask mask = LayerMask.GetMask("Default");
            if (Physics.Raycast(ray, out hit, 10f, mask))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("PlacedObject"))
                {
                    selectedObject = hitObject;
                    statusText.text = "Selected object";

                    selectedObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    statusText.text = "Bad hit [ Obj: " + hitObject.name + " ] [ Lay: " + LayerMask.LayerToName(hitObject.layer) + " ]";
                }
            }
            else
            {
                statusText.text = "No raycast hit";
            }
        }
        else
        {
            // STEP 2: Move selected object
            if (arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                selectedObject.transform.position = hitPose.position;

                // In case we need to reset the rotation of the object
                // Vector3 lookPos = Camera.main.transform.position - selectedObject.transform.position;
                // lookPos.y = 0;
                // selectedObject.transform.rotation = Quaternion.LookRotation(lookPos);

                statusText.text = "Moved object";

                selectedObject.GetComponent<Outline>().enabled = false;

                // Done moving
                selectedObject = null;
            }
            else
            {
                statusText.text = "No raycast hit";
            }
        }
    }

    private void rotateObject(Vector3 position)
    {
        if (selectedObject == null)
        {
            // STEP 1: Select object
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;

            LayerMask mask = LayerMask.GetMask("Default");
            if (Physics.Raycast(ray, out hit, 10f, mask))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("PlacedObject"))
                {
                    selectedObject = hitObject;
                    statusText.text = "Selected for rotation";

                    selectedObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    statusText.text = "Bad hit [ Obj: " + hitObject.name + " ] [ Lay: " + LayerMask.LayerToName(hitObject.layer) + " ]";
                }
            }
            else
            {
                statusText.text = "No raycast hit";
            }
        }
        else
        {
            // STEP 2: Raycast to get world position on plane
            if (arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Compute direction to look at
                Vector3 lookDirection = hitPose.position - selectedObject.transform.position;
                lookDirection.y = 0f; // ignore vertical difference
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    selectedObject.transform.rotation = Quaternion.LookRotation(lookDirection);
                    statusText.text = "Rotated object";
                }
                else
                {
                    statusText.text = "Tap farther to rotate";
                }

                //selectedObject.GetComponent<Outline>().enabled = false;
                //selectedObject = null;
            }
            else
            {
                statusText.text = "No plane hit";
            }

            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }

    private void rotateObjectPlaneless(Vector3 position)
    {
        if (selectedObject == null)
        {
            // STEP 1: Select object
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit;

            LayerMask mask = LayerMask.GetMask("Default");
            if (Physics.Raycast(ray, out hit, 10f, mask))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.CompareTag("PlacedObject"))
                {
                    selectedObject = hitObject;
                    statusText.text = "Selected for rotation";

                    selectedObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    statusText.text = "Bad hit [ Obj: " + hitObject.name + " ] [ Lay: " + LayerMask.LayerToName(hitObject.layer) + " ]";
                }
            }
            else
            {
                statusText.text = "No raycast hit";
            }
        }
        else
        {
            Vector3 targetPosition;

            // Try AR raycast first
            if (arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
            {
                targetPosition = hits[0].pose.position;
                statusText.text = "Rotating using arRaycastManager";
            }
            else
            {
                // Fallback: use physics raycast onto Default layer
                Ray ray = Camera.main.ScreenPointToRay(position);
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("Default");

                if (Physics.Raycast(ray, out hit, 10f, mask))
                {
                    targetPosition = hit.point;
                    statusText.text = "Rotating using Physics Raycast";
                }
                else
                {
                    // Final fallback: project to some fixed Y plane (e.g., ground level)
                    //Plane ground = new Plane(Vector3.up, Vector3.zero);
                    Plane ground = new Plane(Vector3.up, selectedObject.transform.position);

                    float distance;
                    if (ground.Raycast(ray, out distance))
                    {
                        targetPosition = ray.GetPoint(distance);
                        statusText.text = "Rotating using Plane Raycast";
                    }
                    else
                    {
                        statusText.text = "No valid direction from tap";
                        return;
                    }
                }
            }

            // Calculate horizontal look direction
            Vector3 lookDirection = targetPosition - selectedObject.transform.position;
            lookDirection.y = 0;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                selectedObject.transform.rotation = Quaternion.LookRotation(lookDirection);
                statusText.text = "Rotated object";
            }
            else
            {
                statusText.text = "Tap farther to rotate";
            }

            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }

    private void deleteObject(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        // Make sure it doesn't hit UI layer
        // and that the ray is long enough
        LayerMask mask = LayerMask.GetMask("Default");
        if (Physics.Raycast(ray, out hit, 10f, mask))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if the object has the "PlacedObject" tag
            if (hitObject != null && hitObject.CompareTag("PlacedObject"))
            {
                Destroy(hitObject);
                statusText.text = "Deleted object";
            }
            else
            {
                 statusText.text = "Bad hit [ Obj: " + hitObject.name + " ] [ Lay: " + LayerMask.LayerToName(hitObject.layer) + " ]";
            }
        }
        else
        {
            statusText.text = "No raycast hit";
        }
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerData, results);

        // Debug text no longer needed
        //statusText.text = "Count: " + results.Count;

        return results.Count > 0;
    }
}
