using System;

namespace FrameKnowledgeModel.Data
{
    public enum InheritType
    {
        Unique, Same
    }

    public enum DataType
    {
        Real, Integer, Boolean, Text, Frame, Lisp
    }

    [Serializable]
    public class Slot : IReadOnlySlot
    {
        public Slot(string name, InheritType inheritType, DataType dataType, object dataValue)
        {
            Name = name;
            InheritType = inheritType;
            DataType = dataType;
            DataValue = dataValue;
        }

        public string Name { get; }
        public InheritType InheritType { get; }
        public DataType DataType { get; }
        public object DataValue { get; set; }
    }

    public interface IReadOnlySlot
    {
        public string Name { get; }
        public InheritType InheritType { get; }
        public DataType DataType { get; }
        public object DataValue { get; }
    }
}
