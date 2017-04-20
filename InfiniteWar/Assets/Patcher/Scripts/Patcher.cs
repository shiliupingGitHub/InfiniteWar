using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LiteJSON;
using System;
using System.IO.Compression;
using System.Collections;
public class Patcher  {
   public PatcherDownloader mDownloader;
    System.Action mOnFinish;
    #region Elems
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
        }
    }
    #endregion
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
                string aseetPath = mCurElems.mDebug ? Application.dataPath + "/Patcher/ABs/" + GetABsPath(RuntimePlatform.WindowsEditor) + "/" + path : Application.persistentDataPath + "/" + path;
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
    int mVersion = 0;
    public  PatcherElem mCurElems = null;
    public bool IsNeedUnPack(int p)
    {
        int v = PlayerPrefs.GetInt("UnPackVersion", 0);
        return p > v;
    }
    public void BeingDownload(string url,bool debug, System.Action onfinish)
    {
        if(!debug)
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
            });
        }
        else
        {
            LoadLocalPatcher(delegate (PatcherElem e)
            {
                mCurElems.mDebug = true;
                mCurElems = e;
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
       int v =  PlayerPrefs.GetInt("UnPackVersion", 0);
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
            PlayerPrefs.SetInt("UnPackVersion", mVersion);         
        }
        if (null != onFinish)
            onFinish();

    }

}



