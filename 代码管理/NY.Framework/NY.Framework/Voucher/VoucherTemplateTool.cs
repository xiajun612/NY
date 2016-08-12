using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace NY.Framework.Voucher
{
    /// <summary>
    /// Author:xxp
    /// Remark:单据模版工具类
    /// CreateTime:20160113 11:39
    /// </summary>
    public class VoucherTemplateTool
    {

        /// <summary>
        /// 获取列表配置信息
        /// </summary>
        /// <param name="strVT_CardNum">单据编号</param>
        /// <param name="strFilePath">配置文件地址</param>
        /// <returns></returns>
        public static ListTemplateConfiguration GetListTemplateConfig(string strVT_CardNum, string strFilePath = "Config\\KHYFM_VoucherTemplate.xml")
        {
            ListTemplateConfiguration vgvListConfig = new ListTemplateConfiguration();

            DataTable dtColumns = TemplateInitTool.GetListTemplate();
            //读取IO文件配置
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(strFileName);
            string strBinFile = System.IO.Directory.GetCurrentDirectory() + "\\";
            if (File.Exists(strBinFile + strFilePath))
            {
                FileStream stream = null;
                try
                {
                    using (stream = new FileStream(strBinFile + strFilePath, FileMode.Open))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(stream);
                        XmlNode node = doc.SelectSingleNode(String.Format("xml/voucher[@VT_CardNum='{0}']", strVT_CardNum));
                        if (node != null)
                        {
                            XmlElement element = node as XmlElement;
                            vgvListConfig.VT_CardNum = strVT_CardNum;
                            vgvListConfig.VT_Name = element.GetAttribute("VT_Name");

                            XmlNodeList nodelist = node.SelectNodes(string.Format("list//item"));
                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                XmlElement elementItem = nodelist[i] as XmlElement;
                                DataRow dNewRow = dtColumns.NewRow();

                                dNewRow["cFieldName"] = elementItem.GetAttribute("cFieldName");
                                dNewRow["cCaption"] = elementItem.GetAttribute("cCaption");

                                dNewRow["iWidth"] = elementItem.GetAttribute("iWidth") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iWidth"));

                                dNewRow["bVisible"] = elementItem.GetAttribute("bVisible") == "" ? false : XmlConvert.ToBoolean(elementItem.GetAttribute("bVisible"));

                                dNewRow["bSum"] = elementItem.GetAttribute("bSum") == "" ? false : XmlConvert.ToBoolean(elementItem.GetAttribute("bSum"));
                                dNewRow["iOrder"] = elementItem.GetAttribute("iOrder") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iOrder")); ;

                                dNewRow["cFormat"] = elementItem.GetAttribute("cFormat");

                                dNewRow["iAlignment"] = elementItem.GetAttribute("iAlignment") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iAlignment")); ;

                                dtColumns.Rows.Add(dNewRow);
                            }
                            vgvListConfig.dtColumns = dtColumns;
                        }
                        else
                        {
                            throw new Exception("未查找到相关配置信息,请联系管理员");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
            }
            else
            {
                throw new Exception("本地配置文件已经不存在,请联系管理员");
            }

            return vgvListConfig;
        }

        /// <summary>
        /// 获取单据配置信息
        /// </summary>
        /// <param name="strVT_CardNum">单据编号</param>
        /// <param name="strVT_Code">模板编号</param>
        /// <param name="strFilePath">配置文件地址</param>
        /// <returns></returns>
        public static VoucherTemplateConfiguration GetVoucherTemplateConfig(string strVT_CardNum, string strVT_Code, string strFilePath = "Config\\KHYFM_VoucherTemplate.xml")
        {
            VoucherTemplateConfiguration vgvVoucherConfig = new VoucherTemplateConfiguration();

            DataTable dtColumns = TemplateInitTool.GetVoucherTemplate();

            //读取IO文件配置
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(strFileName);
            string strBinFile = System.IO.Directory.GetCurrentDirectory() + "\\";
            if (File.Exists(strBinFile + strFilePath))
            {
                FileStream stream = null;
                try
                {
                    using (stream = new FileStream(strBinFile + strFilePath, FileMode.Open))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(stream);
                        XmlNode node = doc.SelectSingleNode(String.Format("xml/voucher[@VT_CardNum='{0}']//vouchertemplates[@VT_Code='{1}']", strVT_CardNum, strVT_Code));
                        if (node != null)
                        {
                            XmlElement element = node as XmlElement;
                            vgvVoucherConfig.VT_CardNum = strVT_CardNum;
                            vgvVoucherConfig.VT_Title = element.GetAttribute("VT_Title");
                            vgvVoucherConfig.VT_Code = strVT_Code;
                            vgvVoucherConfig.VT_Mode = XmlConvert.ToInt32(element.GetAttribute("VT_Mode"));

                            XmlNodeList nodelist = node.ChildNodes;

                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                XmlElement elementItem = nodelist[i] as XmlElement;
                                DataRow dNewRow = dtColumns.NewRow();
                                dNewRow["cFieldName"] = elementItem.GetAttribute("cFieldName");
                                dNewRow["iFieldType"] = XmlConvert.ToInt32(elementItem.GetAttribute("iFieldType"));
                                dNewRow["cCardItemName"] = elementItem.GetAttribute("cCardItemName");
                                dNewRow["cCaption"] = elementItem.GetAttribute("cCaption") == "" ? elementItem.GetAttribute("cCardItemName") : elementItem.GetAttribute("cCaption");

                                dNewRow["cColType"] = elementItem.GetAttribute("cColType") == "" ? "txt" : elementItem.GetAttribute("cColType");

                                dNewRow["iWidth"] = elementItem.GetAttribute("iWidth") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iWidth"));

                                dNewRow["iHeigth"] = elementItem.GetAttribute("iHeigth") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iHeigth"));

                                dNewRow["cCardSection"] = elementItem.GetAttribute("cCardSection");
                                dNewRow["iCOX"] = elementItem.GetAttribute("iCOX") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iCOX"));
                                dNewRow["iCOY"] = elementItem.GetAttribute("iCOY") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iCOY"));

                                dNewRow["bVisible"] = elementItem.GetAttribute("bVisible") == "" ? true : XmlConvert.ToBoolean(elementItem.GetAttribute("bVisible"));

                                dNewRow["bReadOnly"] = elementItem.GetAttribute("bReadOnly") == "" ? true : XmlConvert.ToBoolean(elementItem.GetAttribute("bReadOnly"));

                                dNewRow["bMain"] = elementItem.GetAttribute("bMain") == "" ? false : XmlConvert.ToBoolean(elementItem.GetAttribute("bMain"));

                                dNewRow["bNeedSum"] = elementItem.GetAttribute("bNeedSum") == "" ? true : XmlConvert.ToBoolean(elementItem.GetAttribute("bNeedSum"));

                                dNewRow["cDefaultValue"] = elementItem.GetAttribute("cDefaultValue");
                                dNewRow["cDataRule"] = elementItem.GetAttribute("cDataRule");

                                dNewRow["bIsNull"] = elementItem.GetAttribute("bIsNull") == "" ? false : XmlConvert.ToBoolean(elementItem.GetAttribute("bIsNull"));

                                dNewRow["iOrder"] = elementItem.GetAttribute("iOrder") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iOrder"));

                                dNewRow["cFormat"] = elementItem.GetAttribute("cFormat");

                                dNewRow["iAlignment"] = elementItem.GetAttribute("iAlignment") == "" ? 0 : XmlConvert.ToInt32(elementItem.GetAttribute("iAlignment"));

                                dNewRow["refObject"] = elementItem.GetAttribute("refObject");
                                dtColumns.Rows.Add(dNewRow);
                            }
                            vgvVoucherConfig.dtColumns = dtColumns;
                        }
                        else
                        {
                            throw new Exception("未查找到相关配置信息,请联系管理员");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (stream != null)
                        stream.Dispose();
                }
            }
            else
            {
                throw new Exception("本地配置文件已经不存在,请联系管理员");
            }
            return vgvVoucherConfig;
        }

        /// <summary>
        /// 设置列表显示
        /// </summary>
        /// <param name="templateCofig">列表配置</param>
        /// <param name="dgv">GridView控件</param>
        /// <param name="strLocalCacheLayoutFilePath">本地缓存文件</param>
        /// <param name="bolClearColumns">是否清空</param>
        /// <returns></returns>
        public static void SetListGridView(ListTemplateConfiguration templateCofig, GridView dgv, string strLocalCacheLayoutFilePath, bool bolClearColumns = true)
        {
            DataTable dtColums = templateCofig.dtColumns;
            dgv.OptionsSelection.MultiSelect = true;
            dgv.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            dgv.SelectionChanged += templateCofig.GridView_SelectionChanged;
            dgv.OptionsBehavior.ReadOnly = true;
            dgv.OptionsView.ColumnAutoWidth = false;

            if (dtColums != null)
            {
                DataView dataView = dtColums.DefaultView;
                dataView.Sort = "iOrder asc"; //处理排序问题
                DataTable dt = dataView.ToTable();
                //dgv.AutoGenerateColumns = false;
                //dgv.AllowUserToAddRows = false;
                //dgv.AllowUserToDeleteRows = false;
                //dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (bolClearColumns)
                {
                    dgv.Columns.Clear();
                }

                if (templateCofig.bSelect) //设置多选
                {
                    templateCofig.gcmMoreCheck = new NY.Utility.GridCheckBoxMarks(dgv);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    GridColumn dgvTxtCol = new GridColumn();
                    dgvTxtCol.Name = string.Format("txt{0}", dr["cFieldName"].ToString());
                    dgvTxtCol.FieldName = dr["cFieldName"].ToString();
                    dgvTxtCol.Caption = dr["cCaption"].ToString();
                    dgvTxtCol.Width = Convert.ToInt32(dr["iWidth"]);
                    dgvTxtCol.Visible = Convert.ToBoolean(dr["bVisible"]) == true ? false : true;
                    if (Convert.ToBoolean(dr["bSum"]))
                    {
                        dgvTxtCol.SummaryItem.FieldName = dr["cFieldName"].ToString();
                        dgvTxtCol.SummaryItem.SummaryType = SummaryItemType.Sum;
                        if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                        {
                            dgvTxtCol.SummaryItem.DisplayFormat = "{0:" + dr["cFormat"].ToString() + "}";
                        }
                    }
                    if (Convert.ToInt32(dr["iAlignment"]) != 0)
                    {
                        SetAlignment(dgvTxtCol, Convert.ToInt32(dr["iAlignment"]));
                    }
                    if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                    {
                        SetFormat(dgvTxtCol, dr["cFormat"].ToString());
                    }

                    dgv.Columns.Add(dgvTxtCol);
                }
            }
            else
            {
                throw new Exception("未查到到任何配置信息");
            }
        }


        /// <summary>
        /// 设置单据显示
        /// </summary>
        /// <param name="templateCofig">单据配置</param>
        /// <param name="control">控件</param>
        /// <param name="dgv">列表控件</param>
        /// <param name="strLocalCacheLayoutFilePath">本地缓存文件</param>
        /// <returns></returns>
        public void SetVoucherControl(VoucherTemplateConfiguration templateCofig, System.Windows.Forms.Control control, GridView dgv, string strLocalCacheLayoutFilePath)
        {
            DataTable dtColums = templateCofig.dtColumns;
            dgv.OptionsBehavior.ReadOnly = true;
            dgv.OptionsView.ColumnAutoWidth = false;
            dgv.OptionsSelection.MultiSelect = true;
            dgv.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            dgv.SelectionChanged += templateCofig.GridView_SelectionChanged;
            if (dtColums != null)
            {
                //DataView dataView = dtColums.DefaultView;
                //dataView.Sort = "iOrder asc"; //处理排序问题
                //DataTable dt = dataView.ToTable();
                //dgv.AutoGenerateColumns = false;
                //dgv.AllowUserToAddRows = false;
                //dgv.AllowUserToDeleteRows = false;
                //dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                List<DataRow> lstT = dtColums.Select("cCardSection='T'", "iOrder asc").ToList();
                List<DataRow> lstB = dtColums.Select("cCardSection='B'", "iOrder asc").ToList();
                //表头处理

                //表体处理
                foreach (DataRow dr in lstB)
                {
                    GridColumn dgvTxtCol = new GridColumn();
                    dgvTxtCol.Name = string.Format("txt{0}", dr["cFieldName"].ToString());
                    dgvTxtCol.FieldName = dr["cFieldName"].ToString();
                    dgvTxtCol.Caption = dr["cCaption"].ToString();
                    dgvTxtCol.Width = Convert.ToInt32(dr["iWidth"]);
                    dgvTxtCol.OptionsColumn.ReadOnly = Convert.ToBoolean(dr["bReadOnly"]);
                    dgvTxtCol.Visible = Convert.ToBoolean(dr["bVisible"]) == true ? false : true;

                    switch (dr["cColType"].ToString().ToLower())
                    {
                        case "cmb":
                            var cmb = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
                            // refer.ButtonClick += templateCofig.bodyRefer_ButtonClick;
                            dgvTxtCol.ColumnEdit = cmb;
                            cmb.AutoHeight = false;
                            break;
                        case "chk":

                            break;
                        case "date":
                            var date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                            // refer.ButtonClick += templateCofig.bodyRefer_ButtonClick;
                            dgvTxtCol.ColumnEdit = date;
                            date.AutoHeight = false;

                            break;
                        case "refer":
                            //DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
                            //GridColumn dgvReferCol = new GridColumn();
                            //dgvReferCol.Name = string.Format("txt{0}", dr["cFieldName"].ToString());
                            //dgvReferCol.FieldName = dr["cFieldName"].ToString();
                            //dgvReferCol.Caption = dr["cCaption"].ToString();
                            //dgvReferCol.Width = Convert.ToInt32(dr["iWidth"]);
                            //dgvReferCol.Visible = Convert.ToBoolean(dr["bVisible"]) == true ? false : true;
                            //if (Convert.ToBoolean(dr["bNeedSum"]))
                            //{
                            //    dgvReferCol.SummaryItem.FieldName = dr["cFieldName"].ToString();
                            //    dgvReferCol.SummaryItem.SummaryType = SummaryItemType.Sum;
                            //}
                            //if (Convert.ToInt32(dr["iAlignment"]) != 0)
                            //{
                            //    SetAlignment(dgvTxtCol, Convert.ToInt32(dr["iAlignment"]));
                            //}
                            //if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                            //{
                            //    SetFormat(dgvTxtCol, dr["cFormat"].ToString());
                            //}
                            //if (Convert.ToBoolean(dr["bIsNull"]))
                            //{
                            //    dgvTxtCol.AppearanceHeader.ForeColor = DXColor.Blue;
                            //    dgvTxtCol.AppearanceHeader.Options.UseForeColor = true;
                            //}

                            var refer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();

                            dgvTxtCol.ColumnEdit = refer;
                            refer.AutoHeight = false;
                            refer.Buttons[0].Caption = dr["refObject"].ToString();
                            refer.ButtonClick += templateCofig.BodyRefer_ButtonClick;
                            refer.Validating += templateCofig.Body_Validating;

                            //refer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
                            //new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, dr["refObject"].ToString(), -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});

                            break;
                        case "calc":
                            var Calc = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
                            // refer.ButtonClick += templateCofig.bodyRefer_ButtonClick;
                            Calc.Validating += templateCofig.Body_Validating;
                            dgvTxtCol.ColumnEdit = Calc;
                            Calc.AutoHeight = false;

                            if (Convert.ToBoolean(dr["bNeedSum"]))
                            {
                                dgvTxtCol.SummaryItem.FieldName = dr["cFieldName"].ToString();
                                dgvTxtCol.SummaryItem.SummaryType = SummaryItemType.Sum;
                                if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                                {
                                    dgvTxtCol.SummaryItem.DisplayFormat = "{0:" + dr["cFormat"].ToString() + "}";
                                }
                            }
                            break;
                        default: //txt
                            var Txt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                            Txt.AutoHeight = false;
                            Txt.Validating += templateCofig.Body_Validating;
                            dgvTxtCol.ColumnEdit = Txt;
                            //if (Convert.ToBoolean(dr["bNeedSum"]))
                            //{
                            //    dgvTxtCol.SummaryItem.FieldName = dr["cFieldName"].ToString();
                            //    dgvTxtCol.SummaryItem.SummaryType = SummaryItemType.Sum;
                            //    if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                            //    {
                            //        dgvTxtCol.SummaryItem.DisplayFormat = "{0:" + dr["cFormat"].ToString() + "}";
                            //    }
                            //}
                            break;
                    }

                    if (Convert.ToInt32(dr["iAlignment"]) != 0)
                    {
                        SetAlignment(dgvTxtCol, Convert.ToInt32(dr["iAlignment"]));
                    }

                    if (string.IsNullOrEmpty(dr["cFormat"].ToString()) == false)
                    {
                        SetFormat(dgvTxtCol, dr["cFormat"].ToString());
                    }
                    if (Convert.ToBoolean(dr["bIsNull"]))
                    {
                        dgvTxtCol.AppearanceHeader.ForeColor = DXColor.Blue;
                        dgvTxtCol.AppearanceHeader.Options.UseForeColor = true;
                    }

                    dgv.Columns.Add(dgvTxtCol);
                }
            }
            else
            {
                throw new Exception("未查到到任何配置信息");
            }
        }



        /// <summary>
        /// 设置栏目格式
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="strFormat"></param>
        private static void SetFormat(GridColumn gc, string strFormat)
        {
            switch (strFormat.ToLower().ToString())
            {
                case "datetime":
                    gc.DisplayFormat.FormatType = FormatType.DateTime;
                    gc.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
                    break;
                case "date":
                    gc.DisplayFormat.FormatType = FormatType.DateTime;
                    gc.DisplayFormat.FormatString = "yyyy-MM-dd";
                    break;
                case "f":
                    gc.DisplayFormat.FormatType = FormatType.Numeric;
                    gc.DisplayFormat.FormatString = "F";
                    break;
                case "f0":
                    gc.DisplayFormat.FormatType = FormatType.Numeric;
                    gc.DisplayFormat.FormatString = "F0";
                    break;
                case "f2":
                    gc.DisplayFormat.FormatType = FormatType.Numeric;
                    gc.DisplayFormat.FormatString = "F2";
                    break;
                case "f4":
                    gc.DisplayFormat.FormatType = FormatType.Numeric;
                    gc.DisplayFormat.FormatString = "F4";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置栏目格式
        /// </summary>
        /// <param name="gc">列表</param>
        /// <param name="iAlignment">位置:1:Near,2:Center,3:Far</param>
        private static void SetAlignment(GridColumn gc, int iAlignment)
        {
            gc.AppearanceCell.Options.UseTextOptions = true;
            switch (iAlignment)//1:Near,2:Center,3:Far
            {
                case 1:
                    gc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    break;
                case 2:
                    gc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    break;
                case 3:
                    gc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    break;
                default:
                    gc.AppearanceCell.Options.UseTextOptions = false;
                    gc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
                    break;
            }
        }


        private static string SetData(string strValue, Type t)
        {
            //if (t == typeof(DateTime) && strValue.IsNullOrEmpty() == false)
            //{
            //    DateTime basetime = DateTime.Parse("1970-01-01");
            //    DateTime time = DateTime.Parse(strValue);
            //    long ticks = (time.ToUniversalTime().Ticks - basetime.Ticks) / 10000;
            //    //long ticks = (long)(time.ToUniversalTime() - basetime).TotalMilliseconds;

            //    return string.Format(@"\/Date({0})\/", ticks);
            //}
            return strValue;

        }

        ///// <summary>    
        ///// 将时间字符串转为Json时间    
        ///// </summary>    
        //private static string ConvertDateStringToJsonDate(Match m)
        //{
        //    string result = string.Empty;
        //    DateTime dt = DateTime.Parse(m.Groups[0].Value);
        //    dt = dt.ToUniversalTime();
        //    TimeSpan ts = dt - DateTime.Parse("1970-01-01");
        //    result = string.Format("///Date({0}+0800)///", ts.TotalMilliseconds);
        //    return result;
        //}

    }
}
