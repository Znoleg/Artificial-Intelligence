using FrameKnowledgeModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrameKnowledgeModel
{
    class Program
    {
        private const string FrameDatabasePath = "FrameDatabase.json";
        private static List<Frame> _createdFrames;
        private static Frame _openedFrame;

        static void Main(string[] args)
        {
            UserInterface userInterface = new UserInterface();
            FrameFinder frameFinder = new FrameFinder(userInterface);
            FrameSerializer frameSerializer = new FrameSerializer(FrameDatabasePath);
            _createdFrames = frameSerializer.Load();
            _createdFrames ??= new List<Frame>();

            frameFinder.SetStartingFrameInheritance(_createdFrames);

            IEnumerable<Lisp> availableLisps = _createdFrames
                .SelectMany(frame => frame.Slots)
                .Where(slot => slot.DataType == DataType.Lisp)
                .Select(slot => slot.DataValue as Lisp);

            IEnumerable<Lisp> inheritTypeLisps = availableLisps.Where(lisp => lisp.LispType == LispType.Inherit);

            IEnumerable<string> frameNames = _createdFrames.Select(frame => frame.Name);

            while (true)
            {
                CommandType command = userInterface.RequestCommand(_openedFrame);
                switch (command)
                {
                    case CommandType.ExitApplication:
                        ExitApp();
                        break;

                    case CommandType.CreateFrame:
                        CreateFrame(userInterface, frameNames);
                        break;

                    case CommandType.CreateSlot:
                        Slot slot = CreateSlot(userInterface);
                        TryExecLisps(availableLisps, _openedFrame, slot, userInterface);
                        break;

                    case CommandType.OpenFrame:
                        OpenFrame(userInterface, frameNames);
                        break;

                    case CommandType.PrintAllFrames:
                        userInterface.PrintAllFrames(_createdFrames);
                        break;

                    case CommandType.SaveToDisk:
                        frameSerializer.Save(_createdFrames);
                        userInterface.PrintSuccessDiskSave();
                        break;

                    case CommandType.FindFrameByName:
                        frameFinder.FindByFrameName(_createdFrames);
                        break;

                    case CommandType.FindFrameBySlotName:
                        frameFinder.FindBySlotName(_createdFrames);
                        break;

                    case CommandType.FindFrameBySlotValue:
                        frameFinder.FindBySlotValue(_createdFrames);
                        break;

                    case CommandType.RedactSlotValue:
                        userInterface.RequestSlotEditing(_openedFrame);
                        break;

                    case CommandType.CloseFrame:
                        CloseFrame(userInterface);
                        break;

                    case CommandType.DeleteFrame:
                        DeleteFrame(userInterface);
                        break;

                    case CommandType.SetInheritance:
                        SetInheritance(userInterface, frameNames, inheritTypeLisps);
                        break;

                    default: Console.WriteLine($"Command {command} not implemented");
                        break;
                }
            }
        }

        private static void SetInheritance(UserInterface userInterface, IEnumerable<string> frameNames,
            IEnumerable<Lisp> availableLisps)
        {
            Frame parentFrame = GetFrameByInputName(userInterface, frameNames, "родителя");
            if (parentFrame.Name == _openedFrame.Name)
            {
                Console.WriteLine("Фрейм не может наследоваться от себя! Попробуйте ещё раз.");
                SetInheritance(userInterface, frameNames, availableLisps);
                return;
            }

            _openedFrame.SetInheritance(parentFrame);
            userInterface.PrintSuccessInhertaince(_openedFrame.Name, parentFrame.Name);

            TryExecLisps(availableLisps, _openedFrame, null, userInterface);
        }

        private static void DeleteFrame(UserInterface userInterface)
        {
            string frameName = _openedFrame.Name;
            _createdFrames.Remove(_openedFrame);
            _openedFrame = null;
            userInterface.PrintSuccessDelete(frameName);
        }

        private static void OpenFrame(UserInterface userInterface, IEnumerable<string> createdFrameNames)
        {
            Frame foundFrame = GetFrameByInputName(userInterface, createdFrameNames);
            _openedFrame = foundFrame;
        }

        private static Frame GetFrameByInputName(UserInterface userInterface, IEnumerable<string> createdFrameNames,
            string additionalMessage = "")
        {
            string frameName = userInterface.RequestCreatedFrameName(createdFrameNames, additionalMessage);
            Frame foundFrame = _createdFrames.First(frame => frame.Name == frameName);
            return foundFrame;
        }

        private static Slot CreateSlot(UserInterface userInterface)
        {
            Slot slot = userInterface.RequestSlotCreation(_openedFrame);
            _openedFrame.AddSlot(slot);
            return slot;
        }

        private static void ExitApp()
        {
            Environment.Exit(0);
        }

        private static void CreateFrame(UserInterface userInterface, IEnumerable<string> frameNames)
        {
            string frameName = userInterface.RequestFrameCreationName(frameNames);
            _createdFrames.Add(new Frame(frameName));
            userInterface.PrintSuccessFrameCreation(frameName);
        }

        private static void CloseFrame(UserInterface userInterface)
        {
            string frameName = _openedFrame.Name;
            _openedFrame = null;
            userInterface.PrintFrameClosed(frameName);
        }

        private static void TryExecLisps(IEnumerable<Lisp> availableLisps, Frame slotAddedTo, Slot addedSlot,
            UserInterface userInterface)
        {
            var executableLisps = availableLisps
                .Where(lisp => lisp.CanBeExecuted(slotAddedTo, addedSlot));

            foreach (Lisp lisp in executableLisps)
            {
                userInterface.PrintExecutedLisp(lisp);
            }
        }
    }
}
