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
	public string Type; // ����
	public string FiledDetail; // �ֶ�����
	public string ConstructorDetail; // ����������
}
