using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using rMap.Zalla;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace rMap
{
    public delegate void Show3D(string fullPath);
    public partial class rMapForm : Form
    {
        public Viewer.Window MeshWindow = new Viewer.Window();
        ObjectSelector objSelector = null;
        private Scene OpenScene;
        private string OpenFile;
        private const int RecentLimit = 5;
        public static rMapForm Instance;

        public rMapForm(string[] args)
        {
            Instance = this;

            MeshWindow.OnMouseClick += new Action<Microsoft.Xna.Framework.Ray>(MeshWindow_OnMouseClick);
            InitializeComponent();
            SetIcons();
            FileClosedMenus();
            NoRecentMenus();
            PopulateRecent();

            if (args.Length == 1 && !string.IsNullOrEmpty(args[0]))
            {
                string ext = System.IO.Path.GetExtension(args[0]);

                if (ext == ".z3s")
                    Import(args[0]);
            }

            UpdateLicenseStatus();
        }

        public void UpdateLicenseStatus()
        {
            if (Security.Checkin.TimeAccount)
                statusLicenseStatus.Text = "Remaining " + (SpanToString(TimeSpan.FromSeconds(Security.Checkin.RemainingTimeOnAcc)) ?? "Today (in GMT+2)");
            else if (Security.Checkin.ExpiresAt.HasValue)
                statusLicenseStatus.Text = "Expires in " + (SpanToString(Security.Checkin.ExpiresAt.Value - DateTime.Now) ?? "Expired");
            else
                statusLicenseStatus.Text = "Unlimited";
        }

        private string SpanToString(TimeSpan span)
        {
            int days = (int)Math.Floor(span.TotalDays);

            if (days > 0)
                return days.ToString() + (days == 1 ? " day" : " days");
            if (span.Hours > 0)
                return span.Hours.ToString() + (span.Hours == 1 ? " hour" : " hours");
            if (span.Minutes > 0)
                return span.Minutes.ToString() + (span.Minutes == 1 ? " minute" : " minutes");

            return null;
        }

        public void Reload()
        {
            treeObjects.Close();
            treeObjects.Load(OpenScene);
        }

        private void FileClosedMenus()
        {
            closeToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
        }

        private void FileOpenedMenus()
        {
            closeToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
        }

        private void NoRecentMenus()
        {
            recentClear.Visible = false;
            recentClearSplit1.Visible = false;
            recentClearSplit2.Visible = false;
        }

        private void GotRecentMenus()
        {
            recentClear.Visible = true;
            recentClearSplit1.Visible = true;
            recentClearSplit2.Visible = true;
        }

        private void OpenRecent(string file)
        {
            if (!CloseFile())
                return;

            if (File.Exists(file))
                Import(file);
            else
            {
                rMap.Properties.Settings.Default.Recent.Remove(file);

                ClearRecents(false);
                NoRecentMenus();
                PopulateRecent();

                MessageBox.Show("File doesn't exist. Removing from recent list", "File opening", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Export(string file)
        {
            if (OpenScene == null)
                MessageBox.Show("No map is open", "Error exporting", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (string.IsNullOrEmpty(file))
            {
                if (exportMapDiag.ShowDialog() == DialogResult.OK)
                    file = exportMapDiag.FileName;
                else
                    return;
            }

            OpenFile = file;

            AsyncLoader.Start(delegate
            {
                byte[] data = rMap.Properties.Settings.Default.UseCrypto ?
                    OpenScene.SaveWithCrypto(CryptoPanel.GetFromSerialized(rMap.Properties.Settings.Default.CryptoKey), Security.Checkin.GetLoader())
                    :
                    OpenScene.Save(Security.Checkin.GetLoader());

                Security.Checkin.DisposeVM();
                if (data == null)
                    return;

                File.WriteAllBytes(file, data);
            }, 
            null, 
            delegate(Exception ex)
            {
                LogDetailView.Instance.AddInfo(ex.ToString());
            }, true);

        }

        private void Import(string file)
        {
            if (!CloseFile())
                return;

            int zone = 0;

            try
            {
                if (Path.GetFileNameWithoutExtension(file).StartsWith("zone", StringComparison.CurrentCultureIgnoreCase))
                {
                    string f = Path.GetFileNameWithoutExtension(file);
                    string zz = f.Substring(4);

                    int.TryParse(zz, out zone);
                }
            }
            catch { }

            Scene scene = null;
            AsyncLoader.Start(delegate
            {
                byte[] fileCont = File.ReadAllBytes(file);

                if (rMap.Properties.Settings.Default.UseCrypto)
                    scene = Scene.LoadWithCrypto(fileCont, zone, CryptoPanel.GetFromSerialized(rMap.Properties.Settings.Default.CryptoKey), Security.Checkin.GetLoader());
                else
                    scene = Scene.Load(fileCont, zone, Security.Checkin.GetLoader());

                Security.Checkin.DisposeVM();
                if (scene == null)
                    return;

                scene.SetThumbnail(rMap.Properties.Settings.Default.RYLFolder);
            },
            delegate
            {
                AddNewRecent(file);
                Open(scene);
                OpenFile = file;

                if (zone < 1)
                {
                    MessageBox.Show("The zone number couldn't be parsed from the file name.\r\nPlease provide it in the scene options.");
                }
            },
            delegate(Exception ex)
            {
                LogDetailView.Instance.AddInfo(ex.ToString());
            }, true);
        }

        private void Open(Scene scene)
        {
            OpenScene = scene;
            treeObjects.Load(OpenScene);
            FileOpenedMenus();

            if (objSelector != null)
                objSelector.LoadScene(OpenScene, rMap.Properties.Settings.Default.RYLFolder);
        }

        private bool CloseFile()
        {
            if (OpenScene == null)
                return true;

            OpenScene = null;
            OpenFile = null;

            FileClosedMenus();
            treeObjects.Close();
            editor.Close();
            MeshWindow.Clear();
            if (objSelector != null)
                objSelector.Unload();

            return true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void SetIcons()
        {
            openToolStripMenuItem.Image = Properties.Resources.FILE.ToBitmap();
            closeToolStripMenuItem.Image = Properties.Resources.Eject.ToBitmap();
            exitToolStripMenuItem.Image = Properties.Resources.CD.ToBitmap();
            saveToolStripMenuItem.Image = Properties.Resources.Library.ToBitmap();

            fileToolStripMenuItem1.Image = Properties.Resources.Generic.ToBitmap();
            statusLicenseStatus.Image = Properties.Resources.Documents.ToBitmap();
        }

        private void PopulateRecent()
        {
            if (rMap.Properties.Settings.Default.Recent == null || rMap.Properties.Settings.Default.Recent.Count < 1)
                return;

            foreach (string file in rMap.Properties.Settings.Default.Recent)
                AddRecent(file);
        }

        private void AddNewRecent(string file)
        {
            if (rMap.Properties.Settings.Default.Recent == null)
                rMap.Properties.Settings.Default.Recent = new System.Collections.Specialized.StringCollection();

            if (rMap.Properties.Settings.Default.Recent.Contains(file))
                return;

            rMap.Properties.Settings.Default.Recent.Add(file);
            rMap.Properties.Settings.Default.Save();

            AddRecent(file);
        }

        private void ClearRecents(bool deleteAswell = true)
        {
            if (rMap.Properties.Settings.Default.Recent == null || rMap.Properties.Settings.Default.Recent.Count < 1)
                return;

            if (deleteAswell)
            {
                rMap.Properties.Settings.Default.Recent.Clear();
                rMap.Properties.Settings.Default.Save();
            }

            for (int i = openToolStripMenuItem.DropDownItems.Count - 3; i > 1; i-- )
                openToolStripMenuItem.DropDownItems.RemoveAt(i);

            NoRecentMenus();
        }

        private void AddRecent(string file)
        {
            string name = file;

            if (name.Length > 20)
                name = "..." + name.Substring(name.Length - 17);

            ToolStripMenuItem mi = new ToolStripMenuItem(
                name,
                null, // image
                new EventHandler(RecentClicked));

            mi.Tag = file;

            openToolStripMenuItem.DropDownItems.Insert(2, mi);
            GotRecentMenus();

            if (openToolStripMenuItem.DropDownItems.Count > RecentLimit + 4)
            {
                openToolStripMenuItem.DropDownItems.RemoveAt(openToolStripMenuItem.DropDownItems.Count - 3);
            }
        }

        private void RecentClicked(object sender, EventArgs args)
        {
            ToolStripMenuItem ag = sender as ToolStripMenuItem;

            OpenRecent((string)ag.Tag);
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            importMapDiag.InitialDirectory = rMap.Properties.Settings.Default.RYLFolder;
            if (importMapDiag.ShowDialog() == DialogResult.OK)
            {
                Import(importMapDiag.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export(OpenFile);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearRecents();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export(null);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void rMapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!CloseFile())
                    return;
            }

            rMap.Properties.Settings.Default.Location = Location;
            rMap.Properties.Settings.Default.Size = Size;
            rMap.Properties.Settings.Default.Save();

            if (MeshWindow != null && MeshWindow.IsActive)
                MeshWindow.Dispose();

            AsyncLoader.Shutdown();
        }

        private void rMapForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if ((e.KeyData & Keys.S) == Keys.S)
                {
                    saveToolStripMenuItem_Click(null, null);
                }
                else if ((e.KeyData & Keys.O) == Keys.O)
                {
                    fileToolStripMenuItem1_Click(null, null);
                }
                else if ((e.KeyData & Keys.Q) == Keys.Q)
                {
                    exitToolStripMenuItem_Click(null, null);
                }
            }
        }

        private void rMapForm_Load(object sender, EventArgs e)
        {
            if (!rMap.Properties.Settings.Default.Size.IsEmpty && !rMap.Properties.Settings.Default.Location.IsEmpty)
            {
                Size = rMap.Properties.Settings.Default.Size;
                Location = rMap.Properties.Settings.Default.Location;
            }

            if (string.IsNullOrEmpty(rMap.Properties.Settings.Default.RYLFolder))
            {
                MessageBox.Show("RYL folder not set. Please specify it in the options.");
                optionsToolStripMenuItem_Click(this, null);
            }
        }

        private void treeObjects_OnSelect(object obj)
        {
            if (obj != null)
            {
                editor.Load(obj);
                Show3D(obj);
            }
            else
                editor.Close();
        }

        private void editor_ObjectChanged(object sender, EventArgs e)
        {
            try
            {
                treeObjects.ObjectChanged(sender);
                RefreshModelViewerPositions();

                if (objSelector != null)
                {
                    if (sender != null && sender == OpenScene && OpenScene.TextureZone != objSelector.OpenZone)
                        objSelector.LoadScene(OpenScene, rMap.Properties.Settings.Default.RYLFolder);

                    if (sender != null)
                        objSelector.ObjectChanged(sender);
                }
            }
            catch (Exception ex)
            {
                LogDetailView.Instance.AddInfo(ex.ToString());
            }
        }

        private void windowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MeshWindow.IsActive)
                {
                    MeshWindow.Dispose();
                }
                else
                {
                    MeshWindow.Run(rMap.Properties.Settings.Default.DeviceProfileReach);
                }
            }
            catch (Exception ex)
            {
                LogDetailView.Instance.AddInfo(ex.ToString());
            }
        }

        private Dictionary<Asset.DrawableModel, SceneObject> _shownModels = new Dictionary<Asset.DrawableModel, SceneObject>();
        private SceneObject _focusModel = null;
        private void Show3D(object obj)
        {
            if (MeshWindow.IsActive)
            {
                _focusModel = null;
                _shownModels.Clear();
                bool dontMoveCamera = _dontMoveCamera;

                AsyncLoader.Start(delegate
                {
                    MeshWindow.Clear();

                    if (obj is SceneObject)
                    {
                        if (obj is HeightTable || obj is NatureObject || obj is FieldObject || obj is House || obj is WaterData)
                        {
                            _focusModel = obj as SceneObject;

                            RefreshModelViewerPositions();

                            Asset.DrawableModel asset = _shownModels.SingleOrDefault(x => x.Value == _focusModel).Key;

                            if (asset != null)
                            {
                                asset.CalcEdges();

                                if (!dontMoveCamera)
                                    MeshWindow.SetCamera(asset.Edges);
                            }
                        }
                    }
                    else if (obj is string)
                    {
                        Asset.DrawableModel model = new Asset.DrawableModel(MeshWindow.Game) { TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"texture\object") };

                        Asset.FileTypes.NMesh mesh = new Asset.FileTypes.NMesh();
                        mesh.Load(obj as string);
                        model.Parts.AddRange(mesh.Model.Parts);

                        model.CalcEdges();

                        MeshWindow.Show(new[] { model });
                        MeshWindow.SetCamera(model.Edges);
                    }
                }, null, null, false);
            }
        }

        private Matrix GetWorldForObject(SceneObject obj)
        {
            if (obj is WorldMatrixObject)
                return (obj as WorldMatrixObject).WorldM;
            else if (obj is NatureObject)
                return Matrix.CreateTranslation((obj as NatureObject).GetPosition());
            else if (obj is HeightTable)
                return Matrix.CreateTranslation((obj as HeightTable).GetPosition());
            else if (obj is WaterData)
                return Matrix.CreateTranslation((obj as WaterData).GetPosition());
            else
                return default(Matrix);
        }

        private Asset.DrawableModel GetModelForObject(object obj)
        {
            if (!(obj is SceneObject))
                return null;

            Asset.DrawableModel asset = null;

            try
            {
                asset = (obj as SceneObject).GetModel(MeshWindow.Game);
            }
            catch { } // errors happen

            if (asset == null)
                return null;

            SceneObject o = obj as SceneObject;
            bool focused = o == _focusModel || o is HeightTable || o is WaterData;

            if ((_focusModel is HeightTable || _focusModel is WaterData) && _focusModel != o)
                focused = false;

            asset.Parts.ForEach(p => p.WireMode = !focused);
            return asset;
        }

        private void ShowNearbyModels()
        {
            List<SceneObject> foundObjects = new List<SceneObject>();

            float range = rMap.Properties.Settings.Default.ObjectsViewerLoadingRange;
            Vector3 pos = GetWorldForObject(_focusModel).Translation;

            foreach (House h in OpenScene.Houses.Where(a => a.WorldM.Translation.DistanceIgnoreY(pos) <= range))
                foundObjects.Add(h);
            foreach (FieldObject h in OpenScene.FieldObjects.Where(a => a.WorldM.Translation.DistanceIgnoreY(pos) <= range))
                foundObjects.Add(h);
            foreach (NatureObject h in OpenScene.NatureObjects.Where(a => a.GetPosition().DistanceIgnoreY(pos) <= range))
                foundObjects.Add(h);

            HeightTable ground = _focusModel is HeightTable ? _focusModel as HeightTable : OpenScene.HeightTables.FirstOrDefault(a => a.GetBox().Contains(pos) != Microsoft.Xna.Framework.ContainmentType.Disjoint);

            if (ground != null)
            {
                foreach (Vector3 corner in ground.GetBox().GetCorners())
                {
                    foreach (HeightTable h in OpenScene.HeightTables.Where(a => a.GetBox().Contains(corner) != Microsoft.Xna.Framework.ContainmentType.Disjoint))
                        if (!foundObjects.Contains(h))
                            foundObjects.Add(h);

                    foreach (WaterData h in OpenScene.WaterDatas.Where(a => a.GetBox().Contains(corner) != Microsoft.Xna.Framework.ContainmentType.Disjoint))
                        if (!foundObjects.Contains(h))
                            foundObjects.Add(h);
                }
            }

            foreach (SceneObject obj in foundObjects)
            {
                if (!_shownModels.ContainsValue(obj))
                {
                    var asset = GetModelForObject(obj);

                    if (asset != null)
                    {
                        asset.World = GetWorldForObject(obj);
                        _shownModels.Add(asset, obj);
                    }
                }
            }

            foreach (KeyValuePair<Asset.DrawableModel, SceneObject> kv in _shownModels.ToArray())
            {
                if (!foundObjects.Contains(kv.Value))
                    _shownModels.Remove(kv.Key);
            }

            MeshWindow.SyncObjects(_shownModels.Keys);
        }

        private void RefreshModelViewerPositions()
        {
            if (MeshWindow.IsActive && _focusModel != null)
            {
                ShowNearbyModels();

                Matrix primary = Matrix.CreateTranslation(-GetWorldForObject(_focusModel).Translation);

                foreach (var asset in _shownModels)
                {
                    if (asset.Value != null)
                    {
                        asset.Key.World = GetWorldForObject(asset.Value) * primary;
                        asset.Key.CalcEdges();
                    }
                }
            }
        }

        private bool _dontMoveCamera = false;
        private void MeshWindow_OnMouseClick(Microsoft.Xna.Framework.Ray obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Microsoft.Xna.Framework.Ray>(MeshWindow_OnMouseClick), obj);
                return;
            }

            float? best = null;
            SceneObject bestObj = null;

            foreach (var kv in _shownModels)
            {
                if (kv.Value is HeightTable || kv.Value is WaterData) // ignore height tables
                    continue;

                float? intersects = kv.Key.Intersects(obj);

                if (intersects.HasValue)
                {
                    if (!best.HasValue || intersects.Value < best.Value)
                    {
                        best = intersects.Value;
                        bestObj = kv.Value;
                    }
                }
            }

            if (bestObj != null)
            {
                _dontMoveCamera = true;
                treeObjects.Select(bestObj);
                _dontMoveCamera = false;
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OptionsForm().ShowDialog();

            if (string.IsNullOrEmpty(rMap.Properties.Settings.Default.RYLFolder))
                Close();

            if (objSelector != null && rMap.Properties.Settings.Default.RYLFolder != objSelector.RylDir && objSelector.OpenZone > 0)
            {
                objSelector.LoadScene(OpenScene, rMap.Properties.Settings.Default.RYLFolder);
            }
        }

        private void cryptoToolstrip_CheckedChanged(object sender, EventArgs e)
        {
            rMap.Properties.Settings.Default.UseCrypto = cryptoToolstrip.Checked;
        }

        private void modelBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModelBrowser b = new ModelBrowser();
            b.View += new Show3D(b_View);
            b.Show();
        }

        private void b_View(string fullPath)
        {
            Show3D(fullPath);
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            objSelector = new ObjectSelector();
            objSelector.FormClosed += new FormClosedEventHandler(objSelector_FormClosed);
            objSelector.Show();
            objSelector.ShowObject += new MapObjEvent(objSelector_ShowObject);

            if (OpenScene != null)
                objSelector.LoadScene(OpenScene, rMap.Properties.Settings.Default.RYLFolder);
        }

        private void objSelector_ShowObject(object obj)
        {
            treeObjects.Select(obj);
        }

        private void objSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            objSelector = null;
        }

        private void statusLicenseStatus_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://alpha.lutsu.ee/payment/products.php");
        }

        private void timerLicenseStatusUpdate_Tick(object sender, EventArgs e)
        {
            UpdateLicenseStatus();
        }
    }
}
