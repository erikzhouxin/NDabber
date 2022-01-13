namespace System.Data.Dabber
{
    public static partial class SqlMapper
    {
        /// <summary>
        /// Permits specifying certain SqlMapper values globally.
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// 默认情况下禁用单个结果; 在正确检测到选择后防止错误
            /// </summary>
            internal static CommandBehavior AllowedCommandBehaviors { get; set; } = ~CommandBehavior.SingleResult;
            private static CommandBehavior SetAllowedCommandBehaviors(CommandBehavior behavior, bool enabled)
            {
                return enabled ? (AllowedCommandBehaviors |= behavior) : (AllowedCommandBehaviors &= (~behavior));
            }
            /// <summary>
            /// 获取或设置Dapper是否应该使用命令行为。 SingleResult优化
            /// 注意，启用此选项的结果是，可能不会报告第一次选择之后发生的错误  
            /// </summary>
            public static bool UseSingleResultOptimization
            {
                get { return (AllowedCommandBehaviors & CommandBehavior.SingleResult) != 0; }
                set { SetAllowedCommandBehaviors(CommandBehavior.SingleResult, value); }
            }
            /// <summary>
            /// 获取或设置Dapper是否应该使用命令行为。 回转支承的优化 
            /// 注意，在某些DB提供上，这种优化可能会对性能产生不利影响  
            /// </summary>
            public static bool UseSingleRowOptimization
            {
                get { return (AllowedCommandBehaviors & CommandBehavior.SingleRow) != 0; }
                set { SetAllowedCommandBehaviors(CommandBehavior.SingleRow, value); }
            }

            internal static bool DisableCommandBehaviorOptimizations(CommandBehavior behavior, Exception ex)
            {
                if (AllowedCommandBehaviors == (~CommandBehavior.SingleResult) && (behavior & (CommandBehavior.SingleResult | CommandBehavior.SingleRow)) != 0)
                {
                    if (ex.Message.Contains(nameof(CommandBehavior.SingleResult)) || ex.Message.Contains(nameof(CommandBehavior.SingleRow)))
                    {
                        // 有些提供只是允许这些，所以: 在没有它们的情况下再试一次，并停止发布它们
                        SetAllowedCommandBehaviors(CommandBehavior.SingleResult | CommandBehavior.SingleRow, false);
                        return true;
                    }
                }
                return false;
            }
            /// <summary>
            /// 为所有查询指定默认的命令超时时间
            /// </summary>
            public static int? CommandTimeout { get; set; } = null;
            /// <summary>
            /// 指示数据中的空值是被静默忽略(默认)还是主动应用并分配给成员
            /// </summary>
            public static bool ApplyNullValues { get; set; } = false;
            /// <summary>
            /// 列表扩展是否应该用空值参数填充，以防止查询计划饱和? 例如，一个'in @foo'扩展有7、8或9个值，将被发送为一个包含10个值的列表，其中3、2或1个值为空。  
            /// 填充的大小是相对于列表的大小; 150以下的“下10”，500以下的“下50”，1500以下的“下100”，等等。
            /// 注意:如果你的数据库提供程序(或特定的配置)允许null相等(aka "ansi nulls off")，这应该小心处理，因为这可能会改变你的查询的意图; 
            /// 因此，这在默认情况下是禁用的，必须启用。 
            /// </summary>
            public static bool PadListExpansions { get; set; }
            /// <summary>
            /// 如果设置(非负)，当执行整数类型的列表扩展(“where id in @ids”等)时，切换到基于string_split的  
            /// 如果有这么多或更多的元素。 请注意，此特性需要SQL Server 2016 /兼容性级别130(或以上)。
            /// </summary>
            public static int InListStringSplitCount { get; set; } = -1;
        }
    }
}
