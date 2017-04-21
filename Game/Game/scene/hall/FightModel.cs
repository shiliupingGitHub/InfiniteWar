using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial    class FightModel : BaseFrame
{
    public override void Init(GameObject root)
    {
        base.Init(root);
        Init(root.GetComponent<mTonBehaviour>());
        btn_ok.onClick += delegate (GameObject go)
        {

            Game.Instance.EnterFight(mMapName);
        };
    }
}

