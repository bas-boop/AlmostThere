using UnityEngine;

namespace UI.EndingPrototype
{
    [CreateAssetMenu(fileName = "NewMessage", menuName = "AlmostThere/EndingMessage")]
    public class MessageData : ScriptableObject
    {
        public bool isPlayerSendingMessage;
        public bool hasText = true;
        public string message;
        public Sprite imageSprite;
    }
}
