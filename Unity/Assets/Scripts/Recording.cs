using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.XR.ARCore;
#endif
using UnityEngine.XR.ARFoundation;

public class Recording : MonoBehaviour
{
    public ARSession arSession;

    // Start is called before the first frame update
    void Start()
    {
        StartRecording();
    }

    public void StartRecording()
    {
#if UNITY_ANDROID
        // ARCore Session Subsystemを取得
        if (arSession.subsystem is not ARCoreSessionSubsystem subsystem)
        {
            return;
        }

        // セッション記録の設定
        // 20220611165503のようなタイムスタンプ付きで保存するように
        using var recordingConfig = new ArRecordingConfig(subsystem.session);
        var mp4path = Path.Combine(Application.persistentDataPath, $"arcore-session{DateTime.Now:yyyyMMddHHHmmss}.mp4");
        recordingConfig.SetMp4DatasetFilePath(subsystem.session, mp4path);

        var screenRotation = Screen.orientation switch
        {
            ScreenOrientation.Portrait => 0,
            ScreenOrientation.LandscapeLeft => 90,
            ScreenOrientation.PortraitUpsideDown => 180,
            ScreenOrientation.LandscapeRight => 270,
            _ => 0
        };
        recordingConfig.SetRecordingRotation(subsystem.session, screenRotation);

        subsystem.StartRecording(recordingConfig);
#endif
    }
}
