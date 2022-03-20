using Lab2.UI;
using System.Collections.Generic;
using System.IO;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2.Data
{
    public class ConditionsSerialzer
    {
        private readonly string _localBasePath;
        private readonly string _globalBasePath;
        private readonly IGlobalConditionStringProvider _globalConditionProvider;
        private readonly FileMode _openMode;

        public ConditionsSerialzer(string localBaseFilePath, string globalBaseFilePath, 
            IGlobalConditionStringProvider globalConditionProvider, FileMode openMode = FileMode.OpenOrCreate)
        {
            _localBasePath = localBaseFilePath;
            _globalBasePath = globalBaseFilePath;
            _globalConditionProvider = globalConditionProvider;
            _openMode = openMode;
        }

        public void SaveLocalBase(IEnumerable<MaterialCondition> conditions)
        {
            using StreamWriter writer = new StreamWriter(new FileStream(_localBasePath, _openMode,
                FileAccess.Write));
            writer.BaseStream.SetLength(0);
            
            foreach (MaterialCondition condition in conditions)
            {
                writer.WriteLine(StringProvider.GetNonParametrizedConditionString(condition));
            }
        }

        public void SaveGlobalBase(IEnumerable<MaterialCondition> conditions, IEnumerable<Material> materials)
        {
            using StreamWriter writer = new StreamWriter(new FileStream(_globalBasePath, _openMode,
                FileAccess.Write));
            writer.BaseStream.SetLength(0);

            foreach (Material material in materials)
            {
                 writer.WriteLine(_globalConditionProvider.GetMaterialConditions(conditions, material));
            }
        }
    }
}
