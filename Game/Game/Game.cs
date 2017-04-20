using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Game :SingleTon<Game>
{
    LoginScene mLoginScene = new LoginScene();
    void OnUpdate()
    {
        mLoginScene.Update();
    }
    void startScene()
    {
        mLoginScene.Start();
    }
    void Start()
    {
        initDelegate();
        startScene();
        mLoginScene.Enter();
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

