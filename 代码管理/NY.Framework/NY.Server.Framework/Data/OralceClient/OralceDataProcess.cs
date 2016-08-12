using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NY.Server.Data
{
    /// <summary>
    /// Author:xxp
    /// Desc:数据库处理类
    /// CreateTime:20160808
    /// </summary>
    public class DataProcess
    {
        /// <summary>
        /// 获取OracleParameter集合
        /// </summary>
        /// <param name="dtFilter">dtFilter Columns{cField(字段),cValues(值),cDataType(数据类型),cValues1(值1),cCompare(过滤符号)}</param>
        /// <param name="strWhere">查询条件sql语句</param>
        /// <returns>返回Oracle参数</returns>
        public List<OracleParameter> GetOracleParameterList(DataTable dtFilter, ref StringBuilder strWhere)
        {
            //xxp 20150616 修改Oracle过滤条件处理，将原有的使用@符号+参数更改为:符号+参数
            List<OracleParameter> listParameter = new List<OracleParameter>();
            if (dtFilter != null)  //xxp 20150801 添加过滤条件对象允许传入NULL;
            {
                for (int i = 0; i < dtFilter.Rows.Count; i++)
                {
                    DataRow dRow = dtFilter.Rows[i];
                    if (dRow["cField"].ToString().Trim().Equals("") == false)
                    {
                        string cCompare = "=";
                        string strDataType = dRow["cDataType"].ToString().ToLower();
                        if (dRow["cCompare"].ToString().Equals("") == false)
                            cCompare = dRow["cCompare"].ToString().ToLower();

                        //xxp 20150901 添加传入使用 in和NOT in 适用int类型
                        if (cCompare.Contains("in") && strDataType.Equals("int"))
                        {
                            List<int> lst = dRow["cValues"] as List<int>;
                            string idStr = InsertParameters(lst.ToArray(), dRow["cField"].ToString(), ref listParameter);
                            strWhere.AppendFormat(" AND {0} {1}({2})", dRow["cField"].ToString(), cCompare, idStr);
                            continue;
                        }
                        else if (cCompare.Contains("in") && strDataType.Equals("string"))  //xxp 20151103 添加传入string类型
                        {
                            List<string> lst = dRow["cValues"] as List<string>;
                            string idStr = InsertParameters(lst.ToArray(), dRow["cField"].ToString(), ref listParameter);
                            strWhere.AppendFormat(" AND {0} {1}({2})", dRow["cField"].ToString(), cCompare, idStr);
                            continue;
                        }

                        OracleParameter parameter = new OracleParameter();
                        //parameter.ParameterName = string.Format("@" + dRow["cField"].ToString());
                        parameter.ParameterName = string.Format(":" + dRow["cField"].ToString());
                        parameter.Value = SetValues(dRow, cCompare);

                        SetDbType(strDataType, parameter);
                        listParameter.Add(parameter);

                        //strWhere.AppendFormat(" And {0}{1}@{0}", dRow["cField"].ToString(), cCompare.Contains("like") ? string.Format(" {0} ", cCompare) : cCompare);
                        if (dRow["cValues1"].ToString().Trim().Equals(""))

                            strWhere.AppendFormat(" And {0}{1}", dRow["cField"].ToString(), SetFormat(strDataType, dRow["cField"].ToString(), cCompare));
                        else
                        {
                            parameter = new OracleParameter();
                            //parameter.ParameterName = string.Format("@{0}1", dRow["cField"].ToString());
                            parameter.ParameterName = string.Format(":{0}1", dRow["cField"].ToString());
                            parameter.Value = dRow["cValues1"];
                            SetDbType(strDataType, parameter);
                            listParameter.Add(parameter);
                            //strWhere.AppendFormat(" And ({0}>=@{0} and {0}<=@{0}1)", dRow["cField"].ToString());
                            strWhere.AppendFormat(" And ({0}>={1} and {0}<={2})", dRow["cField"].ToString(), SetFormat1(strDataType, dRow["cField"].ToString()), SetFormat1(strDataType, dRow["cField"].ToString() + "1"));
                        }
                    }
                }
            }
            return listParameter;
        }

        /// <summary>
        /// Author:xxp
        /// Reamark:获取OracleParameter集合 用于自定义字段查询(例如："cInvCode"=11);
        /// CreateTime:20160114
        /// </summary>
        /// <param name="dtFilter">dtFilter Columns{cField(字段),cValues(值),cDataType(数据类型),cValues1(值1),cCompare(过滤符号)}</param>
        /// <param name="strWhere">查询条件sql语句</param>
        /// <returns>返回Oracle参数</returns>
        public List<OracleParameter> GetOracleParameterList1(DataTable dtFilter, ref StringBuilder strWhere)
        {
            //xxp 20150616 修改Oracle过滤条件处理，将原有的使用@符号+参数更改为:符号+参数
            List<OracleParameter> listParameter = new List<OracleParameter>();
            if (dtFilter != null)  //xxp 20150801 添加过滤条件对象允许传入NULL;
            {
                for (int i = 0; i < dtFilter.Rows.Count; i++)
                {
                    DataRow dRow = dtFilter.Rows[i];
                    if (dRow["cField"].ToString().Trim().Equals("") == false)
                    {
                        string cCompare = (dRow["cCompare"] == DBNull.Value || dRow["cCompare"].ToString().Equals("")) ? "=" : dRow["cCompare"].ToString().ToLower();
                        string strDataType = dRow["cDataType"].ToString().ToLower();

                        //xxp 20150901 添加传入使用 in和NOT in 适用int类型
                        if (cCompare.Contains("in") && strDataType.Equals("int"))
                        {
                            List<int> lst = dRow["cValues"] as List<int>;
                            string idStr = InsertParameters(lst.ToArray(), dRow["cField"].ToString(), ref listParameter);
                            strWhere.AppendFormat(" AND \"{0}\" {1}({2})", dRow["cField"].ToString(), cCompare, idStr);
                            continue;
                        }
                        else if (cCompare.Contains("in") && strDataType.Equals("string"))  //xxp 20151103 添加传入string类型
                        {
                            List<string> lst = dRow["cValues"] as List<string>;
                            string idStr = InsertParameters(lst.ToArray(), dRow["cField"].ToString(), ref listParameter);
                            strWhere.AppendFormat(" AND \"{0}\" {1}({2})", dRow["cField"].ToString(), cCompare, idStr);
                            continue;
                        }

                        OracleParameter parameter = new OracleParameter();
                        //parameter.ParameterName = string.Format("@" + dRow["cField"].ToString());
                        parameter.ParameterName = string.Format(":" + dRow["cField"].ToString());
                        parameter.Value = SetValues(dRow, cCompare);

                        SetDbType(strDataType, parameter);
                        listParameter.Add(parameter);

                        //strWhere.AppendFormat(" And {0}{1}@{0}", dRow["cField"].ToString(), cCompare.Contains("like") ? string.Format(" {0} ", cCompare) : cCompare);
                        if (dRow["cValues1"].ToString().Trim().Equals(""))

                            strWhere.AppendFormat(" And \"{0}\"{1}", dRow["cField"].ToString(), SetFormat(strDataType, dRow["cField"].ToString(), cCompare));
                        else
                        {
                            parameter = new OracleParameter();
                            //parameter.ParameterName = string.Format("@{0}1", dRow["cField"].ToString());
                            parameter.ParameterName = string.Format(":{0}1", dRow["cField"].ToString());
                            parameter.Value = dRow["cValues1"];
                            SetDbType(strDataType, parameter);
                            listParameter.Add(parameter);
                            //strWhere.AppendFormat(" And ({0}>=@{0} and {0}<=@{0}1)", dRow["cField"].ToString());
                            strWhere.AppendFormat(" And (\"{0}\">={1} and \"{0}\"<={2})", dRow["cField"].ToString(), SetFormat1(strDataType, dRow["cField"].ToString()), SetFormat1(strDataType, dRow["cField"].ToString() + "1"));
                        }
                    }
                }
            }
            return listParameter;
        }

        private string SetFormat(string strDataType, string strField, string strCompare)
        {
            return string.Format("{0}{1}", strCompare.Contains("like") ? string.Format(" {0} ", strCompare) : strCompare, SetFormat1(strDataType, strField));
        }

        private string SetFormat1(string strDataType, string strField)
        {
            switch (strDataType)
            {
                case "date":
                    return string.Format("to_date(:{0},'yyyy-MM-dd')", strField);
                case "datetime":
                    return string.Format("to_date(:{0},'yyyy-MM-dd HH24:mi:ss')", strField);
                default:
                    return string.Format(":{0}", strField);
            }
        }

        private object SetValues(DataRow dRow, string cCompare)
        {
            switch (cCompare)
            {
                case "like":
                    return string.Format("%{0}%", dRow["cValues"]);
                case "beforelike":
                    return string.Format("%{0}", dRow["cValues"]);
                case "afterlike":
                    return string.Format("{0}%", dRow["cValues"]);
                default:
                    return dRow["cValues"];
            }
        }

        private void SetDbType(String strDataType, OracleParameter parameter)
        {
            // if (dRow["cDataType"].ToString().Equals("") == false)
            //  {
            switch (strDataType)
            {
                case "string":
                    parameter.OracleDbType = OracleDbType.NVarchar2;
                    break;
                case "int":
                    parameter.OracleDbType = OracleDbType.Int64;
                    break;
                case "float":
                case "double":
                    parameter.OracleDbType = OracleDbType.Double;
                    break;
                case "date":
                    parameter.OracleDbType = OracleDbType.Varchar2;
                    break;
                case "bit":
                    parameter.OracleDbType = OracleDbType.Char;
                    break;
                case "datetime":
                    parameter.OracleDbType = OracleDbType.Varchar2;
                    break;
                default:
                    parameter.OracleDbType = OracleDbType.NVarchar2;
                    break;
            }
            // }
        }

        /// <summary>
        /// 把ID集合转换成字符串，用“,”隔开
        /// </summary>
        /// <param name="lst">ID集合</param>
        /// <returns>返回转换后的字符串</returns>
        public static string LstParsestring(List<int> lst)
        {
            string str = string.Join(",", lst);
            return str;
        }

        /// <summary>
        /// xxp 20150801添加用于 in OracleParameter使用
        /// 1 - 鉴于INT独特的命名使用uniqueParName数组，创建一个的OracleParameter为每一个和值assigin，
        /// 2 - 将创建成裁判名单的OracleParameter。
        /// 3 - 返回字符串被用来连接到主SQL
        /// </summary>
        /// <param name="orclParameters"></param>
        /// <param name="lsIds"></param>
        /// <param name="uniqueParName"></param>
        /// <returns></returns>
        public static string InsertParameters(int[] lsIds, string uniqueParName, ref List<OracleParameter> orclParameters)
        {
            string strParametros = string.Empty;
            for (int i = 0; i <= lsIds.Length - 1; i++)
            {
                strParametros += i == 0 ? ":" + uniqueParName + i : ", :" + uniqueParName + i;

                OracleParameter param = new OracleParameter(":" + uniqueParName + i.ToString(), OracleDbType.Int64);
                param.Value = lsIds[i];
                orclParameters.Add(param);
            }
            return strParametros;
        }

        /// <summary>
        /// xxp 20151031 添加用于 in OracleParameter使用
        /// 1 - 鉴于INT独特的命名使用uniqueParName数组，创建一个的OracleParameter为每一个和值assigin，
        /// 2 - 将创建成裁判名单的OracleParameter。
        /// 3 - 返回字符串被用来连接到主SQL
        /// </summary>
        /// <param name="orclParameters"></param>
        /// <param name="lsIds"></param>
        /// <param name="uniqueParName"></param>
        /// <returns></returns>
        public static string InsertParameters(string[] lsIds, string uniqueParName, ref List<OracleParameter> orclParameters)
        {
            string strParametros = string.Empty;
            for (int i = 0; i <= lsIds.Length - 1; i++)
            {
                strParametros += i == 0 ? ":" + uniqueParName + i : ", :" + uniqueParName + i;

                OracleParameter param = new OracleParameter(":" + uniqueParName + i.ToString(), OracleDbType.NVarchar2);
                param.Value = lsIds[i];
                orclParameters.Add(param);
            }
            return strParametros;
        }


        /// <summary>
        /// 创建DataTable过滤对象Columns{cField,cValues,cDataType,cValues1,cCompare}
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable CreateFilterTable()
        {
            var dtFilter = new DataTable();
            dtFilter.Columns.AddRange(new DataColumn[] { 
             new DataColumn("cField",typeof(System.String)),
             new DataColumn("cValues",typeof(System.Object)),
             new DataColumn("cDataType",typeof(System.String)),
             new DataColumn("cValues1",typeof(System.Object)),
             new DataColumn("cCompare",typeof(System.String))
            });
            return dtFilter;
        }
    }
}
