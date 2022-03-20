using System;

namespace Lab2.Data
{
    [Serializable]
    public struct Material
    {
        public string MaterialName;
        public MaterialStats Stats;

        public Material(string materialName, MaterialStats stats)
        {
            MaterialName = materialName;
            Stats = stats;
        }
    }
}
