using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
public partial  class LoginScene : BaseScene
{
    LoginFrame mLoginFrame = new LoginFrame();
    public override string ResName
    {
        get
        {
            return "loginscene";
        }
    }
    void seriliszeFrame()
    {
        mLoginFrame.Init(mGoLoginFrame);
    }
    public override void onserilizeRoot(mTonBehaviour mTon)
    {
        base.onserilizeRoot(mTon);
        Init(mTon);
        seriliszeFrame();
    }
}

