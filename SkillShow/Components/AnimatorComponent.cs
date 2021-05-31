using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#pragma warning disable CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)
using Spine.Unity;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“Spine”(是否缺少 using 指令或程序集引用?)

/// <summary>
/// 动画组件
/// </summary>
public class AnimatorComponent : ComponentBase
{
    public string skillName;
    private AnimatorStateInfo info;
    public DataAnimator dataBase;
    public AnimatorComponent(PlayerEntity playerEntity, SkillEntity skillEntity, string comName, int index, bool isLoop) : base(playerEntity, skillEntity, comName)
    {
        dataBase = new DataAnimator(comName, ComType.CAnimator, index, isLoop);
    }

    public override void Init()
    {
       
    }

    public override void Show()
    {
        //Spine播放动画
        dataBase.isLoop = false;
        if (skillName != null)
        {
            playerEntity.skeletonGraphic.AnimationState.SetAnimation(0, skillName, dataBase.isLoop);
        }
            
    }

    public override void OnGui()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.fontStyle = FontStyle.Bold;
        foldout = EditorGUILayout.Foldout(foldout, "<color=#88FF99><size=12>" + comName+"</size></color>", true, style);
        EditorGUILayout.EndHorizontal();
        if (foldout)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);
            dataBase.index = EditorGUILayout.Popup("动画", dataBase.index, playerEntity.animationName);
            dataBase.isLoop = GUILayout.Toggle(dataBase.isLoop, "Loop");
            skillName = playerEntity.animationName[dataBase.index];
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(185);
            if (GUILayout.Button("切换动画", GUILayout.MaxWidth(120)) && skillName != null)
            {
                playerEntity.skeletonGraphic.AnimationState.SetAnimation(0, skillName, dataBase.isLoop);
            }
            base.OnGui();
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }
    }
}
