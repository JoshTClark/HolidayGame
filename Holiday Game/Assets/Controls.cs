//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.1
//     from Assets/Controls.inputactions
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

public partial class @Controls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Game Map"",
            ""id"": ""677951a5-5dea-4386-9d90-1b606f84079c"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""dac57d7d-332c-4a93-95fd-2d2f6b897675"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""DisplayStats"",
                    ""type"": ""Button"",
                    ""id"": ""279bea6a-b13f-4035-a703-88df1ee7fef0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""7012a19f-5575-4558-8cf9-f05c681bd50a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ButtonThatGivesXP"",
                    ""type"": ""Button"",
                    ""id"": ""e2aabd56-83cc-471b-acf0-c93d7db5a29a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""b603fbb3-dae8-488b-8d53-2688a59cc334"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d555ae6d-62e3-4df8-b9d1-e616189cc4ea"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""356bfb04-d2ae-4108-a688-e1d44b177b90"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""299139a2-d22a-449d-a35a-230e808340f9"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6c9fcef8-1d97-4e30-80e9-1b4e6b23cacc"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""262c2ea6-a51a-4af3-8664-d89de4a08293"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""607ac337-401f-4b95-ac85-82d38fe240f6"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""172ca7dd-57c6-41d4-b4ef-c7dc4bae4c4b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8143d9a4-d6f7-4230-a702-9785a71b127b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2d433bfe-98f9-4d8b-86f6-9fb7593af8e3"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""613daaa0-8846-47d1-98d7-70d3b2f6895c"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DisplayStats"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""602d9fd0-15b0-4a98-8a98-7f443bfa815e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""306073dc-7beb-499b-a7aa-867a906ad112"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ButtonThatGivesXP"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Game Map
        m_GameMap = asset.FindActionMap("Game Map", throwIfNotFound: true);
        m_GameMap_Movement = m_GameMap.FindAction("Movement", throwIfNotFound: true);
        m_GameMap_DisplayStats = m_GameMap.FindAction("DisplayStats", throwIfNotFound: true);
        m_GameMap_Pause = m_GameMap.FindAction("Pause", throwIfNotFound: true);
        m_GameMap_ButtonThatGivesXP = m_GameMap.FindAction("ButtonThatGivesXP", throwIfNotFound: true);
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

    // Game Map
    private readonly InputActionMap m_GameMap;
    private IGameMapActions m_GameMapActionsCallbackInterface;
    private readonly InputAction m_GameMap_Movement;
    private readonly InputAction m_GameMap_DisplayStats;
    private readonly InputAction m_GameMap_Pause;
    private readonly InputAction m_GameMap_ButtonThatGivesXP;
    public struct GameMapActions
    {
        private @Controls m_Wrapper;
        public GameMapActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_GameMap_Movement;
        public InputAction @DisplayStats => m_Wrapper.m_GameMap_DisplayStats;
        public InputAction @Pause => m_Wrapper.m_GameMap_Pause;
        public InputAction @ButtonThatGivesXP => m_Wrapper.m_GameMap_ButtonThatGivesXP;
        public InputActionMap Get() { return m_Wrapper.m_GameMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameMapActions set) { return set.Get(); }
        public void SetCallbacks(IGameMapActions instance)
        {
            if (m_Wrapper.m_GameMapActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_GameMapActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_GameMapActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_GameMapActionsCallbackInterface.OnMovement;
                @DisplayStats.started -= m_Wrapper.m_GameMapActionsCallbackInterface.OnDisplayStats;
                @DisplayStats.performed -= m_Wrapper.m_GameMapActionsCallbackInterface.OnDisplayStats;
                @DisplayStats.canceled -= m_Wrapper.m_GameMapActionsCallbackInterface.OnDisplayStats;
                @Pause.started -= m_Wrapper.m_GameMapActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameMapActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameMapActionsCallbackInterface.OnPause;
                @ButtonThatGivesXP.started -= m_Wrapper.m_GameMapActionsCallbackInterface.OnButtonThatGivesXP;
                @ButtonThatGivesXP.performed -= m_Wrapper.m_GameMapActionsCallbackInterface.OnButtonThatGivesXP;
                @ButtonThatGivesXP.canceled -= m_Wrapper.m_GameMapActionsCallbackInterface.OnButtonThatGivesXP;
            }
            m_Wrapper.m_GameMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @DisplayStats.started += instance.OnDisplayStats;
                @DisplayStats.performed += instance.OnDisplayStats;
                @DisplayStats.canceled += instance.OnDisplayStats;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @ButtonThatGivesXP.started += instance.OnButtonThatGivesXP;
                @ButtonThatGivesXP.performed += instance.OnButtonThatGivesXP;
                @ButtonThatGivesXP.canceled += instance.OnButtonThatGivesXP;
            }
        }
    }
    public GameMapActions @GameMap => new GameMapActions(this);
    public interface IGameMapActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnDisplayStats(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnButtonThatGivesXP(InputAction.CallbackContext context);
    }
}