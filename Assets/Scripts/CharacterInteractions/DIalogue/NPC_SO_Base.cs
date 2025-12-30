using TMPro;
using UnityEngine;
[CreateAssetMenu(menuName = "NPC", fileName = "Generic NPC")]

public class NPC_SO_Base : ScriptableObject
{
    [Header("NPC Stats")]
    public Dialogue dialogue;
}