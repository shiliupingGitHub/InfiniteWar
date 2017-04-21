using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BaseScene
{
    public Scene mScene;
    bool mInited = false;
    public virtual void Start()
    {
       
    }
    public virtual void OnPreEnter(System.Object arg)
    {

    }
    public virtual void OnPostEnter(System.Object arg)
    {

    }
    public virtual bool IsActive
    {
        get
        {
            return SceneManager.GetActiveScene() == mScene;
        }
    }

    public virtual void Enter()
    {
        ResourceManager.Instance.Clear();
        mScene = SceneManager.GetSceneByName(ResName);
        if (!mScene.IsValid())
        {
            ResourceManager.Instance.LoadAsset(ResName);
            SceneManager.LoadScene(ResName, LoadSceneMode.Single);
            mScene = SceneManager.GetSceneByName(ResName);
            mInited = false;
        }
        else if(mScene.isLoaded)
        {
            SceneManager.SetActiveScene(mScene);
        }
    }
    public virtual void onInitScene()
    {
        serilizeScene();
    }
    public virtual void serilizeScene()
    {
        serilizeRoot();
    }
    public virtual void serilizeRoot()
    {
        GameObject[] roots = mScene.GetRootGameObjects();
        if (null != roots)
        {
            foreach (var r in roots)
            {
                if (r.name == "root")
                {
                    onserilizeRoot(r.GetComponent<mTonBehaviour>());
                    break;
                }
            }
        }
    }
    public virtual void onserilizeRoot(mTonBehaviour mTon)
    {

    }
    public virtual void Update()
    {
        if (!mInited)
        {
            if (mScene.IsValid())
            {
                if (mScene.isLoaded)
                {
                    onInitScene();
                    mInited = true;
                }
            }
        }
    }
    public virtual string ResName
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

 
}

