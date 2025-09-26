using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rMap.Zalla;
using System.Reflection;

namespace rMap
{
    public delegate void OnSelectDelegate(object obj);

    public class ObjectTree : TreeView
    {
        public event OnSelectDelegate OnSelect;

        private ContextMenuStrip menu;

        private Scene scene;
        private IEnumerable<Type> editorSupportedTypes = ObjectEditor.GetSupportedTypes();

        public ObjectTree()
        {
            menu = CreateMenu();
            TreeViewNodeSorter = new TreeNodeComparer();
        }

        public void Load(Scene pScene)
        {
            scene = pScene;

            SuspendLayout();

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + " a");
            Populate(scene, null); // 16
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + " b");
            Sort(); // 5
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + " c");
            Nodes[0].Expand();
            ResumeLayout();
        }

        private void Populate(object obj, SceneNode parent)
        {
            if (!editorSupportedTypes.Contains(obj.GetType()))
                return;

            SceneNode n = new SceneNode(obj);
            if (parent == null)
                Nodes.Add(n);
            else
                parent.Nodes.Add(n);

            if (obj is SceneObject && (obj as SceneObject).Scene == null)
                (obj as SceneObject).Scene = scene;

            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType == typeof(Scene))
                    continue;

                object val = fi.GetValue(obj);

                if (editorSupportedTypes.Contains(fi.FieldType))
                {
                    Populate(val, n);
                }
                else if (fi.FieldType.IsGenericType &&
                    fi.FieldType.GetInterfaces().SingleOrDefault(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)) != null)
                {
                    Type dec = fi.FieldType.GetInterfaces().Single(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];

                    if (!editorSupportedTypes.Contains(dec))
                        continue;

                    IList list = val as IList;

                    if (list.Count > 0)
                    {
                        SceneNode sn2 = new SceneNode(fi.Name, list, dec);

                        n.Nodes.Add(sn2);
                        AddHandlers(sn2);

                        foreach (object o in list)
                        {
                            Populate(o, sn2);
                        }
                    }
                }
            }

            AddHandlers(n);
        }

        private void AddHandlers(SceneNode n)
        {
            n.ContextMenuStrip = menu;
        }

        void n_mnuAdd(SceneNode node)
        {
            Type t = null;
            if (node.List != null)
                t = node.ListType;
            else
            {
                Dictionary<string, Type> choices = new Dictionary<string, Type>();
                FieldInfo[] fields = node.Object.GetType().GetFields();

                foreach (FieldInfo fi in fields)
                {
                    if (fi.FieldType.IsGenericType &&
                    fi.FieldType.GetInterfaces().SingleOrDefault(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)) != null)
                    {
                        Type dec = fi.FieldType.GetInterfaces().Single(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];

                        choices.Add(fi.Name, dec);
                    }
                }

                if (!(node.Object is Scene))
                    choices.Add(node.Object.GetType().Name, node.Object.GetType());

                if (choices.Count < 2)
                    t = choices.First().Value;
                else
                {
                    t = NewObject.Get(choices);

                    if (t == null)
                        return;
                }
            }

            AddObjects(node, new object[] { Activator.CreateInstance(t) });
        }

        void n_mnuPaste(SceneNode node)
        {
            if (Clipboard.ContainsData("rMapObj"))
            {
                object obj = Clipboard.GetData("rMapObj");

                if (obj != null)
                    AddObjects(node, new object[] { obj });
            }
            else if (Clipboard.ContainsData("rMapList"))
            {
                object obj = Clipboard.GetData("rMapList");

                if (obj != null && obj is IList && ((IList)obj).Count > 0)
                    AddObjects(node, (IList)obj);
            }
        }

        public void Select(object obj)
        {
            SceneNode n = Find(obj, Nodes[0] as SceneNode);
            n.EnsureVisible();
            SelectedNode = n;
        }

        private SceneNode Find(object obj, SceneNode under)
        {
            if (under.Object == obj)
                return under;
            else
            {
                foreach (SceneNode n in under.Nodes.OfType<SceneNode>())
                {
                    SceneNode found = Find(obj, n);

                    if (found != null)
                        return found;
                }
                return null;
            }
        }

        private void AddObjects(SceneNode under, IList objects)
        {
            // Scene
            // |- IList // meshes
            // |   |- Obj1 // mesh container
            // |   |   |- IList // meshes 2
            // |   |   |    |- bObj1 // mesh

            if (under.Parent != null && under.Object != null && under.Object.GetType() == objects[0].GetType())
            {
                SceneNode parent = under.Parent as SceneNode;

                foreach (object o in objects)
                {
                    parent.List.Add(o);
                    Populate(o, parent);
                }
            }
            else if (under.List != null && under.ListType == objects[0].GetType())
            {
                foreach (object o in objects)
                {
                    under.List.Add(o);
                    Populate(o, under);
                }
            }
            else if (under.Object != null)
            {
                FieldInfo[] fields = under.Object.GetType().GetFields();

                foreach (FieldInfo fi in fields)
                {
                    if (fi.FieldType.IsGenericType &&
                    fi.FieldType.GetInterfaces().SingleOrDefault(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)) != null)
                    {
                        Type dec = fi.FieldType.GetInterfaces().Single(tt => tt.IsGenericType && tt.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];

                        if (dec == objects[0].GetType())
                        {
                            object val = fi.GetValue(under.Object);
                            IList list = val as IList;

                            SceneNode parent = under.Nodes.OfType<SceneNode>().SingleOrDefault(sn => sn.List != null && sn.ListType == dec);

                            if (parent == null)
                            {
                                parent = new SceneNode(fi.Name, list, dec);
                                under.Nodes.Add(parent);
                                AddHandlers(parent);

                                foreach (object o in list)
                                {
                                    Populate(o, parent);
                                }
                            }

                            foreach (object o in objects)
                            {
                                list.Add(o);
                                Populate(o, parent);
                            }
                        }
                    }
                }
            }
        }

        void n_mnuCut(SceneNode node)
        {
            ClearClipboard();
            Clipboard.SetData("rMap" + (node.Object == null ? "List" : "Obj"), node.Object ?? node.List);

            n_mnuRemove(node);
        }

        void n_mnuCopy(SceneNode node)
        {
            ClearClipboard();
            Clipboard.SetData("rMap" + (node.Object == null ? "List" : "Obj"), node.Object ?? node.List);
        }

        private void ClearClipboard()
        {
            Clipboard.Clear();
        }

        void n_mnuRemove(SceneNode node)
        {
            if (node.Parent == null)
            {
                Close();

                if (OnSelect != null)
                    OnSelect(null);
            }
            else
            {
                SceneNode parent = node.Parent as SceneNode;
                node.Remove();

                if (node.Object != null)
                {
                    parent.List.Remove(node.Object);

                    if (parent.List.Count < 1)
                        parent.Remove();
                }
                else
                    node.List.Clear();
            }
        }

        public void ObjectChanged(object obj)
        {
            if (obj is Scene)
            {
                Close();
                Load(obj as Scene);
            }
            else if (obj != null && (SelectedNode as SceneNode).Object == obj)
            {
                if (SelectedNode.Text != SelectedNode.ToString())
                    SelectedNode.Text = SelectedNode.ToString();
            }
        }

        public void Close()
        {
            scene = null;
            Nodes.Clear();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (OnSelect != null)
                OnSelect((e.Node as SceneNode).Object);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            base.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (SelectedNode == null)
                return;

            SceneNode node = SelectedNode as SceneNode;

            if (node.Object != null && node.Object is Scene)
            {
                if (e.Modifiers == Keys.Control)
                {
                    if ((e.KeyData & Keys.V) == Keys.V) ;
                    else if ((e.KeyData & Keys.A) == Keys.A) ;
                    else
                        return;
                }
                else
                    return;
            }

            if (e.Modifiers == Keys.None)
            {
                if ((e.KeyData & Keys.Delete) == Keys.Delete)
                    n_mnuRemove(node);
            }
            else if (e.Modifiers == Keys.Control)
            {
                if ((e.KeyData & Keys.X) == Keys.X)
                    n_mnuCut(node);
                else if ((e.KeyData & Keys.C) == Keys.C)
                    n_mnuCopy(node);
                else if ((e.KeyData & Keys.V) == Keys.V)
                    n_mnuPaste(node);
                else if ((e.KeyData & Keys.A) == Keys.A)
                    n_mnuAdd(node);
            }
        }

        private ContextMenuStrip CreateMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripItem add = menu.Items.Add("Add");
            menu.Items.Add(new ToolStripSeparator());
            ToolStripItem cut = menu.Items.Add("Cut");
            ToolStripItem copy = menu.Items.Add("Copy");
            ToolStripItem paste = menu.Items.Add("Paste");
            menu.Items.Add(new ToolStripSeparator());
            ToolStripItem remove = menu.Items.Add("Remove");

            menu.Opening += new System.ComponentModel.CancelEventHandler(menu_Opening);

            add.Click += new EventHandler(add_Click);
            cut.Click += new EventHandler(cut_Click);
            copy.Click += new EventHandler(copy_Click);
            paste.Click += new EventHandler(paste_Click);
            remove.Click += new EventHandler(remove_Click);

            return menu;
        }

        void menu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SelectedNode == null || !(SelectedNode is SceneNode))
                return;

            SceneNode node = SelectedNode as SceneNode;
            object Object = node.Object;

            bool isObj = Object != null && !(Object is Scene);

            menu.Items.OfType<ToolStripItem>().Single(ts => ts.Text == "Cut").Enabled = isObj;
            menu.Items.OfType<ToolStripItem>().Single(ts => ts.Text == "Copy").Enabled = isObj;
            menu.Items.OfType<ToolStripItem>().Single(ts => ts.Text == "Remove").Enabled = isObj;

            ToolStripItem paste = menu.Items.OfType<ToolStripItem>().Single(ts => ts.Text == "Paste");
            paste.Enabled = Clipboard.ContainsData("rMapList") || Clipboard.ContainsData("rMapObj");
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                SelectedNode = e.Node; // fixes the issue where when right clicking then the selected node is NOT the node under the cursor

            base.OnNodeMouseClick(e);
        }

        #region Handlers
        void remove_Click(object sender, EventArgs e)
        {
            n_mnuRemove(SelectedNode as SceneNode);
        }

        void paste_Click(object sender, EventArgs e)
        {
            n_mnuPaste(SelectedNode as SceneNode);
        }

        void copy_Click(object sender, EventArgs e)
        {
            n_mnuCopy(SelectedNode as SceneNode);
        }

        void cut_Click(object sender, EventArgs e)
        {
            n_mnuCut(SelectedNode as SceneNode);
        }

        void add_Click(object sender, EventArgs e)
        {
            n_mnuAdd(SelectedNode as SceneNode);
        }
        #endregion
    }

    class SceneNode : TreeNode, IComparable, IComparable<SceneNode>
    {
        public object Object = null;
        public IList List = null;
        public Type ListType = null;

        public SceneNode(string name, IList list, Type type)
            : base(name)
        {
            List = list;
            ListType = type;
        }

        public SceneNode(object obj)
            : base(obj.ToString())
        {
            Object = obj;
        }

        public override string ToString()
        {
            return Object == null ? base.Text : Object.ToString();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return Text.CompareTo(obj);
        }

        #endregion

        #region IComparable<SceneNode> Members

        public int CompareTo(SceneNode other)
        {
            return Text.CompareTo(other.Text);
        }

        #endregion
    }

    class TreeNodeComparer : IComparer, IComparer<TreeNode>
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x is TreeNode && y is TreeNode)
            {
                return (x as TreeNode).Text.CompareTo((y as TreeNode).Text);
            }
            else
                throw new ArgumentException("Not a treenode", "x or y");
        }

        #endregion

        #region IComparer<TreeNode> Members

        public int Compare(TreeNode x, TreeNode y)
        {
            return x.Text.CompareTo(y.Text);
        }

        #endregion
    }
}
