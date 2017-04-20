using UnityEngine;
public  partial class LoginScene : mTonBase 
{
	UIRoot	mUIRoot;	//NGUI父节点
	public  void Init(mTonBehaviour s)
	{
		mUIRoot=(UIRoot) s.get_object("mUIRoot","UIRoot");

	}
}