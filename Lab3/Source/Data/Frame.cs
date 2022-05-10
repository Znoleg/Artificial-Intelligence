using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameKnowledgeModel.Data
{
    [Serializable]
    public class Frame
    {
        private readonly List<Slot> _slots = new List<Slot>();
        private readonly Dictionary<string, object> _uniqueSlotValues = new Dictionary<string, object>();
        private Frame _parent;

        public Frame(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string ParentName => _parent?.Name;
        public IEnumerable<Slot> Slots
        {
            get => _parent is null ? _slots : _slots.Concat(_parent.Slots
                .Select(slot => slot.InheritType == InheritType.Unique 
                ? CopyUniqueSlot(slot) 
                : slot));
        }

        public void AddSlot(Slot slot) => _slots.Add(slot);
        public bool HasSlot(IReadOnlySlot slot) => Slots.Any(sl => sl.Name == slot.Name);
        public void EditSlotValue(IReadOnlySlot slotToEdit, object slotValue)
        {
            if (!HasSlot(slotToEdit))
            {
                throw new ArgumentException($"Фрейм {Name} не имеет слота {slotToEdit.Name}, запрашимаего " +
                    $"для редактирования");
            }

            Slot slot = slotToEdit as Slot;
            if (_parent?.HasSlot(slot) ?? false)
            {
                if (slot.InheritType == InheritType.Same)
                {
                    _parent.EditSlotValue(slotToEdit, slotValue);
                }
                else
                {
                    _uniqueSlotValues[slot.Name] = slotValue;
                }
            }
            else
            {
                slot.DataValue = slotValue;
            }
        }
        
        public void SetInheritance(Frame parent) => _parent = parent;

        private Slot CopyUniqueSlot(Slot slot)
        {
            if (!_uniqueSlotValues.TryGetValue(slot.Name, out object value))
            {
                value = slot.DataValue;
            }

            return new Slot(slot.Name, slot.InheritType, slot.DataType, value);
        }
    }
}
