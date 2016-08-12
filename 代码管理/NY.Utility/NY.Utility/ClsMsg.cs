using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using System;

namespace NY.Utility
{
    /// <summary>
    /// Author:xxp
    /// Remark:消息类，用于显示各类信息
    /// CreateTime:20150609
    /// </summary>
    public class ClsMsg
    {
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowInfoMsg(string strTxt)
        {
            XtraMessageBox.Show(strTxt, "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowInfoMsg(IWin32Window owner, string strTxt)
        {
            XtraMessageBox.Show(owner, strTxt, "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// 警告提示信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowWarningEmptyMsg(string strTxt)
        {
            XtraMessageBox.Show(string.Format("{0}不能为空！", strTxt), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 警告提示信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowWarningEmptyMsg(
        IWin32Window owner, string strTxt)
        {
            XtraMessageBox.Show(owner, string.Format("{0}不能为空！", strTxt), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        /// <summary>
        /// 警告提示信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowWarningMsg(string strTxt)
        {
            XtraMessageBox.Show(strTxt, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        public static void ShowErrMsg(string strTxt)
        {
            XtraMessageBox.Show(strTxt, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        ///  选择信息
        /// </summary>
        /// <param name="strTxt">信息说明</param>
        /// <returns>返回Yes||No</returns>
        public static DialogResult ShowQuestionMsg(string strTxt)
        {
            return XtraMessageBox.Show(strTxt, "选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
