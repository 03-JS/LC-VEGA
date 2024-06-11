using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace LC_VEGA
{
    internal class SaveManager
    {
        public static string fileName = "\\VEGA_Interactions.bin";

        public static bool playedIntro;
        public static bool firstTimeDiversity;
        public static bool hadDiversity;

        public static void SaveToFile()
        {
            using BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + fileName, FileMode.Create));
            writer.Write(playedIntro);
            writer.Write(firstTimeDiversity);
            writer.Write(hadDiversity);
        }

        public static List<bool> ReadFromFile()
        {
            List<bool> values = new List<bool>();
            using (FileStream fs = new FileStream(Application.persistentDataPath + fileName, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    values.Add(reader.ReadBoolean());
                }
            }
            return values;
        }

        public static bool GetValueFromIndex(int index)
        {
            List<bool> values = ReadFromFile();
            if (values.Count > index)
            {
                return values[index];
            }
            return false;
        }
    }
}
