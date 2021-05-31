using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ö������
/// </summary>
public enum ComType
{
    CBase,
    CAnimator,
    CEffect
}
/// <summary>
/// Ԥ�����ݻ���
/// </summary>
public class DataBase 
{
    public string comName;
    public ComType comType;
    
    public DataBase(string comName, ComType comType)
    {
        this.comName = comName;
        this.comType = comType;
    }
}
/// <summary>
/// ��Ч������
/// </summary>
public class DateEffect:DataBase
{
    public string effectName;
    public DateEffect (string comName,ComType comType, string effectName):base(comName,comType)
    {
        this.effectName = effectName;
    }
}
/// <summary>
/// �������������
/// </summary>
public class DataAnimator:DataBase
{
    public int index;
    public bool isLoop;
    public DataAnimator(string comName, ComType comType, int index, bool isLoop):base(comName, comType)
    {
        this.index = index;
        this.isLoop = isLoop;
    }
}