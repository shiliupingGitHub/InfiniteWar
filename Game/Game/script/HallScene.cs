using UnityEngine;
public  partial class HallScene  
{
	GameObject	mModel_0;	//1 v 1 模式
	public  void Init(mTonBehaviour s)
	{
		mModel_0=(GameObject) s.get_object("mModel_0","GameObject");

	}
}