using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Mabber
{
    /// <summary>
    /// Cron 表达式抽象类
    /// </summary>
    /// <remarks>主要将 Cron 表达式转换成 OOP 类进行操作</remarks>
    public sealed partial class Crontab
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>禁止外部 new 实例化</remarks>
        private Crontab()
        {
            Parsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();
            Format = CronStringFormat.Default;
        }

        /// <summary>
        /// Cron 字段解析器字典集合
        /// </summary>
        private Dictionary<CrontabFieldKind, List<ICronParser>> Parsers { get; set; }

        /// <summary>
        /// Cron 表达式格式化类型
        /// </summary>
        /// <remarks>禁止运行时更改</remarks>
        public CronStringFormat Format { get; private set; }

        /// <summary>
        /// 解析 Cron 表达式并转换成 <see cref="Crontab"/> 对象
        /// </summary>
        /// <param name="expression">Cron 表达式</param>
        /// <param name="format">Cron 表达式格式化类型</param>
        /// <returns><see cref="Crontab"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public static Crontab Parse(string expression, CronStringFormat format = CronStringFormat.Default)
        {
            // 处理 Macro 表达式
            if (expression.StartsWith("@"))
            {
                return expression switch
                {
                    "@secondly" => Secondly,
                    "@minutely" => Minutely,
                    "@hourly" => Hourly,
                    "@daily" => Daily,
                    "@monthly" => Monthly,
                    "@weekly" => Weekly,
                    "@yearly" => Yearly,
                    "@workday" => Workday,
                    _ => throw new NotImplementedException(),
                };
            }

            return new Crontab
            {
                Format = format,
                Parsers = ParseToDictionary(expression, format)
            };
        }

        /// <summary>
        /// 解析 Cron Macro 符号并转换成 <see cref="Crontab"/> 对象
        /// </summary>
        /// <param name="macro">Macro 符号</param>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public static Crontab ParseAt(string macro, params object[] fields)
        {
            // 空检查
            if (string.IsNullOrEmpty(macro)) throw new ArgumentNullException(nameof(macro));

            return macro switch
            {
                "@secondly" => SecondlyAt(fields),
                "@minutely" => MinutelyAt(fields),
                "@hourly" => HourlyAt(fields),
                "@daily" => DailyAt(fields),
                "@monthly" => MonthlyAt(fields),
                "@weekly" => WeeklyAt(fields),
                "@yearly" => YearlyAt(fields),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// 解析 Cron 表达式并转换成 <see cref="Crontab"/> 对象
        /// </summary>
        /// <remarks>解析失败返回 default</remarks>
        /// <param name="expression">Cron 表达式</param>
        /// <param name="format">Cron 表达式格式化类型</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab TryParse(string expression, CronStringFormat format = CronStringFormat.Default)
        {
            try
            {
                return Parse(expression, format);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 判断 Cron 表达式是否有效
        /// </summary>
        /// <param name="expression">Cron 表达式</param>
        /// <param name="format">Cron 表达式格式化类型</param>
        /// <returns><see cref="Crontab"/></returns>
        public static bool IsValid(string expression, CronStringFormat format = CronStringFormat.Default)
        {
            return TryParse(expression, format) != null;
        }

        /// <summary>
        /// 获取起始时间下一个发生时间
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <returns><see cref="DateTime"/></returns>
        public DateTime GetNextOccurrence(DateTime baseTime)
        {
            return GetNextOccurrence(baseTime, DateTime.MaxValue);
        }

        /// <summary>
        /// 获取特定时间范围下一个发生时间
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns><see cref="DateTime"/></returns>
        public DateTime GetNextOccurrence(DateTime baseTime, DateTime endTime)
        {
            return InternalGetNextOccurence(baseTime, endTime);
        }

        /// <summary>
        /// 获取特定时间范围所有发生时间
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
        {
            for (var occurrence = GetNextOccurrence(baseTime, endTime);
                 occurrence < endTime;
                 occurrence = GetNextOccurrence(occurrence, endTime))
            {
                yield return occurrence;
            }
        }

        /// <summary>
        /// 计算距离下一个发生时间相差毫秒数
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <returns></returns>
        public double GetSleepMilliseconds(DateTime baseTime)
        {
            // 采用 DateTimeKind.Unspecified 转换当前时间并忽略毫秒之后部分
            var startAt = new DateTime(baseTime.Year
                , baseTime.Month
                , baseTime.Day
                , baseTime.Hour
                , baseTime.Minute
                , baseTime.Second
                , baseTime.Millisecond);

            // 计算总休眠时间
            return (GetNextOccurrence(startAt) - startAt).TotalMilliseconds;
        }

        /// <summary>e
        /// 计算距离下一个发生时间相差时间戳
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <returns></returns>
        public TimeSpan GetSleepTimeSpan(DateTime baseTime)
        {
            return TimeSpan.FromMilliseconds(GetSleepMilliseconds(baseTime));
        }

        /// <summary>
        /// 将 <see cref="Crontab"/> 对象转换成 Cron 表达式字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var paramList = new List<string>();

            // 判断当前 Cron 格式化类型是否包含秒字段域
            if (Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears)
            {
                JoinParsers(paramList, CrontabFieldKind.Second);
            }

            // Cron 常规字段域
            JoinParsers(paramList, CrontabFieldKind.Minute);
            JoinParsers(paramList, CrontabFieldKind.Hour);
            JoinParsers(paramList, CrontabFieldKind.Day);
            JoinParsers(paramList, CrontabFieldKind.Month);
            JoinParsers(paramList, CrontabFieldKind.DayOfWeek);

            // 判断当前 Cron 格式化类型是否包含年字段域
            if (Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears)
            {
                JoinParsers(paramList, CrontabFieldKind.Year);
            }

            // 空格分割并输出
            return string.Join(" ", paramList.ToArray());
        }
    }
    #region // Internal
    /// <summary>
    /// Cron 表达式抽象类
    /// </summary>
    /// <remarks>主要将 Cron 表达式转换成 OOP 类进行操作</remarks>
    public sealed partial class Crontab
    {
        /// <summary>
        /// 解析 Cron 表达式字段并存储其 所有发生值 字符解析器
        /// </summary>
        /// <param name="expression">Cron 表达式</param>
        /// <param name="format">Cron 表达式格式化类型</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        private static Dictionary<CrontabFieldKind, List<ICronParser>> ParseToDictionary(string expression, CronStringFormat format)
        {
            // Cron 表达式空检查
            if (string.IsNullOrEmpty(expression))
            {
                throw new TimeCrontabException("The provided cron string is null, empty or contains only whitespace.");
            }

            // 通过空白符切割 Cron 表达式每个字段域
            var instructions = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // 验证当前 Cron 格式化类型字段数量和表达式字段数量是否一致
            var expectedCount = Constants.ExpectedFieldCounts[format];
            if (instructions.Length > expectedCount)
            {
                throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too many parameters.", expression));
            }
            if (instructions.Length < expectedCount)
            {
                throw new TimeCrontabException(string.Format("The provided cron string <{0}> has too few parameters.", expression));
            }

            // 初始化字段偏移量和字段字符解析器
            var defaultFieldOffset = 0;
            var fieldParsers = new Dictionary<CrontabFieldKind, List<ICronParser>>();

            // 判断当前 Cron 格式化类型是否包含秒字段域，如果包含则优先解析秒字段域字符解析器
            if (format == CronStringFormat.WithSeconds || format == CronStringFormat.WithSecondsAndYears)
            {
                fieldParsers.Add(CrontabFieldKind.Second, ParseField(instructions[0], CrontabFieldKind.Second));
                defaultFieldOffset = 1;
            }

            // Cron 常规字段域
            fieldParsers.Add(CrontabFieldKind.Minute, ParseField(instructions[defaultFieldOffset + 0], CrontabFieldKind.Minute));   // 偏移量 1
            fieldParsers.Add(CrontabFieldKind.Hour, ParseField(instructions[defaultFieldOffset + 1], CrontabFieldKind.Hour));   // 偏移量 2
            fieldParsers.Add(CrontabFieldKind.Day, ParseField(instructions[defaultFieldOffset + 2], CrontabFieldKind.Day)); // 偏移量 3
            fieldParsers.Add(CrontabFieldKind.Month, ParseField(instructions[defaultFieldOffset + 3], CrontabFieldKind.Month)); // 偏移量 4
            fieldParsers.Add(CrontabFieldKind.DayOfWeek, ParseField(instructions[defaultFieldOffset + 4], CrontabFieldKind.DayOfWeek)); // 偏移量 5

            // 判断当前 Cron 格式化类型是否包含年字段域，如果包含则解析年字段域字符解析器
            if (format == CronStringFormat.WithYears || format == CronStringFormat.WithSecondsAndYears)
            {
                fieldParsers.Add(CrontabFieldKind.Year, ParseField(instructions[defaultFieldOffset + 5], CrontabFieldKind.Year));   // 偏移量 6
            }

            // 检查非法字符解析器，如 2 月没有 30 和 31 号
            CheckForIllegalParsers(fieldParsers);

            return fieldParsers;
        }

        /// <summary>
        /// 解析 Cron 单个字段域所有发生值 字符解析器
        /// </summary>
        /// <param name="field">字段值</param>
        /// <param name="kind">Cron 表达式格式化类型</param>
        /// <returns><see cref="List{T}"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        private static List<ICronParser> ParseField(string field, CrontabFieldKind kind)
        {
            /*
             * 在 Cron 表达式中，单个字段域值也支持定义多个值（我们称为值中值），如 1,2,3 或 SUN,FRI,SAT
             * 所以，这里需要将字段域值通过 , 进行切割后独立处理
             */

            try
            {
                return field.Split(',').Select(parser => ParseParser(parser, kind)).ToList();
            }
            catch (Exception ex)
            {
                throw new TimeCrontabException(
                    string.Format("There was an error parsing '{0}' for the {1} field.", field, Enum.GetName(typeof(CrontabFieldKind), kind))
                    , ex);
            }
        }

        /// <summary>
        /// 解析 Cron 字段域值中值
        /// </summary>
        /// <param name="parser">字段值中值</param>
        /// <param name="kind">Cron 表达式格式化类型</param>
        /// <returns><see cref="ICronParser"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        private static ICronParser ParseParser(string parser, CrontabFieldKind kind)
        {
            // Cron 字段中所有字母均采用大写方式，所以需要转换所有为大写再操作
            var newParser = parser.ToUpper();

            try
            {
                // 判断值是否以 * 字符开头
                if (newParser.StartsWith("*", StringComparison.OrdinalIgnoreCase))
                {
                    // 继续往后解析
                    newParser = newParser.Substring(1);

                    // 判断是否以 / 字符开头，如果是，则该值为带步长的 Cron 值
                    if (newParser.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        // 继续往后解析
                        newParser = newParser.Substring(1);

                        // 解析 Cron 值步长并创建 StepParser 解析器
                        var steps = GetValue(ref newParser, kind);
                        return new StepParser(0, steps, kind);
                    }

                    // 处理 * 携带意外值
                    if (newParser != string.Empty)
                    {
                        throw new TimeCrontabException(string.Format("Invalid parser '{0}'.", parser));
                    }

                    // 否则，创建 AnyParser 解析器
                    return new AnyParser(kind);
                }

                // 判断值是否以 L 字符开头
                if (newParser.StartsWith("L") && kind == CrontabFieldKind.Day)
                {
                    // 继续往后解析
                    newParser = newParser.Substring(1);

                    // 是否是 LW 字符，如果是，创建 LastWeekdayOfMonthParser 解析器
                    if (newParser == "W")
                    {
                        return new LastWeekdayOfMonthParser(kind);
                    }
                    // 否则创建 LastDayOfMonthParser 解析器
                    else
                    {
                        return new LastDayOfMonthParser(kind);
                    }
                }

                // 判断值是否等于 ?
                if (newParser == "?")
                {
                    // 创建 BlankDayOfMonthOrWeekParser 解析器
                    return new BlankDayOfMonthOrWeekParser(kind);
                }

                /*
                 * 如果上面均不匹配，那么该值类似取值有：2，1/2，1-10，1-10/2，SUN，SUNDAY，SUNL，JAN，3W，3L，2#5 等
                 */

                // 继续推进解析
                var firstValue = GetValue(ref newParser, kind);

                // 如果没有返回新的待解析字符，则认为这是一个具体值
                if (string.IsNullOrEmpty(newParser))
                {
                    // 对年份进行特别处理
                    if (kind == CrontabFieldKind.Year)
                    {
                        return new SpecificYearParser(firstValue, kind);
                    }
                    else
                    {
                        // 创建 SpecificParser 解析器
                        return new SpecificParser(firstValue, kind);
                    }
                }

                // 如果存在待解析字符，如 - / # L W 值，则进一步解析
                switch (newParser[0])
                {
                    // 判断值是否以 / 字符开头
                    case '/':
                        {
                            // 继续往后解析
                            newParser = newParser.Substring(1);

                            // 解析 Cron 值步长并创建 StepParser 解析器
                            var steps = GetValue(ref newParser, kind);
                            return new StepParser(firstValue, steps, kind);
                        }
                    // 判断值是否以 - 字符开头
                    case '-':
                        {
                            // 继续往后解析
                            newParser = newParser.Substring(1);

                            // 获取范围结束值
                            var endValue = GetValue(ref newParser, kind);
                            int? steps = null;

                            // 继续推进解析，判断是否以 / 开头，如果是，则获取步长
                            if (newParser.StartsWith("/"))
                            {
                                newParser = newParser.Substring(1);
                                steps = GetValue(ref newParser, kind);
                            }

                            // 创建 RangeParser 解析器
                            return new RangeParser(firstValue, endValue, steps, kind);
                        }
                    // 判断值是否以 # 字符开头
                    case '#':
                        {
                            // 继续往后解析
                            newParser = newParser.Substring(1);

                            // 获取第几个
                            var weekNumber = GetValue(ref newParser, kind);

                            // 继续推进解析，如果存在其他字符，则抛异常
                            if (!string.IsNullOrEmpty(newParser))
                            {
                                throw new TimeCrontabException(string.Format("Invalid parser '{0}.'", parser));
                            }

                            // 创建 SpecificDayOfWeekInMonthParser 解析器
                            return new SpecificDayOfWeekInMonthParser(firstValue, weekNumber, kind);
                        }
                    // 判断解析值是否等于 L 或 W
                    default:
                        // 创建 LastDayOfWeekInMonthParser 解析器
                        if (newParser == "L" && kind == CrontabFieldKind.DayOfWeek)
                        {
                            return new LastDayOfWeekInMonthParser(firstValue, kind);
                        }
                        // 创建 NearestWeekdayParser 解析器
                        else if (newParser == "W" && kind == CrontabFieldKind.Day)
                        {
                            return new NearestWeekdayParser(firstValue, kind);
                        }
                        break;
                }

                throw new TimeCrontabException(string.Format("Invalid parser '{0}'.", parser));
            }
            catch (Exception ex)
            {
                throw new TimeCrontabException(string.Format("Invalid parser '{0}'. See inner exception for details.", parser), ex);
            }
        }

        /// <summary>
        /// 将 Cron 字段值中值进一步解析
        /// </summary>
        /// <param name="parser">当前解析值</param>
        /// <param name="kind">Cron 表达式格式化类型</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        private static int GetValue(ref string parser, CrontabFieldKind kind)
        {
            // 值空检查
            if (string.IsNullOrEmpty(parser))
            {
                throw new TimeCrontabException("Expected number, but parser was empty.");
            }

            // 字符偏移量
            int offset;

            // 判断首个字符是数字还是字符串
            var isDigit = char.IsDigit(parser[0]);
            var isLetter = char.IsLetter(parser[0]);

            // 推进式遍历值并检查每一个字符，一旦出现类型不连贯则停止检查
            for (offset = 0; offset < parser.Length; offset++)
            {
                // 如果存在不连贯数字或字母则跳出循环
                if ((isDigit && !char.IsDigit(parser[offset])) || (isLetter && !char.IsLetter(parser[offset])))
                {
                    break;
                }
            }

            var maximum = Constants.MaximumDateTimeValues[kind];

            // 前面连贯类型的值
            var valueToParse = parser.Substring(0, offset);

            // 处理数字开头的连贯类型值
            if (int.TryParse(valueToParse, out var value))
            {
                // 导出下一轮待解析的值（依旧采用推进式）
                parser = parser.Substring(offset);

                var returnValue = value;

                // 验证值范围
                if (returnValue > maximum)
                {
                    throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
                }

                return returnValue;
            }
            // 处理字母开头的连贯类型值，通常认为这是一个单词，如SUN，JAN
            else
            {
                List<KeyValuePair<string, int>> replaceVal = null;

                // 判断当前 Cron 字段类型是否是星期，如果是，则查找该单词是否在 Constants.Days 定义之中
                if (kind == CrontabFieldKind.DayOfWeek)
                {
                    replaceVal = Constants.Days.Where(x => valueToParse.StartsWith(x.Key)).ToList();
                }
                // 判断当前 Cron 字段类型是否是月份，如果是，则查找该单词是否在 Constants.Months 定义之中
                else if (kind == CrontabFieldKind.Month)
                {
                    replaceVal = Constants.Months.Where(x => valueToParse.StartsWith(x.Key)).ToList();
                }

                // 如果存在且唯一，则进入下一轮判断
                // 接下来的判断是处理 SUN + L 的情况，如 SUNL == 0L == SUNDAY，它们都是合法的 Cron 值
                if (replaceVal != null && replaceVal.Count == 1)
                {
                    var missingParser = "";

                    // 处理带 L 和不带 L 的单词问题
                    if (parser.Length == offset
                        && parser.EndsWith("L")
                        && kind == CrontabFieldKind.DayOfWeek)
                    {
                        missingParser = "L";
                    }
                    parser = parser.Substring(offset) + missingParser;

                    // 转换成 int 值返回（SUN，JAN.....）
                    var returnValue = replaceVal.First().Value;

                    // 验证值范围
                    if (returnValue > maximum)
                    {
                        throw new TimeCrontabException(string.Format("Value for {0} parser exceeded maximum value of {1}.", Enum.GetName(typeof(CrontabFieldKind), kind), maximum));
                    }

                    return returnValue;
                }
            }

            throw new TimeCrontabException("Parser does not contain expected number.");
        }

        /// <summary>
        /// 检查非法字符解析器，如 2 月没有 30 和 31 号
        /// </summary>
        /// <remarks>检查 2 月份是否存在 30 和 31 天的非法数值解析器</remarks>
        /// <param name="parsers">Cron 字段解析器字典集合</param>
        /// <exception cref="TimeCrontabException"></exception>
        private static void CheckForIllegalParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers)
        {
            // 获取当前 Cron 表达式月字段和天字段所有数值
            var monthSingle = GetSpecificParsers(parsers, CrontabFieldKind.Month);
            var daySingle = GetSpecificParsers(parsers, CrontabFieldKind.Day);

            // 如果月份为 2 月单天数出现 30 和 31 天，则是无效数值
            if (monthSingle.Any() && monthSingle.All(x => x.SpecificValue == 2))
            {
                if (daySingle.Any() && daySingle.All(x => (x.SpecificValue == 30) || (x.SpecificValue == 31)))
                {
                    throw new TimeCrontabException("The February 30 and 31 don't exist.");
                }
            }
        }

        /// <summary>
        /// 查找 Cron 字段类型所有具体值解析器
        /// </summary>
        /// <param name="parsers">Cron 字段解析器字典集合</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <returns><see cref="List{T}"/></returns>
        private static List<SpecificParser> GetSpecificParsers(Dictionary<CrontabFieldKind, List<ICronParser>> parsers, CrontabFieldKind kind)
        {
            var kindParsers = parsers[kind];

            // 查找 Cron 字段类型所有具体值解析器
            return kindParsers.Where(x => x.GetType() == typeof(SpecificParser)).Cast<SpecificParser>()
                .Union(
                kindParsers.Where(x => x.GetType() == typeof(RangeParser)).SelectMany(x => ((RangeParser)x).SpecificParsers)
                ).Union(
                    kindParsers.Where(x => x.GetType() == typeof(StepParser)).SelectMany(x => ((StepParser)x).SpecificParsers)
                ).ToList();
        }

        /// <summary>
        /// 获取特定时间范围下一个发生时间
        /// </summary>
        /// <param name="baseTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns><see cref="DateTime"/></returns>
        private DateTime InternalGetNextOccurence(DateTime baseTime, DateTime endTime)
        {
            // 判断当前 Cron 格式化类型是否支持秒
            var isSecondFormat = Format == CronStringFormat.WithSeconds || Format == CronStringFormat.WithSecondsAndYears;

            // 由于 Cron 格式化类型不包含毫秒，则裁剪掉毫秒部分
            var newValue = baseTime;
            newValue = newValue.AddMilliseconds(-newValue.Millisecond);

            // 如果当前 Cron 格式化类型不支持秒，则裁剪掉秒部分
            if (!isSecondFormat)
            {
                newValue = newValue.AddSeconds(-newValue.Second);
            }

            // 获取分钟、小时所有字符解析器
            var minuteParsers = Parsers[CrontabFieldKind.Minute].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
            var hourParsers = Parsers[CrontabFieldKind.Hour].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

            // 获取秒、分钟、小时解析器中最小起始值
            // 该值主要用来获取下一个发生值的输入参数
            var firstSecondValue = newValue.Second;
            var firstMinuteValue = minuteParsers.Select(x => x.First()).Min();
            var firstHourValue = hourParsers.Select(x => x.First()).Min();

            // 定义一个标识，标识当前时间下一个发生时间值是否进入新一轮循环
            // 如：如果当前时间的秒数为 59，那么下一个秒数应该为 00，那么当前时间分钟就应该 +1
            // 以此类推，如果 +1 后分钟数为 59，那么下一个分钟数也应该为 00，那么当前时间小时数就应该 +1
            // ....
            var overflow = true;

            // 处理 Cron 格式化类型包含秒的情况 =================================================================
            var newSeconds = newValue.Second;
            if (isSecondFormat)
            {
                // 获取秒所有字符解析器
                var secondParsers = Parsers[CrontabFieldKind.Second].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();

                // 获取秒解析器最小起始值
                firstSecondValue = secondParsers.Select(x => x.First()).Min();

                // 获取秒下一个发生值
                newSeconds = Increment(secondParsers, newValue.Second, firstSecondValue, out overflow);

                // 设置起始时间为下一个秒时间
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

                // 如果当前秒并没有进入下一轮循环但存在不匹配的字符过滤器
                if (!overflow && !IsMatch(newValue))
                {
                    // 重置秒为起始值并标记 overflow 为 true 进入新一轮循环
                    newSeconds = firstSecondValue;

                    // 此时计算时间秒部分应该为起始值
                    // 如 22:10:59 -> 22:10:00
                    newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newValue.Minute, newSeconds);

                    // 标记进入下一轮循环
                    overflow = true;
                }

                // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一秒时间
                if (!overflow)
                {
                    return MinDate(newValue, endTime);
                }
            }

            // 程序到达这里，说明秒部分已经标识进入新一轮循环，那么分支就应该获取下一个分钟发生值 =================================================================
            var newMinutes = Increment(minuteParsers, newValue.Minute + (overflow ? 0 : -1), firstMinuteValue, out overflow);

            // 设置起始时间为下一个分钟时间
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, overflow ? firstSecondValue : newSeconds);

            // 如果当前分钟并没有进入下一轮循环但存在不匹配的字符过滤器
            if (!overflow && !IsMatch(newValue))
            {
                // 重置秒，分钟为起始值并标记 overflow 为 true 进入新一轮循环
                newSeconds = firstSecondValue;
                newMinutes = firstMinuteValue;

                // 此时计算时间秒和分钟部分应该为起始值
                // 如 22:59:59 -> 22:00:00
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newValue.Hour, newMinutes, firstSecondValue);

                // 标记进入下一轮循环
                overflow = true;
            }

            // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一分钟时间
            if (!overflow)
            {
                return MinDate(newValue, endTime);
            }

            // 程序到达这里，说明分钟部分已经标识进入新一轮循环，那么分支就应该获取下一个小时发生值 =================================================================
            var newHours = Increment(hourParsers, newValue.Hour + (overflow ? 0 : -1), firstHourValue, out overflow);

            // 设置起始时间为下一个小时时间
            newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, newHours,
                overflow ? firstMinuteValue : newMinutes,
                overflow ? firstSecondValue : newSeconds);

            // 如果当前小时并没有进入下一轮循环但存在不匹配的字符过滤器
            if (!overflow && !IsMatch(newValue))
            {
                // 此时计算时间秒，分钟和小时部分应该为起始值
                // 如 23:59:59 -> 23:00:00
                newValue = new DateTime(newValue.Year, newValue.Month, newValue.Day, firstHourValue, firstMinuteValue, firstSecondValue);

                // 标记进入下一轮循环
                overflow = true;
            }

            // 如果程序到达这里，说明并没有进入上面分支，则直接返回下一小时时间
            if (!overflow)
            {
                return MinDate(newValue, endTime);
            }

            // 如果程序达到这里，说明天数变了（一旦天数变了，那么月份可能也变了，星期可能也变了，年份也可能变了）
            // 所以这里的计算最为复杂
            List<ITimeParser> yearParsers = null;

            // 首先先判断当前 Cron 格式化类型是否支持年份
            var isYearFormat = Format == CronStringFormat.WithYears || Format == CronStringFormat.WithSecondsAndYears;

            // 如果支持，读取年份字符过滤器
            if (isYearFormat)
            {
                yearParsers = Parsers[CrontabFieldKind.Year].Where(x => x is ITimeParser).Cast<ITimeParser>().ToList();
            }

            // 程序能够执行到这里，那么说明时间已经是 23:59:59，所以起始时间追加 1 天
            // 这里的代码看起来很奇怪，但是是为了处理终止时间为 12/31/9999 23:59:59.999 的情况，也就是世界末日了~~~
            try
            {
                newValue = newValue.AddDays(1);
            }
            catch
            {
                return endTime;
            }

            // 在有效的年份时间内死循环至天、周、月、年全部匹配才终止循环
            while (!(IsMatch(newValue, CrontabFieldKind.Day)
                && IsMatch(newValue, CrontabFieldKind.DayOfWeek)
                && IsMatch(newValue, CrontabFieldKind.Month)
                && (!isYearFormat || IsMatch(newValue, CrontabFieldKind.Year))))
            {
                // 如果当前匹配到的时间已经大于或等于终止时间，则直接返回
                if (newValue >= endTime)
                {
                    return MinDate(newValue, endTime);
                }

                // 如果 Cron 年份字段域获取下一个发生值为 null，那么直接返回 终止时间
                // 也就是已经没有匹配项了
                if (isYearFormat && yearParsers!.Select(x => x.Next(newValue.Year - 1)).All(x => x == null))
                {
                    return endTime;
                }

                // 同样防止终止时间为 12/31/9999 23:59:59.999 的情况
                try
                {
                    // 不断增加 1 天直至匹配成功
                    newValue = newValue.AddDays(1);
                }
                catch
                {
                    return endTime;
                }
            }

            return MinDate(newValue, endTime);
        }

        /// <summary>
        /// 获取当前时间解析器下一个发生值
        /// </summary>
        /// <param name="parsers">解析器</param>
        /// <param name="value">当前值</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="overflow">控制秒、分钟、小时到达59秒/分和23小时开关</param>
        /// <returns><see cref="int"/></returns>
        private static int Increment(IEnumerable<ITimeParser> parsers, int value, int defaultValue, out bool overflow)
        {
            var nextValue = parsers.Select(x => x.Next(value))
                .Where(x => x > value)
                .Min()
                ?? defaultValue;

            // 如果此时秒或分钟或23到达最大值，则应该返回起始值
            overflow = nextValue <= value;

            return nextValue;
        }

        /// <summary>
        /// 处理下一个发生时间边界值
        /// </summary>
        /// <remarks>如果发生时间大于终止时间，则返回终止时间，否则返回发生时间</remarks>
        /// <param name="newTime">下一个发生时间</param>
        /// <param name="endTime">终止时间</param>
        /// <returns><see cref="DateTime"/></returns>
        private static DateTime MinDate(DateTime newTime, DateTime endTime)
        {
            return newTime >= endTime ? endTime : newTime;
        }

        /// <summary>
        /// 判断 Cron 所有字段字符解析器是否都能匹配当前时间各个部分
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        private bool IsMatch(DateTime datetime)
        {
            return Parsers.All(fieldKind =>
                fieldKind.Value.Any(parser => parser.IsMatch(datetime))
            );
        }

        /// <summary>
        /// 判断当前 Cron 字段类型字符解析器和当前时间至少存在一种匹配
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <returns></returns>
        private bool IsMatch(DateTime datetime, CrontabFieldKind kind)
        {
            return Parsers.Where(x => x.Key == kind)
                .SelectMany(x => x.Value)
                .Any(parser => parser.IsMatch(datetime));
        }

        /// <summary>
        /// 将 Cron 字段解析器转换成字符串
        /// </summary>
        /// <param name="paramList">Cron 字段字符串集合</param>
        /// <param name="kind">Cron 字段种类</param>
        private void JoinParsers(List<string> paramList, CrontabFieldKind kind)
        {
            paramList.Add(
                string.Join(",", Parsers
                    .Where(x => x.Key == kind)
                    .SelectMany(x => x.Value.Select(y => y.ToString())).ToArray()
                )
            );
        }
    }
    #endregion Internal
    #region // Macro
    /// <summary>
    /// Cron 表达式抽象类
    /// </summary>
    /// <remarks>主要将 Cron 表达式转换成 OOP 类进行操作</remarks>
    public sealed partial class Crontab
    {
        /// <summary>
        /// 表示每秒的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Secondly = Parse("* * * * * *", CronStringFormat.WithSeconds);

        /// <summary>
        /// 表示每分钟的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Minutely = Parse("* * * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每小时开始 的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Hourly = Parse("0 * * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每天（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Daily = Parse("0 0 * * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每月1号（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Monthly = Parse("0 0 1 * *", CronStringFormat.Default);

        /// <summary>
        /// 表示每周日（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Weekly = Parse("0 0 * * 0", CronStringFormat.Default);

        /// <summary>
        /// 表示每年1月1号（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Yearly = Parse("0 0 1 1 *", CronStringFormat.Default);

        /// <summary>
        /// 表示每周一至周五（午夜）开始的 <see cref="Crontab"/> 对象
        /// </summary>
        public static readonly Crontab Workday = Parse("0 0 * * 1-5", CronStringFormat.Default);
    }
    #endregion Macro
    #region // MicroAt
    /// <summary>
    /// Cron 表达式抽象类
    /// </summary>
    /// <remarks>主要将 Cron 表达式转换成 OOP 类进行操作</remarks>
    public sealed partial class Crontab
    {
        /// <summary>
        /// 创建指定特定秒开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab SecondlyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"{FieldsToString(fields)} * * * * *", CronStringFormat.WithSeconds);
        }

        /// <summary>
        /// 创建每分钟特定秒开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab MinutelyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"{FieldsToString(fields)} * * * * *", CronStringFormat.WithSeconds);
        }

        /// <summary>
        /// 创建每小时特定分钟开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab HourlyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"{FieldsToString(fields)} * * * *", CronStringFormat.Default);
        }

        /// <summary>
        /// 创建每天特定小时开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab DailyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"0 {FieldsToString(fields)} * * *", CronStringFormat.Default);
        }

        /// <summary>
        /// 创建每月特定天（午夜）开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab MonthlyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"0 0 {FieldsToString(fields)} * *", CronStringFormat.Default);
        }

        /// <summary>
        /// 创建每周特定星期几（午夜）开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab WeeklyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"0 0 * * {FieldsToString(fields)}", CronStringFormat.Default);
        }

        /// <summary>
        /// 创建每年特定月1号（午夜）开始作业触发器构建器
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="Crontab"/></returns>
        public static Crontab YearlyAt(params object[] fields)
        {
            // 检查字段合法性
            CheckFieldsNotNullOrEmpty(fields);

            return Parse($"0 0 1 {FieldsToString(fields)} *", CronStringFormat.Default);
        }

        /// <summary>
        /// 检查字段域 非 Null 非空数组
        /// </summary>
        /// <param name="fields">字段值</param>
        private static void CheckFieldsNotNullOrEmpty(params object[] fields)
        {
            // 空检查
            if (fields == null || fields.Length == 0) throw new ArgumentNullException(nameof(fields));

            // 检查 fields 只能是 int, long，string 和非 null 类型
            if (fields.Any(f => f == null || (f.GetType() != typeof(int) && f.GetType() != typeof(long) && f.GetType() != typeof(string)))) throw new InvalidOperationException("Invalid Cron expression.");
        }

        /// <summary>
        /// 将字段域转换成 string
        /// </summary>
        /// <param name="fields">字段值</param>
        /// <returns><see cref="string"/></returns>
        private static string FieldsToString(params object[] fields)
        {
            return string.Join(",", fields.Select(f => f.ToString()).ToArray());
        }
    }
    #endregion MicroAt
    #region // Constants
    /// <summary>
    /// TimeCrontab 模块常量
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Cron 字段种类最大值
        /// </summary>
        internal static readonly Dictionary<CrontabFieldKind, int> MaximumDateTimeValues = new()
        {
            { CrontabFieldKind.Second, 59 },
            { CrontabFieldKind.Minute, 59 },
            { CrontabFieldKind.Hour, 23 },
            { CrontabFieldKind.DayOfWeek, 7 },
            { CrontabFieldKind.Day, 31 },
            { CrontabFieldKind.Month, 12 },
            { CrontabFieldKind.Year, 9999 },
        };

        /// <summary>
        /// Cron 字段种类最大值
        /// </summary>
        internal static readonly Dictionary<CrontabFieldKind, int> MinimumDateTimeValues = new()
        {
            { CrontabFieldKind.Second, 0 },
            { CrontabFieldKind.Minute, 0 },
            { CrontabFieldKind.Hour, 0 },
            { CrontabFieldKind.DayOfWeek, 0 },
            { CrontabFieldKind.Day, 1 },
            { CrontabFieldKind.Month, 1 },
            { CrontabFieldKind.Year, 1 },
        };

        /// <summary>
        /// Cron 不同格式化类型字段数量
        /// </summary>
        internal static readonly Dictionary<CronStringFormat, int> ExpectedFieldCounts = new()
        {
            { CronStringFormat.Default, 5 },
            { CronStringFormat.WithYears, 6 },
            { CronStringFormat.WithSeconds, 6 },
            { CronStringFormat.WithSecondsAndYears, 7 },
        };

        /// <summary>
        /// 配置 C# 中 <see cref="DayOfWeek"/> 枚举元素值
        /// </summary>
        /// <remarks>主要解决 C# 中该类型和 Cron 星期字段域不对应问题</remarks>
        internal static readonly Dictionary<DayOfWeek, int> CronDays = new()
        {
            { DayOfWeek.Sunday, 0 },
            { DayOfWeek.Monday, 1 },
            { DayOfWeek.Tuesday, 2 },
            { DayOfWeek.Wednesday, 3 },
            { DayOfWeek.Thursday, 4 },
            { DayOfWeek.Friday, 5 },
            { DayOfWeek.Saturday, 6 },
        };

        /// <summary>
        /// 定义 Cron 星期字段域值支持的星期英文缩写
        /// </summary>
        internal static readonly Dictionary<string, int> Days = new()
        {
            { "SUN", 0 },
            { "MON", 1 },
            { "TUE", 2 },
            { "WED", 3 },
            { "THU", 4 },
            { "FRI", 5 },
            { "SAT", 6 },
        };

        /// <summary>
        /// 定义 Cron 月字段域值支持的星期英文缩写
        /// </summary>
        internal static readonly Dictionary<string, int> Months = new()
        {
            { "JAN", 1 },
            { "FEB", 2 },
            { "MAR", 3 },
            { "APR", 4 },
            { "MAY", 5 },
            { "JUN", 6 },
            { "JUL", 7 },
            { "AUG", 8 },
            { "SEP", 9 },
            { "OCT", 10 },
            { "NOV", 11 },
            { "DEC", 12 },
        };
    }
    /// <summary>
    /// Cron 字段种类
    /// </summary>
    internal enum CrontabFieldKind
    {
        /// <summary>
        /// 秒
        /// </summary>
        Second = 0,

        /// <summary>
        /// 分
        /// </summary>
        Minute = 1,

        /// <summary>
        /// 时
        /// </summary>
        Hour = 2,

        /// <summary>
        /// 天
        /// </summary>
        Day = 3,

        /// <summary>
        /// 月
        /// </summary>
        Month = 4,

        /// <summary>
        /// 星期
        /// </summary>
        DayOfWeek = 5,

        /// <summary>
        /// 年
        /// </summary>
        Year = 6
    }
    /// <summary>
    /// Cron 表达式格式化类型
    /// </summary>
    public enum CronStringFormat
    {
        /// <summary>
        /// 默认格式
        /// </summary>
        /// <remarks>书写顺序：分 时 天 月 周</remarks>
        Default = 0,

        /// <summary>
        /// 带年份格式
        /// </summary>
        /// <remarks>书写顺序：分 时 天 月 周 年</remarks>
        WithYears = 1,

        /// <summary>
        /// 带秒格式
        /// </summary>
        /// <remarks>书写顺序：秒 分 时 天 月 周</remarks>
        WithSeconds = 2,

        /// <summary>
        /// 带秒和年格式
        /// </summary>
        /// <remarks>书写顺序：秒 分 时 天 月 周 年</remarks>
        WithSecondsAndYears = 3
    }
    #endregion Constants
    #region // Exceptions
    /// <summary>
    /// TimeCrontab 模块异常类
    /// </summary>
    public sealed class TimeCrontabException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TimeCrontabException()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public TimeCrontabException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public TimeCrontabException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    #endregion Exceptions
    #region // Extensions
    /// <summary>
    /// <see cref="DayOfWeek"/> 拓展类
    /// </summary>
    internal static class DayOfWeekExtensions
    {
        /// <summary>
        /// 将 C# 中 <see cref="DayOfWeek"/> 枚举元素转换成数值
        /// </summary>
        /// <param name="dayOfWeek"><see cref="DayOfWeek"/> 枚举</param>
        /// <returns><see cref="int"/></returns>
        internal static int ToCronDayOfWeek(this DayOfWeek dayOfWeek)
        {
            return Constants.CronDays[dayOfWeek];
        }

        /// <summary>
        /// 将数值转换成 C# 中 <see cref="DayOfWeek"/> 枚举元素
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        internal static DayOfWeek ToDayOfWeek(this int dayOfWeek)
        {
            return Constants.CronDays.First(x => x.Value == dayOfWeek).Key;
        }

        /// <summary>
        /// 获取当前年月最后一个星期几
        /// </summary>
        /// <param name="dayOfWeek">星期几，<see cref="DayOfWeek"/> 类型</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns><see cref="int"/></returns>
        internal static int LastDayOfMonth(this DayOfWeek dayOfWeek, int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var currentDay = new DateTime(year, month, daysInMonth);

            // 从月底天数进行递归查找
            while (currentDay.DayOfWeek != dayOfWeek)
            {
                currentDay = currentDay.AddDays(-1);
            }

            return currentDay.Day;
        }
    }
    #endregion Extensions
    #region // Parsers
    /// <summary>
    /// Cron 字段值含 * 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>* 表示任意值，该字符支持在 Cron 所有字段域中设置</para>
    /// </remarks>
    internal sealed class AnyParser : ICronParser, ITimeParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kind">Cron 字段种类</param>
        public AnyParser(CrontabFieldKind kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            return true;
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int? Next(int currentValue)
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
            }

            // 默认递增步长为 1
            int? newValue = currentValue + 1;

            // 验证最大值
            var maximum = Constants.MaximumDateTimeValues[Kind];
            return newValue > maximum ? null : newValue;
        }

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int First()
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
            }

            return 0;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return "*";
        }
    }
    /// <summary>
    /// Cron 字段值含 ? 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>只能用在 Day 和 DayOfWeek 两个域使用。它也匹配域的任意值，但实际不会。因为 Day 和 DayOfWeek 会相互影响</para>
    /// <para>例如想在每月的 20 日触发调度，不管 20 日到底是星期几，则只能使用如下写法：13 15 20 * ?</para>
    /// <para>其中最后一位只能用 ?，而不能使用 *，如果使用 * 表示不管星期几都会触发，实际上并不是这样</para>
    /// <para>所以 ? 起着 Day 和 DayOfWeek 互斥性作用</para>
    /// <para>仅在 <see cref="CrontabFieldKind.Day"/> 或 <see cref="CrontabFieldKind.DayOfWeek"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class BlankDayOfMonthOrWeekParser : ICronParser
    {
        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public BlankDayOfMonthOrWeekParser(CrontabFieldKind kind)
        {
            // 验证 ? 字符是否在 DayOfWeek 和 Day 字段域中使用
            if (kind != CrontabFieldKind.DayOfWeek && kind != CrontabFieldKind.Day)
            {
                throw new TimeCrontabException("The <?> parser can only be used in the Day-of-Week or Day-of-Month fields.");
            }

            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            return true;
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int? Next(int currentValue)
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
            }

            // 默认递增步长为 1
            int? newValue = currentValue + 1;

            // 验证最大值
            var maximum = Constants.MaximumDateTimeValues[Kind];
            return newValue >= maximum ? null : newValue;
        }

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int First()
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
            }

            return 0;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return "?";
        }
    }
    /// <summary>
    /// Cron 字段值含 L 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>L 表示月中最后一天，仅在 <see cref="CrontabFieldKind.Day"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class LastDayOfMonthParser : ICronParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public LastDayOfMonthParser(CrontabFieldKind kind)
        {
            // 验证 L 字符是否在 Day 字段域中使用
            if (kind != CrontabFieldKind.Day)
            {
                throw new TimeCrontabException("The <L> parser can only be used with the Day field.");
            }

            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            return DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return "L";
        }
    }
    /// <summary>
    /// Cron 字段值含 {0}L 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示月中最后一个星期{0}，仅在 <see cref="CrontabFieldKind.DayOfWeek"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class LastDayOfWeekInMonthParser : ICronParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dayOfWeek">星期，0 = 星期天，7 = 星期六</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public LastDayOfWeekInMonthParser(int dayOfWeek, CrontabFieldKind kind)
        {
            // 验证 {0}L 字符是否在 DayOfWeek 字段域中使用
            if (kind != CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException(string.Format("The <{0}L> parser can only be used in the Day of Week field.", dayOfWeek));
            }

            DayOfWeek = dayOfWeek;
            DateTimeDayOfWeek = dayOfWeek.ToDayOfWeek();
            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 星期
        /// </summary>
        public int DayOfWeek { get; }

        /// <summary>
        /// <see cref="DayOfWeek"/> 类型星期
        /// </summary>
        private DayOfWeek DateTimeDayOfWeek { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            return datetime.Day == DateTimeDayOfWeek.LastDayOfMonth(datetime.Year, datetime.Month);
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return string.Format("{0}L", DayOfWeek);
        }
    }
    /// <summary>
    /// Cron 字段值含 LW 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示月中最后一个工作日，即最后一个非周六周末的日期，仅在 <see cref="CrontabFieldKind.Day"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class LastWeekdayOfMonthParser : ICronParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public LastWeekdayOfMonthParser(CrontabFieldKind kind)
        {
            // 验证 LW 字符是否在 Day 字段域中使用
            if (kind != CrontabFieldKind.Day)
            {
                throw new TimeCrontabException("The <LW> parser can only be used in the Day field.");
            }

            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            /*
             * W：表示有效工作日(周一到周五),只能出现在 Day 域，系统将在离指定日期的最近的有效工作日触发事件
             * 例如：在 Day 使用 5W，如果 5 日是星期六，则将在最近的工作日：星期五，即 4 日触发
             * 如果 5 日是星期天，则在 6 日(周一)触发；如果 5 日在星期一到星期五中的一天，则就在 5 日触发
             * 另外一点，W 的最近寻找不会跨过月份
             */

            // 获取当前时间所在月最后一天
            var specificValue = DateTime.DaysInMonth(datetime.Year, datetime.Month);
            var specificDay = new DateTime(datetime.Year, datetime.Month, specificValue);

            // 最靠近的工作日时间
            DateTime closestWeekday;

            // 处理月中最后一天的不同情况
            switch (specificDay.DayOfWeek)
            {
                // 如果最后一天是周六，则退一天
                case DayOfWeek.Saturday:
                    closestWeekday = specificDay.AddDays(-1);

                    break;

                // 如果最后一天是周天，则进一天
                case DayOfWeek.Sunday:
                    closestWeekday = specificDay.AddDays(1);

                    // 如果进一天不在本月，则退到上周五
                    if (closestWeekday.Month != specificDay.Month)
                    {
                        closestWeekday = specificDay.AddDays(-2);
                    }

                    break;

                // 处理恰好是工作日情况，直接使用
                default:
                    closestWeekday = specificDay;
                    break;
            }

            return datetime.Day == closestWeekday.Day;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return "LW";
        }
    }
    /// <summary>
    /// Cron 字段值含 {0}W 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示离指定日期最近的工作日，即最后一个非周六周末日，仅在 <see cref="CrontabFieldKind.Day"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class NearestWeekdayParser : ICronParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="specificValue">天数（具体值）</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException">Cron 字段种类</exception>
        public NearestWeekdayParser(int specificValue, CrontabFieldKind kind)
        {
            // 验证 {0}W 字符是否在 Day 字段域中使用
            if (kind != CrontabFieldKind.Day)
            {
                throw new TimeCrontabException(string.Format("The <{0}W> parser can only be used in the Day field.", specificValue));
            }

            // 判断天数是否在有效取值范围内
            var maximum = Constants.MaximumDateTimeValues[CrontabFieldKind.Day];
            if (specificValue <= 0 || specificValue > maximum)
            {
                throw new TimeCrontabException(string.Format("The <{0}W> is out of bounds for the Day field.", specificValue));
            }

            SpecificValue = specificValue;
            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 天数（具体值）
        /// </summary>
        public int SpecificValue { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            /*
             * W：表示有效工作日(周一到周五),只能出现在 Day 域，系统将在离指定日期的最近的有效工作日触发事件
             * 例如：在 Day 使用 5W，如果 5 日是星期六，则将在最近的工作日：星期五，即 4 日触发
             * 如果 5 日是星期天，则在 6 日(周一)触发；如果 5 日在星期一到星期五中的一天，则就在 5 日触发
             * 另外一点，W 的最近寻找不会跨过月份
             */

            // 如果这个月没有足够的天数则跳过（例如，二月没有 30 和 31 日）
            if (DateTime.DaysInMonth(datetime.Year, datetime.Month) < SpecificValue)
            {
                return false;
            }

            // 获取当前时间特定天数时间
            var specificDay = new DateTime(datetime.Year, datetime.Month, SpecificValue);

            // 最靠近的工作日时间
            DateTime closestWeekday;

            // 处理当天的不同情况
            switch (specificDay.DayOfWeek)
            {
                // 如果当天是周六，则退一天
                case DayOfWeek.Saturday:
                    closestWeekday = specificDay.AddDays(-1);

                    // 如果退一天不在本月，则转到下周一
                    if (closestWeekday.Month != specificDay.Month)
                    {
                        closestWeekday = specificDay.AddDays(2);
                    }

                    break;

                // 如果当天是周天，则进一天
                case DayOfWeek.Sunday:
                    closestWeekday = specificDay.AddDays(1);

                    // 如果进一天不在本月，则退到上周五
                    if (closestWeekday.Month != specificDay.Month)
                    {
                        closestWeekday = specificDay.AddDays(-2);
                    }

                    break;

                // 处理恰好是工作日情况，直接使用
                default:
                    closestWeekday = specificDay;
                    break;
            }

            return datetime.Day == closestWeekday.Day;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return string.Format("{0}W", SpecificValue);
        }
    }
    /// <summary>
    /// Cron 字段值含 - 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示特定取值范围，如 1-5 或 1-5/2，该字符支持在 Cron 所有字段域中设置</para>
    /// </remarks>
    internal sealed class RangeParser : ICronParser, ITimeParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start">起始值</param>
        /// <param name="end">终止值</param>
        /// <param name="steps">步长</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public RangeParser(int start, int end, int? steps, CrontabFieldKind kind)
        {
            var maximum = Constants.MaximumDateTimeValues[kind];

            // 验证起始值有效性
            if (start < 0 || start > maximum)
            {
                throw new TimeCrontabException(string.Format("Start = {0} is out of bounds for <{1}> field.", start, Enum.GetName(typeof(CrontabFieldKind), kind)));
            }

            // 验证终止值有效性
            if (end < 0 || end > maximum)
            {
                throw new TimeCrontabException(string.Format("End = {0} is out of bounds for <{1}> field.", end, Enum.GetName(typeof(CrontabFieldKind), kind)));
            }

            // 验证步长有效性
            if (steps != null && (steps <= 0 || steps > maximum))
            {
                throw new TimeCrontabException(string.Format("Steps = {0} is out of bounds for <{1}> field.", steps, Enum.GetName(typeof(CrontabFieldKind), kind)));
            }

            Start = start;
            End = end;
            Kind = kind;
            Steps = steps;

            // 计算所有满足范围计算的解析器
            var parsers = new List<SpecificParser>();
            for (var evalValue = Start; evalValue <= End; evalValue++)
            {
                if (IsMatch(evalValue))
                {
                    parsers.Add(new SpecificParser(evalValue, Kind));
                }
            }

            SpecificParsers = parsers;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 起始值
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 终止值
        /// </summary>
        public int End { get; }

        /// <summary>
        /// 步长
        /// </summary>
        public int? Steps { get; }

        /// <summary>
        /// 所有满足范围计算的解析器
        /// </summary>
        public IEnumerable<SpecificParser> SpecificParsers { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            // 获取不同 Cron 字段种类对应时间值
            var evalValue = Kind switch
            {
                CrontabFieldKind.Second => datetime.Second,
                CrontabFieldKind.Minute => datetime.Minute,
                CrontabFieldKind.Hour => datetime.Hour,
                CrontabFieldKind.Day => datetime.Day,
                CrontabFieldKind.Month => datetime.Month,
                CrontabFieldKind.DayOfWeek => datetime.DayOfWeek.ToCronDayOfWeek(),
                CrontabFieldKind.Year => datetime.Year,
                _ => throw new ArgumentOutOfRangeException(nameof(datetime), Kind, null),
            };

            return IsMatch(evalValue);
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int? Next(int currentValue)
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
            }

            // 默认递增步长为 1
            int? newValue = currentValue + 1;

            // 获取下一个匹配的发生值
            var maximum = Constants.MaximumDateTimeValues[Kind];
            while (newValue < maximum && !IsMatch(newValue.Value))
            {
                newValue++;
            }

            return newValue > maximum ? null : newValue;
        }

        /// <summary>
        /// 存储起始值，避免重复计算
        /// </summary>
        private int? FirstCache { get; set; }

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int First()
        {
            // 判断是否缓存过起始值，如果有则跳过
            if (FirstCache.HasValue)
            {
                return FirstCache.Value;
            }

            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
            }

            var maximum = Constants.MaximumDateTimeValues[Kind];
            var newValue = 0;

            // 获取首个符合的起始值
            while (newValue < maximum && !IsMatch(newValue))
            {
                newValue++;
            }

            // 验证起始值有效性
            if (newValue > maximum)
            {
                throw new TimeCrontabException(
                    string.Format("Next value for {0} on field {1} could not be found!",
                    ToString(),
                    Enum.GetName(typeof(CrontabFieldKind), Kind))
                );
            }

            // 缓存起始值
            FirstCache = newValue;
            return newValue;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return Steps.HasValue
                     ? string.Format("{0}-{1}/{2}", Start, End, Steps)
                     : string.Format("{0}-{1}", Start, End);
        }

        /// <summary>
        /// 判断是否符合范围或带步长范围解析规则
        /// </summary>
        /// <param name="evalValue">当前值</param>
        /// <returns><see cref="bool"/></returns>
        private bool IsMatch(int evalValue)
        {
            return evalValue >= Start && evalValue <= End
                && (!Steps.HasValue || ((evalValue - Start) % Steps) == 0);
        }
    }
    /// <summary>
    /// Cron 字段值含 {0}#{1} 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示月中第{0}个星期{1}，仅在 <see cref="CrontabFieldKind.DayOfWeek"/> 字段域中使用</para>
    /// </remarks>
    internal sealed class SpecificDayOfWeekInMonthParser : ICronParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dayOfWeek">星期，0 = 星期天，7 = 星期六</param>
        /// <param name="weekNumber">月中第几个星期</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public SpecificDayOfWeekInMonthParser(int dayOfWeek, int weekNumber, CrontabFieldKind kind)
        {
            // 验证星期数有效性
            if (weekNumber <= 0 || weekNumber > 5)
            {
                throw new TimeCrontabException(string.Format("Week number = {0} is out of bounds.", weekNumber));
            }

            // 验证 L 字符是否在 DayOfWeek 字段域中使用
            if (kind != CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException(string.Format("The <{0}#{1}> parser can only be used in the Day of Week field.", dayOfWeek, weekNumber));
            }

            DayOfWeek = dayOfWeek;
            DateTimeDayOfWeek = dayOfWeek.ToDayOfWeek();
            WeekNumber = weekNumber;
            Kind = kind;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 星期
        /// </summary>
        public int DayOfWeek { get; }

        /// <summary>
        /// <see cref="DayOfWeek"/> 类型星期
        /// </summary>
        private DayOfWeek DateTimeDayOfWeek { get; }

        /// <summary>
        /// 月中第几个星期
        /// </summary>
        public int WeekNumber { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            // 获取当前时间所在月第一天
            var currentDay = new DateTime(datetime.Year, datetime.Month, 1);

            // 第几个星期计数器
            var weekCount = 0;

            // 限制当前循环仅在本月
            while (currentDay.Month == datetime.Month)
            {
                // 首先确认星期是否相等，如果相等，则计数器 + 1
                if (currentDay.DayOfWeek == DateTimeDayOfWeek)
                {
                    weekCount++;

                    // 如果计算器和指定 WeekNumber 一致，则退出循环
                    if (weekCount == WeekNumber)
                    {
                        break;
                    }

                    // 否则，则追加一周（即7天）进入下一次循环
                    currentDay = currentDay.AddDays(7);
                }
                // 如果星期不相等，则追加一天i将纳入下一次循环
                else
                {
                    currentDay = currentDay.AddDays(1);
                }
            }

            // 如果最后计算出现跨月份情况，则不匹配
            if (currentDay.Month != datetime.Month)
            {
                return false;
            }

            return datetime.Day == currentDay.Day;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return string.Format("{0}#{1}", DayOfWeek, WeekNumber);
        }
    }
    /// <summary>
    /// Cron 字段值含 数值 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示具体值，该字符支持在 Cron 所有字段域中设置</para>
    /// </remarks>
    internal class SpecificParser : ICronParser, ITimeParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="specificValue">具体值</param>
        /// <param name="kind">Cron 字段种类</param>
        public SpecificParser(int specificValue, CrontabFieldKind kind)
        {
            SpecificValue = specificValue;
            Kind = kind;

            // 验证值有效性
            ValidateBounds(specificValue);
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 具体值
        /// </summary>
        public int SpecificValue { get; private set; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            // 获取不同 Cron 字段种类对应时间值
            var evalValue = Kind switch
            {
                CrontabFieldKind.Second => datetime.Second,
                CrontabFieldKind.Minute => datetime.Minute,
                CrontabFieldKind.Hour => datetime.Hour,
                CrontabFieldKind.Day => datetime.Day,
                CrontabFieldKind.Month => datetime.Month,
                CrontabFieldKind.DayOfWeek => datetime.DayOfWeek.ToCronDayOfWeek(),
                CrontabFieldKind.Year => datetime.Year,
                _ => throw new ArgumentOutOfRangeException(nameof(datetime), Kind, null),
            };

            return evalValue == SpecificValue;
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public virtual int? Next(int currentValue)
        {
            return SpecificValue;
        }

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int First()
        {
            return SpecificValue;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return SpecificValue.ToString();
        }

        /// <summary>
        /// 验证值有效性
        /// </summary>
        /// <param name="value">具体值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ValidateBounds(int value)
        {
            var minimum = Constants.MinimumDateTimeValues[Kind];
            var maximum = Constants.MaximumDateTimeValues[Kind];

            // 验证值有效性
            if (value < minimum || value > maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} should be between {minimum} and {maximum} (was {SpecificValue}).");
            }

            // 兼容星期日可以同时用 0 或 7 表示
            if (Kind == CrontabFieldKind.DayOfWeek)
            {
                SpecificValue %= 7;
            }
        }
    }
    /// <summary>
    /// Cron 字段值含 数值 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示具体值，这里仅处理 <see cref="CrontabFieldKind.Year"/> 字段域</para>
    /// </remarks>
    internal sealed class SpecificYearParser : SpecificParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="specificValue">年（具体值)</param>
        /// <param name="kind">Cron 字段种类</param>
        public SpecificYearParser(int specificValue, CrontabFieldKind kind)
            : base(specificValue, kind)
        {
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public override int? Next(int currentValue)
        {
            // 如果当前年份小于具体值，则返回具体值，否则返回 null
            // 因为一旦指定了年份，那么就必须等到那一年才触发
            return currentValue < SpecificValue ? SpecificValue : null;
        }
    }
    /// <summary>
    /// Cron 字段值含 / 字符解析器
    /// </summary>
    /// <remarks>
    /// <para>表示从某值开始，每隔固定值触发，该字符支持在 Cron 所有字段域中设置</para>
    /// </remarks>
    internal sealed class StepParser : ICronParser, ITimeParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="start">起始值</param>
        /// <param name="steps">步长</param>
        /// <param name="kind">Cron 字段种类</param>
        /// <exception cref="TimeCrontabException"></exception>
        public StepParser(int start, int steps, CrontabFieldKind kind)
        {
            // 验证步长有效性：不能小于或等于0，且不能大于 Cron 字段种类取值最大值
            var minimum = Constants.MinimumDateTimeValues[kind];
            var maximum = Constants.MaximumDateTimeValues[kind];
            if (steps <= 0 || steps > maximum)
            {
                throw new TimeCrontabException(string.Format("Steps = {0} is out of bounds for <{1}> field.", steps, Enum.GetName(typeof(CrontabFieldKind), kind)));
            }

            Start = start;
            Steps = steps;
            Kind = kind;

            // 控制循环起始值，并不一定从 Start 开始
            var loopStart = Math.Max(start, minimum);

            // 计算所有满足间隔步长计算的解析器
            var parsers = new List<SpecificParser>();
            for (var evalValue = loopStart; evalValue <= maximum; evalValue++)
            {
                if (IsMatch(evalValue))
                {
                    parsers.Add(new SpecificParser(evalValue, Kind));
                }
            }

            SpecificParsers = parsers;
        }

        /// <summary>
        /// Cron 字段种类
        /// </summary>
        public CrontabFieldKind Kind { get; }

        /// <summary>
        /// 起始值
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 步长
        /// </summary>
        public int Steps { get; }

        /// <summary>
        /// 所有满足间隔步长计算的解析器
        /// </summary>
        public IEnumerable<SpecificParser> SpecificParsers { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        public bool IsMatch(DateTime datetime)
        {
            // 获取不同 Cron 字段种类对应时间值
            var evalValue = Kind switch
            {
                CrontabFieldKind.Second => datetime.Second,
                CrontabFieldKind.Minute => datetime.Minute,
                CrontabFieldKind.Hour => datetime.Hour,
                CrontabFieldKind.Day => datetime.Day,
                CrontabFieldKind.Month => datetime.Month,
                CrontabFieldKind.DayOfWeek => datetime.DayOfWeek.ToCronDayOfWeek(),
                CrontabFieldKind.Year => datetime.Year,
                _ => throw new ArgumentOutOfRangeException(nameof(datetime), Kind, null),
            };

            return IsMatch(evalValue);
        }

        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int? Next(int currentValue)
        {
            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call Next for Day, Month or DayOfWeek types.");
            }

            // 默认递增步长为 1
            int? newValue = currentValue + 1;

            // 获取下一个匹配的发生值
            var maximum = Constants.MaximumDateTimeValues[Kind];
            while (newValue < maximum && !IsMatch(newValue.Value))
            {
                newValue++;
            }

            return newValue > maximum ? null : newValue;
        }

        /// <summary>
        /// 存储起始值，避免重复计算
        /// </summary>
        private int? FirstCache { get; set; }

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        public int First()
        {
            // 判断是否缓存过起始值，如果有则跳过
            if (FirstCache.HasValue)
            {
                return FirstCache.Value;
            }

            // 由于天、月、周计算复杂，所以这里排除对它们的处理
            if (Kind == CrontabFieldKind.Day
                || Kind == CrontabFieldKind.Month
                || Kind == CrontabFieldKind.DayOfWeek)
            {
                throw new TimeCrontabException("Cannot call First for Day, Month or DayOfWeek types.");
            }

            var maximum = Constants.MaximumDateTimeValues[Kind];
            var newValue = 0;

            // 获取首个符合的起始值
            while (newValue < maximum && !IsMatch(newValue))
            {
                newValue++;
            }

            // 验证起始值有效性
            if (newValue > maximum)
            {
                throw new TimeCrontabException(
                    string.Format("Next value for {0} on field {1} could not be found!",
                    ToString(),
                    Enum.GetName(typeof(CrontabFieldKind), Kind))
                );
            }

            // 缓存起始值
            FirstCache = newValue;
            return newValue;
        }

        /// <summary>
        /// 将解析器转换成字符串输出
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", Start == 0 ? "*" : Start.ToString(), Steps);
        }

        /// <summary>
        /// 判断是否符合间隔或带步长间隔解析规则
        /// </summary>
        /// <param name="evalValue">当前值</param>
        /// <returns><see cref="bool"/></returns>
        private bool IsMatch(int evalValue)
        {
            return evalValue >= Start && (evalValue - Start) % Steps == 0;
        }
    }
    #region // Dependencies
    /// <summary>
    /// Cron 字段字符解析器依赖接口
    /// </summary>
    internal interface ICronParser
    {
        /// <summary>
        /// Cron 字段种类
        /// </summary>
        CrontabFieldKind Kind { get; }

        /// <summary>
        /// 判断当前时间是否符合 Cron 字段种类解析规则
        /// </summary>
        /// <param name="datetime">当前时间</param>
        /// <returns><see cref="bool"/></returns>
        bool IsMatch(DateTime datetime);
    }
    /// <summary>
    /// DateTime 时间解析器依赖接口
    /// </summary>
    /// <remarks>主要用于计算 DateTime 主要组成部分（秒，分，时，年）的下一个取值</remarks>
    internal interface ITimeParser
    {
        /// <summary>
        /// 获取 Cron 字段种类当前值的下一个发生值
        /// </summary>
        /// <param name="currentValue">时间值</param>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        int? Next(int currentValue);

        /// <summary>
        /// 获取 Cron 字段种类字段起始值
        /// </summary>
        /// <returns><see cref="int"/></returns>
        /// <exception cref="TimeCrontabException"></exception>
        int First();
    }
    #endregion Dependencies
    #endregion Parsers
}
