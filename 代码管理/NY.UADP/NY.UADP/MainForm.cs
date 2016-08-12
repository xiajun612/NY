using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Xml;
using System.Reflection;
using DevExpress.XtraBars.Ribbon;

namespace NY.UADP
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public MainForm()
        {
            InitializeComponent();

            #region 初始化控件
            try
            {
                System.IO.Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("NY.UADP.Menu.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(s);
                if (doc == null) throw new Exception("无功能菜单配置");

                foreach (XmlNode item in doc.SelectNodes("page"))
                {
                    RibbonPage page = new RibbonPage();

                    page.Name = item.Attributes.GetNamedItem("id").Value.ToString();
                    page.Text = item.Attributes.GetNamedItem("name").Value.ToString();

                    foreach (XmlNode item1 in item.ChildNodes)
                    {
                        RibbonPageGroup group = new RibbonPageGroup();
                        group.Name = item1.Attributes.GetNamedItem("id").Value.ToString();
                        group.Text = item1.Attributes.GetNamedItem("name").Value.ToString();

                        foreach (XmlNode item2 in item1.ChildNodes)
                        {
                            BarButtonItem btn = new BarButtonItem();
                            btn.Name = item2.Attributes.GetNamedItem("id").Value.ToString();
                            btn.Caption = item2.Attributes.GetNamedItem("name").Value.ToString();
                            btn.RibbonStyle = RibbonItemStyles.Large;
                            btn.ItemClick += ribbon_ItemClick;
                            group.ItemLinks.Add(btn);
                        }
                        page.Groups.Add(group);
                    }

                    ribbon.Pages.Add(page);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败！" + ex.ToString());
                Application.Exit();
            }

            #endregion

            this.WindowState = FormWindowState.Maximized;
        }


        private void RibbonForm1_Load(object sender, EventArgs e)
        {


        }


        void ribbon_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form frm = null;
            switch (e.Item.Name)
            {
                case "base_fun_menu":
                    frm = new frmMenu();
                    frm.Text = e.Item.Caption;

                    break;
                case "base_fun_template":
                    frm = new frmMenu();
                    frm.ShowDialog();
                    return;
                    break;
                default:
                    break;
            }
            ShowMDIForm(frm);
        }

        #region 方法

        private void ShowMDIForm(Form childForm)
        {
            if (childForm == null) return;
            foreach (Form frm in this.MdiChildren)
            {
                if (frm.GetType() == childForm.GetType() && frm.Text == childForm.Text)
                {
                    frm.BringToFront();
                    if (frm.WindowState == FormWindowState.Minimized)
                    {
                        frm.WindowState = FormWindowState.Normal;
                    }
                    return;
                }
            }
            if (this.MdiChildren.Length == 0)
            {
                childForm.WindowState = FormWindowState.Minimized;
                childForm.WindowState = FormWindowState.Maximized;
            }

            childForm.MdiParent = this;
            childForm.Show();
            childForm.Activate();
        }

        #endregion
    }
}