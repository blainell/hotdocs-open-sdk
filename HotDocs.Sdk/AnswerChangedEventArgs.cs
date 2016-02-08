using System;

namespace HotDocs.Sdk
{
    public class AnswerChangedEventArgs : EventArgs
    {
        private readonly string _variableName;
        private readonly int[] _indices;
        private readonly ValueChangeType _changeType;

        public string VariableName { get { return _variableName; } }
        public int[] Indices { get { return _indices; } }
        public ValueChangeType ChangeType { get { return _changeType; } }

        public AnswerChangedEventArgs(string variableName, int[] indices, ValueChangeType changeType)
        {
            _variableName = variableName;
            _indices = indices;
            _changeType = changeType;
        }
    }
}