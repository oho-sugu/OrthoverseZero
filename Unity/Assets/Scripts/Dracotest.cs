using Cysharp.Threading.Tasks;
using Draco;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Dracotest : MonoBehaviour
{
    private string baseURL = "https://oho-sugu.github.io/plateaudrcdata/";

    public Material material;

    // Start is called before the first frame update
    async void Start()
    {
        int x = 58204;
        int y = 25795;
        string dracoDLURL = $"{baseURL}{x}_{y}.draco";

        Debug.Log("DracoDL:" + dracoDLURL);

        byte[] dracoData = await DownloadDraco(new Uri(dracoDLURL));

        Debug.Log("Draco Data Success");

        var draco = new DracoMeshLoader();
        var mesh = await draco.ConvertDracoMeshToUnity(dracoData);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Debug.Log("Draco Mesh Success");

        if (mesh != null)
        {
            GameObject go = new GameObject();
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            (var lon, var lat) = SpatialCode.tile2deg(2 * x + 1, 2 * y + 1, 17);
            Debug.Log($"{lon} {lat}");
        }
    }

    async UniTask<byte[]> DownloadDraco(Uri uri)
    {
        UnityWebRequest req = UnityWebRequest.Get(uri);
        req.downloadHandler = new DownloadHandlerBuffer();

        await req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
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
