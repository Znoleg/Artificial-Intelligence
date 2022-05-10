using FrameKnowledgeModel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameKnowledgeModel
{
    public enum CommandType
    {
        ExitApplication, CreateFrame, OpenFrame, 
        FindFrameByName, FindFrameBySlotName, FindFrameBySlotValue, PrintAllFrames,
        SaveToDisk,
        DeleteFrame, CreateSlot, RedactSlotValue, DeleteSlot, SetInheritance, CloseFrame
    }

    public class UserInterface
    {
        private const CommandType OpenedFrameCommandsStart = CommandType.DeleteFrame;

        private readonly static Dictionary<CommandType, string> _commandTranslations = new Dictionary<CommandType, string>
        {
            { CommandType.ExitApplication, "Завершить программу" },
            { CommandType.CreateFrame, "Создать фрейм" },
            { CommandType.OpenFrame, "Открыть фрейм" },
            { CommandType.FindFrameByName, "Найти фрейм по имени" },
            { CommandType.FindFrameBySlotName, "Найти фрейм(ы) по имени слота" },
            { CommandType.FindFrameBySlotValue, "Найти фрейм(ы) по значению слота" },
            { CommandType.PrintAllFrames, "Вывести все фреймы" },
            { CommandType.SaveToDisk, "Сохранить данные на диск" },
            { CommandType.DeleteFrame, "Удалить фрейм" },
            { CommandType.CreateSlot, "Создать слот" },
            { CommandType.RedactSlotValue, "Отредактировать значение слота" },
            { CommandType.DeleteSlot, "Удалить слот" },
            { CommandType.SetInheritance, "Установить наследование" },
            { CommandType.CloseFrame, "Закрыть фрейм" }
        };

        private Frame _openedFrame;

        public CommandType RequestCommand(Frame openedFrame)
        {
            _openedFrame = openedFrame;

            if (openedFrame != null)
            {
                PrintNewLine();
                Console.WriteLine("Открытый фрейм: ");
                PrintFrame(openedFrame);
                PrintNewLine();
            }

            Console.WriteLine("Что вы хотите сделать?\n");
            IEnumerable<CommandType> commands = Enum.GetValues(typeof(CommandType)).Cast<CommandType>();
            foreach (var command in commands)
            {
                if (command == OpenedFrameCommandsStart)
                {
                    if (openedFrame == null)
                    {
                        Console.WriteLine("Откройте фрейм чтобы увидеть больше команд.");
                        break;
                    }
                }

                Console.WriteLine($"{(int)command}: {_commandTranslations[command]}");
            }

            PrintNewLine();

            string reply = Console.ReadLine();
            if (!int.TryParse(reply, out int commandId) || !Enum.IsDefined(typeof(CommandType), commandId))
            {
                Console.WriteLine("Неправильный ввод! Попробуйте ещё раз.");
                return RequestCommand(openedFrame);
            }

            return (CommandType)commandId;
        }

        public void RequestIsFrameCreatedCheck(IEnumerable<string> createdFrameNames)
        {
            RequestFrameName(createdFrameNames, "", out string frameName, out bool alreadyCreated);
            string applyResult = alreadyCreated ? "" : " не";
            Console.WriteLine($"Фрейм {frameName}{applyResult} создан.");
            PrintNewLine();
        }

        public string RequestCreatedFrameName(IEnumerable<string> createdFrameNames, string additionalRequestMessage = "")
        {
            RequestFrameName(createdFrameNames, additionalRequestMessage, out string frameName, out bool alreadyCreated);
            if (!alreadyCreated)
            {
                Console.WriteLine($"Не найден фрейм с именем {frameName}! Введите другое имя.");
                return RequestCreatedFrameName(createdFrameNames);
            }

            return frameName;
        }

        public string RequestFrameCreationName(IEnumerable<string> createdFrameNames, string additionalRequestMessage = "")
        {
            RequestFrameName(createdFrameNames, additionalRequestMessage, out string frameName, out bool alreadyCreated);
            if (alreadyCreated)
            {
                Console.WriteLine($"Фрейм с именем {frameName} уже создан! Введите другое имя.");
                return RequestFrameCreationName(createdFrameNames);
            }

            return frameName;
        }

        public void PrintSuccessDelete(string frameName)
        {
            Console.WriteLine($"Фрейм {frameName} успешно удалён!");
        }

        public void PrintFoundFrames(IEnumerable<Frame> foundFrames)
        {
            bool anyFrameFound = foundFrames.Any();
            if (anyFrameFound)
            {
                Console.WriteLine($"Найденные фреймы: {foundFrames.Select(frame => frame.Name).ToSingleStr()}");
            }
            else
            {
                Console.WriteLine("Фреймы, удовлетворяющие условию, не найдены.");
            }

            PrintNewLine();
        }

        public void PrintSuccessFrameCreation(string frameName) =>
            Console.WriteLine($"Фрейм {frameName} успешно создан!");

        public void PrintSuccessInhertaince(string childName, string parentName) =>
            Console.WriteLine($"Успешно установлено наследование {childName}<-{parentName}!");

        public Slot RequestSlotCreation(Frame openedFrame)
        {
            IEnumerable<string> createdSlotNames = openedFrame.Slots.Select(slot => slot.Name);
            string slotName = RequestSlotCreationName(createdSlotNames, openedFrame.Name);

            InheritType inheritType = Extenstions.RequestEnumValue<InheritType>(
                "Введите тип указателя наследования",
                "Неправильный ввод указателя наследования! Попробуйте ещё раз.");

            DataType dataType = RequestSlotValueType();

            object slotValue = RequestSlotValue(dataType);

            return new Slot(slotName, inheritType, dataType, slotValue);
        }

        public void PrintAllFrames(IEnumerable<Frame> frames)
        {
            PrintNewLine();
            foreach (var frame in frames)
            {
                PrintFrame(frame);
                PrintNewLine();
            }
        }

        public void PrintFrame(Frame frame)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Имя фрейма: {frame.Name}");
            foreach (Slot slot in frame.Slots)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(string.Format("Имя слота: {0,-33}", slot.Name));
                Console.Write($"Указатель наследования: {slot.InheritType}\t");
                Console.Write($"Указатель типа данных: {slot.DataType}\t");
                Console.Write($"Значение слота: {slot.DataValue}\t");
                PrintNewLine();
            }

            Console.ResetColor();
            PrintNewLine();
        }

        public void PrintFrameClosed(string frameName) =>
            Console.WriteLine($"Фейм {frameName} закрыт.");

        public void RequestSlotEditing(Frame frameToEdit)
        {
            IReadOnlySlot slotToEdit = FindSlotByName(frameToEdit);
            object slotValue = RequestSlotValue(slotToEdit.DataType);
            frameToEdit.EditSlotValue(slotToEdit, slotValue);
            Console.WriteLine($"Успешно изменили значение слота {slotToEdit.Name} на {slotToEdit.DataValue}");
        }

        public void PrintSuccessDiskSave()
        {
            PrintNewLine();
            Console.WriteLine("Данные успешно сохранены!");
        }

        public void PrintNewLine() => Console.WriteLine(string.Empty);

        private static IReadOnlySlot FindSlotByName(Frame frame)
        {
            Console.WriteLine($"Введите имя слота: ");
            string slotName = Console.ReadLine();
            IReadOnlySlot slot = frame.Slots.FirstOrDefault(slot => slot.Name == slotName);

            while (slot == default)
            {
                Console.WriteLine($"Слот с именем {slotName} не найден во фрейме {frame.Name}! Введите другое имя.");
                slotName = Console.ReadLine();
                slot = frame.Slots.FirstOrDefault(slot => slot.Name == slotName);
            }

            return slot;
        }

        public string RequestSlotName(string additionalMessage = "")
        {
            Console.WriteLine($"Введите имя слота: {additionalMessage}");
            string slotName = Console.ReadLine();
            return slotName;
        }

        private string RequestSlotCreationName(IEnumerable<string> createdSlotNames, string frameName)
        {
            string slotName = RequestSlotName();
            while (createdSlotNames.Contains(slotName))
            {
                Console.WriteLine($"Слот с именем {slotName} уже создан во фрейме {frameName}! Введите другое имя.");
                slotName = Console.ReadLine();
            }

            return slotName;
        }

        public string RequestAnySlotValue()
        {
            Console.WriteLine($"Введите значение слота: ");
            return Console.ReadLine().Trim().ToLower();
        }

        public object RequestSlotValue(DataType dataType)
        {
            object slotValue = null;
            while (slotValue == null)
            {
                Console.WriteLine($"Введите значение слота (тип {dataType}): ");

                if (dataType == DataType.Lisp)
                {
                    Console.WriteLine($"Поддерживается функция print. Введите текст для вывода функцией: ");
                }

                string valueReply = Console.ReadLine().Trim().ToLower();
                switch (dataType)
                {
                    case DataType.Boolean:
                        if (bool.TryParse(valueReply, out bool boolValue))
                            slotValue = boolValue;
                        break;

                    case DataType.Integer:
                        if (int.TryParse(valueReply, out int intValue) && CheckNumberValid(intValue))
                            slotValue = intValue;
                        break;

                    case DataType.Real:
                        if (double.TryParse(valueReply, out double doubleValue) && CheckNumberValid((int)doubleValue))
                            slotValue = doubleValue;
                        break;

                    case DataType.Text:
                        slotValue = valueReply;
                        break;

                    case DataType.Frame:
                        throw new NotImplementedException();

                    case DataType.Lisp:
                        if (TryCreateInputLisp(out Lisp lisp, valueReply))
                            slotValue = lisp;
                        break;

                    default: 
                        throw new NotImplementedException();
                }

                if (slotValue != null) break;
                Console.WriteLine("Неправильный ввод! Попробуйте ещё раз.");
            }

            return slotValue;
        }

        public static void RequestFrameName(IEnumerable<string> createdFrameNames, string additionalRequestMessage, 
            out string frameName, out bool alreadyCreated)
        {
            Console.WriteLine($"Введите имя фрейма {additionalRequestMessage}: ");
            frameName = Console.ReadLine();
            alreadyCreated = createdFrameNames.Contains(frameName);
        }

        public void PrintExecutedLisp(Lisp lisp) => Console.WriteLine($"{lisp.OwnerName}: {lisp.PrintMessage}");

        private bool TryCreateInputLisp(out Lisp lisp, string lispPrintMessage)
        {
            lisp = null;
            Console.WriteLine($"Введите когда должна сработать функция: \n" +
                            $"{(int)LispType.ThisFrame}: При добавлении слота в этот фрейм\n" +
                            $"{(int)LispType.SlotName}: При добавлении слота с особым именем в любой фрейм\n" +
                            $"{(int)LispType.SlotValue}: При добавлении слота со значением в любой фрейм\n" +
                            $"{(int)LispType.Inherit}: При добавлении слота-наследника");

            string typeInput = Console.ReadLine();
            if (!Enum.TryParse(typeInput, out LispType lispType))
            {
                return false;
            }

            string slotSearchName = null;
            object slotSearchValue = null;
            string searchInheritorName = null;

            switch (lispType)
            {
                case LispType.SlotName:
                    Console.WriteLine("Введите имя слота: ");
                    slotSearchName = Console.ReadLine();
                    break;

                case LispType.SlotValue:
                    DataType typeToPrint = RequestSlotValueType(DataType.Lisp, DataType.Frame);
                    slotSearchValue = RequestSlotValue(typeToPrint);
                    break;

                case LispType.Inherit:
                    Console.WriteLine("Введите имя наследника: ");
                    searchInheritorName = Console.ReadLine();
                    break;
            }

            lisp = new Lisp(_openedFrame.Name, lispPrintMessage, lispType,
                slotSearchName, slotSearchValue, searchInheritorName);
            return true;
        }

        private DataType RequestSlotValueType(params DataType[] toExclude)
        {
            const DataType notReleased = DataType.Frame;

            return Extenstions.RequestEnumValue(
                "Введите тип данных слота",
                "Неправильный ввод типа данных! Попробуйте ещё раз.",
                toExclude.Append(notReleased));
        }
        
        private bool CheckNumberValid(int number)
        {
            bool valid = number >= 0;
            if (!valid)
            {
                Console.WriteLine($"Число {number} не удовлетворяет условию >= 0");
            }
            return valid;
        }
    }
}
