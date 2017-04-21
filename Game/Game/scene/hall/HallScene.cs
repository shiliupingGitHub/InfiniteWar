using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;
public partial  class HallScene : BaseScene
{
    FightModel mModel_1v1 = new FightModel();
    public override void onserilizeRoot(mTonBehaviour mTon)
    {
        base.onserilizeRoot(mTon);
        Init(mTon);
        InitModel();
    }
    void InitModel()
    {
        mModel_1v1.Init(mModel_0);
       
    }
    public override string ResName
    {
        get
        {
            return "hallscene";
        }
    }

}

