using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 public partial   class FightModelScene : BaseScene
{
    string mSceneName;
    public override void OnPreEnter(object arg)
    {
        base.OnPreEnter(arg);
        mSceneName = (string)arg;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override string ResName
    {
        get
        {
            return mSceneName;
        }
    }
}

