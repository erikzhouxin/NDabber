using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

#if NET40
namespace System.Collections.Generic
{
    /// <summary>
    /// 表示元素的强类型化只读集合。
    /// </summary>
    /// <typeparam name="T">元素的类型。</typeparam>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// 获取集合中的元素数。
        /// </summary>
        int Count { get; }
    }
    /// <summary>
    /// 只读集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
    }
    /// <summary>
    /// 表示键/值对的泛型只读集合。
    /// </summary>
    /// <typeparam name="TKey">只读字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">只读字典中的值的类型。</typeparam>
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        /// <summary>
        /// 获取在只读目录中有指定键的元素。
        /// </summary>
        /// <param name="key">要定位的键。</param>
        /// <returns>在只读目录中有指定键的元素。</returns>
        /// <exception cref="ArgumentNullException">key 为 null。</exception>
        /// <exception cref="KeyNotFoundException">检索了属性但没有找到 key。</exception>
        TValue this[TKey key] { get; }
        /// <summary>
        /// 获取包含只读字典中的键的可枚举集合。
        /// </summary>
        /// <returns>包含只读字典中的键的可枚举集合。</returns>
        IEnumerable<TKey> Keys { get; }
        /// <summary>
        /// 获取包含只读字典中的值的可枚举集合。
        /// </summary>
        /// <returns>包含只读字典中的值的可枚举集合。</returns>
        IEnumerable<TValue> Values { get; }
        /// <summary>
        /// 确定只读字典是否包含具有指定键的元素。
        /// </summary>
        /// <param name="key">要定位的键。</param>
        /// <returns>如果该只读词典包含一具有指定键的元素，则为 true；否则为 false。</returns>
        /// <exception cref="ArgumentNullException">key 为 null。</exception>
        bool ContainsKey(TKey key);
        /// <summary>
        /// 获取与指定的键关联的值。
        /// </summary>
        /// <param name="key">要定位的键。</param>
        /// <param name="value">当此方法返回时，如果找到指定键，则返回与该键相关联的值；否则，将返回 value 参数的类型的默认值。 此参数未经初始化即被传递。</param>
        /// <returns>如果实现 System.Collections.Generic.IReadOnlyDictionary`2 接口的对象包含具有指定键的元素，则为 true；否则为false。</returns>
        /// <exception cref="ArgumentNullException">key 为 null。</exception>
        bool TryGetValue(TKey key, out TValue value);
    }
}
namespace System.Collections.ObjectModel
{
    /// <summary>
    /// 只读集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class EReadOnlyCollection<T> : ReadOnlyCollection<T>, IList<T>, IList, IReadOnlyList<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="list"></param>
        public EReadOnlyCollection(IList<T> list) : base(list) { }
    }
    /// <summary>
    /// 表示元素的强类型化只读集合。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> m_dictionary;
        [NonSerialized]
        private Object m_syncRoot;
        [NonSerialized]
        private KeyCollection m_keys;
        [NonSerialized]
        private ValueCollection m_values;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dictionary"></param>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            Contract.EndContractBlock();
            m_dictionary = dictionary;
        }
        /// <summary>
        /// 字典
        /// </summary>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return m_dictionary; }
        }
        /// <summary>
        /// 值
        /// </summary>
        public KeyCollection Keys
        {
            get
            {
                Contract.Ensures(Contract.Result<KeyCollection>() != null);
                if (m_keys == null)
                {
                    m_keys = new KeyCollection(m_dictionary.Keys);
                }
                return m_keys;
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public ValueCollection Values
        {
            get
            {
                Contract.Ensures(Contract.Result<ValueCollection>() != null);
                if (m_values == null)
                {
                    m_values = new ValueCollection(m_dictionary.Values);
                }
                return m_values;
            }
        }

        #region IDictionary<TKey, TValue> Members
        /// <summary>
        /// 包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return m_dictionary.ContainsKey(key);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return Keys;
            }
        }
        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return Values;
            }
        }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                return m_dictionary[key];
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return m_dictionary[key];
            }
            set
            {
                throw new NotSupportedException("尝试修改只读字典");
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members
        /// <summary>
        /// 计数
        /// </summary>
        public int Count
        {
            get { return m_dictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            m_dictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_dictionary).GetEnumerator();
        }

        #endregion

        #region IDictionary Members

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key不能为空");
            }
            return key is TKey;
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        void IDictionary.Clear()
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        bool IDictionary.Contains(object key)
        {
            return IsCompatibleKey(key) && ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            IDictionary d = m_dictionary as IDictionary;
            if (d != null)
            {
                return d.GetEnumerator();
            }
            return new DictionaryEnumerator(m_dictionary);
        }

        bool IDictionary.IsFixedSize
        {
            get { return true; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return true; }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return Keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException("尝试修改只读字典");
        }

        ICollection IDictionary.Values
        {
            get
            {
                return Values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    return this[(TKey)key];
                }
                return null;
            }
            set
            {
                throw new NotSupportedException("尝试修改只读字典");
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array不能为空");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("array的秩(维数)[Rank]不是1");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("array不是从零开始的数组");
            }

            if (index < 0 || index > array.Length)
            {
                throw new IndexOutOfRangeException("索引超出范围");
            }

            if (array.Length - index < Count)
            {
                throw new IndexOutOfRangeException("索引超出范围");
            }

            KeyValuePair<TKey, TValue>[] pairs = array as KeyValuePair<TKey, TValue>[];
            if (pairs != null)
            {
                m_dictionary.CopyTo(pairs, index);
            }
            else
            {
                DictionaryEntry[] dictEntryArray = array as DictionaryEntry[];
                if (dictEntryArray != null)
                {
                    foreach (var item in m_dictionary)
                    {
                        dictEntryArray[index++] = new DictionaryEntry(item.Key, item.Value);
                    }
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArrayTypeMismatchException();
                    }

                    try
                    {
                        foreach (var item in m_dictionary)
                        {
                            objects[index++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
                        }
                    }
                    catch (ArrayTypeMismatchException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (m_syncRoot == null)
                {
                    ICollection c = m_dictionary as ICollection;
                    if (c != null)
                    {
                        m_syncRoot = c.SyncRoot;
                    }
                    else
                    {
                        System.Threading.Interlocked.CompareExchange<Object>(ref m_syncRoot, new Object(), null);
                    }
                }
                return m_syncRoot;
            }
        }

        [Serializable]
        private struct DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IDictionary<TKey, TValue> m_dictionary;
            private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

            public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
            {
                m_dictionary = dictionary;
                m_enumerator = m_dictionary.GetEnumerator();
            }

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(m_enumerator.Current.Key, m_enumerator.Current.Value); }
            }

            public object Key
            {
                get { return m_enumerator.Current.Key; }
            }

            public object Value
            {
                get { return m_enumerator.Current.Value; }
            }

            public object Current
            {
                get { return Entry; }
            }

            public bool MoveNext()
            {
                return m_enumerator.MoveNext();
            }

            public void Reset()
            {
                m_enumerator.Reset();
            }
        }

        #endregion

        #region IReadOnlyDictionary members

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return Keys;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return Values;
            }
        }

        #endregion IReadOnlyDictionary members
        /// <summary>
        /// 键列表
        /// </summary>
        [Serializable]
        public sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
        {
            private readonly ICollection<TKey> m_collection;
            [NonSerialized]
            private Object m_syncRoot;

            internal KeyCollection(ICollection<TKey> collection)
            {
                if (collection == null)
                {
                    throw new ArgumentNullException("键集合不能为null");
                }
                m_collection = collection;
            }

            #region ICollection<T> Members

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException("尝试修改只读字典键");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("尝试修改只读字典键");
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return m_collection.Contains(item);
            }
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                m_collection.CopyTo(array, arrayIndex);
            }
            /// <summary>
            /// 计数
            /// </summary>
            public int Count
            {
                get { return m_collection.Count; }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException("尝试修改只读字典键");
            }

            #endregion

            #region IEnumerable<T> Members
            /// <summary>
            /// 迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TKey> GetEnumerator()
            {
                return m_collection.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)m_collection).GetEnumerator();
            }

            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int index)
            {
                CopyToNonGenericICollectionHelper<TKey>(m_collection, array, index);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    if (m_syncRoot == null)
                    {
                        ICollection c = m_collection as ICollection;
                        if (c != null)
                        {
                            m_syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            System.Threading.Interlocked.CompareExchange<Object>(ref m_syncRoot, new Object(), null);
                        }
                    }
                    return m_syncRoot;
                }
            }

            #endregion
        }
        /// <summary>
        /// 值列表
        /// </summary>
        [Serializable]
        public sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
        {
            private readonly ICollection<TValue> m_collection;
            [NonSerialized]
            private Object m_syncRoot;

            internal ValueCollection(ICollection<TValue> collection)
            {
                if (collection == null)
                {
                    throw new ArgumentNullException("值集合不能为null");
                }
                m_collection = collection;
            }

            #region ICollection<T> Members

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("尝试修改只读字典值");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("尝试修改只读字典值");
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return m_collection.Contains(item);
            }
            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                m_collection.CopyTo(array, arrayIndex);
            }
            /// <summary>
            /// 计数
            /// </summary>
            public int Count
            {
                get { return m_collection.Count; }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("尝试修改只读字典值");
            }

            #endregion

            #region IEnumerable<T> Members
            /// <summary>
            /// 迭代器
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                return m_collection.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)m_collection).GetEnumerator();
            }

            #endregion

            #region ICollection Members

            void ICollection.CopyTo(Array array, int index)
            {
                CopyToNonGenericICollectionHelper<TValue>(m_collection, array, index);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    if (m_syncRoot == null)
                    {
                        ICollection c = m_collection as ICollection;
                        if (c != null)
                        {
                            m_syncRoot = c.SyncRoot;
                        }
                        else
                        {
                            System.Threading.Interlocked.CompareExchange<Object>(ref m_syncRoot, new Object(), null);
                        }
                    }
                    return m_syncRoot;
                }
            }

            #endregion ICollection Members
        }

        #region Helper method for our KeyCollection and ValueCollection

        // Abstracted away to avoid redundant implementations.
        internal static void CopyToNonGenericICollectionHelper<T>(ICollection<T> collection, Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array不能为空");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("array的秩(维数)[Rank]不是1");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("array不是从零开始的数组");
            }

            if (index < 0)
            {
                throw new IndexOutOfRangeException("索引超出范围");
            }

            if (array.Length - index < collection.Count)
            {
                throw new IndexOutOfRangeException("索引超出范围");
            }

            // Easy out if the ICollection<T> implements the non-generic ICollection
            ICollection nonGenericCollection = collection as ICollection;
            if (nonGenericCollection != null)
            {
                nonGenericCollection.CopyTo(array, index);
                return;
            }

            T[] items = array as T[];
            if (items != null)
            {
                collection.CopyTo(items, index);
            }
            else
            {
                //
                // Catch the obvious case assignment will fail.
                // We can found all possible problems by doing the check though.
                // For example, if the element type of the Array is derived from T,
                // we can't figure out if we can successfully copy the element beforehand.
                //
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                {
                    throw new ArrayTypeMismatchException();
                }

                //
                // We can't cast array of value type to object[], so we don't support 
                // widening of primitive types here.
                //
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArrayTypeMismatchException();
                }

                try
                {
                    foreach (var item in collection)
                    {
                        objects[index++] = item;
                    }
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw ex;
                }
            }
        }

        #endregion Helper method for our KeyCollection and ValueCollection
    }
}
#endif
