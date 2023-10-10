//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Scripts/Inputs/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Main"",
            ""id"": ""f4c6a683-bce8-4163-8309-ff2326d39e51"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""cd22a19f-f300-49e9-8c83-007191f3e377"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveDelta"",
                    ""type"": ""Value"",
                    ""id"": ""6cccdcfc-d44e-4f80-861f-7e7f97f7e40b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""Value"",
                    ""id"": ""fd63394e-b2fc-48ad-8d0a-41797f5853fa"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""c498f511-be82-4f90-af2b-a7061a446641"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TouchZoom"",
                    ""type"": ""Button"",
                    ""id"": ""0f512c74-a3d6-450b-a1d2-f00762c6ad0e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition0"",
                    ""type"": ""Value"",
                    ""id"": ""17331456-182a-4ac8-b4e5-1d188c3b8dad"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TouchPosition1"",
                    ""type"": ""Value"",
                    ""id"": ""952b9e28-5e02-4f1b-a289-479292b6d5a9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PointerPosition"",
                    ""type"": ""Value"",
                    ""id"": ""76f8934b-ed7d-45ac-a4d2-f257e0dc015b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PointerClick"",
                    ""type"": ""Button"",
                    ""id"": ""170bc9fb-c16e-4920-a98f-a7a6817347ca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7a4abc4c-b2e2-4cc4-a5c9-608fb50d9641"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec4c8135-e917-4695-8bd9-74f983e2ba5c"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1a24d40-dd1d-4bfc-8496-ec9b22daf454"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77ba0671-5045-47b1-a3bc-43d77f3c293d"",
                    ""path"": ""<Touchscreen>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20f0ac5e-857d-4594-85d5-c32eea2ac563"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8748a649-de88-4180-a23e-f46335b00ed1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""One Modifier"",
                    ""id"": ""5dd98af0-4798-49ae-a3b9-8585ca2b6775"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""dd7d0d9c-aa50-4e39-87a5-d7a88d9f9ede"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""133d46a2-b27a-4daf-9dc9-86d803e4a445"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d0af7efb-51c7-48c6-84d0-8e20475596b2"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4a6d367-28a5-40ac-8157-1c819c2ce3e0"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""714e75d8-7c55-4bb8-9d1e-5ea0dfb9519a"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7a8cf98-0c63-4ab5-8dce-21aa70557b98"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""72d4a412-0326-4993-8d38-438c6c21ea55"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53be77ed-c4f4-4bbc-8c15-5d00ae591d21"",
                    ""path"": ""<Touchscreen>/primaryTouch/tap"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Main
        m_Main = asset.FindActionMap("Main", throwIfNotFound: true);
        m_Main_Move = m_Main.FindAction("Move", throwIfNotFound: true);
        m_Main_MoveDelta = m_Main.FindAction("MoveDelta", throwIfNotFound: true);
        m_Main_MouseScroll = m_Main.FindAction("MouseScroll", throwIfNotFound: true);
        m_Main_MousePosition = m_Main.FindAction("MousePosition", throwIfNotFound: true);
        m_Main_TouchZoom = m_Main.FindAction("TouchZoom", throwIfNotFound: true);
        m_Main_TouchPosition0 = m_Main.FindAction("TouchPosition0", throwIfNotFound: true);
        m_Main_TouchPosition1 = m_Main.FindAction("TouchPosition1", throwIfNotFound: true);
        m_Main_PointerPosition = m_Main.FindAction("PointerPosition", throwIfNotFound: true);
        m_Main_PointerClick = m_Main.FindAction("PointerClick", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Main
    private readonly InputActionMap m_Main;
    private List<IMainActions> m_MainActionsCallbackInterfaces = new List<IMainActions>();
    private readonly InputAction m_Main_Move;
    private readonly InputAction m_Main_MoveDelta;
    private readonly InputAction m_Main_MouseScroll;
    private readonly InputAction m_Main_MousePosition;
    private readonly InputAction m_Main_TouchZoom;
    private readonly InputAction m_Main_TouchPosition0;
    private readonly InputAction m_Main_TouchPosition1;
    private readonly InputAction m_Main_PointerPosition;
    private readonly InputAction m_Main_PointerClick;
    public struct MainActions
    {
        private @Controls m_Wrapper;
        public MainActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Main_Move;
        public InputAction @MoveDelta => m_Wrapper.m_Main_MoveDelta;
        public InputAction @MouseScroll => m_Wrapper.m_Main_MouseScroll;
        public InputAction @MousePosition => m_Wrapper.m_Main_MousePosition;
        public InputAction @TouchZoom => m_Wrapper.m_Main_TouchZoom;
        public InputAction @TouchPosition0 => m_Wrapper.m_Main_TouchPosition0;
        public InputAction @TouchPosition1 => m_Wrapper.m_Main_TouchPosition1;
        public InputAction @PointerPosition => m_Wrapper.m_Main_PointerPosition;
        public InputAction @PointerClick => m_Wrapper.m_Main_PointerClick;
        public InputActionMap Get() { return m_Wrapper.m_Main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
        public void AddCallbacks(IMainActions instance)
        {
            if (instance == null || m_Wrapper.m_MainActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MainActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @MoveDelta.started += instance.OnMoveDelta;
            @MoveDelta.performed += instance.OnMoveDelta;
            @MoveDelta.canceled += instance.OnMoveDelta;
            @MouseScroll.started += instance.OnMouseScroll;
            @MouseScroll.performed += instance.OnMouseScroll;
            @MouseScroll.canceled += instance.OnMouseScroll;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
            @TouchZoom.started += instance.OnTouchZoom;
            @TouchZoom.performed += instance.OnTouchZoom;
            @TouchZoom.canceled += instance.OnTouchZoom;
            @TouchPosition0.started += instance.OnTouchPosition0;
            @TouchPosition0.performed += instance.OnTouchPosition0;
            @TouchPosition0.canceled += instance.OnTouchPosition0;
            @TouchPosition1.started += instance.OnTouchPosition1;
            @TouchPosition1.performed += instance.OnTouchPosition1;
            @TouchPosition1.canceled += instance.OnTouchPosition1;
            @PointerPosition.started += instance.OnPointerPosition;
            @PointerPosition.performed += instance.OnPointerPosition;
            @PointerPosition.canceled += instance.OnPointerPosition;
            @PointerClick.started += instance.OnPointerClick;
            @PointerClick.performed += instance.OnPointerClick;
            @PointerClick.canceled += instance.OnPointerClick;
        }

        private void UnregisterCallbacks(IMainActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @MoveDelta.started -= instance.OnMoveDelta;
            @MoveDelta.performed -= instance.OnMoveDelta;
            @MoveDelta.canceled -= instance.OnMoveDelta;
            @MouseScroll.started -= instance.OnMouseScroll;
            @MouseScroll.performed -= instance.OnMouseScroll;
            @MouseScroll.canceled -= instance.OnMouseScroll;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
            @TouchZoom.started -= instance.OnTouchZoom;
            @TouchZoom.performed -= instance.OnTouchZoom;
            @TouchZoom.canceled -= instance.OnTouchZoom;
            @TouchPosition0.started -= instance.OnTouchPosition0;
            @TouchPosition0.performed -= instance.OnTouchPosition0;
            @TouchPosition0.canceled -= instance.OnTouchPosition0;
            @TouchPosition1.started -= instance.OnTouchPosition1;
            @TouchPosition1.performed -= instance.OnTouchPosition1;
            @TouchPosition1.canceled -= instance.OnTouchPosition1;
            @PointerPosition.started -= instance.OnPointerPosition;
            @PointerPosition.performed -= instance.OnPointerPosition;
            @PointerPosition.canceled -= instance.OnPointerPosition;
            @PointerClick.started -= instance.OnPointerClick;
            @PointerClick.performed -= instance.OnPointerClick;
            @PointerClick.canceled -= instance.OnPointerClick;
        }

        public void RemoveCallbacks(IMainActions instance)
        {
            if (m_Wrapper.m_MainActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMainActions instance)
        {
            foreach (var item in m_Wrapper.m_MainActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MainActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MainActions @Main => new MainActions(this);
    public interface IMainActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMoveDelta(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnTouchZoom(InputAction.CallbackContext context);
        void OnTouchPosition0(InputAction.CallbackContext context);
        void OnTouchPosition1(InputAction.CallbackContext context);
        void OnPointerPosition(InputAction.CallbackContext context);
        void OnPointerClick(InputAction.CallbackContext context);
    }
}
