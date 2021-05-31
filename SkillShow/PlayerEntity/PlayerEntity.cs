using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#pragma warning disable CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
using Spine.Unity;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
using System;

/// <summary>
/// 角色实体
/// </summary>
public class PlayerEntity : MonoBehaviour
{
    /// <summary>
    /// 技能列表
    /// </summary>
    public List<SkillEntity> skillDic;

    /// <summary>
    /// 技能名称
    /// </summary>
    public string skillName;

    /// <summary>
    /// 动画列表
    /// </summary>
    public string[] animationName;

    /// <summary>
    /// 角色动画组件
    /// </summary>
#pragma warning disable CS0246 // 未能找到类型或命名空间名“SkeletonGraphic”(是否缺少 using 指令或程序集引用?)
    public SkeletonGraphic skeletonGraphic;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“SkeletonGraphic”(是否缺少 using 指令或程序集引用?)

    public PlayerEntity()
    {
        skillDic = new List<SkillEntity>();
        skillName = "";
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="skillName"></param>
    public void AddSkill(string skillName, bool init, List<DataBase> datas = null)
    {
        if (!CheckRepeate(skillName))
        {
            SkillEntity skillEntity = new SkillEntity(skillName, this, init, datas);
            skillDic.Add(skillEntity);
        }
        else
            Debug.LogError("技能名字重复！！");
    }

    /// <summary>
    /// 删除技能
    /// </summary>
    /// <param name="skill"></param>
    public void RemoveSkill(SkillEntity skill)
    {
        if (CheckRepeate(skill.SkillName))
        {
            skillDic.Remove(skill);
        }
    }

    /// <summary>
    /// 渲染技能模块UI
    /// </summary>
    public void OnGui()
    {

        EditorGUILayout.BeginVertical();
        GUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("添加技能对象:");
        skillName = GUILayout.TextField(skillName);
        if (GUILayout.Button("New"))
        {
            if (skillName != "")
            {
                AddSkill(skillName,true);
                skillName = "";
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(15);
        for (int i = skillDic.Count-1; i >=0 ; i--)
        {
            skillDic[i].OnGui();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 检查技能是否重复
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool CheckRepeate(string name)
    {
        for (int i = 0; i < skillDic.Count; i++)
        {
            if (skillDic[i].SkillName == name)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 从SkeletonData中读取动画列表
    /// </summary>
    /// <param name="currentData">当前角色数据</param>
#pragma warning disable CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
    public void ReadAnimationList(Spine.SkeletonData currentData)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
    {
        Spine.Animation[] animations = currentData.Animations.Items;
        animationName = new string[animations.Length];
        for (int id = 0; id < animations.Length; id++)
        {
            animationName[id] = animations[id].Name;
        }
    }
}
