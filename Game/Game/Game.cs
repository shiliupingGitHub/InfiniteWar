using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Game :SingleTon<Game>
{
    LoginScene mLoginScene = new LoginScene();
    HallScene mHallScene = new HallScene();
    FightModelScene mFightModelScene = new FightModelScene();
    public networklib mNetwork = new networklib();
    BaseScene mActiveScene = null;
    void OnUpdate()
    {
        mLoginScene.Update();
        mHallScene.Update();
        mFightModelScene.Update();
        mNetwork.Update();
    }
    void startScene()
    {
        mLoginScene.Start();
        mHallScene.Start();
        mFightModelScene.Start();
    }
    void Start()
    {
        initDelegate();
        startScene();
        EnterLogin();
    }
   public void EnterHall()
    {
        if (null != mActiveScene)
            mActiveScene.OnPostEnter(null);
        mHallScene.OnPreEnter(null);
        mHallScene.Enter();
        mActiveScene = mHallScene;
    }
    public void EnterLogin()
    {
        if (null != mActiveScene)
            mActiveScene.OnPostEnter(null);
        mLoginScene.OnPreEnter(null);
        mLoginScene.Enter();
        mActiveScene = mLoginScene;
    }
    public void EnterFight(string mMapName)
    {
        if (null != mActiveScene)
            mActiveScene.OnPostEnter(null);
        mFightModelScene.OnPreEnter(mMapName);
        mFightModelScene.Enter();
        mActiveScene = mFightModelScene;
    }
    void initDelegate()
    {
        Patcher.Instance.mOnUpdate += Game.Instance.OnUpdate;
    }
    public static void StartGame()
    {
        Game.Instance.Start();
        
       
    }
}

