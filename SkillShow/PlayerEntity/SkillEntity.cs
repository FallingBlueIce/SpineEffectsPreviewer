using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// 技能实体
/// </summary>
public class SkillEntity 
{
    public PlayerEntity playerEntity;
    public List<ComponentBase> comDic;
    public string SkillName;
    private string comName;
    public string[] Components = new string[] {"null","Animator","Effect" };
    public int index;
    bool foldout;
    private ComponentBase com;
    public SkillEntity(string SkillName, PlayerEntity playerEntity, bool init, List<DataBase> datas = null)
    {
        this.playerEntity = playerEntity;
        this.SkillName = SkillName;
        comDic = new List<ComponentBase>();
        index = 0;
        comName = "";
        foldout = true;
        Init(init, datas);

    }

    private void Init(bool init, List<DataBase> datas = null)
    {
        if (init)
        {
            ComponentBase com1 = new AnimatorComponent(playerEntity, this, "动画实例1", 0, false);
            ComponentBase com2 = new EffectComponent(playerEntity, this, "特效实例1", "");

            Add(com1);
            Add(com2);
        }
        else
        {
            ComponentBase com = null;
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].comType == ComType.CAnimator)
                {
                    DataAnimator data = (DataAnimator)datas[i];
                    com = new AnimatorComponent(playerEntity, this, data.comName, data.index, data.isLoop);
                }
                else if (datas[i].comType == ComType.CEffect)
                {
                    DateEffect data = (DateEffect)datas[i];
                    com = new EffectComponent(playerEntity, this, data.comName, data.effectName);
                }
                Add(com);
            }
        }


        for (int i = comDic.Count - 1; i >= 0; i--)
        {
            comDic[i].Init();
        }
    }

    public void Add(ComponentBase com)
    {
        if(!CheckRepeate(com.comName))
        {
            comDic.Add(com);
        }
    }
    public void Remove(ComponentBase com)
    {
        if(CheckRepeate(com.comName))
        {
            comDic.Remove(com);
        }
    }

    public void OnGui()
    {

        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.fontStyle = FontStyle.Bold;

        GUIStyle style2 = new GUIStyle();
        style2.richText = true;

        foldout = EditorGUILayout.Foldout(foldout, SkillName);
        if(foldout)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("<color=#FFFFEF>技能名称： </color><color=#99FF99>" + SkillName + "</color>", style);
            
            if (GUILayout.Button("Delete", GUILayout.Width(150)))
            {
                playerEntity.RemoveSkill(this);
            }
            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("<color=#666666><size=12>       __________________________________________________________________________________</size></color>", style2);
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label("添加组件对象:                           ");
            comName = GUILayout.TextField(comName, GUILayout.Width(120));
            index = EditorGUILayout.Popup(index, Components);
            if (GUILayout.Button("Add"))
            {
                if (comName != "" && index != 0)
                {
                    switch (index)
                    {
                        case 1:
                            com = new AnimatorComponent(playerEntity, this, comName, 0, false);
                            break;
                        case 2:
                            com = new EffectComponent(playerEntity, this, comName, "");
                            break;
                        default:
                            break;
                    }
                    Add(com);
                    comName = "";
                    index = 0;
                }

            }
            EditorGUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("<color=#444444><size=12>           ____________________________________________________________________________</size></color>", style2);
            for (int i = comDic.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.Space(10);
                comDic[i].OnGui();
                GUILayout.Label("<color=#444444><size=12>           ____________________________________________________________________________</size></color>", style2);
            }
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Play"))
            {

                for (int i = comDic.Count - 1; i >= 0; i--)
                {
                    comDic[i].Show();
                }
            }
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("<color=#666666><size=12>       __________________________________________________________________________________</size></color>", style2);
            EditorGUILayout.EndVertical();
            
        }
     
    }

    public bool CheckRepeate(string name)
    {
        for (int i = 0; i < comDic.Count; i++)
        {
            if (comDic[i].comName == name)
                return true;
        }
        return false;
    }
}
