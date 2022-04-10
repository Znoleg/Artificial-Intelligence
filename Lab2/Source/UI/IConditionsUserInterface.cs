using System;
using System.Collections.Generic;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2.UI
{
    public interface IConditionsUserInterface
    {
        void PrintExitSuggestion();
        string RequestHypothesis();
        string RequestConditionValue(MaterialCondition condition);
        void PrintNewLine();
        void PrintMemoryPool(IEnumerable<string> memoryPool);
        void PrintHypothesisConclusion(bool confirmed, string materialName);
        void PrintNoOptionsLeft();
    }
}