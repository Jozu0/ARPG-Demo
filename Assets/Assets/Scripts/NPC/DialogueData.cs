using UnityEngine;


[CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/Dialogue Data")]
public class DialogueData : ScriptableObject
{
   [Header("NPC Settings")]
   public string npcName;
  
   [Header("Dialogue")]
   [TextArea(3, 10)]
   public string[] dialogueLines;
}
