/*using com.meta.xr.colocation;
using com.meta.xr.colocation.fusion;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.meta.xr.colocation.samples.fusion
{
    public class ShareAnchors : NetworkBehaviour
    {
        private SharedAnchorManager _sharedAnchorManager;
        public FusionNetworkData _fusionNetworkData;
        public GameObject[] spawnables;
        public Transform handAnchor;
        private List<OVRSpatialAnchor> _OVRSpatialAnchors = new();
        private List<OVRSpaceUser> _OVRSpaceUsers = new();

        // Start is called before the first frame update
        void Awake()
        {
            _sharedAnchorManager = FindObjectOfType<SharedAnchorManager>();
            _fusionNetworkData = FindObjectOfType<FusionNetworkData>();
        }

        // Update is called once per frame
        public void SpawnObjectAndShareSpatialAnchors(GameObject SpawnedObject)
        {
            _sharedAnchorManager.InstantiateSpatialAnchor(SpawnedObject, handAnchor.position, handAnchor.rotation);
            _OVRSpatialAnchors.Add(SpawnedObject.GetComponent<OVRSpatialAnchor>());
            _sharedAnchorManager.ShareSpatialAnchors(_OVRSpatialAnchors, _sharedspatialAnchorCore._userShareList);


            //_spatialAnchorCore.ShareSpatialAnchors();


        }
    }
}


*/
