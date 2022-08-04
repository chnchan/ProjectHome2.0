using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

// NOTE: this allows movement onto area without teleportation area. Disable script to
//       use normal movement from Valem instead.

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider provider;
    private bool _isActive = false;
    private InputAction _thumbstick;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;

        var activate = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.performed += onTeleportActivate;

        var cancel = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Teleport Mode Cancel");
        cancel.Enable();
        cancel.performed += onTeleportCancel;

        _thumbstick = actionAsset.FindActionMap("XRI LeftHand Locomotion").FindAction("Move");
        _thumbstick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Teleport is not activated
        if (!_isActive)
            return;

        // User is still pushing the thumbstick forward, not let go yet.
        if (_thumbstick.triggered)
            return;

        // Ray hit something invalid
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            rayInteractor.enabled = false;
            _isActive = false;
            return;
        }

        Debug.Log("valid hit!" + hit.point);

        // Valid ray hit, proceed with teleport request
        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = hit.point,
            // destinationRotation = ?,
        };

        provider.QueueTeleportRequest(request);
    }

    private void onTeleportActivate(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = true;
        _isActive = true;
    }

    private void onTeleportCancel(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = false;
        _isActive = false;
    }
}
