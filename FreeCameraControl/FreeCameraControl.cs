﻿#region usings

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
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

#endregion

namespace FreeCameraControl
{
    //TODO Resetting the camera doesn't always work
    //TODO camera should proibably be reset when the scene changes
    [UsedImplicitly]
    public class CameraPlus : GenericSystemBase, IModSystem
    {
        private InputAction _toggleFreeMoveCameraAction;
        private InputAction _positionCameraOnPlayerAction;
        private InputAction _resetCameraPositionAction;
        private InputAction _zoomAction;
        private InputAction _panAction;

        // When this is set to true, the user will be able to move the camera
        private bool _freeMoveCameraEnabled;

        // This is the action used by Plate Up to move the characters
        private List<InputAction> _movePlayerActions;

        //TODO rename + comment
        private Gamepad _gamepad;

        protected override void Initialise()
        {
            LogWarning($"v{ModInfo.MOD_VERSION} in use!");
            InitKeybindings();
        }

        private void InitKeybindings()
        {
            // Toggle static camera - Resets camera back to default
            _toggleFreeMoveCameraAction = new InputAction(nameof(ToggleFreeMoveCamera), InputActionType.Button, "<Gamepad>/rightStickPress/");
            _toggleFreeMoveCameraAction.performed += context =>
            {
                ToggleFreeMoveCamera(context);
            };
            _toggleFreeMoveCameraAction.Enable();

            // Centers the camera on the current player
            _positionCameraOnPlayerAction = new InputAction(nameof(PositionCameraOnPlayer), InputActionType.Button, "<Gamepad>/rightShoulder/");
            _positionCameraOnPlayerAction.performed += (InputAction.CallbackContext context) =>
            {
                PositionCameraOnPlayer(context);
            };
            _positionCameraOnPlayerAction.Enable();

            // Resets the camera back to its original position
            _resetCameraPositionAction = new InputAction("Reset Camera", InputActionType.Button, "<Gamepad>/leftShoulder/");
            _resetCameraPositionAction.performed += context =>
            {
                ResetCamera(context);
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

        private void ToggleFreeMoveCamera(InputAction.CallbackContext context)
        {
            // Only allowing the person who unlocked the camera to re-lock the camera
            if (_freeMoveCameraEnabled && context.control.device.deviceId != _gamepad.deviceId)
            {
                return;
            }

            // Need to disable this so we can control the camera ourselves
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            _freeMoveCameraEnabled = !_freeMoveCameraEnabled;

            if (_freeMoveCameraEnabled)
            {
                // Find and save the gamepad for the user who triggered the event.  Will use it later so only they can move the camera.
                _gamepad = Gamepad.all.FirstOrDefault(e => e.deviceId == context.control.device.deviceId);

                // Finding and saving the existing PlateUp action that handles player movement, so we can re-enable it later.
                _movePlayerActions = InputSystem.ListEnabledActions().Where(e => e.name == "Movement").ToList();

                //TODO comment and refactor
                foreach (InputAction action in _movePlayerActions)
                {
                    var deviceIds = action.actionMap.devices.Value.Select(e => e.deviceId).ToList();
                    if (deviceIds.Contains(context.control.device.deviceId))
                    {
                        // Disabling movement only for the player that toggled the camera
                        action.Disable();
                    }
                }
            }
            else
            {
                // Reset for next time this is triggered
                _gamepad = null;
                foreach (var action in _movePlayerActions)
                {
                    action.Enable();
                }
            }
        }

        /// <summary>
        /// Resets the camera position back to the initial starting point.
        /// </summary>
        private void ResetCamera(InputAction.CallbackContext context)
        {
            if (!_freeMoveCameraEnabled)
            {
                return;
            }

            // Only allowing the person who unlocked the camera to move it
            if (context.control.device.deviceId != _gamepad.deviceId)
            {
                return;
            }

            var mainCamera = Camera.main;

            // Getting the default camera position by disabling manual control of the camera, which will snap it back to the original position
            Camera.main.GetComponent<CinemachineBrain>().enabled = true;
            var originalPosition = mainCamera.transform.position;
            var originalFov = mainCamera.fieldOfView;

            LogInfo($"Original camera position: {originalPosition} Original FOV: {originalFov}");

            // Re-enabling our control of the camera
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            mainCamera.transform.position = originalPosition;
            mainCamera.fieldOfView = originalFov;
        }

        private void UpdateZoom()
        {
            // Inverting controls so that pushing up is zoom in
            float controllerScrollInput = _gamepad.rightStick.ReadValue().y * -1;
            var newFov = Camera.main.fieldOfView;
            newFov += controllerScrollInput * .10f;

            Camera.main.fieldOfView = Mathf.Clamp(newFov, 3.0f, 69.0f);
        }

        private void UpdatePan()
        {
            var controllerInput = _gamepad.leftStick.ReadValue();

            var cameraPosition = Camera.main.transform.position;
            cameraPosition.x += controllerInput.x * .10f;
            cameraPosition.z += controllerInput.y * .10f;

            Camera.main.transform.position = cameraPosition;
        }

        private void PositionCameraOnPlayer(InputAction.CallbackContext context)
        {
            if (!_freeMoveCameraEnabled)
            {
                return;
            }

            // Only allowing the person who unlocked the camera to move it
            if (context.control.device.deviceId != _gamepad.deviceId)
            {
                return;
            }

            InputDevice device = context.control.device;

            var inputUser = InputUser.FindUserPairedToDevice(device).Value;
            var currentPlayer = Players.Main.All().First(e => e.Index == inputUser.index);

            // Disallowing online players to control our camera
            if (!currentPlayer.IsLocalUser)
            {
                return;
            }

            PlayerView[] PlayerViewArray = Object.FindObjectsOfType<PlayerView>();

            foreach (PlayerView view in PlayerViewArray)
            {
                int id = ((PlayerView.ViewData)typeof(PlayerView).GetField("Data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(view)).PlayerID;

                if (id != currentPlayer.ID)
                {
                    continue;
                }

                Vector3 viewPosition = view.transform.position;

                // default 35.82595
                float height = 35.82595f;
                viewPosition.y = height;

                float deltaZ = 30f;

                Camera.main.transform.position = new Vector3(viewPosition.x, height, viewPosition.z - deltaZ);
                LogInfo($"Positioning camera on player - {currentPlayer.Name} - New Camera Position: {Camera.main.transform.position}");
            }
        }
    }
}
