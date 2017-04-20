using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
public partial  class LoginScene : BaseScene
{
    public const string rs_login_scene = "level_login";
    bool mInited = false;
    public override void Enter()
    {
        base.Enter();
        ResourceManager.Instance.Clear();
        if (!mScene.IsValid())
        {
            ResourceManager.Instance.LoadAsset(rs_login_scene);
            SceneManager.LoadScene(rs_login_scene, LoadSceneMode.Single);
            mScene = SceneManager.GetSceneByName(rs_login_scene);
            mInited = false;
            //SerilizeScene("root");
        }          
    }
    void InitScene()
    {
        SerilizeScene();
    }
    public override void Update()
    {
        base.Update();
        if(!mInited)
        {
            if (mScene.IsValid())
            {
                if (mScene.isLoaded)
                {
                    InitScene();
                    mInited = true;
                }
            }
        }

    }
    void SerilizeRoot()
    {
        GameObject[] roots = mScene.GetRootGameObjects();
        if (null != roots)
        {
            foreach (var r in roots)
            {
                if (r.name == "root")
                {
                    Init(r.GetComponent<mTonBehaviour>());
                    break;
                }
            }
        }
    }
    public  void SerilizeScene()
    {
        SerilizeRoot();
    }
}

