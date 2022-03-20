using Lab2.Data;
using System.Collections.Generic;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2.UI
{
    public interface IGlobalConditionStringProvider
    {
        string GetMaterialConditions(IEnumerable<MaterialCondition> conditions, Material material);
    }
}