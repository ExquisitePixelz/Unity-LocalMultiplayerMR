// Copyright (c) Meta Platforms, Inc. and affiliates.

using com.meta.xr.colocation.fusion;
using com.meta.xr.colocation.fusion.debug;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityAssert = UnityEngine.Assertions.Assert;

namespace com.meta.xr.colocation.samples.fusion
{
    /// <summary>
    ///     A class that handles setting up and initializing colocation
    /// </summary>
    public class FusionNetworkBootstrapper : NetworkBehaviour
    {
        [SerializeField] private GameObject anchorPrefab;
        [SerializeField] private Nametag nametagPrefab;

        [SerializeField] private FusionNetworkData networkData;
        [SerializeField] private FusionMessenger networkMessenger;

        private string _myOculusName;
        private ulong _myPlayerId;
        private ulong _myOculusId;
        private OVRCameraRig _ovrCameraRig;
        public SharedAnchorManager _sharedAnchorManager;
        private AutomaticColocationLauncher _colocationLauncher;

        public List<Player> _players = new List<Player>();
        public List<Anchor> _anchors = new List<Anchor>();

        public GameObject prefab;

        private void Awake()
        {
            UnityAssert.IsNotNull(anchorPrefab, $"{nameof(anchorPrefab)} cannot be null.");
            UnityAssert.IsNotNull(nametagPrefab, $"{nameof(nametagPrefab)} cannot be null.");
            UnityAssert.IsNotNull(networkData, $"{nameof(networkData)} cannot be null.");
            UnityAssert.IsNotNull(networkMessenger, $"{nameof(networkMessenger)} cannot be null.");

            _ovrCameraRig = FindObjectOfType<OVRCameraRig>();
        }

        public void SetUpAndStartAutomaticColocation(ulong myOculusId, ulong myPlayerId, string playerDisplayName)
        {
            Logger.Log(
                $"SetUpAndStartAutomaticColocation was called myOculusId {myOculusId}, myPlayerId {myPlayerId}, playerDisplayName {playerDisplayName}",
                LogLevel.Verbose);
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Starting colocation.", LogLevel.Verbose);
            _myOculusName = playerDisplayName;

            networkMessenger.RegisterLocalPlayer(myPlayerId);

            _sharedAnchorManager = new SharedAnchorManager();
            _sharedAnchorManager.AnchorPrefab = anchorPrefab;

            NetworkAdapter.SetConfig(networkData, networkMessenger);

            _colocationLauncher = new AutomaticColocationLauncher();
            _colocationLauncher.Init(
                NetworkAdapter.NetworkData,
                NetworkAdapter.NetworkMessenger,
                _sharedAnchorManager,
                _ovrCameraRig.gameObject,
                myPlayerId,
                myOculusId
            );

            _colocationLauncher.ColocationReady += OnColocationReady;
            _colocationLauncher.ColocationFailed += OnColocationFailed;
            _colocationLauncher.ColocateAutomatically();
        }

        private void OnColocationReady()
        {
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Colocation is Ready!", LogLevel.Info);
            SpawnNametagHostRPC(Runner.LocalPlayer, _myOculusName);
        }

        private void OnColocationFailed(ColocationFailedReason e)
        {
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Colocation failed - {e}", LogLevel.Error);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable,
            HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void SpawnNametagHostRPC(PlayerRef owner, string playerName)
        {
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Creating nametag for player {owner.PlayerId}: {playerName}", LogLevel.Info);

            Nametag nametag = Runner.Spawn(nametagPrefab, inputAuthority: owner);
            nametag.Name = playerName;
        }

        private void Update()
        {
            if (Input.GetKeyUp("space"))
            {
                StartCoroutine(SpawnCube());
            }

            if (OVRInput.GetUp(OVRInput.Button.One))
            {
                StartCoroutine(SpawnCube());
            }
        }

        private IEnumerator SpawnCube()
        {
            Vector3 cubePosition = new Vector3(0, 0, 0);
            Quaternion cubeRotation = Quaternion.identity;

            // Call the async function and wait for it to complete
            var task = SpawnNetworkedCube(cubePosition, cubeRotation); 
            while (!task.IsCompleted)
            {
                yield return null; // Wait for the next frame
            }

            // Optionally, handle exceptions
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
        }

        public async Task SpawnNetworkedCube(Vector3 position, Quaternion rotation)
        {
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Creating and saving anchor for the cube.", LogLevel.Verbose);

            // Step 1: Create an anchor at the specified position
            var anchor = await _sharedAnchorManager.CreateAnchor(position, rotation);

            if (anchor == null)
            {
                Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Failed to create anchor.", LogLevel.Error);
                return;
            }

            // Step 2: Save the anchor to the cloud
            bool saved = await _sharedAnchorManager.SaveLocalAnchorsToCloud();
            if (!saved)
            {
                Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Failed to save anchor to the cloud.", LogLevel.Error);
                return;
            }

            // Step 3: Share the anchor with other users
            ulong[] userIds = { /* List of user IDs to share the anchor with */ };
            foreach (ulong userId in userIds)
            {
                bool shared = await _sharedAnchorManager.ShareAnchorsWithUser(userId);
                if (!shared)
                {
                    Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Failed to share anchor with user {userId}.", LogLevel.Error);
                }
            }

            // Step 4: Spawn the networked cube and attach it to the anchor
            SpawnCubeWithAnchor(anchor);
        }

        private void SpawnCubeWithAnchor(OVRSpatialAnchor anchor)
        {
            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Spawning networked cube.", LogLevel.Verbose);

            // Assuming you have a prefab for the cube
            GameObject cubePrefab = prefab; // Assign your cube prefab here

            // Spawn the networked cube
            NetworkObject cubeNetworkObject = Runner.Spawn(cubePrefab.GetComponent<NetworkObject>(), anchor.transform.position, anchor.transform.rotation);

            // Access the GameObject from the NetworkObject
            GameObject cube = cubeNetworkObject.gameObject;

            // Attach the cube to the anchor
            cube.transform.SetParent(anchor.transform);

            Logger.Log($"{nameof(FusionNetworkBootstrapper)}: Networked cube spawned and attached to anchor.", LogLevel.Info);
        }
    }
}
