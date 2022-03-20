using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Lab2.Data
{
    public class MaterialSerializer
    {
        private readonly string _filePath;
        private readonly FileMode _openMode;
        private readonly List<Material> _materials;

        public MaterialSerializer(string filePath, FileMode openMode = FileMode.OpenOrCreate)
        {
            _filePath = filePath;
            _openMode = openMode;
            _materials = new List<Material>();
        }

        public void Add(Material material, bool forceSave)
        {
            _materials.Add(material);
            if (forceSave)
            {
                SaveAll();
            }
        }

        public IReadOnlyList<Material> ReadAll(bool refreshOld = true)
        {
            if (refreshOld)
            {
                _materials.Clear();
            }

            using (StreamReader reader = new StreamReader(new FileStream(_filePath, _openMode, FileAccess.Read)))
            {
                string materialsStr = reader.ReadLine();
                var uniqueMaterials = JsonConvert.DeserializeObject<Material[]>(materialsStr);
                _materials.AddRange(uniqueMaterials);
            }

            return _materials;
        }

        public void SaveAll()
        {
            using StreamWriter writer = new StreamWriter(new FileStream(_filePath, _openMode, FileAccess.Write));
            writer.BaseStream.SetLength(0);

            string materialsStr = JsonConvert.SerializeObject(_materials.ToArray());
            writer.WriteLine(materialsStr);
        }
    }
}
