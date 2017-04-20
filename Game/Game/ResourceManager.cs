using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 public   class ResourceManager : SingleTon<ResourceManager>
{
    public UnityEngine.Object LoadAsset(string szName)
    {
        return Patcher.Instance.GetAsset(szName);
    }
    public void Clear()
    {
        Patcher.Instance.Clear();
    }
    public void LoadAsset(string szName, System.Action<UnityEngine.Object> callback)
    {
        Patcher.Instance.LoadAssetAsyn(szName, callback);
    }
}

