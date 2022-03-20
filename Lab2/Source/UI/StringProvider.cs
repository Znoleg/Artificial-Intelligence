using Lab2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lab2.Predicates.MaterialConditions;

namespace Lab2.UI
{
    public static class StringProvider
    {
        public const string Material = "Материал";
        public const string MaterialTypeName = "Тип сырья";
        public const string PriceName = "Цена за метр";
        public const string QualityName = "Качество";
        public const string AllergicName = "Уровень аллергии";
        public const string AbsorbtionName = "Уровень поглощения влаги";
        public const string BreathabilityName = "Уровень воздухопроницаемости";
        public const string ThermoregulationName = "Уровень терморегуляции";

        private static readonly Dictionary<MaterialCondition, Func<MaterialStats, string>> _parametrizedConditionStrings;
        private static readonly Dictionary<MaterialCondition, string> _nonParametrizedConditionStrings;

        static StringProvider()
        {
            _parametrizedConditionStrings = new Dictionary<MaterialCondition, Func<MaterialStats, string>>
            {
                { IsRawMaterialTypeEquals, IsRawMaterialTypeEqualsToString },
                { IsPriceNotHigher, IsPriceNotHigherToString },
                { IsQualityNotLower, IsQualityNotLowerToString },
                { IsAllergicNotHigher, IsAllergicNotHigherToString },
                { IsAbsorptionNotLower, IsAbsorptionNotLowerToString },
                { IsBreathabilityNotLower, IsBreathabilityNotLowerToString },
                { IsThermoregulationNotLower, IsThermoregulationNotLowerToString }
            };

            _nonParametrizedConditionStrings = new Dictionary<MaterialCondition, string>
            {
                { IsRawMaterialTypeEquals, $"{MaterialTypeName} " +
                    $"[{MaterialTypeValues.ToSingleStr()}]"},
                { IsPriceNotHigher, $"Максимальная {PriceName} " +
                    $"[значения больше 0]"},
                { IsQualityNotLower, $"Минимальное {QualityName} " +
                    $"[{RangeToString(MaterialStats.QualityRange)}]"},
                { IsAllergicNotHigher, $"Максимальный {AllergicName} " +
                    $"[{RangeToString(MaterialStats.AllergicRange)}]"},
                { IsAbsorptionNotLower, $"Минимальный {AbsorbtionName} " +
                    $"[{RangeToString(MaterialStats.AbsorptionRange)}]"},
                { IsBreathabilityNotLower, $"Минимальный {BreathabilityName} " +
                    $"[{RangeToString(MaterialStats.BreatabilityRange)}]"},
                { IsThermoregulationNotLower, $"Минимальный {ThermoregulationName} " +
                    $"[{RangeToString(MaterialStats.ThermoregulationRange)}]"}
            };
        }

        public static string[] MaterialTypeValues => new[] { "Натуральный", "Искусственный", "Синтетический" };

        public static string RangeToString(Range range)
        {
            string[] values = new string[range.End.Value - range.Start.Value];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = $"{range.Start.Value + i}";
            }

            return values.ToSingleStr();
        }

        public static string GetParametrizedConditionString(MaterialCondition condition, MaterialStats userStats) =>
            _parametrizedConditionStrings[condition](userStats);

        public static string GetNonParametrizedConditionString(MaterialCondition condition) =>
            _nonParametrizedConditionStrings[condition];

        public static string IsRawMaterialTypeEqualsToString(MaterialStats userStats) =>
            $"{MaterialTypeName} = {RawMaterialTypeToStr(userStats.RawMaterialType)}";

        public static string IsPriceNotHigherToString(MaterialStats userStats) =>
            $"Максимальная {PriceName} = {userStats.PricePerMeter}";

        public static string IsQualityNotLowerToString(MaterialStats userStats) =>
            $"Минимальное {QualityName} = {userStats.Quality}";

        public static string IsAllergicNotHigherToString(MaterialStats userStats) =>
            $"Максимальный {AllergicName} = {userStats.AllergicLevel}";

        public static string IsAbsorptionNotLowerToString(MaterialStats userStats) =>
            $"Минимальный {AbsorbtionName} = {userStats.MoistureAbsorption}";

        public static string IsBreathabilityNotLowerToString(MaterialStats userStats) =>
            $"Минимальный {BreathabilityName} = {userStats.Breatability}";

        public static string IsThermoregulationNotLowerToString(MaterialStats userStats) =>
            $"Минимальный {ThermoregulationName} = {userStats.Thermoregulation}";

        public static IEnumerable<string> GetMaterialsNames(IEnumerable<Material> materials) =>
            materials.Select(mat => mat.MaterialName);

        public static RawMaterialType StrToMaterialType(string materialType)
        {
            materialType = materialType.ToLower();
            return materialType switch
            {
                "натуральный" => RawMaterialType.Natural,
                "искусственный" => RawMaterialType.Artificial,
                "синтетический" => RawMaterialType.Synthetic,
                _ => throw new ArgumentException(),
            };
        }

        public static string RawMaterialTypeToStr(RawMaterialType materialType)
        {
            return materialType switch
            {
                RawMaterialType.Natural => "натуральный",
                RawMaterialType.Artificial => "искусственный",
                RawMaterialType.Synthetic => "синтетический",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
