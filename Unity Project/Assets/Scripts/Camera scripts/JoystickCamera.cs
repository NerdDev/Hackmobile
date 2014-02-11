//Filename: maxCamera.cs
//
// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
//
// --01-18-2010 - create temporary target, if none supplied at start

using UnityEngine;
using System.Collections;


[AddComponentMenu("Camera-Control/JoystickCamera")]
public class JoystickCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    internal float xDeg = 0.0f;
    private float yDeg = 44.5f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion startRotation;
    private Quaternion rotation;
    private Vector3 position;
    bool useMouse = true;
    bool useTouch = true;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init()
    {
        #region Backup
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }
        #endregion

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        startRotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

#if !UNITY_EDITOR
        useMouse = false;
#endif
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        #region Mouse Controls
        if (useMouse)
        {
            // If Control and Alt and Middle button? ZOOM!
            if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
            {
                desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
            }
            // If middle mouse and left alt are selected? ORBIT
            else if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                ////////OrbitAngle
                //Clamp the vertical axis for the orbit
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                // set camera rotation 
                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                currentRotation = transform.rotation;

                rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = rotation;
            }

            ////////Orbit Position
            // affect the desired Zoom distance if we roll the scrollwheel
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
            //clamp the zoom min/max
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
            // For smoothing of the zoom, lerp distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

            // calculate position based on the new currentDistance 
            position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
            transform.position = position;
        }
        #endregion
    }

    public void Rotate(float x, float y)
    {
        xDeg += x * xSpeed * 0.02f;
        yDeg -= y * ySpeed * 0.02f;

        ////////OrbitAngle
        //Clamp the vertical axis for the orbit
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        // set camera rotation 
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;

        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;
    }

    public void Reset()
    {
        rotation = startRotation;
        xDeg = 0;
        yDeg = 44.5f;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void zoom(float f)
    {
        desiredDistance -= f * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance) * .025f;
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }
}