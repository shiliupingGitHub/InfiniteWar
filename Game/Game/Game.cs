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
    void OnUpdate()
    {
        mLoginScene.Update();
        mHallScene.Update();
    }
    void startScene()
    {
        mLoginScene.Start();
        mHallScene.Start();
    }
    void Start()
    {
        initDelegate();
        startScene();
        EnterLogin();
    }
   public void EnterHall()
    {
        mHallScene.Enter();
    }
    public void EnterLogin()
    {
        mLoginScene.Enter();
    }
    public void EnterFight(string mMapName)
    {
        mFightModelScene.OnPreEnter(mMapName);
        mFightModelScene.Enter();
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

