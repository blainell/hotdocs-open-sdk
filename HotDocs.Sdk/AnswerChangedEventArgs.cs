using System;

namespace HotDocs.Sdk
{
    public class AnswerChangedEventArgs : EventArgs
    {
        public AnswerChangedEventArgs(string variableName, int[] indices, ValueChangeType changeType)
        {
            VariableName = variableName;
            Indices = indices;
            ChangeType = changeType;
        }

        public string VariableName { get; }
        public int[] Indices { get; }
        public ValueChangeType ChangeType { get; }
    }
}