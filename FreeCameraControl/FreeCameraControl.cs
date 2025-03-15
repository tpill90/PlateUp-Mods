namespace FreeCameraControl
{
    //TODO Camera position should probably be reset when the scene changes, like when going back to HQ.  Look into FranchiseFirstFrameSystem as a base class, might give me what I want.
    //TODO The camera pan speed appears to be tied to your refresh rate.  The higher the frame rate the faster it moves
    //TODO rename this to FreeCameraControl
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

        // This is the action used by Plate Up to move the characters, which we will disable/re-enable later.
        private InputAction _movePlayerAction;

        //TODO rename + comment
        private Gamepad _gamepad;

        public override void Initialise()
        {
            LogWarning($"v{ModInfo.ModVersion} in use!");
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
                var deviceIdsPerAction = InputSystem.ListEnabledActions()
                                                    .Where(e => e.name == "Movement")
                                                    .Select(e => new
                                                    {
                                                        Action = e,
                                                        DeviceIds = e.actionMap.devices.Value.Select(e2 => e2.deviceId).ToList()
                                                    });

                // Disabling movement only for the player that toggled the camera
                _movePlayerAction = deviceIdsPerAction.First(e => e.DeviceIds.Contains(context.control.device.deviceId)).Action;
                _movePlayerAction.Disable();
            }
            else
            {
                // Reset for next time this is triggered
                _gamepad = null;
                _movePlayerAction.Enable();
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

            // Getting the default camera position by disabling manual control of the camera, which will snap it back to the original position
            var cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = true;
            cinemachineBrain.ManualUpdate();

            var originalPosition = Camera.main.transform.position;
            var originalFov = Camera.main.fieldOfView;

            LogInfo($"Original camera position: {originalPosition} Original FOV: {originalFov}");

            // Re-enabling our control of the camera
            cinemachineBrain.enabled = false;

            Camera.main.transform.position = originalPosition;
            Camera.main.fieldOfView = originalFov;
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

            InputDevice device = context.control.device;
            // Only allowing the person who unlocked the camera to move it
            if (device.deviceId != _gamepad.deviceId)
            {
                return;
            }

            var inputUser = InputUser.FindUserPairedToDevice(device).Value;
            var currentPlayer = Kitchen.Players.Main.All().First(e => e.Index == inputUser.index);

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
