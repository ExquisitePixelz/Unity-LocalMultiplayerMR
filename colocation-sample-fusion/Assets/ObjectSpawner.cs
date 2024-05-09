using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using Fusion;
using com.meta.xr.colocation.fusion;
using com.meta.xr.colocation.fusion.debug;*/

public class ObjectSpawner : MonoBehaviour
    {
        public GameObject cubePrefab;
        public GameObject _cameraRig;

/*        private SharedAnchorManager _sharedAnchorManager;
        private ulong _myPlayerId;
        private ulong _myOculusId;
        private GameObject _cameraRig;

        private INetworkData _networkData;
        private INetworkMessenger _networkMessenger;

        public void Init(
            INetworkData networkData,
            INetworkMessenger networkMessenger,
            SharedAnchorManager sharedAnchorManager,
            GameObject cameraRig,
            ulong myPlayerId,
            ulong myOculusId
        )
        {
            Logger.Log($"{nameof(AutomaticColocationLauncher)}: Init function called", LogLevel.Verbose);
            _networkData = networkData;
            _networkMessenger = networkMessenger;
            _sharedAnchorManager = sharedAnchorManager;
            _cameraRig = cameraRig;
            _myPlayerId = myPlayerId;
            _myOculusId = myOculusId;
        }*/

        public void SpawnCube()
        {
            Instantiate(cubePrefab, _cameraRig.transform.GetChild(0).GetChild(0).gameObject.transform.position, _cameraRig.transform.GetChild(0).GetChild(0).gameObject.transform.rotation);
        }
}

