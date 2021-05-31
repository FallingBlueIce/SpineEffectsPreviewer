using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 特效组件
/// </summary>
public class EffectComponent : ComponentBase
{
    //挂点
    private string point;
    private Transform pointTrans;
    private GameObject effect1;
    private GameObject effect2;
    public DateEffect dataBase;

    public EffectComponent(PlayerEntity playerEntity, SkillEntity skillEntity, string comName, string effectName) : base(playerEntity, skillEntity, comName)
    {
        dataBase = new DateEffect(comName, ComType.CEffect, effectName);
    }
    public override void Init()
    {
        //point = "Point";
        pointTrans = playerEntity.transform;
        //playerEntity.transform.Find(point);
        if (dataBase.effectName != "")
            effect1 = Resources.Load<GameObject>(dataBase.effectName);
    }
    public override void Show()
    {
        if(effect1!=null)
        {
            if(effect2!=null)
            {
                GameObject.DestroyImmediate(effect2,true);
                effect2 = null;
            }
            GameObject temp = GameObject.Instantiate(effect1);
            temp.transform.SetParent(pointTrans);
            temp.transform.position = pointTrans.position;
            effect2 = temp;
            effect2.transform.localScale = new Vector3(1, 1, 1);
            SortingGroup sg = effect2.AddComponent<SortingGroup>();
            sg.sortingOrder = 1;
        }
    }
    public override void OnGui()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.fontStyle = FontStyle.Bold;
        foldout = EditorGUILayout.Foldout(foldout, "<color=#88FF99><size=12>" + comName + "</size></color>", true, style);
        GUILayout.Space(30);
        EditorGUILayout.EndHorizontal();
        if (foldout)
        {
           
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);

            //这里写拖拽逻辑
            //effect1=拖拽API
            effect1 = (GameObject)EditorGUILayout.ObjectField("特效", effect1, typeof(GameObject), false);
	if(effect1!=null)
            dataBase.effectName = effect1.name;
            base.OnGui();
            GUILayout.Space(35);
            EditorGUILayout.EndHorizontal();
        }
   
    }

}