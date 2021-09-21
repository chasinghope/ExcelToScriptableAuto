//CODE GENERATE: 2021/9/21 17:02:16
using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo : ScriptableObject
{
   public List<PlayerInfo_Ele> Elements;
}

[System.Serializable]
public class PlayerInfo_Ele
{

	/// <summary>
	/// 标识
	/// </summary>
	public int ID;

	/// <summary>
	/// 名字
	/// </summary>
	public string Name;

	/// <summary>
	/// 属性
	/// </summary>
	public float Prop;

	/// <summary>
	/// int数组
	/// </summary>
	public int[] intArray;

	/// <summary>
	/// float数组
	/// </summary>
	public float[] floatArray;

	/// <summary>
	/// double数组
	/// </summary>
	public double[] doubleArray;

	/// <summary>
	/// string数组
	/// </summary>
	public string[] stringArray;

	public PlayerInfo_Ele(params string[] parameter)
	{
		ID = int.Parse(parameter[0]);
		Name = parameter[1];
		Prop = float.Parse(parameter[2]);

		string[] temp3 = parameter[3].Split('|');
		intArray = new int[temp3.Length];
		for (int i = 0; i < temp3.Length; i++)  intArray[i] = int.Parse(temp3[i]);

		string[] temp4 = parameter[4].Split('|');
		floatArray = new float[temp4.Length];
		for (int i = 0; i < temp4.Length; i++)  floatArray[i] = float.Parse(temp4[i]);

		string[] temp5 = parameter[5].Split('|');
		doubleArray = new double[temp5.Length];
		for (int i = 0; i < temp5.Length; i++)  doubleArray[i] = double.Parse(temp5[i]);

		stringArray = parameter[6].Split('|');
	}
}
