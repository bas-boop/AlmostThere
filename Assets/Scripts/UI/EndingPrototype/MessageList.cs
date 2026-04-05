using System;
using System.Collections.Generic;

namespace UI.EndingPrototype
{
    [Serializable]
    public struct MessageList
    {
        public List<MessageData> beginningMessages;
        public List<MessageData> eventMessages;
        public List<MessageData> endingMessages;

        public List<MessageData> All()
        {
            List<MessageData> all = new();
            
            all.AddRange(beginningMessages);
            all.AddRange(eventMessages);
            all.AddRange(endingMessages);

            return all;
        }
    }
}