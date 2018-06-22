using UnityEngine;
using System.Collections;

public class ScoutCamera : MonoBehaviour
{

    public float distance = 7.0f;
    public float height = 3.0f;
    public float heightSmoothlag = 0.3f;
    public Vector3 centerOffset = Vector3.zero;
    public bool followOnStart = false;

    private Transform cameraTransform;
    private bool isFollowing;
    private float heightvelocity = 0.0f;
    private float targetHeight = 100000.0f;

    // Use this for initialization
    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }
        if (isFollowing)
        {
            Apply();
        }
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }

    void Apply()
    {
        Vector3 targetCenter = transform.position + centerOffset;

        float originalTargetAngle = transform.eulerAngles.y;
        float currentangle = cameraTransform.eulerAngles.y;

        float targetAngle = originalTargetAngle;

        currentangle = targetAngle;

        targetHeight = targetCenter.y + height;

        float currentHeight = cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightvelocity, heightSmoothlag);

        Quaternion currentRotation = Quaternion.Euler(0, currentangle, 0);

        cameraTransform.position = targetCenter;
        cameraTransform.position += currentRotation * Vector3.back * distance;

        cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);

        SetUpRotation(targetCenter);

    }

    void Cut()
    {
        float oldHeightSmooth = heightSmoothlag;
        heightSmoothlag = 0.001f;

        Apply();

        heightSmoothlag = oldHeightSmooth;
    }

    void SetUpRotation(Vector3 centerPos)
    {
        Vector3 cameraPos = cameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;

        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
    }
}
