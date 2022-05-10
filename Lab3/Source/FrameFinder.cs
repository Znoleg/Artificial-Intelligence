using FrameKnowledgeModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameKnowledgeModel
{
    public class FrameFinder
    {
        private readonly UserInterface _userInterface;

        public FrameFinder(UserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public void SetStartingFrameInheritance(IEnumerable<Frame> frames)
        {
            foreach (var frame in frames)
            {
                if (frame.ParentName == null) continue;

                Frame parentFrame = frames.FirstOrDefault(f => f.Name == frame.ParentName);
                frame.SetInheritance(parentFrame);
            }
        }

        public void FindByFrameName(IEnumerable<Frame> frames)
        {
            _userInterface.RequestIsFrameCreatedCheck(frames.Select(frame => frame.Name));
        }

        public void FindBySlotValue(IEnumerable<Frame> frames)
        {
            string slotValue = _userInterface.RequestAnySlotValue();
            IEnumerable<IReadOnlySlot> foundSlots = frames.SelectMany(frame => frame.Slots)
                .Where(slot => slot.DataValue.ToString() == slotValue);
            IEnumerable<Frame> foundFrames = frames.Where(frame => frame.Slots.Intersect(foundSlots).Any());

            _userInterface.PrintFoundFrames(foundFrames);
        }

        public void FindBySlotName(IEnumerable<Frame> frames)
        {
            var framesWithSlot = GetFramesWithSlotName(frames, out _, out _);
            _userInterface.PrintFoundFrames(framesWithSlot);
        }

        private IEnumerable<Frame> GetFramesWithSlotName(IEnumerable<Frame> frames, out IReadOnlySlot commonSlot,
            out string slotName)
        {
            string slotNamee = slotName = _userInterface.RequestSlotName();
            var framesWithSlot = frames.Where(frame => frame.Slots.Any(slot => slot.Name == slotNamee));
            commonSlot = framesWithSlot.First().Slots.First(slot => slot.Name == slotNamee);
            return framesWithSlot;
        }
    }
}
