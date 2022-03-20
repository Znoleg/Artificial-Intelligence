using System;

namespace Lab2.Data
{
    public enum RawMaterialType
    {
        Natural, Artificial, Synthetic
    }

    [Serializable]
    public class MaterialStats
    {
        public RawMaterialType RawMaterialType;
        public ulong PricePerMeter;
        public uint Quality;
        public uint AllergicLevel;
        public uint MoistureAbsorption;
        public uint Breatability;
        public uint Thermoregulation;

        public readonly static Range QualityRange = new Range(0, 9);
        public readonly static Range AllergicRange = new Range(0, 7);
        public readonly static Range AbsorptionRange = new Range(0, 5);
        public readonly static Range BreatabilityRange = new Range(0, 7);
        public readonly static Range ThermoregulationRange = new Range(0, 7);

        public MaterialStats()
        { }

        public MaterialStats(RawMaterialType rawMaterialType, 
            ulong pricePerMeter, 
            uint quality, 
            uint allergicLevel, 
            uint moistureAbsorption, 
            uint breatability, 
            uint thermoregulation)
        {
            RawMaterialType = rawMaterialType;
            PricePerMeter = pricePerMeter;
            Quality = ClampStat(quality, QualityRange);
            AllergicLevel = ClampStat(allergicLevel, AllergicRange);
            MoistureAbsorption = ClampStat(moistureAbsorption, AbsorptionRange);
            Breatability = ClampStat(breatability, BreatabilityRange);
            Thermoregulation = ClampStat(thermoregulation, ThermoregulationRange);
        }

        private uint ClampStat(uint stat, Range range) =>
            Math.Clamp(stat, (uint)range.Start.Value, (uint)range.End.Value);
    }
}
