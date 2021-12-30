using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.Dabber
{
    /// <summary>
    /// SQL表达式调用
    /// </summary>
    public static class SqlScriptExpressionCaller
    {
        /// <summary>
        /// 解析SQL
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveSql(this Expression exp)
        {
            if (exp is LambdaExpression lambdaExp)
            {
                return ResolveBooleanSql(lambdaExp.Body);
            }
            if (exp is BinaryExpression binaryExp)
            {
                return ResolveBinarySql(binaryExp);
            }
            if (exp is MemberExpression memberExp)
            {
                return ResolveMemberSql(memberExp);
            }
            if (exp is ConstantExpression constantExp)
            {
                return ResolveConstantSql(constantExp);
            }
            if (exp is UnaryExpression unaryExp)
            {
                return ResolveUnarySql(unaryExp);
            }
            return "";
        }
        /// <summary>
        /// 布尔运算符
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveBooleanSql(Expression exp)
        {
            if (exp == null) { return null; }
            // 关于bool类型特殊处理
            if (exp is BinaryExpression binaryExp)
            {
                return ResolveBinarySql(binaryExp);
            }
            if (exp is MemberExpression memberExp) // n.isDelete
            {
                return ResolveMemberSql(memberExp) + "=1";
            }
            if (exp is UnaryExpression unaryExp) //!n.isDelete
            {
                return ResolveUnarySql(unaryExp) + "<>1";
            }
            if (exp is ConstantExpression constantExp) //
            {
                var str = ResolveConstantSql(constantExp);
                return str == "1" ? "1==1" : "1!=1";
            }
            return "";
        }
        /// <summary>
        /// 一元运算解析
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveUnarySql(UnaryExpression exp)
        {
            return ResolveSql(exp.Operand);
        }
        /// <summary>
        /// 恒值解析
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveConstantSql(ConstantExpression exp)
        {
            object vaule = exp.Value;
            if (vaule == null) { return "NULL"; }
            if (vaule is string valStr) { return $"'{valStr}'"; }
            if (vaule is DateTime valDt) { return valDt.ToString("'yyyy-MM-dd HH:mm:ss'"); }
            if (vaule is Boolean valBool) { return valBool ? "1" : "0"; }
            return vaule.ToString();
        }
        /// <summary>
        /// 二元运算
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveBinarySql(BinaryExpression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    {
                        string left = ResolveBooleanSql(exp.Left);
                        string oper = GetOperStr(exp.NodeType);
                        string right = ResolveBooleanSql(exp.Right);
                        return left + oper + right;
                    }
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Invoke:
                case ExpressionType.Lambda:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.ListInit:
                case ExpressionType.MemberAccess:
                case ExpressionType.MemberInit:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.Parameter:
                case ExpressionType.Power:
                case ExpressionType.Quote:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.Assign:
                case ExpressionType.Block:
                case ExpressionType.DebugInfo:
                case ExpressionType.Decrement:
                case ExpressionType.Dynamic:
                case ExpressionType.Default:
                case ExpressionType.Extension:
                case ExpressionType.Goto:
                case ExpressionType.Increment:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.RuntimeVariables:
                case ExpressionType.Loop:
                case ExpressionType.Switch:
                case ExpressionType.Throw:
                case ExpressionType.Try:
                case ExpressionType.Unbox:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.TypeEqual:
                case ExpressionType.OnesComplement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                default:
                    {
                        string left = ResolveSql(exp.Left);
                        string oper = GetOperStr(exp.NodeType);
                        string right = ResolveSql(exp.Right);
                        if (right == "NULL")
                        {
                            if (oper == "=")
                            {
                                oper = " is ";
                            }
                            else
                            {
                                oper = " is not ";
                            }
                        }
                        return left + oper + right;
                    }
            }
        }
        /// <summary>
        /// 成员SQL
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string ResolveMemberSql(MemberExpression exp)
        {
            return exp.Member.Name;
        }
        /// <summary>
        /// 连接符
        /// </summary>
        /// <param name="e_type"></param>
        /// <returns></returns>
        public static string GetOperStr(ExpressionType e_type)
        {
            switch (e_type)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return "";
        }
    }
}
