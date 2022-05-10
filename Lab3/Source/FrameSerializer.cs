using FrameKnowledgeModel.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FrameKnowledgeModel
{
    public class FrameSerializer
    {
        private readonly string _filePath;

        public FrameSerializer(string filePath)
        {
            _filePath = filePath;
        }

        public void Save(IReadOnlyList<Frame> frames)
        {
            using StreamWriter streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.Truncate, FileAccess.Write));
            string jsonFrames = JsonConvert.SerializeObject(frames);
            streamWriter.Write(jsonFrames);
        }

        public List<Frame> Load()
        {
            using StreamReader streamReader = new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read));
            string fileContent = streamReader.ReadToEnd();
            List<Frame> fileFrames = new List<Frame>();
            fileFrames = JsonConvert.DeserializeObject<List<Frame>>(fileContent);

            IEnumerable<Slot> lispSlots = fileFrames
                .SelectMany(frame => frame.Slots)
                .Where(slot => slot.DataType == DataType.Lisp);

            foreach (var slot in lispSlots)
            {
                JObject data = slot.DataValue as JObject;
                string json = data.ToString();
                slot.DataValue = JsonConvert.DeserializeObject<Lisp>(json);
            }

            return fileFrames;
        }
    }
}
