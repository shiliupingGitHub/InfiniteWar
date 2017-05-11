using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LiteJSON;
using System;
using System.IO.Compression;
using System.Collections;
public class Patcher : MonoBehaviour  {

    public static Patcher Instance = null;
    public System.Action mOnUpdate = null;
    public System.Action<string> mOnMsg = null;
   public PatcherDownloader mDownloader;
    System.Action mOnFinish;
    System.Action<string> mOnError;
    #region Elems
    public class BundleRequest
    {
        public string mPath;
        public System.Action<UnityEngine.Object> mCallback;
    }

    public class PatcherElem : IJsonSerializable, IJsonDeserializable
    {
        public bool mDebug;
        public class Elem  : IJsonSerializable, IJsonDeserializable
        {
            public string szName;
            public string mVersion;
            public int mLength;
            public WWW mDonload;
            public string[] mDepends;
            public string ResPath
            {
                get
                {
                    return Application.persistentDataPath + "/" + szName;
                }
            }

            public   JsonObject ToJson()
            {
                JsonObject obj = new JsonObject();
                obj.Put("name", szName);
                obj.Put("version", mVersion);
                obj.Put("depends", mDepends);
                obj.Put("len", mLength);
                return obj;
            }

            public void FromJson(JsonObject jsonObject)
            {
                szName = jsonObject.GetString("name");
                mVersion = jsonObject.GetString("version");
                mDepends = jsonObject.GetJsonArray("depends").ToArrayString();
                mLength = jsonObject.GetInt("len");
            }
        }
       public List<Elem> mElems = new List<Elem>();
        public Dictionary<string, Elem> mDic = new Dictionary<string, Elem>();
        public void AddElem(Elem e)
        {
            if(mDic.ContainsKey(e.szName))
            {
                mElems.Remove(mDic[e.szName]);
            }
            mElems.Add(e);
            mDic[e.szName] = e;
        }
        public void RemoveElem(Elem e)
        {
            mElems.Remove(e);
            mDic.Remove(e.szName);
        }
        public void Serialize(string outPath)
        {
            string txt = Json.Serialize(this);
            File.WriteAllText(outPath, txt);
        }
        void CreatDic()
        {
            mDic.Clear();
            foreach(var e in mElems)
            {
                mDic[e.szName] = e;
            }
        }
      public  bool IsChange(PatcherElem.Elem elem)
        {
            Elem me = null;
            mDic.TryGetValue(elem.szName, out me);
            if (me == null)
            {
                return true;
            }
            else if (me.mVersion != elem.mVersion)
            {
                return true;
            }
            else
            {
                string path = Application.persistentDataPath + "/" + elem.szName;
                if (!File.Exists(path))
                {
                    return true;
                }
            }
            return false;
        }
        List<PatcherElem.Elem> Compare(PatcherElem elem)
        {
            List<PatcherElem.Elem> ret = new List<Elem>();
            foreach(var d in elem.mDic)
            {
                if (IsChange(d.Value))
                    ret.Add(d.Value);
            }
            return ret;
        }
        public static  PatcherElem DeSerialize(string txt)
        {
            PatcherElem ret = Json.Deserialize<PatcherElem>(txt);
            ret.CreatDic();
           return  ret;
        }
        public JsonObject ToJson()
        {
            JsonObject obj = new JsonObject();
            obj.Put("content", mElems);
            return obj;
        }

        public void FromJson(JsonObject jsonObject)
        {
           mElems = jsonObject.GetJsonArray("content").DeserializeToList<Elem>();
            CreatDic();
        }
    }
    #endregion
    public void Clear()
    {
        foreach(var a in mAssets)
        {
            a.Value.Unload(false);
        }
        mAssets.Clear();
        mObs.Clear();
        mReqs.Clear();
        mBuff.Clear();
    }
    public static string GetABsPath(RuntimePlatform plat)
    {
        switch(plat)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            default:
                return "PC";
        }
    }
     Dictionary<string, AssetBundle> mAssets = new Dictionary<string, AssetBundle>();
     Dictionary<string, UnityEngine.Object> mObs = new Dictionary<string, UnityEngine.Object>();
    Dictionary<string, AssetBundleCreateRequest> mReqs = new Dictionary<string, AssetBundleCreateRequest>();
    List<BundleRequest> mBuff = new List<BundleRequest>();
    public UnityEngine.Object GetAsset(string path)
    {
        UnityEngine.Object ret = null;
        if (mObs.TryGetValue(path, out ret))
            return ret;
        if (null != mCurElems)
        {
            PatcherElem.Elem elem = null;
           if( mCurElems.mDic.TryGetValue(path,out elem ))
            {
                if(elem.mDepends != null && elem.mDepends.Length > 0)
                {
                    foreach(var d in elem.mDepends)
                    {
                        GetAsset(d);
                    }
                }
                string aseetPath = GetAssetPath(path);
                AssetBundle ab = AssetBundle.LoadFromFile(aseetPath);
                if(null != ab)
                {
                   ret = ab.LoadAsset(path);
                    mObs[path] = ret;
                    mAssets[path] = ab;
                }
            }
        }
        return ret;
    }
    public string GetAssetPath(string path)
    {
        return mCurElems.mDebug ? Application.dataPath + "/Patcher/ABs/" + GetABsPath(RuntimePlatform.WindowsEditor) + "/" + path : Application.persistentDataPath + "/" + path;
    }
    int mVersion = 0;
    void Update()
    {
        List<string> rm = new List<string>();
        foreach(var ar in mReqs)
        {
            if(ar.Value.isDone && IsDepenceLoaded(ar.Key))
            {
                mAssets[ar.Key] = ar.Value.assetBundle;
                mObs[ar.Key] = ar.Value.assetBundle.LoadAsset(ar.Key);
                rm.Add(ar.Key);
            }
        }
        foreach(var r in rm)
        {
            mReqs.Remove(r);
        }
        List<BundleRequest> rm2 = new List<BundleRequest>();
        foreach(var b in mBuff)
        {
            if(mObs.ContainsKey(b.mPath))
            {
                b.mCallback(mObs[b.mPath]);
                rm2.Add(b);
            }
        }

        foreach(var r in rm2)
        {
            mBuff.Remove(r);
        }

        if (null != mOnUpdate)
            mOnUpdate();
    }
    void OnMsg(string msg)
    {
        if (null != mOnMsg)
            mOnMsg(msg);
    }
    public void Remove()
    {
        Directory.Delete(Application.persistentDataPath, true);
    }
   bool IsDepenceLoaded(string path)
    {
        if (null != mCurElems)
        {
            PatcherElem.Elem elem = null;
            if (mCurElems.mDic.TryGetValue(path, out elem))
            {
                if (elem.mDepends != null && elem.mDepends.Length > 0)
                {
                    foreach (var d in elem.mDepends)
                    {
                        if (!mObs.ContainsKey(path))
                            return false;
                    }
                }
                else
                    return true;
           
            }
            return true;
        }
        return true;
    }
    public void LoadAssetAsyn(string path,System.Action<UnityEngine.Object> callback)
    {
        if(mObs.ContainsKey(path))
        {
            if (null != callback)
                callback(mObs[path]);
        }
        else
        {
           if(mReqs.ContainsKey(path))
            {
                if(null != callback)
                {
                    BundleRequest br = new BundleRequest();
                    br.mPath = path;
                    br.mCallback = callback;
                    mBuff.Add(br);
                }
             
            }
           else
            {
                if (null != mCurElems)
                {
                    PatcherElem.Elem elem = null;
                    if (mCurElems.mDic.TryGetValue(path, out elem))
                    {
                        if (elem.mDepends != null && elem.mDepends.Length > 0)
                        {
                            foreach (var d in elem.mDepends)
                            {
                                LoadAssetAsyn(d, null);
                            }
                        }
                        AssetBundleCreateRequest ar = AssetBundle.LoadFromFileAsync(GetAssetPath(path));
                        mReqs[path] = ar;

                    }
                }
            }
        }
    }
    public  PatcherElem mCurElems = null;
    public bool IsNeedUnPack(bool debug)
    {
        return  File.Exists(Application.persistentDataPath + "/Pather") && !debug;
    }
    string mSzFile
    {
        get
        {
            return Application.persistentDataPath + "/UnPackVersion";
        }
    }

    public int UnPackBundle
    {
        get
        {
            if (File.Exists(mSzFile))
            {
                string f = File.ReadAllText(mSzFile);
                return System.Convert.ToInt32(f);
            }
            else
                return -1;
        }
        set
        {
            if (!Directory.Exists(Application.persistentDataPath))
                Directory.CreateDirectory(Application.persistentDataPath);
            File.WriteAllText(mSzFile, value.ToString());
        }
    }

    public void BeingDownload(string url,bool debug, System.Action onfinish,System.Action<string> onerror)
    {
        mOnError = onerror;
        if (!debug)
        {
            
            GameObject dGo = new GameObject("Downloader");
            mDownloader = dGo.AddComponent<PatcherDownloader>();
            mDownloader.BeginDownload(url, delegate (PatcherElem e)
            {
                mCurElems = e;
                mCurElems.mDebug = false;
                if(null != dGo)
                     GameObject.Destroy(dGo);
                mDownloader = null;
                if (null != onfinish)
                    onfinish();
            },onerror);
        }
        else
        {
            LoadLocalPatcher(delegate (PatcherElem e)
            {
               
                mCurElems = e;
                mCurElems.mDebug = true;
                mDownloader = null;
                if (null != onfinish)
                    onfinish();
            }
            );
        }

    }
    void LoadLocalPatcher(System.Action<PatcherElem> onfinish)
    {
        string path = Application.dataPath + "/Patcher/ABs/" + GetABsPath(RuntimePlatform.WindowsPlayer) + "/Pather";
        string content = File.ReadAllText(path);
        PatcherElem e = Json.Deserialize<PatcherElem>(content);
        
        if (null != onfinish)
            onfinish(e);
    }
    public  void BeginUnPack(int p,System.Action onFinish,bool debug)
    {
        mVersion = p;
        mOnFinish = onFinish;
        int v = UnPackBundle;
        if(mVersion > v && !debug)
        {
           TextAsset  ta= (TextAsset)  Resources.Load("Game");

            using (MemoryStream ms = new MemoryStream(ta.bytes))
            {
                ZipStorer zip = ZipStorer.Open(ms, FileAccess.Read);
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    string outPath = Application.persistentDataPath + "/" + entry.FilenameInZip;
                    zip.ExtractFile(entry, outPath);
                    
                }
                zip.Close();
            }
            UnPackBundle = mVersion;    
        }
        if (null != onFinish)
            onFinish();

    }

}



