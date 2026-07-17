using System.Collections.Generic;
using HueSeek.Backend;
using HueSeek.Core;
using HueSeek.Detection;
using HueSeek.Networking;
using HueSeek.Paint;
using HueSeek.Player;
using HueSeek.Scoring;
using HueSeek.Taunt;
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
        [SerializeField] private Vector3[] _playerSpawnPositions = new[] { new Vector3(-8f, 0.5f, -8f), new Vector3(8f, 0.5f, 8f) };
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private float _cameraDampingTime = 0.8f;

        private LocalNetworkService _networkService;
        private LocalBackendService _backendService;
        private RoundStateMachine _stateMachine;
        private MatchOrchestrator _matchOrchestrator;
        private MatchScorer _scorer;
        private Camera _mainCamera;
        private ClaylingController _localPlayer;
        private readonly List<ClaylingController> _players = new();
        
        private Vector3 _cameraVelocity = Vector3.zero;
        private float _cameraRotVelX = 0f;
        private float _cameraRotVelY = 0f;

        private void Awake()
        {
            _networkService = gameObject.AddComponent<LocalNetworkService>();
            _backendService = gameObject.AddComponent<LocalBackendService>();
            _stateMachine = gameObject.AddComponent<RoundStateMachine>();
            _matchOrchestrator = gameObject.AddComponent<MatchOrchestrator>();
            _scorer = gameObject.AddComponent<MatchScorer>();
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
            var behindOffset = -_localPlayer.transform.forward * 4.2f + Vector3.up * 2.6f;
            var desiredPos = target + behindOffset;

            _mainCamera.transform.position = Vector3.SmoothDamp(
                _mainCamera.transform.position,
                desiredPos,
                ref _cameraVelocity,
                _cameraDampingTime
            );

            _mainCamera.transform.LookAt(target);
        }

        private void StartLocalMatch()
        {
            CreateEnvironment();
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

                var taunt = player.GetComponent<TauntSystem>();
                taunt?.Initialize(_scorer, player);

                var seekerToolkit = player.GetComponent<SeekerToolkit>();
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

        private void CreateEnvironment()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(1.6f, 1f, 1.6f);
            floor.GetComponent<Renderer>().material = CreateSurfaceMaterial(new Color(0.16f, 0.17f, 0.2f), 0.05f, 0.9f);

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall";
            wall.transform.position = new Vector3(0f, 1.5f, 4.5f);
            wall.transform.localScale = new Vector3(6f, 3f, 0.2f);
            wall.GetComponent<Renderer>().material = CreateSurfaceMaterial(new Color(0.32f, 0.45f, 0.55f), 0.2f, 0.4f);

            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "PatternPanel";
            panel.transform.position = new Vector3(2.5f, 1.2f, 2.2f);
            panel.transform.localScale = new Vector3(2f, 1.8f, 0.2f);
            panel.GetComponent<Renderer>().material = CreateSurfaceMaterial(new Color(0.68f, 0.6f, 0.45f), 0.1f, 0.3f);

            var prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prop.name = "Prop";
            prop.transform.position = new Vector3(-2.2f, 0.6f, 1.2f);
            prop.transform.localScale = new Vector3(1f, 1.2f, 0.8f);
            prop.GetComponent<Renderer>().material = CreateSurfaceMaterial(new Color(0.75f, 0.28f, 0.3f), 0.35f, 0.25f);
        }

        private static Material CreateSurfaceMaterial(Color color, float metallic, float roughness)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = color;
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Roughness", roughness);
            return material;
        }
    }
}
