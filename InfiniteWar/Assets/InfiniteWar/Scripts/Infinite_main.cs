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
        sd.gameObject.SetActive(false);
        s_Patcher = null;
        TextAsset ta = Resources.Load("Config/PackDefine") as TextAsset;
        s_PackDefine = Json.Deserialize<PackDefine>(ta.text);
        StartCoroutine(ObtainRemotePackDefine());

    }
	
	// Update is called once per frame
	void Update () {
        if (null != s_Patcher.mDownloader)
        {
            if (!sd.gameObject.activeSelf)
                sd.gameObject.SetActive(true);

        }
	}
    IEnumerator ObtainRemotePackDefine()
    {
        WWW www = new WWW(s_PackDefine.Url);
        yield return www;
        if(www.error == null)
        {
            s_RemotePackDefine.mDefine = mTonMiniJSON.Json.Deserialize(www.text) as Dictionary<string, System.Object>;
            s_Patcher = new Patcher();
            if(s_Patcher.IsNeedUnPack(s_PackDefine.Bundle))
            {
                
                s_Patcher.BeginUnPack(s_PackDefine.Bundle);
            }
           s_Patcher.BeingDownload(s_RemotePackDefine.mDefine["PatcherUrl"].ToString());
            
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
    public string uncompressTip;
    public void FromJson(JsonObject jsonObject)
    {
        Url = jsonObject.GetString("url");
        Bundle = jsonObject.GetInt("bundle");
        uncompressTip = jsonObject.GetString("uncompresstip");
    }

    public JsonObject ToJson()
    {
        throw new NotImplementedException();
    }
}
