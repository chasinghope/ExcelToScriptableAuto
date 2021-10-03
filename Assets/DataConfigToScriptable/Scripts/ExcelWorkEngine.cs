using ExcelDataReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelWorkEngine
{

    static ExcelTypeDictModel excelTypeConfig = null;
    static StringBuilder gameConfigCode = null;
    static ScriptableObject gameConfigAsset = null;

    public static void Work_Init()
    {
        excelTypeConfig = ExcelTypeDictModel.LoadConfig();
    }

    public static void Work_GenerateCS(string excelFilePath)
    {
        DataSet ds = null;
        using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
            {
                ds = excelReader.AsDataSet();
                excelReader.Close();
            }
        }

        var table = ds.Tables[0];

        List<GenFormat> codeFormatter = new List<GenFormat>();

        for (int i = 0; i < table.Columns.Count; i++)
        {
            GenFormat format = new GenFormat();

            format.filedName = table.Rows[0][i].ToString();
            format.type = table.Rows[1][i].ToString();
            format.describe = table.Rows[2][i].ToString();
            codeFormatter.Add(format);
        }

        CreateGenerateDotCs($"{ExcelToolConfig.ScriptPath}/{table.TableName}.cs", CodeGenerate(table.TableName, codeFormatter).ToString());
    }

    public static ScriptableObject Work_GenerateScriptable(string excelFilePath)
    {
        var assembly = Assembly.Load(Assembly.GetExecutingAssembly().GetName());
        DataSet ds = null;
        using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
            {
                ds = excelReader.AsDataSet();
                excelReader.Close();
            }
        }

        var table = ds.Tables[0];

        var obj = ScriptableObject.CreateInstance(assembly.GetType(table.TableName));
        //Debug.Log(obj);
        var field = obj.GetType().GetField("Elements");
        object elements = Activator.CreateInstance(field.FieldType);
        MethodInfo addMethod = elements.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
        Type insideType = field.FieldType.GetGenericArguments()[0];

        for (int i = 3; i < table.Rows.Count; i++)
        {
            object[] parameter = new string[table.Columns.Count];
            for (int j = 0; j < table.Columns.Count; j++)
            {
                parameter[j] = table.Rows[i][j].ToString();
            }
            object element = Activator.CreateInstance(insideType, BindingFlags.Default, null, parameter, null);
            addMethod.Invoke(elements, new object[] { element });
        }

        field.SetValue(obj, elements);
        obj.name = table.TableName;

        AssetDatabase.CreateAsset(obj, "Assets/"+ ExcelToolConfig.SOPath + obj.name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return obj;
    }

    public static void Work_GenerateGameConfig_Script_Step_Start()
    {
        gameConfigCode = new StringBuilder();
        gameConfigCode.AppendLine($"//CODE GENERATE: {DateTime.Now.ToString()}")
                      .AppendLine("using UnityEngine;")
                      .AppendLine("using System.Collections.Generic;")
                      .AppendLine("using System.Collections;")
                      .AppendLine()
                      .AppendLine("public class GameConfigSO : ScriptableObject")
                      .AppendLine("{");
    }

    public static void Work_GenerateGameConfig_Script_Step_Continue(string fileName)
    {
        gameConfigCode.AppendLine($"    public {fileName} {fileName};");
    }

    public static void Work_GenerateGameConfig_Script_Step_End()
    {
        gameConfigCode.AppendLine("}");
        CreateGenerateDotCs($"{ExcelToolConfig.ScriptPath}/GameConfigSO.cs", gameConfigCode.ToString());
        gameConfigCode = null;
    }

    public static void Work_GenerateGameConfig_Asset_Step_Start()
    {
        var assembly = Assembly.Load(Assembly.GetExecutingAssembly().GetName());
        gameConfigAsset = ScriptableObject.CreateInstance(assembly.GetType("GameConfigSO"));
    }

    public static void Work_GenerateGameConfig_Asset_Step_Continue(string fieldName, ScriptableObject child)
    {
        FieldInfo field = gameConfigAsset.GetType().GetField(fieldName);
        field.SetValue(gameConfigAsset, child);
    }

    public static void Work_GenerateGameConfig_Asset_Step_End()
    {
        AssetDatabase.CreateAsset(gameConfigAsset, "Assets/" + ExcelToolConfig.SOPath + "GameConfigSO.asset");
        gameConfigAsset = null;
    }


    private static void CreateGenerateDotCs(string path, string code)
    {
        File.WriteAllText(Path.Combine(Application.dataPath, path), code);
    }

    private static StringBuilder CodeGenerate(string className, List<GenFormat> genFormatter)
    {
        StringBuilder code = new StringBuilder();
        code.AppendLine($"//CODE GENERATE: {DateTime.Now.ToString()}")
            .AppendLine("using UnityEngine;")
            //.AppendLine("using NaughtyAttributes;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine()
            //.AppendLine($"[CreateAssetMenu(menuName = {"Excelable/PlayerInfo"})]")
            .AppendLine($"public class {className} : ScriptableObject")
            .AppendLine("{")
            .AppendLine($"   public List<{className}_Ele> Elements;")
            .AppendLine("}");

        code.AppendLine()
            .AppendLine("[System.Serializable]")
            .AppendLine($"public class {className}_Ele")
            .AppendLine("{");

        //Add Field
        foreach (var item in genFormatter)
        {
            code.AppendLine(item.Print_Field(excelTypeConfig));
        }

        //Add Constructor
        code.AppendLine($"\n\tpublic {className}_Ele(params string[] parameter)")
            .AppendLine("\t{");

        for (int i = 0; i < genFormatter.Count; i++)
        {
            code.AppendLine(genFormatter[i].Print_Constructor(excelTypeConfig, i));
        }
        code.AppendLine("\t}");
        code.AppendLine("}");

        return code;
    }


    public static void Clear(string path)
    {
        string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, path));
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
    }

}

public class GenFormat
{
    public string type;
    public string filedName;
    public string describe;

    public string Print_Field(ExcelTypeDictModel model)
    {
        //if (!model.Dict.ContainsKey(type))
        //{
        //    Debug.Log(type);
        //}
        return string.Format(model.Dict[type].FiledDetail, describe, filedName);
    }

    public string Print_Constructor(ExcelTypeDictModel model, int index)
    {
        //if (!model.Dict.ContainsKey(type))
        //{
        //    Debug.Log(type);
        //}
        //Debug.Log(model.Dict[type].ConstructorDetail);
        return string.Format(model.Dict[type].ConstructorDetail, filedName, index);
    }
}



public class ExcelWorkEngineTools
{
    public static Color ToColor(int r, int g, int b, int a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Color ToColor(Color32 rColor32)
    {
        return ToColor(rColor32.r, rColor32.g, rColor32.b, rColor32.a);
    }

    /// <summary>
    /// 颜色格式 #00FF00FF
    /// </summary>
    public static Color ToColor(string rColorStr)
    {
        if (rColorStr.Length != 9 || rColorStr[0] != '#')
        {
            Debug.LogErrorFormat("颜色格式错误: ", rColorStr);
            return Color.white;
        }

        string rRStr = rColorStr.Substring(1, 2);
        int nR = Get0XValue(rRStr[0]) * 16 + Get0XValue(rRStr[1]);

        string rGStr = rColorStr.Substring(3, 2);
        int nG = Get0XValue(rGStr[0]) * 16 + Get0XValue(rGStr[1]);

        string rBStr = rColorStr.Substring(5, 2);
        int nB = Get0XValue(rBStr[0]) * 16 + Get0XValue(rBStr[1]);

        string rAStr = rColorStr.Substring(7, 2);
        int nA = Get0XValue(rAStr[0]) * 16 + Get0XValue(rAStr[1]);

        return ToColor(nR, nG, nB, nA);
    }
}