namespace FreeCameraControl
{
    [UsedImplicitly]
    public class FreeCameraControl : GenericSystemBase, IModSystem
    {
        private PreferenceSystemManager _prefManager;
        private const string CameraPanSpeedPreferenceKey = "FreeCameraControl_CameraPanSpeed";
        private const string CameraZoomSpeedPreferenceKey = "FreeCameraControl_CameraZoomSpeed";

        private InputAction _toggleFreeMoveCameraAction;
        private InputAction _positionCameraOnPlayerAction;
        private InputAction _resetCameraPositionAction;
        private InputAction _zoomAction;
        private InputAction _panAction;

        // When this is set to true, the user will be able to move the camera
        private bool _freeMoveCameraEnabled;

        // This is the (built in) action used by Plate Up to move the characters, which we will disable/re-enable later.
        private InputAction _movePlayerAction;

        // This is the input device of the player who toggled the camera lock.  Can be a keyboard or mouse
        private InputDevice _playerInputDevice;

        public override void Initialise()
        {
            LogInfo($"v{ModInfo.ModVersion} in use!");
            InitGlobalKeybindings();

            _prefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            _prefManager.AddLabel("Camera Pan Speed")
                       .AddOption(CameraPanSpeedPreferenceKey, initialValue: 5, values: Enumerable.Range(1, 10).ToArray(), Enumerable.Range(1, 10).Select(e => e.ToString()).ToArray())
                       .AddSpacer()
                       .AddSpacer();
            _prefManager.AddLabel("Camera Zoom Speed")
                        .AddOption(CameraZoomSpeedPreferenceKey, initialValue: 5, values: Enumerable.Range(1, 10).ToArray(), Enumerable.Range(1, 10).Select(e => e.ToString()).ToArray())
                        .AddSpacer()
                        .AddSpacer();
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);
        }

        /// <summary>
        /// These keybindings can be initialized at mod startup because we will be able to check which user triggered them in the performed action callback.
        /// </summary>
        private void InitGlobalKeybindings()
        {
            // Toggle static camera - Resets camera back to default
            _toggleFreeMoveCameraAction = new InputAction(nameof(ToggleFreeMoveCamera), InputActionType.Button, "<Gamepad>/rightStickPress/");
            _toggleFreeMoveCameraAction.AddBinding("<Keyboard>/F1");
            _toggleFreeMoveCameraAction.performed += context => ToggleFreeMoveCamera(context);
            _toggleFreeMoveCameraAction.Enable();

            // Centers the camera on the current player
            _positionCameraOnPlayerAction = new InputAction(nameof(PositionCameraOnPlayer), InputActionType.Button, "<Gamepad>/rightShoulder/");
            _positionCameraOnPlayerAction.AddBinding("<Keyboard>/C");
            _positionCameraOnPlayerAction.performed += context => PositionCameraOnPlayer(context);
            _positionCameraOnPlayerAction.Enable();

            // Resets the camera back to its original position
            _resetCameraPositionAction = new InputAction("Reset Camera", InputActionType.Button, "<Gamepad>/leftShoulder/");
            _resetCameraPositionAction.AddBinding("<Keyboard>/R");
            _resetCameraPositionAction.performed += context => ResetCamera(context);
            _resetCameraPositionAction.Enable();
        }

        /// <summary>
        /// Initializes the pan and zoom bindings for an individual player.  It would be preferable to do this at mod startup, however not all players will be connected
        /// when the mod is initialized.  This could also be done in the method triggered by a player connecting a controller, however I don't feel like adding that complexity in.
        /// We must create the bindings this way because there is no callback context to check which user did the pan/zoom.  Since we don't initially create the player bindings
        /// (which is what the game does), we need to do this nasty workaround instead.
        /// </summary>
        private void InitKeybindingsForPlayer(InputUser inputUser)
        {
            // Skipping initialization if both actions have already been created.
            if (inputUser.actions.Count(e => e.name == nameof(UpdatePan) || e.name == nameof(UpdateZoom)) == 2)
            {
                return;
            }

            var map = inputUser.actions.ToList().First().actionMap;
            // Map must be temporarily disabled otherwise Unity throws an exception
            map.Disable();

            // Pan
            var panAction = map.AddAction(nameof(UpdatePan), InputActionType.Button, "<Gamepad>/leftStick");
            panAction.AddCompositeBinding("2DVector")
                     .With("Up", "<Keyboard>/w")
                     .With("Down", "<Keyboard>/s")
                     .With("Left", "<Keyboard>/a")
                     .With("Right", "<Keyboard>/d");

            // Zoom
            var zoomAction = map.AddAction(nameof(UpdateZoom), InputActionType.Button, "<Gamepad>/rightStick/y");
            zoomAction.AddBinding("<Mouse>/scroll/y");

            map.Enable();
        }

        protected override void OnUpdate()
        {
            if (Has<CSceneFirstFrame>())
            {
                var cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
                cinemachineBrain.enabled = true;
            }

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
            if (_freeMoveCameraEnabled && context.control.device.deviceId != _playerInputDevice.deviceId)
            {
                return;
            }

            // Need to disable this so we can control the camera ourselves
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;

            _freeMoveCameraEnabled = !_freeMoveCameraEnabled;

            if (_freeMoveCameraEnabled)
            {
                // Find and save the gamepad for the user who triggered the event.  Will use it later so only they can move the camera.
                _playerInputDevice = InputSystem.devices.First(e => e.deviceId == context.control.device.deviceId);

                InputUser inputUser = InputUser.FindUserPairedToDevice(context.control.device).Value;
                InitKeybindingsForPlayer(inputUser);

                _panAction = inputUser.actions.First(e => e.name == nameof(UpdatePan));
                _zoomAction = inputUser.actions.First(e => e.name == nameof(UpdateZoom));
                _panAction.Enable();
                _zoomAction.Enable();

                _movePlayerAction = inputUser.actions.First(e => e.name == "Movement");
                _movePlayerAction.Disable();

            }
            else
            {
                // Re-enabling player movement
                _movePlayerAction.Enable();
                _panAction.Disable();
                _zoomAction.Disable();

                _movePlayerAction = null;
                _panAction = null;

                // Reset for next time this is triggered
                _playerInputDevice = null;
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
            if (context.control.device.deviceId != _playerInputDevice.deviceId)
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
            var zoomInput = _zoomAction.ReadValue<float>();
            // Mouse can scroll way faster than the controller.  Making sure that mouse scroll isn't going too fast.
            zoomInput = Mathf.Clamp(zoomInput, -10f, 10f);

            // Inverting controls so that pushing up is zoom in
            float scrollAmount = zoomInput * -1;
            float cameraSpeedModifier = _prefManager.Get<int>(CameraZoomSpeedPreferenceKey);
            var newFov = Camera.main.fieldOfView + (scrollAmount * cameraSpeedModifier * UnityEngine.Time.deltaTime);

            Camera.main.fieldOfView = Mathf.Clamp(newFov, 3.0f, 69.0f);
        }

        private void UpdatePan()
        {
            float cameraSpeedModifier = _prefManager.Get<int>(CameraPanSpeedPreferenceKey);

            var cameraPosition = Camera.main.transform.position;
            cameraPosition.x += _panAction.ReadValue<Vector2>().x * cameraSpeedModifier * UnityEngine.Time.deltaTime;
            cameraPosition.z += _panAction.ReadValue<Vector2>().y * cameraSpeedModifier * UnityEngine.Time.deltaTime;

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
            if (device.deviceId != _playerInputDevice.deviceId)
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