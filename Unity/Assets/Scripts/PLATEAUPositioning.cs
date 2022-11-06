using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;


public class PLATEAUPositioning : MonoBehaviour
{
    public AREarthManager EarthManager;
    public ARAnchorManager AnchorManager;

    private bool _isInitialized = false;

    // Update is called once per frame
    void Update()
    {
        if (!_isInitialized && EarthManager.EarthTrackingState == TrackingState.Tracking)
        {
            var anchor = AnchorManager.AddAnchor(
                35.731038475,
                139.72869019,
                37.1621,
                Quaternion.identity
                );
            gameObject.transform.parent = anchor.transform;
            _isInitialized = true;
        }
    }
}
