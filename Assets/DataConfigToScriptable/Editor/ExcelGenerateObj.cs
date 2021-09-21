using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.IO;

public class ExcelGenerateObj : EditorWindow
{

    private bool IsSelectedAll;

    [MenuItem("GameConfig/ExcelGenerateObj")]
    public static void ShowExample()
    {
        ExcelGenerateObj wnd = GetWindow<ExcelGenerateObj>();
        wnd.titleContent = new GUIContent("GameConfigGenerate");
    }

    public void CreateGUI()
    {

        #region UIToolkit资源加载

        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DataConfigToScriptable/Editor/ExcelGenerateObj.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DataConfigToScriptable/Editor/ExcelGenerateObj.uss");
        root.styleSheets.Add(styleSheet);


        #endregion

        #region 数据获取
        //数据获取
        string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, ExcelToolConfig.ExcelPath));

        //Filters
        var items = new List<ExcelLabel.ExcelLabelModel>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]) == ".xlsx")
            {
                items.Add(new ExcelLabel.ExcelLabelModel() { FileName = files[i], IsSelect = false });
            }
        }
        #endregion

        #region ExcelWorkEngine启动

        ExcelWorkEngine.Work_Init();



        #endregion

        #region UI事件绑定
        // The "makeItem" function will be called as needed
        // when the ListView needs more items to render
        Func<VisualElement> makeItem = () => new ExcelLabel();

        // As the user scrolls through the list, the ListView object
        // will recycle elements created by the "makeItem"
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list)
        Action<VisualElement, int> bindItem = (e, i) => (e as ExcelLabel).Model = items[i];

        //var listView = new ListView(items, itemHeight, makeItem, bindItem);
        ListView listView = root.Q<ListView>("ListView");
        listView.itemsSource = items;
        listView.itemHeight = 25;
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;

        listView.selectionType = SelectionType.Multiple;
        listView.style.flexGrow = 1.0f;
        listView.onSelectionChange += (list) =>
        {
            foreach (var item in list)
            {
                ExcelLabel.ExcelLabelModel temp = item as ExcelLabel.ExcelLabelModel;
                temp.IsSelect = !temp.IsSelect;
            }
        };


        ToolbarMenu toolbarMenu = root.Q<ToolbarMenu>("ToolbarMenu");
        toolbarMenu.menu.AppendAction("ClearAll", (a)=> { ExcelWorkEngine.Clear(ExcelToolConfig.ScriptPath); ExcelWorkEngine.Clear(ExcelToolConfig.SOPath); listView.Refresh(); AssetDatabase.Refresh(); }  );
        toolbarMenu.menu.AppendAction("Clear Script", (a) => { ExcelWorkEngine.Clear(ExcelToolConfig.ScriptPath); listView.Refresh(); AssetDatabase.Refresh(); });
        toolbarMenu.menu.AppendAction("Clear SO", (a) => { ExcelWorkEngine.Clear(ExcelToolConfig.SOPath); listView.Refresh(); AssetDatabase.Refresh(); });
        
        Label PrintOut = root.Q<Label>("PrintOut");


        Button selectBtn = root.Q<Button>("SelectBtn");
        selectBtn.clicked += () =>
        {
            this.IsSelectedAll = !this.IsSelectedAll;
            foreach (var item in listView.itemsSource)
            {
                var model = item as ExcelLabel.ExcelLabelModel;
                model.IsSelect = this.IsSelectedAll;
            }
            listView.Refresh();
        };

        Button refreshBtn = root.Q<Button>("RefreshBtn");
        refreshBtn.clicked += () =>
        {
            listView.Refresh();
        };


        Button generateBtn = root.Q<Button>("GenerateBtn");
        generateBtn.clicked += () =>
        {
            List<string> files = new List<string>();
            foreach (var item in listView.itemsSource)
            {
                var model = item as ExcelLabel.ExcelLabelModel;
                if (model.IsSelect)
                {
                    files.Add(model.FileName);
                }
            }

            ExcelWorkEngine.Work_GenerateGameConfig_Script_Step_Start();
            foreach (var item in files)
            {
                //Debug.Log($"选中的excel： {item}  --> 生成C#脚本");
                PrintOut.text = $"选中的excel： {item}  --> 生成C#脚本";
                ExcelWorkEngine.Work_GenerateCS(item);
                ExcelWorkEngine.Work_GenerateGameConfig_Script_Step_Continue(Path.GetFileName(item.Split('.')[0]));
            }
            ExcelWorkEngine.Work_GenerateGameConfig_Script_Step_End();
            listView.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            PrintOut.text = "Done!";
        };

        Button createBtn = root.Q<Button>("CreateBtn");
        createBtn.clicked += () =>
        {
            List<string> files = new List<string>();
            foreach (var item in listView.itemsSource)
            {
                var model = item as ExcelLabel.ExcelLabelModel;
                if (model.IsSelect)
                {
                    files.Add(model.FileName);
                }
            }

            ExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_Start();
            foreach (var item in files)
            {
                //Debug.Log($"选中的excel： {item}  --> 生成scriptable资源");
                PrintOut.text = $"选中的excel： {item}  --> 生成scriptable资源";
                var obj = ExcelWorkEngine.Work_GenerateScriptable(item);
                ExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_Continue(Path.GetFileName(item.Split('.')[0]), obj);
            }
            ExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_End();
            listView.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            PrintOut.text = "Done!";
        };


        #endregion

    }

}