using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class ExcelLabel : VisualElement
{

    public class ExcelLabelModel
    {
        public string FileName;

        private bool isSelect;
        public bool IsSelect
        {
            get { return isSelect; }
            set
            {
                isSelect = value;
                IsSelectedChanged?.Invoke(isSelect);
            }
        }

        public Action<bool> IsSelectedChanged;
    }


    private const string UXML_PATH = "Assets/DataConfigToScriptable/Editor/ExcelLabel.uxml";
    public new class UxmlFactory : UxmlFactory<ExcelLabel, VisualElement.UxmlTraits> { }


    private string fileName;
    public string FileName
    {
        get { return fileName; }
        set
        {
            fileName = Path.GetFileName(value).Split('.')[0];
            ExcelName.text = fileName + ".xlsx";
            ScriptName.text = fileName + ".cs";
            ScriptableName.text = fileName + ".asset";

            LackScript.visible = !File.Exists(Path.Combine(Application.dataPath, ExcelToolConfig.ScriptPath, ScriptName.text));
            LackScriptable.visible = !File.Exists(Path.Combine(Application.dataPath, ExcelToolConfig.SOPath, ScriptableName.text));
        }
    }



    public Toggle SelectToggle;

    public Label ExcelName;
    public Label ScriptName;
    public Label ScriptableName;
    public Label LackScript;
    public Label LackScriptable;

    private ExcelLabelModel model;
    public ExcelLabelModel Model
    {
        get { return model; }
        set { model = value; SelectToggle.value = model.IsSelect; FileName = model.FileName; model.IsSelectedChanged = (s) => { SelectToggle.value = s; }; }
    }

    public ExcelLabel()
    {
        VisualTreeAsset template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        template.CloneTree(this);
        Init();
    }

    private void Init()
    {
        ExcelName = this.Q<Label>("ExcelText");
        ScriptName = this.Q<Label>("ScriptText");
        ScriptableName = this.Q<Label>("ScriptableText");
        LackScript = this.Q<Label>("lackScript");
        LackScriptable = this.Q<Label>("lackScriptable");
        SelectToggle = this.Q<Toggle>("Toggle");
        SelectToggle.SetEnabled(false);
    }
}
