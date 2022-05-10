using System;

namespace FrameKnowledgeModel.Data
{
    public enum LispType
    {
        ThisFrame, SlotName, SlotValue, Inherit
    }

    [Serializable]
    public class Lisp
    {
        public Lisp(string ownerName, string printMessage, LispType lispType,
            string execSlotName = null, object execSlotValue = null, string execInheritorName = null)
        {
            OwnerName = ownerName;
            PrintMessage = printMessage;
            LispType = lispType;
            ExecSlotName = execSlotName;
            ExecSlotValue = execSlotValue;
            ExecInheritorName = execInheritorName;
        }

        public string PrintMessage { get; }
        public string OwnerName { get; }
        public LispType LispType { get; }
        public string ExecSlotName { get; }
        public object ExecSlotValue { get; }
        public string ExecInheritorName { get; }

        public bool CanBeExecuted(Frame slotAddedTo, Slot addedSlot)
        {
            switch (LispType)
            {
                case LispType.ThisFrame:
                    return slotAddedTo.Name == OwnerName;

                case LispType.SlotName:
                    return addedSlot.Name == ExecSlotName;

                case LispType.SlotValue:
                    return addedSlot.DataValue == ExecSlotValue;

                case LispType.Inherit:
                    return slotAddedTo.ParentName == ExecInheritorName;

                default: 
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return PrintMessage;
        }
    }
}
