using Lab2.Data;
using Lab2.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2
{
    public class Bootstrapper
    {
        private readonly string _materialsFilePath;
        private readonly string _globalBaseFilePath;
        private readonly string _localBaseFilePath;
        private MaterialSerializer _materialSerializer;

        public IEnumerable<Material> Materials { get; private set; }
        public UserInterface UserInterface { get; private set; }
        
        public static readonly IReadOnlyDictionary<MaterialCondition, Action<string, MaterialStats>> OnConditionValueRecieved;
        public static readonly IEnumerable<MaterialCondition> Conditions;

        static Bootstrapper()
        {
            OnConditionValueRecieved = new Dictionary<MaterialCondition, Action<string, MaterialStats>>
            {
                { IsRawMaterialTypeEquals, (str, stats) => stats.RawMaterialType = StringProvider.StrToMaterialType(str) },
                { IsPriceNotHigher, (str, stats) => stats.PricePerMeter = ulong.Parse(str) },
                { IsQualityNotLower, (str, stats) => stats.Quality = uint.Parse(str) },
                { IsAllergicNotHigher, (str, stats) => stats.AllergicLevel = uint.Parse(str) },
                { IsAbsorptionNotLower, (str, stats) => stats.MoistureAbsorption = uint.Parse(str) },
                { IsBreathabilityNotLower, (str, stats) => stats.Breatability = uint.Parse(str) },
                { IsThermoregulationNotLower, (str, stats) => stats.Thermoregulation = uint.Parse(str) }
            };
            Conditions = OnConditionValueRecieved.Keys;
        }

        public Bootstrapper(string materialsFilePath, string globalBaseFilePath, string localBaseFilePath)
        {
            _materialsFilePath = materialsFilePath;
            _globalBaseFilePath = globalBaseFilePath;
            _localBaseFilePath = localBaseFilePath;
        }

        public void Bootstrap()
        {
            _materialSerializer = new MaterialSerializer(_materialsFilePath);

            bool firstCreation = !File.Exists(_materialsFilePath) || !File.Exists(_globalBaseFilePath) ||
                !File.Exists(_localBaseFilePath);
            if (firstCreation)
            {
                SaveMaterials(CreateStandartMaterials());
            }

            Materials = _materialSerializer.ReadAll();
            
            var materialNames = Materials.Select(mat => mat.MaterialName);
            UserInterface = new UserInterface(materialNames);

            if (firstCreation)
            {
                SaveBases();
            }
        }

        private void SaveBases()
        {
            var condtitionsSerializer = new ConditionsSerialzer(_localBaseFilePath,
                            _globalBaseFilePath, UserInterface);
            condtitionsSerializer.SaveLocalBase(Conditions);
            condtitionsSerializer.SaveGlobalBase(Conditions, Materials);
        }

        private void SaveMaterials(IEnumerable<Material> materials)
        {
            foreach (Material material in materials)
            {
                _materialSerializer.Add(material, forceSave: false);
            }

            _materialSerializer.SaveAll();
        }

        private static Material[] CreateStandartMaterials()
        {
            return new[]
            {
                new Material("Хлопок", new MaterialStats(RawMaterialType.Natural,
                    pricePerMeter: 1137, quality: 8, allergicLevel: 2, moistureAbsorption: 3, breatability: 3, thermoregulation: 3)),
                new Material("Лён", new MaterialStats(RawMaterialType.Natural,
                    pricePerMeter: 1669, quality: 7, allergicLevel: 2, moistureAbsorption: 2, breatability: 5, thermoregulation: 5)),
                new Material("Шерсть", new MaterialStats(RawMaterialType.Natural,
                    pricePerMeter: 1200, quality: 8, allergicLevel: 0, moistureAbsorption: 4, breatability: 3, thermoregulation: 3)),
                new Material("Бамбук", new MaterialStats(RawMaterialType.Artificial,
                    pricePerMeter: 600, quality: 4, allergicLevel: 3, moistureAbsorption: 2, breatability: 5, thermoregulation: 2)),
                new Material("Ацетат", new MaterialStats(RawMaterialType.Artificial,
                    pricePerMeter: 800, quality: 4, allergicLevel: 4, moistureAbsorption: 1, breatability: 2, thermoregulation: 4)),
                new Material("Акрил", new MaterialStats(RawMaterialType.Synthetic,
                    pricePerMeter: 400, quality: 2, allergicLevel: 2, moistureAbsorption: 0, breatability: 1, thermoregulation: 2)),
                new Material("Джордан", new MaterialStats(RawMaterialType.Synthetic,
                    pricePerMeter: 300, quality: 3, allergicLevel: 3, moistureAbsorption: 0, breatability: 1, thermoregulation: 3))
            };
        }
    }
}
