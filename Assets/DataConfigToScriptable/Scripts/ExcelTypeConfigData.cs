using System.Collections.Generic;
using UnityEngine;
using LitJson;



[System.Serializable]
public class ExcelTypeDictModel
{
	public Dictionary<string, ExcelTypeModel> Dict;


    public ExcelTypeDictModel()
    {
		TextAsset textAsset = Resources.Load<TextAsset>("ExcelTypeTemplate");
		Dict = JsonMapper.ToObject<Dictionary<string, ExcelTypeModel>>(textAsset.text);
	}
}


// Generate From C:\Users\Administrator\Desktop\ExcelType.xlsx.xlsx
[System.Serializable]
public class ExcelTypeModel
{
	public string Type; // 类型
	public string FiledDetail; // 字段详情
	public string ConstructorDetail; // 构造器详情
}
