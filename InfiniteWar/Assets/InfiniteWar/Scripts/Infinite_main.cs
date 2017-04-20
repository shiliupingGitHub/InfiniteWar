using UnityEngine;
using System.Collections.Generic;
using LiteJSON;
using System;
using System.Collections;
public class Infinite_main : MonoBehaviour {
  

    // Use this for initialization
    public static PackDefine s_PackDefine;
    public static RemovePackDefine s_RemotePackDefine = new RemovePackDefine();
    public static Patcher s_Patcher;
    public UISlider sd;
    public UILabel lb_tip;
    void Start () {
        Debug.Log(Application.persistentDataPath);
        DisableUI();
        s_Patcher = null;
        TextAsset ta = Resources.Load("Config/PackDefine") as TextAsset;
        s_PackDefine = Json.Deserialize<PackDefine>(ta.text);
        StartCoroutine(ObtainRemotePackDefine());

    }
	
	// Update is called once per frame
	void Update () {
        if (null != s_Patcher && null != s_Patcher.mDownloader)
        {
            if(s_Patcher.mDownloader.mStatus == PatcherDownloader.STATUS.DOWNLOADING)
            {
                if(s_Patcher.mDownloader.mTotal != 0)
                {
                    if (!sd.gameObject.activeSelf)
                        sd.gameObject.SetActive(true);
                    if (!lb_tip.gameObject.activeSelf)
                        lb_tip.gameObject.SetActive(true);
                    int download = s_Patcher.mDownloader.mTotal - s_Patcher.mDownloader.mLeft;
                    float downloadM = (float)download / (1024f * 1024f);
                    float totalM =(float) s_Patcher.mDownloader.mTotal / (1024f * 1024f);
                    lb_tip.text =string.Format( s_PackDefine.updateTip , downloadM, totalM) ;
                    sd.value =1- (float)s_Patcher.mDownloader.mLeft / (float)s_Patcher.mDownloader.mTotal;
                }
                
            }
            

        }
	}
    void DisableUI()
    {
        sd.gameObject.SetActive(false);
        lb_tip.gameObject.SetActive(false);
    }
    void UnPackFinish()
    {
        DisableUI();
        s_Patcher.BeingDownload(s_RemotePackDefine.mDefine["PatcherUrl"].ToString(),s_PackDefine.debug, OnFinishDownload);
    }
    void OnFinishDownload()
    {

    }
    IEnumerator ObtainRemotePackDefine()
    {
        WWW www = new WWW(s_PackDefine.Url);
        yield return www;
        if(www.error == null)
        {
            s_RemotePackDefine.mDefine = mTonMiniJSON.Json.Deserialize(www.text) as Dictionary<string, System.Object>;
            s_Patcher = new Patcher();
            if (s_Patcher.IsNeedUnPack(s_PackDefine.Bundle))
            {
                lb_tip.gameObject.SetActive(true);
                lb_tip.text = s_PackDefine.uncompressTip;
                yield return new WaitForFixedUpdate();
                s_Patcher.BeginUnPack(s_PackDefine.Bundle, UnPackFinish,s_PackDefine.debug);
            }
            else
                UnPackFinish();
            
        }
        else
        {
            //do error;
        }
    }

}
public class RemovePackDefine
{
    public Dictionary<string, System.Object> mDefine;
}

public class PackDefine : IJsonSerializable, IJsonDeserializable
{
    public string Url;
    public int Bundle;
    public string updateTip;
    public string uncompressTip;
    public bool debug;
    public void FromJson(JsonObject jsonObject)
    {
        Url = jsonObject.GetString("url");
        Bundle = jsonObject.GetInt("bundle");
        updateTip = jsonObject.GetString("updatetip");
        uncompressTip = jsonObject.GetString("uncompresstip");
        debug = jsonObject.GetBool("debug");
        debug = debug && Application.isEditor;
    }

    public JsonObject ToJson()
    {
        throw new NotImplementedException();
    }
}
