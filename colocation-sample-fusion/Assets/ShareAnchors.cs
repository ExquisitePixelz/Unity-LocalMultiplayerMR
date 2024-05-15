using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace com.meta.xr.colocation
{
    public class ShareAnchors : NetworkBehaviour
    {
        private GameObject _cameraRig;
        private ulong _myPlayerId;
        private ulong _myOculusId;
        private INetworkData _networkData;
        private INetworkMessenger _networkMessenger;
        private SharedAnchorManager _sharedAnchorManager;

        public List<Player> players;
        public bool initialised = false;

        public void Init(
        INetworkData networkData,
        INetworkMessenger networkMessenger,
        SharedAnchorManager sharedAnchorManager,
        GameObject cameraRig,
        ulong myPlayerId,
        ulong myOculusId
)
        {
            _networkData = networkData;
            _networkMessenger = networkMessenger;
            _sharedAnchorManager = sharedAnchorManager;
            _cameraRig = cameraRig;
            _myPlayerId = myPlayerId;
            _myOculusId = myOculusId;
            initialised = true;
        }

        private void Update()
        {
            if (initialised)
            {
                players = _networkData.GetAllPlayers();
            }
        }
    }
}



