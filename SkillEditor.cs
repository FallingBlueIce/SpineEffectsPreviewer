using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
#pragma warning disable CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
using Spine.Unity;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
using System.IO;
using Newtonsoft.Json;
public class SkillEditor : EditorWindow
{
    // properties

    /// <summary>
    /// 实例化角色
    /// </summary>
    private GameObject player;

    /// <summary>
    /// 实例化场景
    /// </summary>
    private GameObject scene;

    /// <summary>
    /// 场景预制体
    /// </summary>
    Sprite scene_obj = null;

    /// <summary>
    /// 角色data
    /// </summary>
#pragma warning disable CS0246 // 未能找到类型或命名空间名“SkeletonDataAsset”(是否缺少 using 指令或程序集引用?)
    SkeletonDataAsset m_Role = null;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“SkeletonDataAsset”(是否缺少 using 指令或程序集引用?)

    /// <summary>
    /// 角色实体
    /// </summary>
    PlayerEntity entity;

    /// <summary>
    /// 版本号
    /// </summary>
    static string version = "SkillEditor V03";

    static bool foldout;
    static int index;
    string path;

    /// <summary>
    /// 角色名
    /// </summary>
    public static List<string> RoleNames;
    private Vector2 scrolView = Vector2.zero;

    [MenuItem("Kit/ShowSkillEditor")]
    public static void Open()
    {
        foldout = false;
        var editorPlatform = GetWindow<SkillEditor>();
        editorPlatform.titleContent = new GUIContent(version);
        index = 0;
        editorPlatform.position = new Rect(
            Screen.width / 2,
            Screen.height * 2 / 3,
            450,
            800
        );
        RoleNames = new List<string>();
        GetAllNames();
        editorPlatform.Show();

        Init();
    }

    /// <summary>
    /// 初始化编辑器
    /// </summary>
    private static void Init()
    {
        GameObject.FindObjectOfType<Camera>().orthographic = true;
    }

    /// <summary>
    /// 获取角色名字
    /// </summary>
    private static void GetAllNames()
    {
        RoleNames.Add("1");
        RoleNames.Add("2");
        RoleNames.Add("3");
    }

    /// <summary>
    /// 渲染UI
    /// </summary>
    public void OnGUI()
    {
        scrolView= EditorGUILayout.BeginScrollView(scrolView);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);


        GUIStyle style1 = new GUIStyle();
        GUIStyle style2 = new GUIStyle();
        style1.richText = true;
        style2.richText = true;
        style1.fontStyle = FontStyle.Bold;
        GUILayout.Label("<color=#FFFFCC><size=14>    注意事项：\n        "+
                        "1. 先运行再打开插件！\n        "+
                        "2. 特效需要先从Hierarchy拖到Project中生成预制体再使用！\n        "+
                        "3. 先加载场景载加载角色</size></color> \n \n" +
                        "<color=#FFFFFF><size=13>    版本新增：\n" +
                        "   1. 优化整体界面; \n"+
                        "   2. 根据Spine角色文件Asset文件,本工具自动读取对应动作; \n" +
                        "   3. 实现编辑数据的读取与保存。</size></color> \n",
                        style1);
        GUILayout.Label("<color=#888888><size=12>  ______________________________________________________________________________________</size></color>", style2);
        GUILayout.Space(10);

        // 1. 场景
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("<color=#FFFFDF><size=12>场景： </size></color>", style1);
        GUILayout.Space(32);
        scene_obj = (Sprite)EditorGUILayout.ObjectField("",scene_obj, typeof(Sprite), false);
        if (GUILayout.Button("NEW") && scene_obj != null)
        {
            if (scene != null)
            {
                GameObject.DestroyImmediate(scene_obj, true);
            }
            CreateScene();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        // 2. 角色
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("<color=#FFFFDF><size=12>Spine 角色：</size></color>", style1);
        m_Role = (SkeletonDataAsset)EditorGUILayout.ObjectField(m_Role, typeof(SkeletonDataAsset), false);
        if (GUILayout.Button("NEW") && m_Role != null)
        {
            if (player != null)
            {
                GameObject.DestroyImmediate(player, true);
            }
            player = new GameObject(m_Role.name.Split('-')[0]);
            SkeletonGraphic graphic =  player.AddComponent<SkeletonGraphic>();
            graphic.skeletonDataAsset = m_Role;
            entity = player.AddComponent<PlayerEntity>();
            entity.skeletonGraphic = graphic;
            entity.ReadAnimationList(m_Role.GetSkeletonData(false));

            //读Json
            ReadJson();

            CheckStage();
            player.transform.SetParent(GameObject.Find("Stage").transform);
            player.transform.localPosition = new Vector3(0, 0, 0);
            player.transform.localScale = new Vector3(1, 1, 1);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        // 3. 技能
        EditorGUILayout.BeginHorizontal();
        EditorShowSkill();
        EditorGUILayout.EndHorizontal();
        //写Json
        WriteJson();
        // End
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    // 技能
    private void EditorShowSkill()
    {
        if (entity != null)
            entity.OnGui();
    }

    // 检查场景是否加载
    private void CheckStage()
    {
        GameObject stage;
        if ((stage = GameObject.Find("Stage")) == null)
        {
            stage = new GameObject("Stage", typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler));
            stage.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            stage.GetComponent<Canvas>().worldCamera = GameObject.FindObjectOfType<Camera>();
        }
    }

    // 创建场景，调整场景渲染属性
    private void CreateScene()
    {
        scene = new GameObject("背景");
        scene.transform.position = new Vector3(0, 0, 10);
        CheckStage();
        scene.transform.SetParent(GameObject.Find("Stage").transform, false);
        scene.AddComponent<Image>().sprite = scene_obj;
        scene.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        scene.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        scene.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }

    private void ReadJson()
    {
        path = Application.dataPath + "/Output";
        string readPath = path + "/" + entity.transform.name;
        if (!File.Exists(readPath))
            entity.AddSkill("当前技能", true);
        else
        {
            string js = File.ReadAllText(readPath);
            Dictionary<string, List<DataBase>> dic = JsonConvert.DeserializeObject<Dictionary<string, List<DataBase>>>(js, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            foreach (var item in dic)
            {
                entity.AddSkill(item.Key, false, item.Value);
            }
        }
    }

    private void WriteJson()
    {
        path = Application.dataPath + "/Output";
        if (entity != null)
            if (GUILayout.Button("保存当前设置"))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = path + "/" + entity.transform.name;
                Dictionary<string, List<DataBase>> dic = new Dictionary<string, List<DataBase>>();
                for (int i = 0; i < entity.skillDic.Count; i++)
                {
                    List<ComponentBase> comList = entity.skillDic[i].comDic;
                    string skiName = entity.skillDic[i].SkillName;
                    dic.Add(skiName, new List<DataBase>());
                    for (int j = 0; j < comList.Count; j++)
                    {
                        if (comList[j] is AnimatorComponent)
                            dic[skiName].Add(((AnimatorComponent)comList[j]).dataBase);
                        else if (comList[j] is EffectComponent)
                            dic[skiName].Add(((EffectComponent)comList[j]).dataBase);
                    }
                }
                string str = JsonConvert.SerializeObject(dic, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                File.WriteAllText(fileName, str);
                AssetDatabase.Refresh();
            }
    }

}
