using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Google.ProtocolBuffers;
using System.IO;
public partial     class LoginFrame  : BaseFrame
{
    public override void Init(GameObject root)
    {
        base.Init(root);
        Init(root.GetComponent<mTonBehaviour>());
        btn_login.onClick += delegate (GameObject go)
        {
            TestMsg t = new TestMsg();
            t.test = "hello world";
            t.test2 = 1;
            byte[] bs = new byte[2048];

            CodedOutputStream os = CodedOutputStream.CreateInstance(bs);
            t.Write(os);

            byte[] ret = new byte[os.Position];
            System.Array.Copy(bs, 0, ret, 0, (int)os.Position);
            TestMsg t2 = new TestMsg();
            CodedInputStream ins = CodedInputStream.CreateInstance(ret);
            t2.Read(ins);
            Debug.Log(t2.test);
            Debug.Log(t2.test2);
            //Game.Instance.EnterHall();
        };
    }
}

