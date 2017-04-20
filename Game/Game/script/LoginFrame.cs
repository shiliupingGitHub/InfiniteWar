using UnityEngine;
public  partial class LoginFrame  
{
	UIEventListener	btn_login;	//登录按钮
	public  void Init(mTonBehaviour s)
	{
		btn_login=(UIEventListener) s.get_object("btn_login","UIEventListener");

	}
}