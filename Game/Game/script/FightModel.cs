using UnityEngine;
public  partial class FightModel  
{
	UIEventListener	btn_ok;	//进入战斗
	public  void Init(mTonBehaviour s)
	{
		btn_ok=(UIEventListener) s.get_object("btn_ok","UIEventListener");

	}
}