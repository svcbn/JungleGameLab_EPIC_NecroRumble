using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    //Fields : names should be same with excel column names
    //SerializeField 없으면 작동 안함. 왠지는 모름.
    [SerializeField] private int idx;
    [SerializeField] private string name;
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private string upgradeValue;
    [SerializeField] private float cooltime;
    [SerializeField] private float cost;

    
    //Properties
    public int Idx => idx;
    public string Name => name;
    public string Displayname=> displayName;
    public string Description=> description;
    public float Cooltime=> cooltime;
    public float Cost=> cost;
    public List<float> UpgradeValues
    {
        get
        {
            List<float> result = new List<float>();
            string[] splitString = upgradeValue.Split(','); //upgradeValue string을 나누기.
            if (splitString.Length > 1)
            {
                for (int j = 0; j < splitString.Length; j++)
                {
                    result.Add(float.Parse(splitString[j]));
                }
            }

            return result;
        }
    }

}