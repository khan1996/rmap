using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace rMap.Zalla
{
    [Serializable]
    public class NatureObject : TileObject
    {
        public const int TileSizeX = 31507;
        public const int TileSizeY = 31507;

        public new int TileX;
        public new int TileY;

        public byte Type;
        public byte PosX;
        public byte PosY;

        public override string ToString()
        {
            return TileX + ":" + TileY + " at " + PosX + ":" + PosY + " - " + Type;
        }

        [System.Xml.Serialization.XmlIgnore]
        public int MapPosX
        {
            get
            {
                return TileX * TileSizeX + PosX * TileSizeX / 65;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public int MapPosY
        {
            get
            {
                return TileY * TileSizeY + PosY * TileSizeY / 65;
            }
        }

        public static NatureObject LoadFrom(BinaryReader br, int tilex, int tiley)
        {
            NatureObject obj = new NatureObject();

            obj.TileX = tilex;
            obj.TileY = tiley;

            obj.Type = br.ReadByte();
            obj.PosX = br.ReadByte();
            obj.PosY = br.ReadByte();

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((byte)Type);
            bw.Write((byte)PosX);
            bw.Write((byte)PosY);
        }

        public override Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            Asset.DrawableModel model = new Asset.DrawableModel(game) { TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"texture\object") };

            Asset.FileTypes.NMesh mesh = new Asset.FileTypes.NMesh();
            mesh.Load(Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"objects\natureobject\zone" + Scene.TextureZone, "normaltree" + (Type + 1) + ".r3s"));

            model.Parts.AddRange(mesh.Model.Parts);
            return model;
        }

        public override Vector3 GetPosition()
        {
            return new Vector3(MapPosX, Scene.GetNatureHeight(this), MapPosY);
        }
    }
}
