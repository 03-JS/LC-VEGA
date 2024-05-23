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
        // public static bool firstTimeGary;

        public static void SaveToFile()
        {
            using BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + fileName, FileMode.Create));
            writer.Write(playedIntro);
            writer.Write(firstTimeDiversity);
            // writer.Write(firstTimeGary);
        }

        public static bool LoadFromFile(int index)
        {
            using BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + fileName, FileMode.Open));
            while (reader.BaseStream.Position == index)
            {
                return reader.ReadBoolean();
            }
            return false;
        }
    }
}
