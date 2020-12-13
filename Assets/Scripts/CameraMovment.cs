using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    private Transform _CameraTransform;
    private ActionMap map;
    private Camera _Camera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _CameraTransform = this.GetComponent<Transform>();

        map = new ActionMap();

        map.CameraController.Enable();

        map.CameraController.DeltaAxis.performed += DeltaAxis_performed;

        _Camera = Camera.main;
    }

    private void DeltaAxis_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        var val = obj.ReadValue<Vector2>();

        float sensitivity = 0.5f;
        Vector3 vp = _Camera.ScreenToViewportPoint(new Vector3(val.x, val.y, _Camera.nearClipPlane));
        vp.x -= 0.5f;
        vp.y -= 0.5f;
        vp.x *= sensitivity;
        vp.y *= sensitivity;
        vp.x += 0.5f;
        vp.y += 0.5f;
        Vector3 sp = _Camera.ViewportToScreenPoint(vp);

        Vector3 v = _Camera.ScreenToWorldPoint(sp);
        transform.LookAt(v, transform.up);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
