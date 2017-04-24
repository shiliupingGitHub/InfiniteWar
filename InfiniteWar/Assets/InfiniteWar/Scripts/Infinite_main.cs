using UnityEngine;
using System.Collections.Generic;
using LiteJSON;
using System;
using System.Collections;
using System.IO;
public class Infinite_main : MonoBehaviour {

    public bool UnityDebug = false;
    // Use this for initialization
    public static PackDefine s_PackDefine;
    public static RemotePackDefine s_RemotePackDefine;
    public static Patcher s_Patcher;
    public UISlider sd;
    public UILabel lb_tip;
    public GameObject go_tipframe;
    public UILabel lb_info_tip;
    public UIEventListener btn;
  
    void Start () {
        if (null != s_Patcher)
        {
            s_Patcher.mOnUpdate = null;
            s_Patcher.Clear();
            if (null != s_Patcher.gameObject)
                GameObject.Destroy(s_Patcher.gameObject);
        }
        s_PackDefine = null;
        s_RemotePackDefine = new RemotePackDefine();
        s_Patcher = null;
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
        go_tipframe.SetActive(false);
        sd.gameObject.SetActive(false);
        lb_tip.gameObject.SetActive(false);
    }
    void UnPackFinish()
    {
        DisableUI();
        s_Patcher.BeingDownload(s_RemotePackDefine.mDefine["PatcherUrl"].ToString(),s_PackDefine.debug, OnFinishDownload,OnError);
    }
    void OnFinishDownload()
    {
        if(!UnityDebug)
        {      
            TextAsset ta = s_Patcher.GetAsset("game") as TextAsset;
            s_Patcher.Clear();
            //if (Application.platform == RuntimePlatform.Android  )
            //{
            //    AppDomain.CurrentDomain.UNL
            //   System.Reflection.Assembly assembly =  AppDomain.CurrentDomain.Load(ta.bytes);
            //    Type type = assembly.GetType("Game");
            //    if (null != ta)
            //    {
            //        System.Reflection.MethodInfo m = type.GetMethod("StartGame");
            //        m.Invoke(null, null);
            //    }
            //}
            //else
            //{
                ILRuntime.Runtime.Enviorment.AppDomain appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
                using (MemoryStream ms = new MemoryStream(ta.bytes))
                {
                    appdomain.LoadAssembly(ms);
                }
                InitILRunTime(appdomain);
                appdomain.Invoke("Game", "StartGame", null);
          //  }

        }
        else
        {
          System.Reflection.Assembly[] assemblys =   AppDomain.CurrentDomain.GetAssemblies();
           foreach(var assembly in  assemblys)
            {
                if(assembly.ManifestModule.Name.Contains("Game"))
                {
                    Type type = assembly.GetType("Game");
                    if(null != type)
                    {
                       System.Reflection.MethodInfo m = type.GetMethod("StartGame");
                        m.Invoke(null, null);
                    }
                }
            }
           // Type.GetType("Game").GetMethod("StartGame").Invoke(null,null);
        }



    }
    void InitILRunTime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
    {
    
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.VoidDelegate>((act) =>
        {
            return new UIEventListener.VoidDelegate((go) =>
            {
                ((Action<UnityEngine.GameObject>)act)(go);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject,bool>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.BoolDelegate>((act) =>
        {
            return new UIEventListener.BoolDelegate((go,state) =>
            {
                ((Action<UnityEngine.GameObject,bool>)act)(go,state);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, float>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.FloatDelegate>((act) =>
        {
            return new UIEventListener.FloatDelegate((go, delta) =>
            {
                ((Action<UnityEngine.GameObject, float>)act)(go, delta);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, Vector2>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.VectorDelegate>((act) =>
        {
            return new UIEventListener.VectorDelegate((go, delta) =>
            {
                ((Action<UnityEngine.GameObject, Vector2>)act)(go, delta);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, UnityEngine.GameObject>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.ObjectDelegate>((act) =>
        {
            return new UIEventListener.ObjectDelegate((go, obj) =>
            {
                ((Action<UnityEngine.GameObject, GameObject>)act)(go, obj);
            });
        });

        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, KeyCode>();
        appdomain.DelegateManager.RegisterDelegateConvertor<UIEventListener.KeyCodeDelegate>((act) =>
        {
            return new UIEventListener.KeyCodeDelegate((go, obj) =>
            {
                ((Action<UnityEngine.GameObject, KeyCode>)act)(go, obj);
            });
        });


        appdomain.DelegateManager.RegisterDelegateConvertor<EventDelegate.Callback>((act) =>
        {
            return new EventDelegate.Callback(() =>
            {
                ((Action)act)();
            });
        });

    }
    void OnError(string text)
    {
        go_tipframe.SetActive(true);
        lb_info_tip.text = text;
        btn.onClick = delegate (GameObject go)
        {
            Application.Quit();
        };
    }
    void OnNewBundle()
    {
        go_tipframe.SetActive(true);
        btn.onClick = delegate (GameObject go)
        {
            string urlkey = "ChannelBundleUrl" + s_PackDefine.channel;
            lb_info_tip.text = s_PackDefine.newBundleTip;
            if (s_RemotePackDefine.mDefine.ContainsKey(urlkey))
            {
                Application.OpenURL(s_RemotePackDefine.mDefine[urlkey].ToString());
            }
        };
    }
    IEnumerator ObtainRemotePackDefine()
    {
        WWW www = new WWW(s_PackDefine.Url);
        yield return www;
        if(www.error == null)
        {
            s_RemotePackDefine.mDefine = mTonMiniJSON.Json.Deserialize(www.text) as Dictionary<string, System.Object>;
            int remoteMinbundle = System.Convert.ToInt32(s_RemotePackDefine.mDefine["minbundle"].ToString());
            GameObject pGo = new GameObject("Patcher");
            GameObject.DontDestroyOnLoad(pGo);
            s_Patcher = pGo.AddComponent<Patcher>();
            Patcher.Instance = s_Patcher;
            int curBundle = Mathf.Max(s_Patcher.UnPackBundle, s_PackDefine.Bundle);
            if(remoteMinbundle > curBundle)
            {
                OnNewBundle();
            }
            else
            {
                if (s_Patcher.IsNeedUnPack(s_PackDefine.Bundle, s_PackDefine.debug))
                {
                    lb_tip.gameObject.SetActive(true);
                    lb_tip.text = s_PackDefine.uncompressTip;
                    yield return new WaitForFixedUpdate();
                    s_Patcher.BeginUnPack(s_PackDefine.Bundle, UnPackFinish, s_PackDefine.debug);
                }
                else
                    UnPackFinish();
            }
            

            
        }
        else
        {
            OnError(www.error);
            //do error;
        }
    }

}
public class RemotePackDefine
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
    public int channel;
    public string newBundleTip;
    public void FromJson(JsonObject jsonObject)
    {
        Url = jsonObject.GetString("url");
        Bundle = jsonObject.GetInt("bundle");
        updateTip = jsonObject.GetString("updatetip");
        uncompressTip = jsonObject.GetString("uncompresstip");
        debug = jsonObject.GetBool("debug");
        channel = jsonObject.GetInt("channel");
        newBundleTip = jsonObject.GetString("newBundleTip");
        debug = debug && Application.isEditor;
    }

    public JsonObject ToJson()
    {
        throw new NotImplementedException();
    }
}
