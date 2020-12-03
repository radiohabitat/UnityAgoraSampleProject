using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;


public class AgoraChatScript : MonoBehaviour
{
    public string AppID;
    public string ChannelName;

    VideoSurface myView;
    VideoSurface remoteView;
    IRtcEngine mRtcEngine;

    Toggle mToggleBtn;

    void Awake()
    {
        SetupUI();
    }

    void Start()
    {
        SetupAgora();
    }

    public void Join()
    {
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        myView.SetEnable(true);
        mRtcEngine.JoinChannel(ChannelName, "", 0);
    }

    public void Leave()
    {
        mRtcEngine.LeaveChannel();
        mRtcEngine.DisableVideo();
        mRtcEngine.DisableVideoObserver();
    }

    void SetupUI()
    {
        GameObject go = GameObject.Find("LocalView");
        myView = go.AddComponent<VideoSurface>();
        go = GameObject.Find("disconnect_btn");
        go?.GetComponent<Button>()?.onClick.AddListener(Leave);
        go = GameObject.Find("connect_btn");
        go?.GetComponent<Button>()?.onClick.AddListener(Join);

        GameObject muteLiveStream = GameObject.FindGameObjectWithTag("MuteButton");
        mToggleBtn = muteLiveStream.GetComponent<Toggle>();
        mToggleBtn.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(mToggleBtn);
        });
    }

    void SetupAgora()
    {
        mRtcEngine = IRtcEngine.GetEngine(AppID);

        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccessHandler;
        mRtcEngine.OnLeaveChannel = OnLeaveChannelHandler;
    }

    void ToggleValueChanged(Toggle change)
    {
        if(mToggleBtn.isOn)
        {
            mRtcEngine.EnableAudio();
        } else
        {
            mRtcEngine.DisableAudio();
        }
    }

    //PLAY VIDEO - THOE THE ACTION IS ACTUALLY RESUMING VIDEO STREAM NEED TO CONFIRM
    public void ShowVideo()
    {
        mRtcEngine.EnableVideo();
    }

    //PAUSE VIDEO - THIS IS ACTUALLY PAUSING THE VIDEO STREAM NEED TO CONFIRM IF NEW SESSION IS STARTED?
    public void HideVideo()
    {
        mRtcEngine.DisableVideo();
    }

    void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        Debug.LogFormat("Joined channel {0} successful, my uid = {1}", channelName, uid);
    }

    void OnLeaveChannelHandler(RtcStats stats)
    {
        myView.SetEnable(false);
        if (remoteView != null)
        {
            remoteView.SetEnable(false);
        }
    }

    void OnUserJoined(uint uid, int elapsed)
    {
        GameObject go = GameObject.Find("RemoteView");

        if (remoteView == null)
        {
            remoteView = go.AddComponent<VideoSurface>();
        }

        remoteView.SetForUser(uid);
        remoteView.SetEnable(true);
        remoteView.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        remoteView.SetGameFps(30);
    }

    void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        remoteView.SetEnable(false);
    }

    void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

}

