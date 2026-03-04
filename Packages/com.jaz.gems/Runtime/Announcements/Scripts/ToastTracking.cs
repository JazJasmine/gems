
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToastTracking : UdonSharpBehaviour
{
    public TextMeshProUGUI textMesh;

    [Header("Positioning")]
    [SerializeField] float distance = 1.0f;
    [SerializeField] float yOffset = -0.10f;

    [Header("Behavior")]
    [SerializeField] float smoothSpeed = 18f; // higher = snappier

    VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        gameObject.SetActive(false);
    }

    public void Show(string message, float duration = 5f)
    {
        if (localPlayer == null) localPlayer = Networking.LocalPlayer;

        if (textMesh != null)
            textMesh.text = message;

        var head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        // Target position
        Vector3 targetPos =
            head.position +
            head.rotation * Vector3.forward * distance +
            head.rotation * Vector3.up * yOffset;
        transform.position = targetPos;

        Vector3 lookDir = transform.position - head.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = targetRot;
        }

        gameObject.SetActive(true);
        SendCustomEventDelayedSeconds("Dismiss", duration);
    }

    void Update()
    {
        if (localPlayer == null) return;

        var head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        // Target position
        Vector3 targetPos =
            head.position +
            head.rotation * Vector3.forward * distance +
            head.rotation * Vector3.up * yOffset;

        // Smooth movement (exponential smoothing)
        float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, t);

        // Billboard toward head
        Vector3 lookDir = transform.position - head.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
        }
    }

    public void Dismiss()
    {
        gameObject.SetActive(false);
    }
}
