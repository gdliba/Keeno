
using System;

namespace Keeno
{
    enum QuestState 
    {
        Inactive, Active, Completed }

    class Quest
    {
        public string Id;
        public string Title;        
        public string Description;  
        public QuestState State;

        public Func<bool> CheckCompletion;
        public Action OnActivate;
        public Action OnComplete;
    }
}
