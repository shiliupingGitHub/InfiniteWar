using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
 public partial   class FightModelScene : BaseScene
{
    string mSceneName;
    public override void Start()
    {
        base.Start();
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

