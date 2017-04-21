using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
 public partial   class FightModelScene : BaseScene
{
    string mSceneName;
    List<int> mBuff = new List<int>();
    public override void Start()
    {
        base.Start();
        for(int i = 0; i < 30; i++)
        {
            mBuff.Add(i);
        }
    }
    public override void OnPreEnter(object arg)
    {
        base.OnPreEnter(arg);
        mSceneName = (string)arg;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if(IsActive)
        {
            foreach (var e in mBuff)
            {
                Debug.Log(e);
            }
        }

    }
    public override string ResName
    {
        get
        {
            return mSceneName;
        }
    }
}

