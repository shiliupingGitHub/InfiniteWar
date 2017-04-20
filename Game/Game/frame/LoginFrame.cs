using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial     class LoginFrame  : BaseFrame
{
    public override void Init(GameObject root)
    {
        base.Init(root);
        Init(root.GetComponent<mTonBehaviour>());
        btn_login.onClick += OnClick;
    }
    void OnClick(GameObject go)
    {
        int a = 0;
        int b = a;
        Debug.Log("init loginframe");
    }
}

