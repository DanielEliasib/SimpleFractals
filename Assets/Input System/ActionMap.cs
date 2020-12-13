// GENERATED AUTOMATICALLY FROM 'Assets/Input System/ActionMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ActionMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ActionMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ActionMap"",
    ""maps"": [
        {
            ""name"": ""CameraController"",
            ""id"": ""a7fbb3a0-f85d-4f67-a359-81e0a41c6455"",
            ""actions"": [
                {
                    ""name"": ""DeltaAxis"",
                    ""type"": ""Value"",
                    ""id"": ""0d79865f-d234-47d3-a2de-b7fc63022d8f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7e8486cd-c425-4580-9e77-eabd34dacf6b"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DeltaAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CameraController
        m_CameraController = asset.FindActionMap("CameraController", throwIfNotFound: true);
        m_CameraController_DeltaAxis = m_CameraController.FindAction("DeltaAxis", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // CameraController
    private readonly InputActionMap m_CameraController;
    private ICameraControllerActions m_CameraControllerActionsCallbackInterface;
    private readonly InputAction m_CameraController_DeltaAxis;
    public struct CameraControllerActions
    {
        private @ActionMap m_Wrapper;
        public CameraControllerActions(@ActionMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @DeltaAxis => m_Wrapper.m_CameraController_DeltaAxis;
        public InputActionMap Get() { return m_Wrapper.m_CameraController; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraControllerActions set) { return set.Get(); }
        public void SetCallbacks(ICameraControllerActions instance)
        {
            if (m_Wrapper.m_CameraControllerActionsCallbackInterface != null)
            {
                @DeltaAxis.started -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnDeltaAxis;
                @DeltaAxis.performed -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnDeltaAxis;
                @DeltaAxis.canceled -= m_Wrapper.m_CameraControllerActionsCallbackInterface.OnDeltaAxis;
            }
            m_Wrapper.m_CameraControllerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DeltaAxis.started += instance.OnDeltaAxis;
                @DeltaAxis.performed += instance.OnDeltaAxis;
                @DeltaAxis.canceled += instance.OnDeltaAxis;
            }
        }
    }
    public CameraControllerActions @CameraController => new CameraControllerActions(this);
    public interface ICameraControllerActions
    {
        void OnDeltaAxis(InputAction.CallbackContext context);
    }
}
