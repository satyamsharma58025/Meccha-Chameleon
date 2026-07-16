using System.Collections.Generic;
using HueSeek.Backend;
using HueSeek.Core;
using HueSeek.Networking;
using HueSeek.Paint;
using HueSeek.Player;
using UnityEngine;

namespace HueSeek.Bootstrap
{
    /// <summary>
    /// Minimal local bootstrap for phase 1: create services, spawn players, and start a playable match.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private Material _paintMaterial;
        [SerializeField] private GameModeType _modeType = GameModeType.Classic;
        [SerializeField] private Vector3[] _playerSpawnPositions = new[] { new Vector3(-2f, 0f, 0f), new Vector3(2f, 0f, 0f) };
        [SerializeField] private bool _autoStart = true;

        private LocalNetworkService _networkService;
        private LocalBackendService _backendService;
        private RoundStateMachine _stateMachine;
        private MatchOrchestrator _matchOrchestrator;
        private Camera _mainCamera;
        private ClaylingController _localPlayer;
        private readonly List<ClaylingController> _players = new();

        private void Awake()
        {
            _networkService = gameObject.AddComponent<LocalNetworkService>();
            _backendService = gameObject.AddComponent<LocalBackendService>();
            _stateMachine = gameObject.AddComponent<RoundStateMachine>();
            _matchOrchestrator = gameObject.AddComponent<MatchOrchestrator>();
            _matchOrchestrator.Initialize(_stateMachine, _modeType);

            if (_autoStart)
                StartLocalMatch();
        }

        private void Update()
        {
            if (_localPlayer == null) return;

            if (_mainCamera == null)
                _mainCamera = Camera.main;

            if (_mainCamera == null) return;

            var target = _localPlayer.transform.position + Vector3.up * 1.1f;
            _mainCamera.transform.position = target + new Vector3(0f, 2.5f, -4f);
            _mainCamera.transform.LookAt(target);
        }

        private void StartLocalMatch()
        {
            var paintMaterial = _paintMaterial ?? ClaylingFactory.CreatePaintMaterial();
            _players.Clear();

            for (var index = 0; index < _playerSpawnPositions.Length; index++)
            {
                var playerId = index + 1;
                var spawnPosition = _playerSpawnPositions[index];
                var cameraRig = new GameObject($"CameraRig_{playerId}").transform;
                cameraRig.position = spawnPosition + Vector3.up * 1.8f;

                var player = ClaylingFactory.CreatePlayer(spawnPosition, playerId, paintMaterial, cameraRig);
                player.PlayerId = playerId;
                _players.Add(player);
                _matchOrchestrator.RegisterPlayer(player);

                var seekerToolkit = player.GetComponent<Detection.SeekerToolkit>();
                if (seekerToolkit != null)
                {
                    seekerToolkit.OnTagged.AddListener(hider => _matchOrchestrator.OnHiderTagged(hider));
                }

                if (playerId == 1)
                    _localPlayer = player;
            }

            if (_mainCamera == null)
                CreateMainCamera();

            var playerIds = new List<int>();
            foreach (var player in _players)
                playerIds.Add(player.PlayerId);

            _matchOrchestrator.StartMatch(playerIds);
        }

        private void CreateMainCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            _mainCamera = cameraObject.AddComponent<Camera>();
            _mainCamera.clearFlags = CameraClearFlags.Skybox;
            _mainCamera.nearClipPlane = 0.1f;
            _mainCamera.farClipPlane = 100f;
        }
    }
}
