using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Dibber
{
    /// <summary>
    /// 兼容助手
    /// </summary>
    public static class CompatAssist
    {
        /// <summary>
        /// 从结果来任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Task<T> FromResult<T>(T result)
        {
#if NET40
            return new Task<T>(() => result);
#else
            return Task.FromResult(result);
#endif
        }
    }
    /// <summary>
    /// 内部调用
    /// </summary>
    internal static class InternalCaller
    {
#if NET40
        public static T GetFieldValue<T>(this DbDataReader _reader, int ordinal) => (T)_reader[ordinal];
        public static Task<T> GetFieldValueAsync<T>(this DbDataReader _reader, int ordinal, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<T>();
            }
            return new Task<T>(() => GetFieldValue<T>(_reader, ordinal));
        }
        public static Stream GetStream(this DbDataReader _reader, int ordinal)
        {
            using (MemoryStream bufferStream = new MemoryStream())
            {
                long bytesRead = 0;
                long bytesReadTotal = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    bytesRead = _reader.GetBytes(ordinal, bytesReadTotal, buffer, 0, buffer.Length);
                    bufferStream.Write(buffer, 0, (int)bytesRead);
                    bytesReadTotal += bytesRead;
                } while (bytesRead > 0);

                return new MemoryStream(bufferStream.ToArray(), false);
            }
        }
        public static TextReader GetTextReader(this DbDataReader _reader, int ordinal)
        {
            return _reader.IsDBNull(ordinal) ? new StringReader(String.Empty) : new StringReader(_reader.GetString(ordinal));
        }
        public static Task<bool> IsDBNullAsync(this DbDataReader _reader, int ordinal, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.IsDBNull(ordinal) ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }
        public static Task<bool> NextResultAsync(this DbDataReader _reader, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.NextResult() ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }
        public static Task<bool> ReadAsync(this DbDataReader _reader, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<bool>();
            }
            else
            {
                try
                {
                    return _reader.Read() ? TrueTask : FalseTask;
                }
                catch (Exception e)
                {
                    return CreatedTaskWithException<bool>(e);
                }
            }
        }

        static Task<bool> TrueTask { get; } = new Task<bool>(() => true);
        static Task<bool> FalseTask { get; } = new Task<bool>(() => false);
        internal static Task<T> CreatedTaskWithCancellation<T>()
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetCanceled();
            return completion.Task;
        }
        internal static Task<T> CreatedTaskWithException<T>(Exception ex)
        {
            TaskCompletionSource<T> completion = new TaskCompletionSource<T>();
            completion.SetException(ex);
            return completion.Task;
        }

        public static Task<int> ExecuteNonQueryAsync(this DbCommand cmd) => ExecuteNonQueryAsync(cmd, CancellationToken.None);

        public static Task<int> ExecuteNonQueryAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<int>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return CompatAssist.FromResult<int>(cmd.ExecuteNonQuery());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<int>(e);
                }
            }
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd)
        {
            return ExecuteReaderAsync(cmd, CommandBehavior.Default, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(cmd, CommandBehavior.Default, cancellationToken);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CommandBehavior behavior)
        {
            return ExecuteReaderAsync(cmd, behavior, CancellationToken.None);
        }

        public static Task<DbDataReader> ExecuteReaderAsync(this DbCommand cmd, CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<DbDataReader>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return CompatAssist.FromResult<DbDataReader>(cmd.ExecuteReader(behavior));
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<DbDataReader>(e);
                }
            }
        }
        public static Task<object> ExecuteScalarAsync(this DbCommand cmd)
        {
            return ExecuteScalarAsync(cmd, CancellationToken.None);
        }

        public static Task<object> ExecuteScalarAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return CreatedTaskWithCancellation<object>();
            }
            else
            {
                CancellationTokenRegistration registration = new CancellationTokenRegistration();
                if (cancellationToken.CanBeCanceled)
                {
                    registration = cancellationToken.Register(() => { try { cmd.Cancel(); } catch { } });
                }

                try
                {
                    return CompatAssist.FromResult<object>(cmd.ExecuteScalar());
                }
                catch (Exception e)
                {
                    registration.Dispose();
                    return CreatedTaskWithException<object>(e);
                }
            }
        }
        public static Task OpenAsync(this DbConnection conn)
        {
            return OpenAsync(conn, CancellationToken.None);
        }

        public static Task OpenAsync(this DbConnection conn, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            if (cancellationToken.IsCancellationRequested)
            {
                taskCompletionSource.SetCanceled();
            }
            else
            {
                try
                {
                    conn.Open();
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            }

            return taskCompletionSource.Task;
        }
#endif
    }
}
