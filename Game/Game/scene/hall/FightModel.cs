using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
public partial    class FightModel : BaseFrame
{
    public override void Init(GameObject root)
    {
        base.Init(root);
        Init(root.GetComponent<mTonBehaviour>());
        btn_ok.onClick += delegate (GameObject go)
        {
            //SceneManager.LoadScene(0);
            Game.Instance.EnterFight(mMapName);
        };
    }
}

