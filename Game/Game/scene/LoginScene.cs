using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
public   class LoginScene : BaseScene
{
    public override void Enter()
    {
        base.Enter();
        ResourceManager.Instance.Clear();
        ResourceManager.Instance.LoadAsset("level_login");    
        SceneManager.LoadScene("level_login");
    }
}

