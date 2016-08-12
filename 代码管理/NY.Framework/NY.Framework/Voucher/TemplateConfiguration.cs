using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using NY.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NY.Framework.Voucher
{
    /// <summary>
    /// Author:xxp
    /// Remark:单据模板配置类
    /// CreateTime:20160113
    /// </summary>
    public class VoucherTemplateConfiguration
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string VT_CardNum { get; set; }

        /// <summary>
        /// 单据标题
        /// </summary>
        public string VT_Title { get; set; }

        /// <summary>
        /// 模板编码
        /// </summary>
        public string VT_Code { get; set; }

        /// <summary>
        /// 模板类型,0:显示,1:打印
        /// </summary>
        public int VT_Mode { get; set; }

        /// <summary>
        /// 列配置DataTable
        /// </summary>
        public System.Data.DataTable dtColumns { get; set; }

        /// <summary>
        /// 单据头
        /// </summary>
        public DataTable model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BindingSource modelSource { get; set; }

        /// <summary>
        /// 单据体
        /// </summary>
        public DataTable modelItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BindingSource modelItemSource { get; set; }

        /// <summary>
        /// 当前窗体
        /// </summary>
        public System.Windows.Forms.Control CurrForm { get; set; }

        /// <summary>
        /// 表体
        /// </summary>
        public GridView BodyGridView { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public UserContext Context { get; set; }

        /// <summary>
        /// 提示控件
        /// </summary>
        public ToolTip toolTip1 = new ToolTip();

        /// <summary>
        /// 选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void GridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (sender is GridView)
            {
                GridView gridView1 = sender as GridView;
                if (gridView1.GetSelectedCells().Count() > 0 && (gridView1.OptionsBehavior.ReadOnly || gridView1.OptionsBehavior.Editable == false))
                {
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcs = gridView1.GetSelectedCells();
                    string strColumnName = "";
                    Decimal dTotal = 0;
                    bool bIsCompute = false;
                    for (int i = 0; i < gcs.Count(); i++)
                    {
                        DevExpress.XtraGrid.Views.Base.GridCell gc = gcs[i];
                        if (i == 0)
                            strColumnName = gc.Column.Name;
                        if (strColumnName != gc.Column.Name)
                        {
                            return;
                        }
                        if (gc.Column.SummaryItem.SummaryType == SummaryItemType.Sum)
                        {
                            if (gridView1.GetRowCellValue(gc.RowHandle, gc.Column) != null && gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString() != "")
                            {
                                dTotal += Convert.ToDecimal(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                                bIsCompute = true;
                            }
                        }
                    }
                    if (bIsCompute)
                        toolTip1.SetToolTip(gridView1.GridControl, ("合计：" + dTotal.ToString("f2")));
                    //Console.WriteLine("选中单元格:" + gridView1.GetSelectedCells().Count().ToString());
                }
            }
        }

        /// <summary>
        /// 单据体 参照控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void HeadRefer_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(e.Button.Caption) == false && model != null && model.Rows.Count > 0)
                //{
                //    var currControl = sender as ButtonEdit;
                //    var Ret = FilterConditions.FilterProcess(Context, e.Button.Caption.ToString(), false, null, currControl.Text, null);
                //    DataRow dRow = model.Rows[0];
                //    if (Ret != null)
                //    {
                //        if (Ret.Item1 != null && Ret.Item1.Rows.Count > 0)
                //        {
                //            for (int i = 0; i < Ret.Item1.Rows.Count; i++)
                //            {
                //                foreach (DataColumn item in Ret.Item1.Columns)
                //                {
                //                    if (dRow.Table.Columns.Contains(item.ColumnName))
                //                    {
                //                        dRow[item.ColumnName] = Ret.Item1.Rows[i][item.ColumnName];
                //                    }
                //                }
                //            }
                //        }
                //        model.AcceptChanges();

                //        //modelSource.EndEdit();
                //    }
                //}
            }
            catch (Exception ex)
            {
                ClsMsg.ShowErrMsg(ex.ToString());
            }
        }

        /// <summary>
        /// 自定义单据头验证事件
        /// </summary>
        public event System.ComponentModel.CancelEventHandler CustomHead_Validating;

        /// <summary>
        /// 单据头校验事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Head_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CustomHead_Validating != null)
                this.CustomHead_Validating(sender, e);
        }

        /// <summary>
        /// 表体参照事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void BodyRefer_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //if (e.Button.Caption.IsNullOrEmpty() == false)
                //{
                //    var currControl = sender as ButtonEdit;
                //    var Ret = FilterConditions.FilterProcess(Context, e.Button.Caption.ToString(), false, null, currControl.Text, null);
                //    DataRow dRow = (BodyGridView.GetRow(BodyGridView.FocusedRowHandle) as DataRowView).Row;

                //    if (Ret != null)
                //    {
                //        if (Ret.Item1 != null && Ret.Item1.Rows.Count > 0)
                //        {
                //            for (int i = 0; i < Ret.Item1.Rows.Count; i++)
                //            {
                //                if (i > 0)
                //                {
                //                    dRow = dRow.Table.NewRow();
                //                    VoucherDataTool.SetDefaultValue(dtColumns, "B", dRow);
                //                    dRow.Table.Rows.Add(dRow);
                //                }
                //                foreach (DataColumn item in Ret.Item1.Columns)
                //                {
                //                    if (dRow.Table.Columns.Contains(item.ColumnName))
                //                    {
                //                        dRow[item.ColumnName] = Ret.Item1.Rows[i][item.ColumnName];
                //                    }
                //                }
                //            }
                //        }
                //        if (dRow["Editprop"] is DBNull || dRow["Editprop"].ToString().IsNullOrEmpty())
                //            dRow["Editprop"] = "E";
                //        modelItem.AcceptChanges();
                //        //modelItemSource.EndEdit();
                //    }
                //}
            }
            catch (Exception ex)
            {
                //ClsMsg.ShowErrMsg(ex.ToString());
            }
        }

        /// <summary>
        /// 自定义表体验证事件
        /// </summary>
        public event System.ComponentModel.CancelEventHandler CustomBody_Validating;

        /// <summary>
        /// 表体验证事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Body_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.CustomBody_Validating != null)
                this.CustomBody_Validating(sender, e);

            if (sender is ButtonEdit)
            {

            }

        }
    }

    /// <summary>
    /// Author:xxp
    /// Remark:列表模板配置类
    /// CreateTime:20160113
    /// </summary>
    public class ListTemplateConfiguration
    {
        /// <summary>
        /// 单据编号
        /// </summary>
        public string VT_CardNum { get; set; }

        /// <summary>
        /// 单据名称
        /// </summary>
        public string VT_Name { get; set; }

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool bSelect { get; set; }

        /// <summary>
        /// 列配置DataTable
        /// </summary>
        public System.Data.DataTable dtColumns { get; set; }

        /// <summary>
        /// 多选控件
        /// </summary>
        public NY.Utility.GridCheckBoxMarks gcmMoreCheck { get; set; }

        /// <summary>
        /// 提示控件
        /// </summary>
        public ToolTip toolTip1 = new ToolTip();

        /// <summary>
        /// 选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void GridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (sender is GridView)
            {
                GridView gridView1 = sender as GridView;
                if (gridView1.GetSelectedCells().Count() > 0 && (gridView1.OptionsBehavior.ReadOnly || gridView1.OptionsBehavior.Editable == false))
                {
                    DevExpress.XtraGrid.Views.Base.GridCell[] gcs = gridView1.GetSelectedCells();
                    string strColumnName = "";
                    Decimal dTotal = 0;
                    bool bIsCompute = false;
                    for (int i = 0; i < gcs.Count(); i++)
                    {
                        DevExpress.XtraGrid.Views.Base.GridCell gc = gcs[i];
                        if (i == 0)
                            strColumnName = gc.Column.Name;
                        if (strColumnName != gc.Column.Name)
                        {
                            return;
                        }
                        if (gc.Column.SummaryItem.SummaryType == SummaryItemType.Sum)
                        {
                            if (gridView1.GetRowCellValue(gc.RowHandle, gc.Column) != null && gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString() != "")
                            {
                                dTotal += Convert.ToDecimal(gridView1.GetRowCellValue(gc.RowHandle, gc.Column).ToString());
                                bIsCompute = true;
                            }
                        }
                    }
                    if (bIsCompute)
                        toolTip1.SetToolTip(gridView1.GridControl, ("合计：" + dTotal.ToString("f2")));
                    //Console.WriteLine("选中单元格:" + gridView1.GetSelectedCells().Count().ToString());
                }
            }
        }
    }


    /// <summary>
    /// Author:xxp
    /// Remark: 单据模板类型 Voucher:单据,Print:打印或数据,List:列表
    /// CreateTime:20160113
    /// </summary>
    public enum VouchTemplateType
    {
        Voucher, Print, List
    }


    /// <summary>
    /// Author:xxp
    /// Remark:模版初始化类
    /// CreateTime:20160113
    /// </summary>
    public class TemplateInitTool
    {
        /// <summary>
        /// 获取单据列表模板Table结构
        /// </summary>
        /// <returns></returns>
        public static DataTable GetListTemplate()
        {
            DataTable dtColumns = new DataTable();
            dtColumns.Columns.AddRange(new DataColumn[] { 
                new DataColumn("cFieldName",Type.GetType("System.String")),
                new DataColumn("cCaption",Type.GetType("System.String")),
                new DataColumn("iWidth",Type.GetType("System.Int32")),
                new DataColumn("bVisible",Type.GetType("System.Boolean")),
                new DataColumn("iOrder",Type.GetType("System.Int32")),
                new DataColumn("bSum",Type.GetType("System.Boolean")),
                new DataColumn("cFormat",Type.GetType("System.String")),
                new DataColumn("iAlignment",Type.GetType("System.Int32"))
            });
            return dtColumns;
        }

        /// <summary>
        /// 获取单据模板Table结构
        /// </summary>
        /// <returns></returns>
        public static DataTable GetVoucherTemplate()
        {
            DataTable dtColumns = new DataTable();
            dtColumns.Columns.AddRange(new DataColumn[] { 
                new DataColumn("cFieldName",Type.GetType("System.String")),
                new DataColumn("iFieldType",Type.GetType("System.Int32")),
                new DataColumn("cCardItemName",Type.GetType("System.String")),
                new DataColumn("cCaption",Type.GetType("System.String")),
                new DataColumn("cColType",Type.GetType("System.String")),
                new DataColumn("iWidth",Type.GetType("System.Int32")),
                new DataColumn("iHeigth",Type.GetType("System.Int32")),
                new DataColumn("cCardSection",Type.GetType("System.String")),
                new DataColumn("iCOX",Type.GetType("System.Int32")),
                new DataColumn("iCOY",Type.GetType("System.Int32")),
                new DataColumn("bVisible",Type.GetType("System.Boolean")),
                new DataColumn("bReadOnly",Type.GetType("System.Boolean")),
                new DataColumn("bMain",Type.GetType("System.Boolean")),
                new DataColumn("bNeedSum",Type.GetType("System.Boolean")),
                new DataColumn("cDefaultValue",Type.GetType("System.String")),
                new DataColumn("cDataRule",Type.GetType("System.String")),
                new DataColumn("bIsNull",Type.GetType("System.Boolean")),
                new DataColumn("iOrder",Type.GetType("System.Int32")),                
                new DataColumn("cFormat",Type.GetType("System.String")),
                new DataColumn("iAlignment",Type.GetType("System.Int32")),
                new DataColumn("refObject",Type.GetType("System.String"))
            });
            return dtColumns;
        }
    }
}
