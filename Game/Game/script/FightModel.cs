using UnityEngine;
public  partial class FightModel  
{
	UIEventListener	btn_ok;	//进入战斗
	int	mType;	//战斗模式
	string	mMapName;	//地图名
	public  void Init(mTonBehaviour s)
	{
		btn_ok=(UIEventListener) s.get_object("btn_ok","UIEventListener");

		mType=s.get_int("mType");

		mMapName=s.get_string("mMapName");

	}
}