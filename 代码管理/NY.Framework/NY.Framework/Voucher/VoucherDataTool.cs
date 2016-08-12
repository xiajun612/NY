using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
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

namespace NY.Framework.Voucher
{
    /// <summary>
    /// Author:xxp
    /// Remark:单据数据处理
    /// CreateTime:20160510
    /// </summary>
    public class VoucherDataTool
    {
        /// <summary>
        /// 动态给控件绑定数据源
        /// </summary>
        /// <param name="lcGroup"></param>
        /// <param name="modelSource"></param>
        /// <param name="dataColumnCollection"></param>
        public static void VoucherDataBindings(LayoutControlGroup lcGroup, BindingSource modelSource, System.Data.DataColumnCollection dataColumnCollection)
        {
            foreach (BaseLayoutItem baseItem in lcGroup.Items)
            {
                LayoutControlItem item = baseItem as LayoutControlItem;
                if (item.Control != null && dataColumnCollection.Contains(item.CustomizationFormText))
                {
                    item.Control.DataBindings.Add(new Binding("EditValue", modelSource, item.CustomizationFormText, true));
                }
            }
        }

        /// <summary>
        /// 动态给控件绑定数据源
        /// </summary>
        /// <param name="lcGroup"></param>
        /// <param name="modelSource"></param>
        public static void VoucherDataBindings(LayoutControlGroup lcGroup, BindingSource modelSource)
        {
            foreach (BaseLayoutItem baseItem in lcGroup.Items)
            {
                LayoutControlItem item = baseItem as LayoutControlItem;
                if (item.Control != null && string.IsNullOrEmpty(item.CustomizationFormText) == false)
                {
                    item.Control.DataBindings.Add(new Binding("EditValue", modelSource, item.CustomizationFormText, true));
                }
            }
        }

        /// <summary>
        /// 将根据配置文件和数据生成Json字符串
        /// </summary>
        /// <param name="dtConfig">配置Table</param>
        /// <param name="dtData">数据Table</param>
        /// <param name="strCardSection">位置</param>
        /// <returns></returns>
        public static string GetDataJson(DataTable dtConfig, DataTable dtData, string strCardSection)
        {
            StringBuilder strJson = new StringBuilder();

            List<DataRow> lstConfig = dtConfig.Select(string.Format("bMain=1 and cCardSection='{0}'", strCardSection), "iOrder desc").ToList();
            if (strCardSection.Equals("T")) //单条数据
            {
                strJson.Append("{");
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow dRow = dtData.Rows[i];

                    StringBuilder strObject = new StringBuilder();

                    foreach (var item in lstConfig)
                    {
                        if (string.IsNullOrEmpty(dRow[item["cFieldName"].ToString()].ToString()) == false)
                        {
                            strObject.AppendFormat("\"{0}\":{1},", item["cFieldName"].ToString(), GetValueByDataType(dRow[item["cFieldName"].ToString()].ToString(), dtData.Columns[item["cFieldName"].ToString()].DataType));
                        }
                    }

                    strJson.Append(strObject.ToString().Substring(0, strObject.Length - 1));
                }
                strJson.Append("}");
            }
            else //多条数据
            {
                StringBuilder strArray = new StringBuilder();
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow dRow = dtData.Rows[i];

                    StringBuilder strObject = new StringBuilder();

                    foreach (var item in lstConfig)
                    {
                        if (string.IsNullOrEmpty(dRow[item["cFieldName"].ToString()].ToString()) == false)
                        {
                            strObject.AppendFormat("\"{0}\":{1},", item["cFieldName"].ToString(), GetValueByDataType(dRow[item["cFieldName"].ToString()].ToString(), dtData.Columns[item["cFieldName"].ToString()].DataType));
                        }
                    }

                    strArray.Append("{" + strObject.ToString().Substring(0, strObject.Length - 1) + "},");
                }

                strJson.AppendFormat("[{0}]", strArray.ToString().Substring(0, strArray.Length - 1));
            }

            return strJson.ToString();
        }

        /// <summary>
        /// 根据数据类型获取相应的Json值
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="tDataType"></param>
        /// <returns></returns>
        private static string GetValueByDataType(string strValue, Type tDataType)
        {
            if (tDataType == typeof(decimal) && string.IsNullOrEmpty(strValue))
            {
                strValue = "\"0\"";
            }
            else if (tDataType == typeof(DateTime) && string.IsNullOrEmpty(strValue) == false)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                //JSON序列化  
                //DataContractJsonSerializer dd = new DataContractJsonSerializer((;
                var serializer1 = new DataContractJsonSerializer(typeof(DateTime));
                var stream = new MemoryStream();
                serializer1.WriteObject(stream, Convert.ToDateTime(strValue));

                byte[] dataBytes = new byte[stream.Length];

                stream.Position = 0;

                stream.Read(dataBytes, 0, (int)stream.Length);

                strValue = Encoding.UTF8.GetString(dataBytes);
                //strValue = serializer.Serialize(Convert.ToDateTime(strValue));
            }
            else
            {
                strValue = string.Format("\"{0}\"", strValue);
            }
            return strValue;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="dtConfig">配置Table</param>
        /// <param name="dtData">数据Table</param>
        /// <param name="strCardSection">所在位置</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public static bool CheckData(DataTable dtConfig, DataTable dtData, string strCardSection, ref string errorMsg)
        {
            List<DataRow> lstConfig = dtConfig.Select(string.Format("bMain=1 And bIsNull=1 And cCardSection='{0}'", strCardSection), "iOrder").ToList();
            if (dtData.TableName.Equals("Head")) //表头
            {
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow dRow = dtData.Rows[i];
                    foreach (var item in lstConfig)
                    {
                        if (string.IsNullOrEmpty(dRow[item["cFieldName"].ToString()].ToString()))
                        {
                            errorMsg = string.Format("{0}不能为空", item["cCaption"].ToString());
                            return false;
                        }
                    }
                }
            }
            else //表体
            {
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow dRow = dtData.Rows[i];
                    foreach (var item in lstConfig)
                    {
                        if (string.IsNullOrEmpty(dRow[item["cFieldName"].ToString()].ToString()))
                        {
                            errorMsg = string.Format("第{0}行[{1}]不能为空", (i + 1).ToString(), item["cCaption"].ToString());
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 默认值设置
        /// </summary>
        /// <param name="dtConfig">配置Table</param>
        /// <param name="strCardSection">所在位置</param>
        /// <param name="dDataRow">数据Table</param>
        public static void SetDefaultValue(DataTable dtConfig, string strCardSection, DataRow dDataRow)
        {
            List<DataRow> lstConfig = dtConfig.Select(string.Format("IsNull(cDefaultValue,'')<>'' And cCardSection='{0}'", strCardSection), "iOrder desc").ToList();

            foreach (var item in lstConfig)
            {
                dDataRow.BeginEdit();
                dDataRow[item["cFieldName"].ToString()] = item["cDefaultValue"].ToString();
                dDataRow.EndEdit();
            }
        }


        /// <summary>
        /// 公式计算
        /// </summary>
        /// <param name="dtConfig">配置Table</param>
        /// <param name="dDataRow">数据行</param>
        /// <param name="strCardSection">所在位置</param>
        /// <param name="strFileName">变更字段</param>
        public static void DataRule(DataTable dtConfig, DataRow dDataRow, string strCardSection, string strFileName)
        {
            var dRows = dtConfig.Select(string.Format("cCardSection='{0}' and cDataRule like '%{1}%'", strCardSection, strFileName));

            foreach (var item in dRows)
            {
                dDataRow.BeginEdit();
                dDataRow[item["cFieldName"].ToString()] = DataRulCompute(item["cDataRule"].ToString(), dDataRow);
                dDataRow.EndEdit();
            }
        }


        /// <summary>
        /// 公式计算
        /// </summary>
        /// <param name="strDataRule">计算公式</param>
        /// <param name="dDataRow">取值数据行</param>
        /// <returns>返回计算后值</returns>
        private static object DataRulCompute(string strDataRule, DataRow dDataRow)
        {
            string strComputeRule = strDataRule;
            MatchCollection match = Regex.Matches(strDataRule, @"\[.*?\]");
            for (int i = 0; i < match.Count; i++)
            {
                string s = match[i].Value;
                string strFileName = s.Substring(1, s.Length - 2);
                if (string.IsNullOrEmpty(dDataRow[strFileName].ToString()) == false)
                {
                    strComputeRule = strComputeRule.Replace(s, dDataRow[strFileName].ToString());
                }
                else
                {
                    return 0;
                }
            }
            return new DataTable().Compute(strComputeRule, "");
        }

        /// <summary>
        /// 自定义列表单元格汇总方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="toolTip1"></param>
        public static void CusGridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e, ToolTip toolTip1)
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
}
