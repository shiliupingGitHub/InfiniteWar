using UnityEngine;
public  partial class FightModelScene  
{
	UIEventListener	btn_exit;	//离开战斗
	public  void Init(mTonBehaviour s)
	{
		btn_exit=(UIEventListener) s.get_object("btn_exit","UIEventListener");

	}
}