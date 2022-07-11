using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.IO;

public class ExcelGenerateWindow : EditorWindow
{

    private bool IsSelectedAll;
    private ExcelWorkEngine mExcelWorkEngine;

    [MenuItem("GameConfig/ExcelGenerateObj")]
    public static void ShowExample()
    {
        ExcelGenerateWindow wnd = GetWindow<ExcelGenerateWindow>();
        wnd.titleContent = new GUIContent("Excel表格管理工具");
        wnd.minSize = new Vector2(800f, 500f);
    }

    public void CreateGUI()
    {

        #region UIToolkit asset Load

        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DataConfigToScriptable/Editor/ExcelGenerateObj.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DataConfigToScriptable/Editor/ExcelGenerateObj.uss");
        root.styleSheets.Add(styleSheet);


        #endregion

        #region Excel LOAD
        //Load Files
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

        #region ExcelWorkEngine Init

        mExcelWorkEngine = new ExcelWorkEngine();
        mExcelWorkEngine.Work_Init();



        #endregion

        #region UI Binding
        // The "makeItem" function will be called as needed
        // when the ListView needs more items to render
        Func<VisualElement> makeItem = () => new ExcelLabel();

        // As the user scrolls through the list, the ListView object
        // will recycle elements created by the "makeItem"
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list)
        Action<VisualElement, int> bindItem = (e, i) => (e as ExcelLabel).Model = items[i];

        ListView listView = root.Q<ListView>("ListView");
        listView.itemsSource = items;
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;

        listView.selectionType = SelectionType.Multiple;
        //listView.style.flexGrow = 1.0f;
        listView.onSelectionChange += (list) =>
        {
            foreach (var item in list)
            {
                ExcelLabel.ExcelLabelModel temp = item as ExcelLabel.ExcelLabelModel;
                temp.IsSelect = !temp.IsSelect;
            }
        };


        ToolbarMenu toolbarMenu = root.Q<ToolbarMenu>("ToolbarMenu");
        toolbarMenu.menu.AppendAction("ClearAll", (a) => { mExcelWorkEngine.Clear(ExcelToolConfig.ScriptPath); mExcelWorkEngine.Clear(ExcelToolConfig.SOPath); listView.Rebuild(); AssetDatabase.Refresh(); });
        toolbarMenu.menu.AppendAction("Clear Script", (a) => { mExcelWorkEngine.Clear(ExcelToolConfig.ScriptPath); listView.Rebuild(); AssetDatabase.Refresh(); });
        toolbarMenu.menu.AppendAction("Clear SO", (a) => { mExcelWorkEngine.Clear(ExcelToolConfig.SOPath); listView.Rebuild(); AssetDatabase.Refresh(); });

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
            listView.Rebuild();
        };

        Button refreshBtn = root.Q<Button>("RefreshBtn");
        refreshBtn.clicked += () =>
        {
            listView.Rebuild();
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

            mExcelWorkEngine.Work_GenerateGameConfig_Script_Step_Start();
            foreach (var item in files)
            {
                //Debug.Log($"ѡ�е�excel�� {item}  --> ����C#�ű�");
                PrintOut.text = $"ѡ�е�excel�� {item}  --> ����C#�ű�";
                mExcelWorkEngine.Work_GenerateCS(item);
                mExcelWorkEngine.Work_GenerateGameConfig_Script_Step_Continue(Path.GetFileName(item.Split('.')[0]));
            }
            mExcelWorkEngine.Work_GenerateGameConfig_Script_Step_End();
            listView.Rebuild();
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

            mExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_Start();
            foreach (var item in files)
            {
                PrintOut.text = $"excel {item}  --> scriptableObject";
                var obj = mExcelWorkEngine.Work_GenerateScriptable(item);
                mExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_Continue(Path.GetFileName(item.Split('.')[0]), obj);
            }
            mExcelWorkEngine.Work_GenerateGameConfig_Asset_Step_End();
            listView.Rebuild();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            PrintOut.text = "Done!";
        };


        #endregion

    }

}