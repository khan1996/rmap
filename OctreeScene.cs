using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rMap.Asset.FileTypes
{
    public class OctreeScene
    {
        public DrawableModel Model { get; set; }
        private Octree Octree;
        private List<Vector3[]> Polygons = new List<Vector3[]>();

        public void CreateModel()
        {
            Model = new DrawableModel();
            ModelPart part = new ModelPart() { LineList = true };
            Model.Parts.Add(part);

            AddOctreeAsModel(part, Octree);
        }

        public float? Intersects(Ray ray)
        {
            return Octree.Intersects(ray);
        }

        private void AddOctreeAsModel(ModelPart part, Octree tree)
        {
            int v = part.Vertexes.Count;

            if (tree.PolyIndicies.Count > 0)
            {
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MaxBox.X, tree.MaxBox.Y, tree.MaxBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MinBox.X, tree.MaxBox.Y, tree.MaxBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MaxBox.X, tree.MaxBox.Y, tree.MinBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MinBox.X, tree.MaxBox.Y, tree.MinBox.Z), Vector3.Zero, Vector2.Zero));

                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MaxBox.X, tree.MinBox.Y, tree.MaxBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MinBox.X, tree.MinBox.Y, tree.MaxBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MaxBox.X, tree.MinBox.Y, tree.MinBox.Z), Vector3.Zero, Vector2.Zero));
                part.Vertexes.Add(new VertexPositionNormalTexture(new Vector3(tree.MinBox.X, tree.MinBox.Y, tree.MinBox.Z), Vector3.Zero, Vector2.Zero));

                part.Indices.Add(v + 0);
                part.Indices.Add(v + 1);
                part.Indices.Add(v + 1);
                part.Indices.Add(v + 3);
                part.Indices.Add(v + 3);
                part.Indices.Add(v + 2);
                part.Indices.Add(v + 2);
                part.Indices.Add(v + 0);

                part.Indices.Add(v + 4);
                part.Indices.Add(v + 5);
                part.Indices.Add(v + 5);
                part.Indices.Add(v + 7);
                part.Indices.Add(v + 7);
                part.Indices.Add(v + 6);
                part.Indices.Add(v + 6);
                part.Indices.Add(v + 4);

                part.Indices.Add(v + 0);
                part.Indices.Add(v + 4);
                part.Indices.Add(v + 1);
                part.Indices.Add(v + 5);
                part.Indices.Add(v + 3);
                part.Indices.Add(v + 7);
                part.Indices.Add(v + 2);
                part.Indices.Add(v + 6);
            }

            foreach (Octree a in tree.Childs)
            {
                if (a != null)
                    AddOctreeAsModel(part, a);
            }
        }


        // Layer 2

        public void Save(string filename)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);

                bw.Write((int)Polygons.Count);
                foreach (Vector3[] poly in Polygons)
                {
                    bw.Write(poly[0]);
                    bw.Write(poly[1]);
                    bw.Write(poly[2]);
                }

                WriteOctree(Octree, bw);

                File.WriteAllBytes(filename, ms.ToArray());
            }
        }

        private void WriteOctree(Octree tree, BinaryWriter bw)
        {
            int hasChilds = 0;
            for (int i = 0; i < 8; i++)
                if (tree.Childs[i] != null)
                    hasChilds |= 1 << i;

            bw.Write((byte)hasChilds);
            bw.Write(tree.MaxBox);
            bw.Write(tree.MinBox);

            bw.Write((int)tree.PolyIndicies.Count);
            foreach (int indi in tree.PolyIndicies)
                bw.Write((int)indi);

            for (int i = 0; i < 8; i++)
                if (tree.Childs[i] != null)
                    WriteOctree(tree.Childs[i], bw);
        }

        public void ApplyFromModel(DrawableModel model, int depth, float extraRoom)
        {
            Polygons.Clear();
            foreach (ModelPart part in model.Parts)
            {
                for (int i = 0; i < part.Indices.Count / 3; i++)
                {
                    AddPoly(new[] 
                    { 
                        Vector3.Transform(part.Vertexes[(int)part.Indices[i * 3 + 0]].Position, part.World * model.World), 
                        Vector3.Transform(part.Vertexes[(int)part.Indices[i * 3 + 1]].Position, part.World * model.World), 
                        Vector3.Transform(part.Vertexes[(int)part.Indices[i * 3 + 2]].Position, part.World * model.World) 
                    });
                }
            }
            GenerateOctree(depth, extraRoom);
        }

        // Layer 1
        public void AddPoly(Vector3[] verts)
        {
            if (verts == null)
                throw new ArgumentNullException();
            if (verts.Length != 3)
                throw new ArgumentOutOfRangeException("verts", "A poly consist of 3 vertex positions");

            Polygons.Add(verts.ToArray());
        }

        public void GenerateOctree(int depth, float extraRoom) // 4, 100 default
        {
            Octree = new Octree();

            for (int i = 0; i < Polygons.Count; i++)
                Octree.PolyIndicies.Add(i);

            Octree.CalcBox(Polygons, extraRoom);
            Octree.SplitNode(Polygons, depth - 1);
        }
    }

    class Octree
    {
        public Octree[] Childs = new Octree[8];
        public Vector3 MaxBox;
        public Vector3 MinBox;
        public List<int> PolyIndicies = new List<int>();

        private static int[] BoxMoveX = new[] { 1, 0, 1, 0, 1, 0, 1, 0 };
        private static int[] BoxMoveY = new[] { 1, 1, 1, 1, 0, 0, 0, 0 };
        private static int[] BoxMoveZ = new[] { 1, 1, 0, 0, 1, 1, 0, 0 };
        private static int[] BoxNormalEdges = new[] { 0, 2, 0, 3, 0, 6 };
        private static int[][] BoxPolys = new[] { 
            new[] { 0, 4, 5 }, new[] { 0, 5, 1 }, new[] { 2, 3, 7 }, new[] { 2, 7, 6 }, 
            new[] { 0, 2, 6 }, new[] { 3, 1, 5 }, new[] { 3, 5, 7 }, new[] { 0, 1, 3 }, 
            new[] { 0, 3, 2 }, new[] { 6, 7, 5 }, new[] { 6, 5, 4 }
        };

        public void CalcBox(List<Vector3[]> polys, float extraRoom)
        {
            float minx, miny, minz, maxx, maxy, maxz;
            if (polys.Count == 0)
                return;

            maxx = minx = polys[0][0].X;
            maxy = miny = polys[0][0].Y;
            maxz = minz = polys[0][0].Z;

            for (int i = 0; i < polys.Count; i++)
            {
                for (int cv = 0; cv < 3; cv++)
                {
                    if (maxx < polys[i][cv].X)
                        maxx = polys[i][cv].X;

                    if (maxy < polys[i][cv].Y)
                        maxy = polys[i][cv].Y;

                    if (maxz < polys[i][cv].Z)
                        maxz = polys[i][cv].Z;

                    if (minx > polys[i][cv].X)
                        minx = polys[i][cv].X;

                    if (miny > polys[i][cv].Y)
                        miny = polys[i][cv].Y;

                    if (minz > polys[i][cv].Z)
                        minz = polys[i][cv].Z;
                }
            }

            MaxBox.X = maxx + extraRoom;
            MaxBox.Y = maxy + extraRoom;
            MaxBox.Z = maxz + extraRoom;
            MinBox.X = minx - extraRoom;
            MinBox.Y = miny - extraRoom;
            MinBox.Z = minz - extraRoom;
        }

        public void SplitNode(List<Vector3[]> polys, int depth)
        {
            if (depth == 0)
                return;

            float fBoxSizeX = MaxBox.X - MinBox.X;
            float fBoxSizeY = MaxBox.Y - MinBox.Y;
            float fBoxSizeZ = MaxBox.Z - MinBox.Z;

            for (int node = 0; node < 8; node++)
            {
                Octree child = new Octree();

                Vector3[] bounds = new Vector3[8];
                for (int i = 0; i < 8; i++)
                    bounds[i] = new Vector3()
                    {
                        X = MinBox.X + BoxMoveX[node] * fBoxSizeX * 0.5f + BoxMoveX[i] * fBoxSizeX * 0.5f,
                        Y = MinBox.Y + BoxMoveY[node] * fBoxSizeY * 0.5f + BoxMoveY[i] * fBoxSizeY * 0.5f,
                        Z = MinBox.Z + BoxMoveZ[node] * fBoxSizeZ * 0.5f + BoxMoveZ[i] * fBoxSizeZ * 0.5f
                    };

                child.MinBox = bounds[7];
                child.MaxBox = bounds[0];

                Vector3[] normals = new Vector3[]
                {
                    (bounds[4]-bounds[0]).Square(bounds[5]-bounds[4]),
                    (bounds[3]-bounds[2]).Square(bounds[7]-bounds[3]),
                    (bounds[2]-bounds[0]).Square(bounds[6]-bounds[2]),
                    (bounds[1]-bounds[3]).Square(bounds[5]-bounds[1]),
                    (bounds[1]-bounds[0]).Square(bounds[3]-bounds[1]),
                    (bounds[7]-bounds[6]).Square(bounds[5]-bounds[7])
                };

                for (int i = 0; i < normals.Length; i++)
                    normals[i].Normalize();

                for (int poly = 0; poly < polys.Count; poly++)
                {
                    bool ok = false;

                    for (int i = 0; i < 3 && !ok; i++)
                    {
                        ok = true;
                        for (int j = 0; j < 6 && ok; j++)
                            ok = normals[j].Distance(polys[poly][i] - bounds[BoxNormalEdges[j]]) > 0f;
                    }


                    for (int i = 0; i < BoxPolys.Length && !ok; i++)
                        ok = Intersection.PolygonToPolygon(polys[poly], new[] { bounds[BoxPolys[i][0]], bounds[BoxPolys[i][1]], bounds[BoxPolys[i][2]] }) == 1;

                    if (ok)
                        child.PolyIndicies.Add(poly);
                }

                if (child.PolyIndicies.Count > 0)
                {
                    child.SplitNode(polys, depth - 1);
                    Childs[node] = child;
                }
            }
            PolyIndicies.Clear(); // only level 0 will have it defined
        }

        public float? Intersects(Ray ray)
        {
            if (PolyIndicies.Count > 0)
                return ray.Intersects(new BoundingBox(MinBox, MaxBox));
            else
            {
                float? smallest = null;
                foreach (Octree child in Childs)
                {
                    if (child == null)
                        continue;

                    float? i = child.Intersects(ray);

                    if (i.HasValue)
                    {
                        if (!smallest.HasValue || smallest.Value > i.Value)
                            smallest = i;
                    }
                }
                return smallest;
            }
        }
    }

    static class Intersection
    {
        public static int PolygonRay(Vector3 vecStart, Vector3 vecEnd, Vector3[] poly, out float interLens)
        {
            Vector3 n = (poly[1] - poly[0]).Square(poly[2] - poly[0]);
            n.Normalize();
            if ((Math.Abs(n.X) <= 0.00000001f) &&
                (Math.Abs(n.Y) <= 0.00000001f) &&
                (Math.Abs(n.Z) <= 0.00000001f))
            {
                interLens = 1000000.0f;
                return 0;
            }
            Vector3 vecDir = (vecEnd - vecStart);
            vecDir.Normalize();
            float h;
            if (Math.Abs((n.X * vecDir.X + n.Y * vecDir.Y + n.Z * vecDir.Z)) < 0.00000001f)
            {
                interLens = 1000000.0f;
                return 0;
            }
            else
            {
                h = ((n.X * poly[0].X + n.Y * poly[0].Y + n.Z * poly[0].Z) - (n.X * vecStart.X + n.Y * vecStart.Y + n.Z * vecStart.Z))
                        / (n.X * vecDir.X + n.Y * vecDir.Y + n.Z * vecDir.Z);
                interLens = h;
            }
            if (h < 0.0f) return 0;
            Vector3 vecInter = vecEnd - vecStart;
            if (vecInter.GetLens() < h)
                return 0;

            Vector3 vecInterpos = vecStart + vecDir * h;


            Vector3 vecEgde, vecEdgeNormal;
            float fHalfPlane;
            long pos = 0, neg = 0;
            vecEgde = poly[0] - poly[1];
            vecEdgeNormal = vecEgde.Square(n);
            vecEdgeNormal.Normalize();
            fHalfPlane = vecInterpos.Distance(vecEdgeNormal) - poly[1].Distance(vecEdgeNormal);
            if (fHalfPlane >= 0.001f)
                pos++;
            if (fHalfPlane <= -0.001f)
                neg++;
            vecEgde = poly[1] - poly[2];
            vecEdgeNormal = vecEgde.Square(n);
            vecEdgeNormal.Normalize();
            fHalfPlane = vecInterpos.Distance(vecEdgeNormal) - poly[2].Distance(vecEdgeNormal);

            if (fHalfPlane >= 0.001f)
                pos++;
            if (fHalfPlane <= -0.001f)
                neg++;

            vecEgde = poly[2] - poly[0];
            vecEdgeNormal = vecEgde.Square(n);
            vecEdgeNormal.Normalize();
            fHalfPlane = vecInterpos.Distance(vecEdgeNormal) - poly[0].Distance(vecEdgeNormal);

            if (fHalfPlane >= 0.001f)
                pos++;
            if (fHalfPlane <= -0.001f)
                neg++;

            if (pos == 0 || neg == 0)
                return 1;
            return 0;
        }

        public static int PolygonToPolygon(Vector3[] vecPoly1, Vector3[] vecPoly2)
        {
            Vector3 vecPolyNormal1 = (vecPoly1[1] - vecPoly1[0]).Square(vecPoly1[2] - vecPoly1[1]);
            Vector3 vecPolyNormal2 = (vecPoly2[1] - vecPoly2[0]).Square(vecPoly2[2] - vecPoly2[1]);
            vecPolyNormal1.Normalize();
            vecPolyNormal2.Normalize();

            float[] fDistance = new[]
            {
	            vecPolyNormal1.Distance(vecPoly2[0]-vecPoly1[0]),
	            vecPolyNormal1.Distance(vecPoly2[1]-vecPoly1[0]),
	            vecPolyNormal1.Distance(vecPoly2[2]-vecPoly1[0])
            };
            if ((fDistance[0] > 0.0f && fDistance[1] > 0.0f && fDistance[2] > 0.0f))
                return -1;

            if ((fDistance[0] < 0.0f && fDistance[1] < 0.0f && fDistance[2] < 0.0f))
                return -2;

            fDistance[0] = vecPolyNormal2.Distance(vecPoly1[0] - vecPoly2[0]);
            fDistance[1] = vecPolyNormal2.Distance(vecPoly1[1] - vecPoly2[0]);
            fDistance[2] = vecPolyNormal2.Distance(vecPoly1[2] - vecPoly2[0]);

            if ((fDistance[0] > 0.0f && fDistance[1] > 0.0f && fDistance[2] > 0.0f) ||
                (fDistance[0] < 0.0f && fDistance[1] < 0.0f && fDistance[2] < 0.0f))
                return 0;

            float fIntersection = 0;
            Vector3 vecLens;
            if (PolygonRay(vecPoly2[0], vecPoly2[1], vecPoly1, out fIntersection) == 1)
            {
                vecLens = vecPoly2[0] - vecPoly2[1];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }
            if (PolygonRay(vecPoly2[1], vecPoly2[2], vecPoly1, out fIntersection) == 1)
            {
                vecLens = vecPoly2[1] - vecPoly2[2];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }
            if (PolygonRay(vecPoly2[2], vecPoly2[0], vecPoly1, out fIntersection) == 1)
            {
                vecLens = vecPoly2[2] - vecPoly2[0];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }

            if (PolygonRay(vecPoly1[0], vecPoly1[1], vecPoly2, out fIntersection) == 1)
            {
                vecLens = vecPoly1[0] - vecPoly1[1];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }

            if (PolygonRay(vecPoly1[1], vecPoly1[2], vecPoly2, out fIntersection) == 1)
            {
                vecLens = vecPoly1[1] - vecPoly1[2];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }

            if (PolygonRay(vecPoly1[2], vecPoly1[0], vecPoly2, out fIntersection) == 1)
            {
                vecLens = vecPoly1[2] - vecPoly1[0];
                if (vecLens.GetLens() >= fIntersection)
                    return 1;
            }
	        return 0;
        }
    }
}
