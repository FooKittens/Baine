using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace MajgEngine
{
    [Serializable]
    static class Serializer
    {
        //Save object
        public static void Serialize(string filename, Object toSave)
        {
            FileStream fileStream = File.Create(filename);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, toSave);
            fileStream.Close();
        }

        //Load object
        public static Object DeSerialize(string filename)
        {
            Object toSave = null;
            if (File.Exists(filename))
            {
                FileStream fileStream = File.OpenRead(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                toSave = binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
            return toSave;
        }
    }
}
