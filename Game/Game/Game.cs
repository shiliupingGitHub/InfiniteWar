using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Game :SingleTon<Game>
{
    void OnUpdate()
    {
        System.Random r = new System.Random();

        Debug.Log(r.Next(1, 10000));
    }
    public static void StartGame()
    {
        Patcher.Instance.mOnUpdate += Game.Instance.OnUpdate;
        Debug.Log("helloworld3");
    }
}

