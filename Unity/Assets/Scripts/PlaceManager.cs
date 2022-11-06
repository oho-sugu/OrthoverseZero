using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using Orthoverse;
using OrthoverseContracts.Contracts.PNSRegistry;
using Nethereum.Web3;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class PlaceManager : MonoBehaviour
{
    public DocumentManager dm;
    public ARAnchorManager am;

    public Text InfoText;

    private long currentSpatialCode = 0;
    private Dictionary<long, bool> spatialGrids = new Dictionary<long, bool>();

    private Web3 web3;
    private PNSRegistryService pns;

    private Queue<long> downloadQueue = new Queue<long>();

    private (int, int)[] aroundLUT =
    {
        (0,0), (1,0), (0,1), (-1,0), (0,-1),
        (1,1), (-1,1), (-1,-1), (1,-1)
    };

    // Start is called before the first frame update
    void Start()
    {
        web3 = new Web3("https://goerli.infura.io/v3/");
        pns = new PNSRegistryService(web3, "0x0D03EFbaccC2f53126bc832c66082ACaf5947B98");
        dm.setPostInitDocumentDelegate(postInitDocument);

        StartCoroutine(DownloadAndInstantiate());
    }

    IEnumerator DownloadAndInstantiate()
    {
        // Infinite loop for watch queue and download, instantiate
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            long code;
            if(!downloadQueue.TryDequeue(out code))
            {
                // no item
                continue;
            }

            bool grid;
            if(spatialGrids.TryGetValue(code, out grid))
            {
                // Item processing or done
                continue;
            }

            // Add Dic to prevent dupe download
            spatialGrids.Add(code, true);

            var pnstask = pns.GetRecordQueryAsync(code);
            while (!pnstask.IsCompletedSuccessfully)
            {
                yield return new WaitForSeconds(0.1f);
            }
            // No http then break
            if (!pnstask.Result.ToString().StartsWith("http"))
            {
                continue;
            }

            InfoText.text = pnstask.Result.ToString();

            // Anchor Generate
            (var x,var y) = SpatialCode.fromSpatialCode(code);
            (var lon,var lat) = SpatialCode.tile2deg(2*x+1,2*y+1,21);

            var anchor = ARAnchorManagerExtensions.ResolveAnchorOnTerrain(
                am, lat, lon, 0, Quaternion.identity);

            if(anchor != null)
            {
                StartCoroutine(CheckTerrainAnchorState(anchor, pnstask.Result.ToString()));
            }
        }
    }

    private IEnumerator CheckTerrainAnchorState(ARGeospatialAnchor anchor, string uri)
    {
        if (anchor == null)
        {
            yield break;
        }

        int retry = 0;
        while (anchor.terrainAnchorState == TerrainAnchorState.TaskInProgress)
        {
            if (retry == 30)
            {
                InfoText.text = "Resolve Terrain Anchor Timeouted";
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            retry = Math.Min(retry + 1, 30);
        }

        anchor.gameObject.SetActive(
            anchor.terrainAnchorState == TerrainAnchorState.Success);

        dm.open(null, uri, OpenMode.blank, anchor);

        yield break;
    }

    void postInitDocument(Orthoverse.Container container, object param)
    {
        InfoText.text = "Document Init Done";
        container.gameObject.transform.SetParent(((ARGeospatialAnchor)param).transform, false);
    }


    public void SetCurrentPosition(double lon, double lat)
    {
        (var x, var y,var z) = SpatialCode.deg2tile(lon, lat, 20);
        long spatialCode = SpatialCode.toSpatialCode(x,y);

        if(currentSpatialCode == spatialCode)
        {
            return;
        }

        InfoText.text = "Change Current Position " + spatialCode;

        // EnQueue around current position
        foreach((var tx, var ty) in aroundLUT)
        {
            downloadQueue.Enqueue(SpatialCode.toSpatialCode(x + tx, y + ty));
        }

        currentSpatialCode = spatialCode;
    }

}
