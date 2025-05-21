using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Skill", menuName = "NewSkill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float coolTime = 1f;
    public float durationTime = 5f;
    public SkillType type;
}
