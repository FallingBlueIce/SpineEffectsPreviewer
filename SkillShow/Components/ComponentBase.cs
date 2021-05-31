using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// <summary>
/// �������Ч������
/// </summary>
public class ComponentBase : MonoBehaviour
{
    protected PlayerEntity playerEntity;
    public string comName;
    public SkillEntity skillEntity;
    public bool foldout;
    public ComponentBase(PlayerEntity playerEntity, SkillEntity skillEntity, string comName)
    {
        foldout = true;
        this.playerEntity = playerEntity;
        this.comName = comName;
        this.skillEntity = skillEntity;
    }
    //��ʼ�����
    public virtual void Init() { }

    //�����Editor��ʾ
    public virtual void OnGui()
    {
        if (GUILayout.Button("Delete", GUILayout.MaxWidth(110)))
            skillEntity.Remove(this);
    }

    //�������ʵ���߼�
    public virtual void Show() { }
}
