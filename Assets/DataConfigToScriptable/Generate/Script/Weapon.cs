//CODE GENERATE: 2022/7/11 21:20:49
using UnityEngine;
using System.Collections.Generic;

public partial class Weapon : ScriptableObject
{
   public List<Weapon_Ele> Elements;
}

[System.Serializable]
public partial class Weapon_Ele
{

	/// <summary>
	/// 标识
	/// </summary>
	public int ID;

	/// <summary>
	/// 名字
	/// </summary>
	public string Name;

	public Weapon_Ele(params string[] parameter)
	{
		ID = int.Parse(parameter[0]);
		Name = parameter[2];
	}
}
