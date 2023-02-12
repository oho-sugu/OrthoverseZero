using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using Orthoverse;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using UnityEngine.Networking;
using Draco;
using Cysharp.Threading.Tasks;

public class PLATEAUManager : MonoBehaviour
{
    public AREarthManager EarthManager;
    public ARAnchorManager am;

//    private string baseURL = "https://oho-sugu.github.io/plateaudrcdata/";
    private string baseURL = "https://ortv.tech/plateau";

    public Text InfoText;

    private long currentSpatialCode = 0;
    private Dictionary<long, bool> spatialGrids = new Dictionary<long, bool>();

    private Queue<long> downloadQueue = new Queue<long>();

    public Material material;

    private (int, int)[] aroundLUT =
    {
        (0,0), (1,0), (0,1), (-1,0), (0,-1),
        (1,1), (-1,1), (-1,-1), (1,-1)
    };

    // Start is called before the first frame update
    void Start()
    {
        DownloadAndInstantiate();
    }

    async void DownloadAndInstantiate()
    {
        // Infinite loop for watch queue and download, instantiate
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1), ignoreTimeScale: false);

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

            (int x, int y) = SpatialCode.fromSpatialCode(code);

            int x0 = x / 1000;
            int x1 = (x / 10) % 100;
            int y0 = y / 1000;
            int y1 = (y / 10) % 100;

            string dracoDLURL = $"{baseURL}/{x0}/{y0}/{x1}/{y1}/{x}_{y}.draco";

            Debug.Log("DracoDL:" + dracoDLURL);

            InfoText.text = "DracoDL:" + dracoDLURL;

            byte[] dracoData = await DownloadDraco(new Uri(dracoDLURL));

            Debug.Log("Draco Data Success");

            var draco = new DracoMeshLoader();
            var mesh = await draco.ConvertDracoMeshToUnity(dracoData);
            if (mesh != null)
            {
                Debug.Log("Draco Mesh Success");

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                GameObject go = new GameObject();
                var meshFilter = go.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = material;
                (var lon, var lat) = SpatialCode.tile2deg(2 * x + 1, 2 * y + 1, 17);
                Debug.Log($"{lon} {lat}");
                // Anchor Generate
                var anchor = ARAnchorManagerExtensions.AddAnchor(am,lat,lon,0,Quaternion.identity);
                go.SetActive(true);
                go.transform.parent = anchor.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = new Vector3(1, 1, -1);
                Debug.Log("PLATEAU Anchor Success");
            }
        }
    }

    public void SetCurrentPosition(double lon, double lat)
    {
        (var x, var y,var z) = SpatialCode.deg2tile(lon, lat, 16);
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

    async UniTask<byte[]> DownloadDraco(Uri uri)
    {
        UnityWebRequest req = UnityWebRequest.Get(uri);
        req.downloadHandler = new DownloadHandlerBuffer();

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(req.error);
            return null;
        }
        else
        {
            return req.downloadHandler.data;
        }
    }
}
