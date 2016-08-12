using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace NY.Utility
{
    /// <summary>
    /// 网格控件选择列
    /// </summary>
    public class GridCheckBoxMarks
    {
        #region 公开事件
        /// <summary>勾选框点击事件</summary>
        public event EventHandler Click;
        #endregion

        #region 私有字段
        /// <summary>需要添加选择列的网格控件</summary>
        protected GridView gridView;
        /// <summary>选中的所有行</summary>
        protected ArrayList selection;
        private GridColumn column;
        private RepositoryItemCheckEdit edit;
        private const int CheckboxIndent = 4;
        private bool isCheckBoxHeader = true;
        private string headerName = "Mark";
        #endregion

        #region 构造与初始化
        /// <summary>构造函数</summary>
        public GridCheckBoxMarks()
        {
            selection = new ArrayList();
        }
        /// <summary>构造函数</summary>
        /// <param name="view">需要添加选择列的网格控件</param>
        public GridCheckBoxMarks(GridView view)
            : this()
        {
            View = view;
        }
        #endregion

        #region 公开属性
        /// <summary>设置或获取需要添加选择列的网格控件</summary>
        public GridView View
        {
            get
            {
                return gridView;
            }
            set
            {
                if (gridView != value)
                {
                    Detach();
                    Attach(value);
                }
            }
        }
        /// <summary>获取选择列</summary>
        public GridColumn CheckMarkColumn
        {
            get
            {
                return column;
            }
        }
        /// <summary>获取选中的行数</summary>
        public int SelectedCount
        {
            get
            {
                return selection.Count;
            }
        }
        #endregion

        #region 公开方法
        /// <summary>获取所有选中行</summary>
        /// <returns>选中行集合</returns>
        public ArrayList GetSelectedRows()
        {
            return selection;
        }
        /// <summary>根据指定索引获取选中行</summary>
        /// <param name="index">行索引</param>
        /// <returns>选中行</returns>
        public object GetSelectedRow(int index)
        {
            return selection[index];
        }
        /// <summary>获取选中行的索引</summary>
        /// <param name="row">选中行</param>
        /// <returns>对应索引</returns>
        public int GetSelectedIndex(object row)
        {
            return selection.IndexOf(row);
        }
        /// <summary>清除所有选择状态</summary>
        public void ClearSelection()
        {
            selection.Clear();
            Invalidate();
        }
        /// <summary>选中所有行</summary>
        public void SelectAll()
        {
            if (isCheckBoxHeader)
            {
                selection.Clear();
                // fast (won't work if the grid is filtered)
                //if(_view.DataSource is ICollection)
                //    selection.AddRange(((ICollection)_view.DataSource));
                //else
                // slow:
                for (int i = 0; i < gridView.DataRowCount; i++)
                {
                    selection.Add(gridView.GetRow(i));
                }
                Invalidate();
            }
        }
        /// <summary>选中或取消指定组</summary>
        /// <param name="rowHandle">行索引</param>
        /// <param name="select">是否选中</param>
        public void SelectGroup(int rowHandle, bool select)
        {
            if (IsGroupRowSelected(rowHandle) && select)
            {
                return;
            }
            for (int i = 0; i < gridView.GetChildRowCount(rowHandle); i++)
            {
                int childRowHandle = gridView.GetChildRowHandle(rowHandle, i);
                if (gridView.IsGroupRow(childRowHandle))
                {
                    SelectGroup(childRowHandle, select);
                }
                else
                {
                    SelectRow(childRowHandle, select, false);
                }
            }
            Invalidate();
        }
        /// <summary>选中或取消指定行</summary>
        /// <param name="rowHandle">行索引</param>
        /// <param name="select">是否选中</param>
        public void SelectRow(int rowHandle, bool select)
        {
            SelectRow(rowHandle, select, true);
        }
        /// <summary>反转选择状态</summary>
        /// <param name="rowHandle">索引</param>
        public void InvertRowSelection(int rowHandle)
        {
            if (Click != null)
            {
                Click(this, new EventArgs());
            }
            if (View.IsDataRow(rowHandle))
            {
                SelectRow(rowHandle, !IsRowSelected(rowHandle));
            }
            if (View.IsGroupRow(rowHandle))
            {
                SelectGroup(rowHandle, !IsGroupRowSelected(rowHandle));
            }
        }
        /// <summary>判断组是否选中</summary>
        /// <param name="rowHandle">索引</param>
        /// <returns>选中返回true，否则返回false</returns>
        public bool IsGroupRowSelected(int rowHandle)
        {
            for (int i = 0; i < gridView.GetChildRowCount(rowHandle); i++)
            {
                int row = gridView.GetChildRowHandle(rowHandle, i);
                if (gridView.IsGroupRow(row))
                {
                    if (!IsGroupRowSelected(row))
                    {
                        return false;
                    }
                }
                else if (!IsRowSelected(row))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>判断行是否选中</summary>
        /// <param name="rowHandle">索引</param>
        /// <returns>选中返回true，否则返回false</returns>
        public bool IsRowSelected(int rowHandle)
        {
            if (gridView.IsGroupRow(rowHandle))
            {
                return IsGroupRowSelected(rowHandle);
            }
            object row = gridView.GetRow(rowHandle);
            return GetSelectedIndex(row) != -1;
        }
        /// <summary>设置选择行名称和表头是否显示全选控件</summary>
        /// <param name="isCheckBoxHeader">是否显示全选控件，默认显示</param>
        /// <param name="headerName">选择列表头名称，在isCheckBoxHeader为false时生效</param>
        public void SetColumnHeader(bool isCheckBoxHeader = true, string headerName = "Mark")
        {
            this.isCheckBoxHeader = isCheckBoxHeader;
            this.headerName = headerName;
        }
        #endregion

        #region 私有方法
        /// <summary>添加CheckBox选择列</summary>
        /// <param name="view"></param>
        protected virtual void Attach(GridView view)
        {
            if (view == null)
            {
                return;
            }
            selection.Clear();
            this.gridView = view;
            view.BeginUpdate();
            try
            {
                edit = view.GridControl.RepositoryItems.Add("CheckEdit") as RepositoryItemCheckEdit;
                edit.Name = "edit";
                column = view.Columns.Insert(0);
                column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                column.Name = "CheckMarkColumn";
                column.Visible = true;
                column.VisibleIndex = 0;
                column.FieldName = "CheckMarkSelection";
                column.Caption = headerName;
                column.OptionsColumn.ShowCaption = false;
                column.OptionsColumn.AllowEdit = false;
                column.OptionsColumn.AllowSize = false;
                column.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
                column.Width = GetCheckBoxWidth();
                column.ColumnEdit = edit;
                column.ColumnEditName = "edit";

                view.Click += new EventHandler(View_Click);
                view.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
                view.CustomDrawGroupRow += new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
                view.CustomUnboundColumnData += new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
                view.DataSourceChanged += new EventHandler(view_DataSourceChanged);
                view.KeyDown += new KeyEventHandler(view_KeyDown);
                view.RowStyle += new RowStyleEventHandler(view_RowStyle);
            }
            finally
            {
                view.EndUpdate();
            }
        }
        /// <summary> 释放CheckBox选择列</summary>
        protected virtual void Detach()
        {
            if (gridView == null)
            {
                return;
            }
            if (column != null)
            {
                column.Dispose();
            }
            if (edit != null)
            {
                gridView.GridControl.RepositoryItems.Remove(edit);
                edit.Dispose();
            }

            gridView.Click -= new EventHandler(View_Click);
            gridView.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
            gridView.CustomDrawGroupRow -= new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
            gridView.CustomUnboundColumnData -= new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
            gridView.KeyDown -= new KeyEventHandler(view_KeyDown);
            gridView.RowStyle -= new RowStyleEventHandler(view_RowStyle);

            gridView = null;
        }

        private int GetCheckBoxWidth()
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            int width = 0;
            GraphicsInfo.Default.AddGraphics(null);
            try
            {
                width = info.CalcBestFit(GraphicsInfo.Default.Graphics).Width;
            }
            finally
            {
                GraphicsInfo.Default.ReleaseGraphics();
            }
            return width + CheckboxIndent * 2;
        }

        private void DrawCheckBox(Graphics g, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }

        private void Invalidate()
        {
            gridView.CloseEditor();
            gridView.BeginUpdate();
            gridView.EndUpdate();
        }

        private void SelectRow(int rowHandle, bool select, bool invalidate)
        {
            if (IsRowSelected(rowHandle) == select)
            {
                return;
            }
            object row = gridView.GetRow(rowHandle);
            if (select)
            {
                selection.Add(row);
            }
            else
            {
                selection.Remove(row);
            }
            if (invalidate)
            {
                Invalidate();
            }
        }

        private void view_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column == CheckMarkColumn)
            {
                if (e.IsGetData)
                {
                    e.Value = IsRowSelected(View.GetRowHandle(e.ListSourceRowIndex));
                }
                else
                {
                    SelectRow(View.GetRowHandle(e.ListSourceRowIndex), (bool)e.Value);
                }
            }
        }

        private void view_KeyDown(object sender, KeyEventArgs e)
        {
            if (View.FocusedColumn != column || e.KeyCode != Keys.Space)
            {
                return;
            }
            InvertRowSelection(View.FocusedRowHandle);
        }

        private void View_Click(object sender, EventArgs e)
        {
            GridHitInfo info;
            Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
            info = gridView.CalcHitInfo(pt);
            if (info.Column == column)
            {
                if (info.InColumn)
                {
                    if (SelectedCount == gridView.DataRowCount)
                    {
                        ClearSelection();
                    }
                    else
                    {
                        SelectAll();
                    }
                }
                if (info.InRowCell)
                {
                    InvertRowSelection(info.RowHandle);
                }
            }
            if (info.InRow && gridView.IsGroupRow(info.RowHandle) && info.HitTest != GridHitTest.RowGroupButton)
            {
                InvertRowSelection(info.RowHandle);
            }
        }

        private void View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (isCheckBoxHeader)
            {
                if (e.Column == column)
                {
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    DrawCheckBox(e.Graphics, e.Bounds, SelectedCount == gridView.DataRowCount);
                    e.Handled = true;
                }
            }
        }

        private void View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info;
            info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            info.GroupText = "         " + info.GroupText.TrimStart();
            e.Info.Paint.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds);
            e.Painter.DrawObject(e.Info);

            Rectangle r = info.ButtonBounds;
            r.Offset(r.Width + CheckboxIndent * 2 - 1, 0);
            DrawCheckBox(e.Graphics, r, IsGroupRowSelected(e.RowHandle));
            e.Handled = true;
        }

        private void view_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (IsRowSelected(e.RowHandle))
            {
                //e.Appearance.BackColor = Color.Red;
                //e.Appearance.ForeColor = SystemColors.HighlightText;
            }
        }

        private void view_DataSourceChanged(object sender, EventArgs e)
        {
            ClearSelection();
        }
        #endregion
    }
}
