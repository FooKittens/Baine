using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Baine
{
    class SaveFileManager
    {
        public enum SaveTypeIdentifier : byte
        {
            Sprite,
            Baine,
            Platform,
            HealthPotion,
            Trap,
            Ladder,
            Enemy2
        }

        public void SaveLevel(string name)
        {
            // Collect data
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(s);
            Game1.level.GetSaveData(w);
            byte[] data = s.ToArray();

            // Save data
            string path = System.IO.Directory.GetCurrentDirectory() + @"\" + name + ".lvl";
            Stream stream = File.Create(path);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        public void SaveProgress(string name)
        {
            // Collect data
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(s);
            w.Write((int)Game1.currentLevel);
            Game1.level.GetSaveData(w);
            Game1.baine.GetSaveData(w);
            byte[] data = s.ToArray();

            // Save data
            string path = System.IO.Directory.GetCurrentDirectory() + @"\" + name + ".pro";
            Stream stream = File.Create(path);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        public void LoadLevel(string name)
        {
            // Collect data
            string path = System.IO.Directory.GetCurrentDirectory() + @"\" + name + ".lvl";
            if (File.Exists(path))
            {
                FileStream stream;
                stream = File.Open(path, FileMode.Open);
                byte[] data;
                data = new byte[(int)stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                // Assign data
                System.IO.MemoryStream s = new System.IO.MemoryStream(data);
                System.IO.BinaryReader r = new System.IO.BinaryReader(s);
                if (r.BaseStream.Position == r.BaseStream.Length)
                {
                    Console.WriteLine("Nothing to load");
                    return;
                }
                Game1.level.ClearLevel();
                Game1.level.startPos.X = r.ReadSingle();
                Game1.level.startPos.Y = r.ReadSingle();
                Game1.level.goalPos.X = r.ReadSingle();
                Game1.level.goalPos.Y = r.ReadSingle();
                for (;;)
                {
                    //try
                    //{
                    SaveTypeIdentifier identifier = (SaveFileManager.SaveTypeIdentifier)r.ReadByte();
                    switch (identifier)
                    {
                        case SaveTypeIdentifier.Sprite:
                            Game1.level.AddSprite(new Sprite(r));
                            break;
                        case SaveTypeIdentifier.Platform:
                            Game1.level.AddSprite(new Platform(r));
                            break;
                        case SaveTypeIdentifier.Trap:
                            Game1.level.AddSprite(new Trap(r));
                            break;
                        case SaveTypeIdentifier.Baine:
                            break;
                        case SaveTypeIdentifier.Enemy2:
                            Game1.level.AddSprite(new Enemy2(r));
                            break;
                        case SaveTypeIdentifier.HealthPotion:
                            Game1.level.AddSprite(new HealthPotion(r));
                            break;
                    }
                    if (r.BaseStream.Position == r.BaseStream.Length)
                    {
                        Console.WriteLine("Load successful");
                        break;
                    }
                    //}
                    //catch
                    //{
                    //    Console.WriteLine("Corrupt save file");
                    //}
                }
                stream.Close();
            }
        }

        public void LoadProgress(string name)
        {
            // Collect data
            string path = System.IO.Directory.GetCurrentDirectory() + @"\" + name + ".pro";
            if (File.Exists(path))
            {
                FileStream stream;
                stream = File.Open(path, FileMode.Open);
                byte[] data;
                data = new byte[(int)stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                // Assign data
                System.IO.MemoryStream s = new System.IO.MemoryStream(data);
                System.IO.BinaryReader r = new System.IO.BinaryReader(s);
                if (r.BaseStream.Position == r.BaseStream.Length)
                {
                    Console.WriteLine("Nothing to load");
                    return;
                }
                Game1.level.ClearLevel();
                Game1.currentLevel = r.ReadInt32();
                Game1.level.startPos.X = r.ReadSingle();
                Game1.level.startPos.Y = r.ReadSingle();
                Game1.level.goalPos.X = r.ReadSingle();
                Game1.level.goalPos.Y = r.ReadSingle();
                for (; ; )
                {
                    //try
                    //{
                    SaveTypeIdentifier identifier = (SaveFileManager.SaveTypeIdentifier)r.ReadByte();
                    switch (identifier)
                    {
                        case SaveTypeIdentifier.Sprite:
                            Game1.level.AddSprite(new Sprite(r));
                            break;
                        case SaveTypeIdentifier.Platform:
                            Game1.level.AddSprite(new Platform(r));
                            break;
                        case SaveTypeIdentifier.Trap:
                            Game1.level.AddSprite(new Trap(r));
                            break;
                        case SaveTypeIdentifier.Baine:
                            Game1.baine = new Baine(r);
                            break;
                        case SaveTypeIdentifier.Enemy2:
                            Game1.level.AddSprite(new Enemy2(r));
                            break;
                        case SaveTypeIdentifier.HealthPotion:
                            Game1.level.AddSprite(new HealthPotion(r));
                            break;
                    }
                    if (r.BaseStream.Position == r.BaseStream.Length)
                    {
                        Console.WriteLine("Load successful");
                        break;
                    }
                    //}
                    //catch
                    //{
                    //    Console.WriteLine("Corrupt save file");
                    //}
                }
                stream.Close();
            }
        }
    }
}
