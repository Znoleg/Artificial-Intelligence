using System;
using System.Collections.Generic;
using System.Linq;
using Lab2.Data;
using Lab2.UI;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2
{
    public class ProductionSystem
    {
        private readonly IEnumerable<Material> _materials;
        private readonly List<Material> _suitableMaterials;
        private readonly IConditionsUserInterface _userInterface;
        private readonly List<(MaterialCondition, string)> _userConditions = 
            new List<(MaterialCondition, string)>();
        private readonly MaterialStats _userMaterialStats;
        private readonly IEnumerable<MaterialCondition> _materialConditions;
        private readonly IReadOnlyDictionary<MaterialCondition, Action<string, MaterialStats>> _onConditionValueRecieved;

        public ProductionSystem(IEnumerable<Material> materials,
            IEnumerable<MaterialCondition> conditions,
            IReadOnlyDictionary<MaterialCondition, Action<string, MaterialStats>> OnConditionValueRecieved,
            IConditionsUserInterface conditionsInterface)
        {
            _materials = materials;
            _suitableMaterials = new List<Material>(_materials);
            _userMaterialStats = new MaterialStats();

            _materialConditions = conditions;
            _onConditionValueRecieved = OnConditionValueRecieved;
            
            _userInterface = conditionsInterface;
        }

        public void StartLooping()
        {
            bool suitableOptionsLeft = true;
            while (suitableOptionsLeft)
            {
                DoLoop();
                suitableOptionsLeft = _suitableMaterials.Any();
            }

            _userInterface.PrintNoOptionsLeft();
        }

        private void DoLoop()
        {
            string requestedMaterialName = _userInterface.RequestHypothesis();
            if (CheckHypothesis(requestedMaterialName) == false)
            {
                PrintConculsion(false, requestedMaterialName);
                return;
            }

            bool conditionsChecked = _userConditions.Count == _materialConditions.Count();
            if (!conditionsChecked)
            {
                RequestConditions(requestedMaterialName);
            }
            else
            {
                PrintConculsion(true, requestedMaterialName);
            }
        }

        private void RequestConditions(string requestedMaterialName)
        {
            IEnumerable<MaterialCondition> conditionToCheck = _materialConditions.Except(_userConditions
                .Select(t => t.Item1));

            foreach (MaterialCondition condition in conditionToCheck)
            {
                string conditionValueStr = _userInterface.RequestConditionValue(condition);
                _onConditionValueRecieved[condition](conditionValueStr, _userMaterialStats);

                ApplyCondition(condition);

                _userInterface.PrintNewLine();
                _userInterface.PrintMemoryPool(_userConditions.Select(t => t.Item2));
                _userInterface.PrintNewLine();

                bool hypothesisAlive = CheckHypothesis(requestedMaterialName);
                if (!hypothesisAlive)
                {
                    PrintConculsion(false, requestedMaterialName);
                    return;
                }
            }

            PrintConculsion(true, requestedMaterialName);
        }

        private bool CheckHypothesis(string requestedMaterialName) =>
            _suitableMaterials
                .Select(mat => mat.MaterialName.ToLower())
                .Contains(requestedMaterialName);

        private void ApplyCondition(MaterialCondition toApply)
        {
            _userConditions.Add((toApply, StringProvider.GetParametrizedConditionString(toApply, _userMaterialStats)));
            _suitableMaterials.RemoveAll(mat => toApply(mat.Stats, _userMaterialStats) == false);
        }

        private void PrintConculsion(bool status, string requestedMaterialName)
        { 
            _userInterface.PrintHypothesisConclusion(status, requestedMaterialName);
            _userInterface.PrintNewLine();
        }
    }
}
