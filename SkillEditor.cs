using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
#pragma warning disable CS0246 // δ���ҵ����ͻ������ռ�����Spine��(�Ƿ�ȱ�� using ָ����������?)
using Spine.Unity;
#pragma warning restore CS0246 // δ���ҵ����ͻ������ռ�����Spine��(�Ƿ�ȱ�� using ָ����������?)
using System.IO;
using Newtonsoft.Json;
public class SkillEditor : EditorWindow
{
    // properties

    /// <summary>
    /// ʵ������ɫ
    /// </summary>
    private GameObject player;

    /// <summary>
    /// ʵ��������
    /// </summary>
    private GameObject scene;

    /// <summary>
    /// ����Ԥ����
    /// </summary>
    Sprite scene_obj = null;

    /// <summary>
    /// ��ɫdata
    /// </summary>
#pragma warning disable CS0246 // δ���ҵ����ͻ������ռ�����SkeletonDataAsset��(�Ƿ�ȱ�� using ָ����������?)
    SkeletonDataAsset m_Role = null;
#pragma warning restore CS0246 // δ���ҵ����ͻ������ռ�����SkeletonDataAsset��(�Ƿ�ȱ�� using ָ����������?)

    /// <summary>
    /// ��ɫʵ��
    /// </summary>
    PlayerEntity entity;

    /// <summary>
    /// �汾��
    /// </summary>
    static string version = "SkillEditor V03";

    static bool foldout;
    static int index;
    string path;

    /// <summary>
    /// ��ɫ��
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
    /// ��ʼ���༭��
    /// </summary>
    private static void Init()
    {
        GameObject.FindObjectOfType<Camera>().orthographic = true;
    }

    /// <summary>
    /// ��ȡ��ɫ����
    /// </summary>
    private static void GetAllNames()
    {
        RoleNames.Add("1");
        RoleNames.Add("2");
        RoleNames.Add("3");
    }

    /// <summary>
    /// ��ȾUI
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
        GUILayout.Label("<color=#FFFFCC><size=14>    ע�����\n        "+
                        "1. �������ٴ򿪲����\n        "+
                        "2. ��Ч��Ҫ�ȴ�Hierarchy�ϵ�Project������Ԥ������ʹ�ã�\n        "+
                        "3. �ȼ��س����ؼ��ؽ�ɫ</size></color> \n \n" +
                        "<color=#FFFFFF><size=13>    �汾������\n" +
                        "   1. �Ż��������; \n"+
                        "   2. ����Spine��ɫ�ļ�Asset�ļ�,�������Զ���ȡ��Ӧ����; \n" +
                        "   3. ʵ�ֱ༭���ݵĶ�ȡ�뱣�档</size></color> \n",
                        style1);
        GUILayout.Label("<color=#888888><size=12>  ______________________________________________________________________________________</size></color>", style2);
        GUILayout.Space(10);

        // 1. ����
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("<color=#FFFFDF><size=12>������ </size></color>", style1);
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

        // 2. ��ɫ
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("<color=#FFFFDF><size=12>Spine ��ɫ��</size></color>", style1);
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

            //��Json
            ReadJson();

            CheckStage();
            player.transform.SetParent(GameObject.Find("Stage").transform);
            player.transform.localPosition = new Vector3(0, 0, 0);
            player.transform.localScale = new Vector3(1, 1, 1);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        // 3. ����
        EditorGUILayout.BeginHorizontal();
        EditorShowSkill();
        EditorGUILayout.EndHorizontal();
        //дJson
        WriteJson();
        // End
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    // ����
    private void EditorShowSkill()
    {
        if (entity != null)
            entity.OnGui();
    }

    // ��鳡���Ƿ����
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

    // ��������������������Ⱦ����
    private void CreateScene()
    {
        scene = new GameObject("����");
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
            entity.AddSkill("��ǰ����", true);
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
            if (GUILayout.Button("���浱ǰ����"))
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
