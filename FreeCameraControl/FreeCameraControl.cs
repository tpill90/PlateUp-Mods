#region usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cinemachine;
using FreeCameraControl.Properties;
using JetBrains.Annotations;
using Kitchen;
using KitchenMods;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;
using static FreeCameraControl.LoggingUtils;

#endregion

namespace FreeCameraControl
{
    //TODO Resetting the camera doesn't always work
    //TODO camera should proibably be reset when the scene changes
    //TODO make it so only the player who clicked the stick will be able to move the camera
    [UsedImplicitly]
    public class CameraPlus : GenericSystemBase, IModSystem
    {
        private InputAction _toggleStaticCameraAction;
        private InputAction _positionCameraOnPlayerAction;
        private InputAction _resetCameraPositionAction;
        private InputAction _zoomAction;
        private InputAction _panAction;

        // When this is set to true, the user will be able to move the camera
        private bool _freeMoveCameraEnabled;

        // This is the action used by Plate Up to move the characters
        private List<InputAction> _movePlayerActions;

        protected override void Initialise()
        {
            LogWarning($"v{ModInfo.MOD_VERSION} in use!");
            InitKeybindings();
        }

        private void InitKeybindings()
        {
            // Toggle static camera - Resets camera back to default
            _toggleStaticCameraAction = new InputAction(nameof(ToggleFreeMoveCamera), InputActionType.Button, "<Gamepad>/rightStickPress/");
            _toggleStaticCameraAction.performed += context =>
            {
                ToggleFreeMoveCamera();
            };
            _toggleStaticCameraAction.Enable();

            // Centers the camera on the current player
            _positionCameraOnPlayerAction = new InputAction(nameof(PositionCameraOnPlayer), InputActionType.Button, "<Gamepad>/rightShoulder/");
            _positionCameraOnPlayerAction.performed += context =>
            {
                PositionCameraOnPlayer();
            };
            _positionCameraOnPlayerAction.Enable();

            // Resets the camera back to its original position
            _resetCameraPositionAction = new InputAction("Reset Camera", InputActionType.Button, "<Gamepad>/leftShoulder/");
            _resetCameraPositionAction.performed += context =>
            {
                ResetCamera();
            };
            _resetCameraPositionAction.Enable();

            // Zoom
            _zoomAction = new InputAction("CameraZoom", InputActionType.Button, "<Gamepad>/rightStick/y");
            _zoomAction.Enable();

            // Pan
            _panAction = new InputAction("CameraPan", InputActionType.Button, "<Gamepad>/leftStick");
            _panAction.Enable();
        }

        protected override void OnUpdate()
        {
            if (!_freeMoveCameraEnabled)
            {
                return;
            }

            UpdateZoom();
            UpdatePan();
        }

        /// <summary>
        /// Resets the camera position back to the initial starting point
        /// </summary>
        private void ResetCamera()
        {
            if (!_freeMoveCameraEnabled)
            {
                return;
            }

            var mainCamera = Camera.main;

            // Getting the default camera position by disabling manual control of the camera, which will snap it back to the original position
            Camera.main.GetComponent<CinemachineBrain>().enabled = true;
            var originalPosition = mainCamera.transform.position;
            var originalFov = mainCamera.fieldOfView;

            // Re-enabling our control of the camera
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            mainCamera.transform.position = originalPosition;
            mainCamera.fieldOfView = originalFov;
        }

        private void ToggleFreeMoveCamera()
        {
            // Need to disable this so we can control the camera ourselves
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            _freeMoveCameraEnabled = !_freeMoveCameraEnabled;

            if (_freeMoveCameraEnabled)
            {
                // Finding the existing PlateUp action that handles player movement, so we can disable it later
                _movePlayerActions = InputSystem.ListEnabledActions().Where(e => e.name == "Movement").ToList();
                foreach (var action in _movePlayerActions)
                {
                    action.Disable();
                }
            }
            else
            {
                foreach (var action in _movePlayerActions)
                {
                    action.Enable();
                }
            }
        }

        private void UpdateZoom()
        {
            // Inverting controls so that pushing up is zoom in
            float controllerScrollInput = _zoomAction.ReadValue<float>() * -1;

            var newFov = Camera.main.fieldOfView;
            newFov += controllerScrollInput * .10f;

            Camera.main.fieldOfView = Mathf.Clamp(newFov, 3.0f, 69.0f);
        }

        private void UpdatePan()
        {
            var controllerInput = _panAction.ReadValue<Vector2>();

            var cameraPosition = Camera.main.transform.position;
            cameraPosition.x += controllerInput.x * .10f;
            cameraPosition.z += controllerInput.y * .10f;

            Camera.main.transform.position = cameraPosition;
        }

        private void PositionCameraOnPlayer()
        {
            if (!_freeMoveCameraEnabled)
            {
                return;
            }


            //PlayerInfo player

            // Find local player ID
            int LocalID = 0;

            foreach (PlayerInfo info in Players.Main.All())
            {
                if (info.IsLocalUser)
                {
                    LocalID = info.ID;
                    LogInfo($"Player {info.ID} {info.Name}");
                    break;
                }
            }

            PlayerView[] PlayerViewArray = Object.FindObjectsOfType<PlayerView>();

            foreach (PlayerView view in PlayerViewArray)
            {
                int ID = ((PlayerView.ViewData)typeof(PlayerView).GetField("Data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view)).PlayerID;

                if (ID != LocalID)
                {
                    continue;
                }

                Vector3 ViewPosition = view.transform.position;

                // default 35.82595
                float Height = 35.82595f;
                ViewPosition.y = Height;

                float DeltaZ = 30f;

                Camera.main.transform.position = new Vector3(ViewPosition.x, Height, ViewPosition.z - DeltaZ);
                LogInfo($"{nameof(PositionCameraOnPlayer)} - New Camera Position: {Camera.main.transform.position}");
            }
        }
    }
}
