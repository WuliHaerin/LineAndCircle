using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {
    //Fade volor
    public Color fadeColor = Color.white;
    public Image soundButtonStart;
    public Image soundButtonEnd;
    public Sprite soundOn;
    public Sprite soundOff;
    public static bool isContinue=false;
    public string clickid;
    private StarkAdManager starkAdManager;

    // Use this for initialization
    void Start () {
        //Set looks for the button at start
        SetSoundButton();
    }
    //Put your leaderboard code here
    public void Leaderboard() {
        Debug.Log("Leaderboard Goes here , Click to Open In IDE");
    }

    //Restart the Game
    public void Replay() {
        Initiate.Fade(Application.loadedLevelName, fadeColor, 2.0f);
    }
    public void Continue()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    isContinue = true;
                    Initiate.Fade(Application.loadedLevelName, fadeColor, 2.0f);
                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);
                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("�ۿ�������Ƶ���ܻ�ȡ����Ŷ��");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("�������쳣�������¿���棡");
            });
        
    }

    public void ChangeSound() {
        //Turn sound on or off

        if (PlayerPrefs.GetInt("Sound", 1) == 1) {
            PlayerPrefs.SetInt("Sound", 0);
        }
        else if (PlayerPrefs.GetInt("Sound", 1) == 0)
        {
            PlayerPrefs.SetInt("Sound", 1);
        }

        SetSoundButton();

    }

    //Set how the audio button looks
    void SetSoundButton() {

        if (!soundOn || !soundOff || !soundButtonEnd || !soundButtonStart)
            Debug.LogError("Please Assign all the variables");

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            AudioListener.volume = 1.0f;
            soundButtonStart.sprite = soundOn;
            soundButtonEnd.sprite = soundOn;
        }
        else {
            AudioListener.volume = 0.0f;
            soundButtonStart.sprite = soundOff;
            soundButtonEnd.sprite = soundOff;
        }
    }




    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-����-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-����-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
}
