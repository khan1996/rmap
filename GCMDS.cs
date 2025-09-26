using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace rMap.Asset.FileTypes
{
    class GCMDS
    {
        public string SkeletonKey;
        public int MeshType;
        public Dictionary<string, int> SkeletonParts = new Dictionary<string, int>();
        public Dictionary<string, int> OutfitSlots = new Dictionary<string, int>();
        public List<string> AttachmentSlots = new List<string>();
        public List<string> StaticSlots = new List<string>();
        public List<AttachmentSet> AttachmentSets = new List<AttachmentSet>();
        public List<MotionSheet> MotionSheets = new List<MotionSheet>();
        /// <summary>
        /// OutfitSets[set] == (mesh, texture)
        /// </summary>
        public List<OutfitSet> OutfitSets = new List<OutfitSet>();
        public string DefaultOutfit;
        public MotionSheet DefaultMotionSheet;
        public MSMotion DefaultMotion;
        public bool? AlphaUsed;
        public int? NameTagBias;
        public List<string> Effects = new List<string>();
        public float? DefaultScale;
        public List<ShaderInfo> Shaders = new List<ShaderInfo>();
        public List<BoundingCylinder> BoundingCylinders = new List<BoundingCylinder>();
        /// <summary>
        /// DefaultAttachmentSets[slot] = set
        /// </summary>
        public Dictionary<string, AttachmentSet> DefaultAttachmentSets = new Dictionary<string, AttachmentSet>();

        public string FileName { get; set; }
        public byte[] CryptoKey { get; set; }

        public GCMDS()
        {
            SkeletonParts.Add("BODY", 0);
            SkeletonParts.Add("HEAD", 0);

            MeshType = 1;
            OutfitSlots.Add("BODY", 1);
        }

        public GCMDS(string fileName)
        {
            Load(fileName);
        }

        public GCMDS(string fileName, byte[] crypto)
        {
            CryptoKey = crypto;
            Load(fileName);
        }

        private float[] ReadFloatArray(string line, bool skipLastLetter = true)
        {
            if (line.EndsWith("A"))
                line = line.Substring(0, line.Length - 1);

            List<float> arr = new List<float>();
            string[] splits = line.Split(new[] { " ", "\t", "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in splits)
            {
                string des = skipLastLetter ? s.Substring(0, s.Length - 1) : s;
                arr.Add(float.Parse(des, System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            }
            return arr.ToArray();
        }

        public void Load(string fileName)
        {
            FileName = fileName;

            byte[] data = File.ReadAllBytes(fileName);

            if (CryptoKey != null && CryptoKey.Length != 0)
                BinaryTools.Xor(data, CryptoKey);

            using (MemoryStream ms = new MemoryStream(data))
            {
                string[] lines = new StreamReader(ms).ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                bool readSkelpart = false;
                bool readMotionSheets = false;
                bool readOutfitSlots = false;
                bool readOutfitSets = false;
                OutfitSet readingSet = null;
                MotionSheet readingSheet = null;
                MSMotion readingMotion = null;
                bool readEffects = false;
                bool readAttachmentSlots = false;
                bool readAttachmentSets = false;
                bool readDefaultAttachmentSets = false;
                AttachmentSet readingAttachmentSet = null;
                AttachmentHolder readingAttachmentHolder = null;
                bool readStaticSlot = false;
                bool readShaders = false;
                bool readBounding = false;

                foreach (string line2 in lines)
                {
                    string line = line2.Trim();

                    if (line == "" || line.StartsWith("//"))
                        continue;

                    if (line.Contains("//"))
                        line = line.Substring(0, line.IndexOf("//")).Trim();

                    string[] splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                    if (readDefaultAttachmentSets)
                    {
                        bool ends = line.Contains(";");

                        if (ends)
                            line = line.Substring(0, line.IndexOf(';')).Trim();
                        if (line != "")
                        {
                            string setname;
                            if (splits[1].StartsWith("\""))
                            {
                                line = line.Substring(splits[0].Length).Trim().Substring(1);
                                setname = line.Substring(0, line.IndexOf("\""));
                            }
                            else
                                setname = splits[1];

                            DefaultAttachmentSets.Add(splits[0], AttachmentSets.Single(x => x.Name == setname));
                        }
                        if (ends)
                            readDefaultAttachmentSets = false;
                    }
                    else if (readAttachmentSets && readingAttachmentSet != null && readingAttachmentHolder != null)
                    {
                        if (splits[0] == ";")
                            readingAttachmentHolder = null;
                        else if (splits[0] == "skel")
                            readingAttachmentHolder.Skeleton = splits[1];
                        else if (splits[0] == "pos")
                        {
                            float[] a = ReadFloatArray(line.Substring(splits[0].Length).Trim());
                            readingAttachmentHolder.Position = new Vector3(a[0], a[1], a[2]);
                        }
                        else if (splits[0] == "rot")
                        {
                            float[] a = ReadFloatArray(line.Substring(splits[0].Length).Trim());
                            readingAttachmentHolder.Rotation = new Quaternion(a[0], a[1], a[2], a[3]);
                        }
                    }
                    else if (readAttachmentSets && readingAttachmentSet != null)
                    {
                        if (splits[0] == ";")
                            readingAttachmentSet = null;
                        else if (splits[0] == "type")
                            readingAttachmentSet.Type = int.Parse(splits[1]);
                        else if (splits[0] == "mesh")
                        {
                            if (splits[1].StartsWith("\""))
                            {
                                line = line.Substring(splits[0].Length).Trim().Substring(1);
                                readingAttachmentSet.Mesh = line.Substring(0, line.IndexOf("\""));
                            }
                            else
                                readingAttachmentSet.Mesh = splits[1];
                        }
                        else if (splits[0] == "tex") // tex		"wp_sword.DDS" : "wp_sword_bump.DDS"
                        {
                            bool escaped = splits[1].StartsWith("\"");
                            if (escaped)
                            {
                                line = line.Substring(splits[0].Length).Trim();
                                readingAttachmentSet.Texture = line.Substring(1, line.IndexOf("\"", 1) - 1);
                            }
                            else
                                readingAttachmentSet.Texture = splits[1];

                            line = line.Substring(readingAttachmentSet.Texture.Length + (escaped ? 2 : 0)).Trim();

                            if (line.StartsWith(":"))
                            {
                                line = line.Substring(1).Trim();
                                escaped = line.StartsWith("\"");
                                if (escaped)
                                {
                                    readingAttachmentSet.BumpTexture = line.Substring(1, line.IndexOf("\"", 1) - 1);
                                }
                                else
                                    readingAttachmentSet.BumpTexture = line;
                            }
                        }
                        else if (splits[0] == "holder")
                        {
                            readingAttachmentHolder = new AttachmentHolder();
                            readingAttachmentSet.Holders.Add(readingAttachmentHolder);
                        }
                    }
                    else if (readAttachmentSets)
                    {
                        if (splits[0] == ";")
                            readAttachmentSets = false;
                        else if (splits[0] == "set")
                        {
                            readingAttachmentSet = new AttachmentSet();

                            if (splits[1].StartsWith("\""))
                            {
                                line = line.Substring(splits[0].Length).Trim().Substring(1);
                                readingAttachmentSet.Name = line.Substring(0, line.IndexOf("\""));
                            }
                            else
                                readingAttachmentSet.Name = splits[1];
                            AttachmentSets.Add(readingAttachmentSet);
                        }
                    }
                    else if (readSkelpart)
                    {
                        if (splits[0] == ";")
                            readSkelpart = false;
                        else
                            SkeletonParts.Add(splits[0], int.Parse(splits[1]));
                    }
                    else if (readMotionSheets)
                    {
                        if (splits[0] == ";" && readingMotion == null)
                        {
                            if (readingSheet != null)
                            {
                                if (readingSheet.Parent != null)
                                    readingSheet = readingSheet.Parent;
                                else
                                    readingSheet = null;
                            }
                            else
                                readMotionSheets = false;
                        }
                        else if (splits[0] == "sheet")
                        {
                            MotionSheet newSheet = new MotionSheet() { Name = splits[1].Replace("\"", "") };
                            if (readingSheet != null)
                            {
                                newSheet.Parent = readingSheet;
                                readingSheet.SubSheets.Add(newSheet);
                            }
                            else
                                MotionSheets.Add(newSheet);

                            readingSheet = newSheet;
                        }
                        else if (readingSheet != null)
                        {
                            bool stopreading = false;
                            if (readingMotion == null)
                            {
                                readingMotion = new MSMotion() { Name = splits[0] };
                                readingSheet.Motions.Add(readingMotion);
                                line = line.Substring(readingMotion.Name.Length).Trim();
                            }
                            if (line.Contains(';'))
                            {
                                stopreading = true;
                                line = line.Substring(0, line.IndexOf(';'));
                            }
                            if (line != "")
                            {
                                splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                int chance = -1;
                                if (splits.Length > 1 && int.TryParse(splits[splits.Length - 1], out chance))
                                    Array.Resize(ref splits, splits.Length - 1);

                                readingMotion.Animations.Add(new MSAnimation() { Files = splits.ToList(), Chance = chance });
                            }
                            if (stopreading)
                                readingMotion = null;
                        }
                    }
                    else if (readOutfitSlots)
                    {
                        if (splits[0] == ";")
                            readOutfitSlots = false;
                        else
                            OutfitSlots.Add(splits[0], int.Parse(splits[1]));
                    }
                    else if (readOutfitSets && readingSet != null)
                    {
                        if (splits[0] == ";")
                            readingSet = null;
                        else
                        {
                            bool ends = line.EndsWith(";");
                            if (ends)
                                line = line.Substring(0, line.IndexOf(';'));

                            string mesh = splits[0];
                            bool gotEscaped = mesh.StartsWith("\"");
                            if (gotEscaped)
                            {
                                string skey = line.Substring(1);
                                mesh = skey.Substring(0, skey.IndexOf('"'));
                            }
                            line = line.Substring(mesh.Length + (gotEscaped ? 2 : 0)).Trim();
                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            string texture = splits[0];
                            gotEscaped = texture.StartsWith("\"");
                            if (gotEscaped)
                            {
                                string skey = texture.Substring(1);
                                texture = skey.Substring(0, skey.IndexOf('"'));
                            }
                            line = line.Substring(texture.Length + (gotEscaped ? 2 : 0)).Trim();
                            string bump = null;
                            if (line.StartsWith(":"))
                            {
                                line = line.Substring(1).Trim();
                                splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                bump = splits[0];
                                gotEscaped = bump.StartsWith("\"");
                                if (gotEscaped)
                                {
                                    string skey = bump.Substring(1);
                                    bump = skey.Substring(0, skey.IndexOf('"'));
                                }
                            }

                            if (mesh == "NULL")
                                mesh = null;
                            if (texture == "NULL")
                                texture = null;
                            if (bump == "NULL")
                                bump = null;

                            readingSet.Items.Add(new OutfitSetItem() { Texture = texture, Mesh = mesh, BumpTexture = bump });

                            if (ends)
                                readingSet = null;
                        }
                    }
                    else if (readOutfitSets)
                    {
                        if (splits[0] == ";")
                            readOutfitSets = false;
                        else if (splits[0] == "set") // set "LEATHER_SHIRTS" link "COTTON_PANTS" LEG
                        {
                            readingSet = new OutfitSet();
                            OutfitSets.Add(readingSet);

                            bool escaped = splits[1].StartsWith("\"");
                            if (escaped)
                            {
                                string skey = line.Substring(splits[0].Length).Trim();
                                readingSet.Name = skey.Substring(1, skey.IndexOf('"', 1) - 1);
                            }
                            else
                                readingSet.Name = splits[1];

                            line = line.Substring("set ".Length + readingSet.Name.Length + (escaped ? 2 : 0)).Trim();
                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            if (splits.Length > 1 && splits[0] == "link")
                            {
                                escaped = splits[1].StartsWith("\"");
                                if (escaped)
                                {
                                    string skey = line.Substring(splits[0].Length).Trim();
                                    readingSet.LinkedSet = skey.Substring(1, skey.IndexOf('"', 1) - 1);
                                }
                                else
                                    readingSet.LinkedSet = splits[1];
                                line = line.Substring("link ".Length + readingSet.LinkedSet.Length + (escaped ? 2 : 0)).Trim();
                                splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                readingSet.LinkedSlot = splits[0];
                            }
                        }
                    }
                    else if (readEffects)
                    {
                        if (splits[0] == ";")
                            readEffects = false;
                        else
                            while (line != "")
                            {
                                bool escaped = line.StartsWith("\"");
                                string item = null;
                                if (escaped)
                                {
                                    line = line.Substring(1);
                                    item = line.Substring(0, line.IndexOf("\""));
                                    line = line.Substring(item.Length + 1).Trim();
                                }
                                else
                                {
                                    splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                                    item = splits[0];
                                    line = line.Substring(item.Length).Trim();
                                }
                                Effects.Add(item);
                            }
                    }
                    else if (readAttachmentSlots)
                    {
                        if (splits[0] == ";")
                            readAttachmentSlots = false;
                        else
                        {
                            bool ends = line.Contains(';');
                            if (ends)
                                line = line.Substring(0, line.IndexOf(';'));

                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string s in splits)
                                AttachmentSlots.Add(s);

                            if (ends)
                                readAttachmentSlots = false;
                        }
                    }
                    else if (readStaticSlot)
                    {
                        if (splits[0] == ";")
                            readStaticSlot = false;
                        else
                        {
                            bool ends = line.Contains(';');
                            if (ends)
                                line = line.Substring(0, line.IndexOf(';'));

                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string s in splits)
                                StaticSlots.Add(s);

                            if (ends)
                                readStaticSlot = false;
                        }
                    }
                    else if (readShaders)
                    {
                        if (splits[0] == ";")
                            readShaders = false;
                        else
                        {
                            bool ends = line.Contains(';');
                            if (ends)
                                line = line.Substring(0, line.IndexOf(';'));

                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            ShaderInfo info = new ShaderInfo();
                            Shaders.Add(info);
                            info.Shader = splits[0];
                            info.Type = int.Parse(splits[1]);

                            line = line.Substring(info.Shader.Length).Trim().Substring(info.Type.ToString().Length).Trim();
                            splits = line.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            bool escaped = splits[0].StartsWith("\"");
                            if (escaped)
                                info.Options = line.Substring(1, line.IndexOf('"', 1) - 1);
                            else
                                info.Options = splits[0];

                            if (ends)
                                readShaders = false;
                        }
                    }
                    else if (readBounding)
                    {
                        bool ending = line.Contains(";");
                        if (ending)
                            line = line.Substring(0, line.IndexOf(';')).Trim();
                        if (line != "")
                        {
                            splits = line.Split(new[] { " ", "\t", "," }, StringSplitOptions.RemoveEmptyEntries);

                            BoundingCylinder cyl = new BoundingCylinder();
                            BoundingCylinders.Add(cyl);

                            cyl.Bone_idx_bottom = int.Parse(splits[0]);
                            cyl.Bone_idx_top = int.Parse(splits[1]);
                            cyl.Radius_bottom = float.Parse(splits[2].Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            cyl.Radius_top = float.Parse(splits[3].Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        }
                        if (ending)
                            readBounding = false;
                    }

                    // base level
                    else if (splits[0] == "skey")
                    {
                        if (splits.Length > 1)
                        {
                            if (splits[1].StartsWith("\""))
                            {
                                string skey = line.Substring(splits[0].Length).Trim().Substring(1);
                                SkeletonKey = skey.Substring(0, skey.Length - 1);
                            }
                            else
                                SkeletonKey = splits[1];
                        }
                        else
                            SkeletonKey = null;
                    }
                    else if (splits[0] == "meshtype")
                        MeshType = int.Parse(splits[1]);
                    else if (splits[0] == "skelpart")
                        readSkelpart = true;
                    else if (splits[0] == "motionsheet")
                        readMotionSheets = true;
                    else if (splits[0] == "outfitslot")
                        readOutfitSlots = true;
                    else if (splits[0] == "staticslot")
                        readStaticSlot = true;
                    else if (splits[0] == "outfitset")
                        readOutfitSets = true;
                    else if (splits[0] == "default_outfit")
                    {
                        if (splits[1].StartsWith("\""))
                        {
                            string skey = line.Substring(splits[0].Length).Trim().Substring(1);
                            DefaultOutfit = skey.Substring(0, skey.Length - 1);
                        }
                        else
                            DefaultOutfit = splits[1];
                    }
                    else if (splits[0] == "default_motion")
                    {
                        MotionSheet selected = null;
                        for (int i = 1; i < splits.Length - 1; i++) // default_motion MS NO_WEAPON IDLE
                        {
                            string s = splits[i];

                            IEnumerable<MotionSheet> searchIn = selected == null ? MotionSheets : selected.SubSheets;
                            selected = searchIn.Single(x => x.Name == s);
                        }
                        DefaultMotionSheet = selected;
                        DefaultMotion = selected.Motions.Single(x => x.Name == splits[splits.Length - 1]);
                    }
                    else if (splits[0] == "alpha_used")
                        AlphaUsed = bool.Parse(splits[1]);
                    else if (splits[0] == "nametag_bias")
                        NameTagBias = int.Parse(splits[1]);
                    else if (splits[0] == "effectinfo")
                        readEffects = true;
                    else if (splits[0] == "default_scale")
                        DefaultScale = float.Parse(splits[1].Replace("f", ""), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    else if (splits[0] == "attachmentslot")
                        readAttachmentSlots = true;
                    else if (splits[0] == "attachmentset")
                        readAttachmentSets = true;
                    else if (splits[0] == "default_attachment")
                        readDefaultAttachmentSets = true;
                    else if (splits[0] == "shaderinfo")
                        readShaders = true;
                    else if (splits[0] == "boundingcylinder")
                        readBounding = true;
                    else
                        throw new Exception("Unknown tag in gcmds");
                }

            }
        }

        public void Save(byte[] crypto)
        {
            CryptoKey = crypto;
            Save();
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(FileName))
                throw new Exception("Filename empty");

            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);

                sw.WriteLine("skey \"{0}\"", SkeletonKey);
                sw.WriteLine();
                sw.WriteLine("meshtype {0}	// 0 = TEXPIECE, 1 = TEXTURE", MeshType);
                sw.WriteLine();
                if (SkeletonParts.Count > 0)
                {
                    sw.WriteLine("skelpart");

                    foreach (KeyValuePair<string, int> kv in SkeletonParts)
                        sw.WriteLine("\t{0} {1}", kv.Key, kv.Value);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (BoundingCylinders.Count > 0)
                {
                    sw.WriteLine("boundingcylinder");

                    foreach (BoundingCylinder cyl in BoundingCylinders)
                        sw.WriteLine("\t{0}\t{1}\t{2}f\t{3}f", cyl.Bone_idx_bottom, cyl.Bone_idx_top, Stringify(cyl.Radius_bottom), Stringify(cyl.Radius_top));

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (MotionSheets.Count > 0)
                {
                    sw.WriteLine("motionsheet");

                    foreach (MotionSheet sheet in MotionSheets)
                        WriteMotionSheet(sw, sheet, 1);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (StaticSlots.Count > 0)
                {
                    sw.WriteLine("staticslot");
                    sw.WriteLine("\t{0};", string.Join(" ", StaticSlots));
                    sw.WriteLine();
                }

                if (OutfitSlots.Count > 0)
                {
                    sw.WriteLine("outfitslot");

                    foreach (KeyValuePair<string, int> kv in OutfitSlots)
                        sw.WriteLine("\t{0} {1}", kv.Key, kv.Value);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (AttachmentSlots.Count > 0)
                {
                    sw.WriteLine("attachmentslot");
                    sw.WriteLine("\t{0};", string.Join(" ", AttachmentSlots));
                    sw.WriteLine();
                }

                if (AttachmentSets.Count > 0)
                {
                    sw.WriteLine("attachmentset");

                    int nSet = 0;
                    foreach (AttachmentSet set in AttachmentSets)
                    {
                        sw.WriteLine("\tset \"{0}\"", set.Name);

                        sw.WriteLine("\t\ttype\t{0}", set.Type);
                        sw.WriteLine("\t\tmesh\t{0}", string.IsNullOrEmpty(set.Mesh) ? "NULL" : "\"" + set.Mesh + "\"\t");
                        sw.WriteLine("\t\ttex\t{0}{1}", 
                            string.IsNullOrEmpty(set.Texture) ? "NULL" : "\"" + set.Texture + "\"\t",
                            string.IsNullOrEmpty(set.BumpTexture) ? "" : " : \"" + set.BumpTexture + "\""
                            );

                        foreach (AttachmentHolder holder in set.Holders)
                        {
                            sw.WriteLine("\t\tholder");
                            sw.WriteLine("\t\t\tskel\t{0}", holder.Skeleton);
                            sw.WriteLine("\t\t\tpos\t{0}f\t{1}f\t{2}f", Stringify(holder.Position.X), Stringify(holder.Position.Y), Stringify(holder.Position.Z));
                            sw.WriteLine("\t\t\trot\t{0}f\t{1}f\t{2}f\t{3}t", Stringify(holder.Rotation.X), Stringify(holder.Rotation.Y), Stringify(holder.Rotation.Z), Stringify(holder.Rotation.W));
                            sw.WriteLine("\t\t;");
                        }

                        sw.WriteLine("\t;");

                        if (nSet != AttachmentSets.Count - 1)
                            sw.WriteLine();
                    }

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (OutfitSets.Count > 0)
                {
                    sw.WriteLine("outfitset");

                    int nSet = 0;
                    foreach (OutfitSet set in OutfitSets)
                    {
                        string link = ""; // set "LEATHER_SHIRTS" link "COTTON_PANTS" LEG
                        if (!string.IsNullOrEmpty(set.LinkedSet))
                            link = " link \"" + set.LinkedSet + "\"";
                        if (!string.IsNullOrEmpty(set.LinkedSlot))
                            link += " " + set.LinkedSlot;

                        sw.WriteLine("\tset \"{0}\"{1}", set.Name, link);

                        foreach (OutfitSetItem look in set.Items)
                        {
                            string item1 = string.IsNullOrEmpty(look.Mesh) ? "NULL" : "\"" + look.Mesh + "\"";
                            string item2 = string.IsNullOrEmpty(look.Texture) ? "NULL" : "\"" + look.Texture + "\"";
                            string item3 = string.IsNullOrEmpty(look.BumpTexture) ? "" : " : \"" + look.BumpTexture + "\"";

                            sw.WriteLine("\t\t{0} {1}{2}", item1, item2, item3);
                        }

                        sw.WriteLine("\t;");

                        if (nSet != OutfitSets.Count - 1)
                            sw.WriteLine();
                    }

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (!string.IsNullOrEmpty(DefaultOutfit))
                {
                    sw.WriteLine("default_outfit \"{0}\"", DefaultOutfit);
                    sw.WriteLine();
                }

                if (DefaultMotion != null && DefaultMotionSheet != null)
                {
                    string msheet = string.Join(" ", DefaultMotionSheet.UnNest(x => x.Parent).Reverse());

                    sw.WriteLine("default_motion {0} {1}", msheet, DefaultMotion.Name);
                    sw.WriteLine();
                }

                if (NameTagBias != null)
                {
                    sw.WriteLine("nametag_bias {0}", NameTagBias.Value);
                    sw.WriteLine();
                }

                if (DefaultScale != null)
                {
                    sw.WriteLine("default_scale {0}", Stringify(DefaultScale.Value));
                    sw.WriteLine();
                }

                if (AlphaUsed != null)
                {
                    sw.WriteLine("alpha_used {0}", AlphaUsed.Value ? "true" : "false");
                    sw.WriteLine();
                }

                if (Effects.Count > 0)
                {
                    sw.WriteLine("effectinfo");

                    foreach (string s in Effects)
                        sw.WriteLine("\t\"{0}\"", s);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (DefaultAttachmentSets.Count > 0)
                {
                    sw.WriteLine("default_attachment");

                    foreach (KeyValuePair<string, AttachmentSet> s in DefaultAttachmentSets)
                        sw.WriteLine("\t{0}\t\"{1}\"", s.Key, s.Value.Name);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                if (Shaders.Count > 0)
                {
                    sw.WriteLine("shaderinfo");

                    foreach (ShaderInfo s in Shaders)
                        sw.WriteLine("\t{0} {1} \"{2}\"", s.Shader, s.Type, s.Options);

                    sw.WriteLine(";");
                    sw.WriteLine();
                }

                sw.Flush();
                byte[] data = ms.ToArray();
                if (CryptoKey != null && CryptoKey.Length != 0)
                    BinaryTools.Xor(data, CryptoKey);

                File.WriteAllBytes(FileName, data);
            }
        }

        private void WriteMotionSheet(StreamWriter sw, MotionSheet sheet, int depth)
        {
            string d = "\t".Repeat(depth);
            sw.WriteLine("{0}sheet {1}", d, sheet.Name);

            int nAction = 0;
            foreach (MSMotion motion in sheet.Motions)
            {
                if (motion.Animations.Count < 2)
                {
                    string files = motion.Animations.Count > 0 ? string.Join("\t", motion.Animations.Single().Files) : "";
                    sw.WriteLine("{0}\t{1}\t{2};", d, motion.Name, files);
                }
                else
                {
                    sw.WriteLine("{0}\t{1}", d, motion.Name);

                    foreach(MSAnimation anim in motion.Animations)
                        sw.WriteLine("{0}\t\t{1}\t{2}{3}", d, string.Join("\t", anim.Files), anim.Chance, anim == motion.Animations.Last() ? ";" : "");
                }

                if (nAction != sheet.Motions.Count - 1)
                    sw.WriteLine();

                nAction++;
            }

            if (sheet.SubSheets.Count > 0)
            {
                sw.WriteLine();
                foreach (MotionSheet child in sheet.SubSheets)
                    WriteMotionSheet(sw, child, depth + 1);
            }

            sw.WriteLine("{0};", d);
        }

        private static string Stringify(float obj)
        {
            return string.Format("{0:0.000000}", obj);
        }
    }

    class MotionSheet
    {
        public string Name;
        public List<MSMotion> Motions = new List<MSMotion>();
        public List<MotionSheet> SubSheets = new List<MotionSheet>();
        public MotionSheet Parent = null;

        public override string ToString()
        {
            return (Parent != null ? Parent.ToString() + "/" : "") + Name;
        }
    }

    class MSMotion
    {
        public string Name;
        public List<MSAnimation> Animations = new List<MSAnimation>();

        public override string ToString()
        {
            return Name;
        }
    }

    class MSAnimation
    {
        public int Chance;
        public List<string> Files = new List<string>();
    }

    class AttachmentSet
    {
        public string Name;
        public int Type;
        public string Mesh;
        public string Texture;
        public string BumpTexture;
        public List<AttachmentHolder> Holders = new List<AttachmentHolder>();
    }

    class AttachmentHolder
    {
        public string Skeleton;
        public Vector3 Position;
        public Quaternion Rotation;

        public override string ToString()
        {
            return Skeleton ?? "<none>";
        }
    }

    class OutfitSet : IComparable<OutfitSet>
    {
        public string Name;
        public List<OutfitSetItem> Items = new List<OutfitSetItem>();

        public string LinkedSet;
        public string LinkedSlot;

        public override string ToString()
        {
            return Name + (LinkedSet != null ? " link " + LinkedSet + (LinkedSlot != null ? " " + LinkedSlot : "") : "");
        }

        #region IComparable<OutfitSet> Members

        public int CompareTo(OutfitSet other)
        {
            if (Name == other.Name)
                return 0;
            else if (Name == "DEFAULT")
                return -1;
            else if (other.Name == "DEFAULT")
                return 1;


            else if (Name.StartsWith("FACE") && other.Name.StartsWith("FACE"))
                return 0;
            else if (Name.StartsWith("FACE"))
                return -1;
            else if (other.Name.StartsWith("FACE"))
                return 1;

            else if (Name.StartsWith("HAIR") && other.Name.StartsWith("HAIR"))
                return 0;
            else if (Name.StartsWith("HAIR"))
                return -1;
            else if (other.Name.StartsWith("HAIR"))
                return 1;
            else
                return 0;
        }

        #endregion
    }

    class OutfitSetItem
    {
        public string Mesh;
        public string Texture;
        public string BumpTexture;
    }

    class ShaderInfo
    {
        public string Shader;
        public int Type;
        public string Options;
    }

    class BoundingCylinder
    {
        public int Bone_idx_bottom;
        public int Bone_idx_top;
        public float Radius_bottom;
        public float Radius_top;
    }
}
