using UnityEngine;
public  partial class LoginScene  
{
	UIRoot	mUIRoot;	//NGUI父节点
	GameObject	mGoLoginFrame;	//登录界面
	public  void Init(mTonBehaviour s)
	{
		mUIRoot=(UIRoot) s.get_object("mUIRoot","UIRoot");

		mGoLoginFrame=(GameObject) s.get_object("mGoLoginFrame","GameObject");

	}
}