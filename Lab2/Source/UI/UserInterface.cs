using Lab2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2.UI
{
    public class UserInterface : IGlobalConditionStringProvider, IConditionsUserInterface
    {
        private readonly IEnumerable<string> _materialNames;
        private readonly string _exitMessage;

        public event Action OnExitPrint;

        public UserInterface(IEnumerable<string> materialNames, string exitMessage = "выйти")
        {
            _materialNames = materialNames;
            _exitMessage = exitMessage.ToLower();
        }

        /// <returns>String representing material name</returns>
        public string RequestHypothesis()
        {
            Console.WriteLine($"Пожалуйста, выдвините гепотезу о подходящем материале, " +
                $"например \"{_materialNames.First()}\"");

            string materialsString = _materialNames.ToSingleStr();

            Console.WriteLine($"Список материалов: {materialsString}");
            string materialName = GetInput();

            bool materialNotExists = !_materialNames.Select(name => name.ToLower()).Contains(materialName);
            if (materialNotExists)
            {
                Console.WriteLine($"Материала {materialName} не существует! Пожалуйста, попробуйте ещё раз!");
                return RequestHypothesis();
            }

            return materialName;
        }

        public void PrintExitSuggestion() =>
            Console.WriteLine($"Напишите \"{_exitMessage}\" для выхода из приложения.");

        public string RequestConditionValue(MaterialCondition condition)
        {
            string reqConditionMessage = StringProvider.GetNonParametrizedConditionString(condition);

            Console.WriteLine($"Введите значение атрибута {reqConditionMessage}: ");
            string conditionValue = GetInput();
            if (!ValidateConditionValue(condition, conditionValue))
            {
                Console.WriteLine($"Неправильный ввод! Попробуйте ещё раз.");
                return RequestConditionValue(condition);
            }

            return conditionValue;
        }

        public void PrintNewLine() => Console.Write(Environment.NewLine);

        public void PrintMemoryPool(IEnumerable<string> memoryPool) =>
            Console.WriteLine($"Основная память: {Environment.NewLine}" +
                $"{string.Join($",{Environment.NewLine}", memoryPool)}");

        public void PrintHypothesisConclusion(bool confirmed, string materialName)
        {
            if (confirmed)
            {
                Console.WriteLine($"Гипотеза подтвердилась! Материал {materialName} вам подходит!");
            }
            else
            {
                Console.WriteLine($"Гипотеза не подтвердилась! Материал {materialName} вам не подходит!");
            }
        }

        public void PrintNoOptionsLeft()
        {
            Console.WriteLine("База знаний не содержит удовлетворяющий критериям результат.");
        }

        public string GetMaterialConditions(IEnumerable<MaterialCondition> conditions, Material material)
        {
            const string andSepatator = " & ";

            string conditionStr = "IF ";
            foreach (MaterialCondition condition in conditions)
            {
                conditionStr += StringProvider.GetParametrizedConditionString(condition, material.Stats) 
                    + andSepatator;
            }

            conditionStr = conditionStr.Substring(0, conditionStr.LastIndexOf(andSepatator));
            conditionStr += $" THEN {StringProvider.Material} = {material.MaterialName}";

            return conditionStr;
        }

        private bool ValidateConditionValue(MaterialCondition condition, string conditionValue)
        {
            if (condition == IsRawMaterialTypeEquals)
            {
                try
                {
                    StringProvider.StrToMaterialType(conditionValue);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                bool inputIsRight = uint.TryParse(conditionValue, out uint printedNumber);
                if (inputIsRight && condition != IsPriceNotHigher)
                {
                    inputIsRight &= IsInRange(printedNumber, GetConditionRange(condition));
                }

                if (!inputIsRight)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInRange(uint number, Range range) =>
            number >= range.Start.Value && number < range.End.Value;

        private string GetInput()
        {
            string userInput = Console.ReadLine().ToLower();
            if (userInput == _exitMessage)
            {
                OnExitPrint?.Invoke();
            }

            return userInput;
        }
    }
}
