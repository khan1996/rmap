using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
{
    [Serializable]
    public class ObjectGroup : SceneObject
    {
        public string[] AppliesTo = new string[4];
        [Private]
        public List<SavedObject> SavedObjects = new List<SavedObject>();
        [Private]
        public List<SavedLight> SavedLights = new List<SavedLight>();

        public override string ToString()
        {
            return string.Join(", ", AppliesTo.Where(s => !string.IsNullOrEmpty(s)).ToArray());
        }

        public static ObjectGroup LoadFrom(BinaryReader br, Scene scene)
        {
            ObjectGroup group = new ObjectGroup();

            for (int i = 0; i < 4; i++)
                group.AppliesTo[i] = br.ReadString(256);

            uint sec70count = br.ReadUInt32();

            for (int s70c = 0; s70c < sec70count; s70c++)
            {
                SavedObject fo2 = SavedObject.LoadFrom(br);
                fo2.Scene = scene;
                group.SavedObjects.Add(fo2);
            }

            uint sec76count = br.ReadUInt32();

            for (int s76c = 0; s76c < sec76count; s76c++)
            {
                SavedLight fo3 = SavedLight.LoadFrom(br);
                fo3.Scene = scene;
                group.SavedLights.Add(fo3);
            }

            return group;
        }

        public void SaveTo(BinaryWriter bw)
        {
            foreach (string applies in AppliesTo)
                bw.Write(applies, 256);

            bw.Write((uint)SavedObjects.Count);
            foreach (SavedObject fo2 in SavedObjects)
                fo2.SaveTo(bw);

            bw.Write((uint)SavedLights.Count);
            foreach (SavedLight fo3 in SavedLights)
                fo3.SaveTo(bw);
        }
    }
}
