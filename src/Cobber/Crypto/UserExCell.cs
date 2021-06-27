using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Cobber
{
    /// <summary>
    /// Excel的单元格
    /// </summary>
    public class ExCell
    {
        /// <summary>
        /// 初始化构造
        /// </summary>
        public ExCell()
        {
            Comment = new List<string>();
        }
        /// <summary>
        /// 单元格列名
        /// </summary>
        public string CName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 单元格行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 单元格列
        /// </summary>
        public int Col { get; set; }
        /// <summary>
        /// 单元格内容
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public List<string> Comment { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        public string Error { get { return string.Format("*{0}", string.Join("\n*", Comment)); } }
        /// <summary>
        /// 验证通过
        /// </summary>
        public bool IsValid { get { return Comment.Count() == 0; } }
        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ExCell AddError(string message)
        {
            Comment.Add(string.Format(message, CName));
            return this;
        }
        /// <summary>
        /// 添加错误
        /// </summary>
        /// <returns></returns>
        public ExCell AddError(string message, params object[] args)
        {
            Comment.Add(string.Format(message, args));
            return this;
        }
        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ExCell AddErrors(params string[] message)
        {
            Comment.AddRange(message);
            return this;
        }
        /// <summary>
        /// 复制单元格
        /// </summary>
        public ExCell Clone()
        {
            return new ExCell
            {
                CName = CName,
                Col = Col,
                PName = PName,
                Value = string.Empty,
            };
        }
    }

    /// <summary>
    /// Excel行
    /// </summary>
    public class ExRow : ExRow<dynamic>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="model"></param>
        public ExRow(object model) : base(model) { }
    }
    /// <summary>
    /// Excel行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExRow<T>
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        public ExRow(T model)
        {
            Cells = new Dictionary<string, ExCell>();
            Model = model;
        }
        /// <summary>
        /// 单元格索引器
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public ExCell this[string pName] { get { return Cells[pName]; } set { Cells[pName] = value; } }
        /// <summary>
        /// 单元格[PName,ExCell]
        /// </summary>
        public virtual Dictionary<string, ExCell> Cells { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public virtual T Model { get; set; }
        /// <summary>
        /// 验证通过
        /// </summary>
        public virtual bool IsValid { get { return Cells.All(s => s.Value.IsValid); } }
        /// <summary>
        /// 标记行格
        /// </summary>
        public virtual ExCell TagCell { get { return Cells.FirstOrDefault().Value; } }
        /// <summary>
        /// 错误数字
        /// </summary>
        public virtual int ErrorCount { get { return Cells.Count(s => !s.Value.IsValid); } }
        /// <summary>
        /// 添加行错误
        /// </summary>
        /// <param name="msg"></param>
        public virtual void AddError(string msg)
        {
            TagCell.AddError(msg);
        }
        /// <summary>
        /// 是否通过
        /// </summary>
        /// <returns></returns>
        public virtual bool Valid()
        {
            return IsValid;
        }
        /// <summary>
        /// 添加验证
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="validFunc"></param>
        /// <returns></returns>
        public virtual ExRow<T> AddValid(string pName, Func<ExCell, ExRow<T>, bool> validFunc)
        {
            var ex = Cells[pName];
            validFunc(ex, this);
            return this;
        }
        /// <summary>
        /// 添加验证
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="validFunc"></param>
        /// <returns></returns>
        public virtual ExRow<T> AddValid(string pName, Action<ExCell, ExRow<T>> validFunc)
        {
            var ex = Cells[pName];
            validFunc(ex, this);
            return this;
        }
        /// <summary>
        /// 添加验证
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="validFunc"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual ExRow<T> AddValid<TArgs>(string pName, Func<ExCell, ExRow<T>, TArgs, bool> validFunc, TArgs args)
        {
            var ex = Cells[pName];
            validFunc(ex, this, args);
            return this;
        }
        /// <summary>
        /// 添加ExCell
        /// </summary>
        /// <returns></returns>
        public virtual ExRow<T> Add(ExCell cell)
        {
            Cells.Add(cell.PName, cell);
            return this;
        }
    }
}
