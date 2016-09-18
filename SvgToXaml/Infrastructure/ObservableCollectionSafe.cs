using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace SvgToXaml.Infrastructure
{
    /// <summary>
    /// ObservableCollection, die Threadsicher ist
    /// Benutzung:
    /// BindingOperations.EnableCollectionSynchronization(Items, null, Items.HandleSynchonizationCallback);
    /// Dadurch kümmert sich das Framework um die korrekte synchronisierung der Callbacks, d.h. man kann auch 
    /// im Thread auf die Liste zugreifen und sowohl der Listenkram, als auch die Benachrichtigung und
    /// Aktualisierung der GUI klappt.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableCollectionSafe<T> : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>, IList, IReadOnlyList<T>, IDisposable
    {
        private readonly Collection<T> _coll;
        // ReSharper disable once InconsistentNaming
        internal ReaderWriterLockSlim _lock;
        private volatile bool _isInCollectionChanged;
        private volatile bool _inBatchUpdate;

        #region Constructor

        public ObservableCollectionSafe()
        {
            _coll = new Collection<T>();
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public ObservableCollectionSafe(IList<T> list)
            : this()
        {
            CopyFrom(list);
        }

        public ObservableCollectionSafe(IEnumerable<T> items)
            : this()
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            CopyFrom(items);
        }

        private void CopyFrom(IEnumerable<T> items)
        {
            if (items != null)
            {
                using (IEnumerator<T> enumerator = items.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        _coll.Add(enumerator.Current);
                    }
                }
            }
        }

        #endregion
        public void Dispose()
        {
            _lock.Dispose();
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                _isInCollectionChanged = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _isInCollectionChanged = false;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Write Access

        public void Move(int oldIndex, int newIndex)
        {
            WriteAccess(false, (ref object dummy) =>
            {
                T removedItem = _coll[oldIndex];

                _coll.RemoveAt(oldIndex);
                _coll.Insert(newIndex, removedItem);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
            });
            //_lock.EnterWriteLock();
            //try
            //{
            //    T removedItem = _coll[oldIndex];

            //    _coll.RemoveAt(oldIndex);
            //    _coll.Insert(newIndex, removedItem);

            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex));
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }

        public int Add(T item)
        {
            // ReSharper disable once RedundantAssignment
            return WriteAccess(true, (ref int index) =>
            {
                _coll.Add(item);
                index = _coll.Count - 1;
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            });

            //_lock.EnterWriteLock();
            //try
            //{
            //    _coll.Add(item);
            //    var index = _coll.Count - 1;
            //    OnPropertyChanged(CountString);
            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            //    return index;
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }

        public void AddRange(IEnumerable<T> items)
        {
            WriteAccess(true, (ref object dummy) =>
            {
                foreach (var item in items)
                {
                    _coll.Add(item);
                }
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            });
        }


        public void RemoveAt(int index)
        {
            WriteAccess(true, (ref object dummy) =>
            {
                T removedItem = _coll[index];
                _coll.RemoveAt(index);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index);
            });
            //_lock.EnterWriteLock();
            //try
            //{
            //    T removedItem = _coll[index];
            //    _coll.RemoveAt(index);

            //    OnPropertyChanged(CountString);
            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }
        public bool Remove(T item)
        {
            // ReSharper disable once RedundantAssignment
            return WriteAccess(true, (ref bool result) =>
            {
                int index = _coll.IndexOf(item);
                if (index < 0)
                {
                    result = false;
                    return null;
                }
                _coll.RemoveAt(index);
                result = true;
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            });

            //_lock.EnterWriteLock();
            //try
            //{
            //    int index = _coll.IndexOf(item);
            //    if (index < 0) return false;
            //    _coll.RemoveAt(index);

            //    OnPropertyChanged(CountString);
            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

            //    return true;
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            WriteAccess(true, (ref object dummy) =>
            {
                foreach (var item in items)
                {
                    _coll.Remove(item);
                }
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            });
        }

        public void Clear()
        {
            WriteAccess(true, (ref object dummy) =>
            {
                _coll.Clear();
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            });
            //_lock.EnterWriteLock();
            //try
            //{
            //    _coll.Clear();

            //    OnPropertyChanged(CountString);
            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }

        public void Insert(int index, T item)
        {
            WriteAccess(true, (ref object dummy) =>
            {
                _coll.Insert(index, item);
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            });
            //_lock.EnterWriteLock();
            //try
            //{
            //    _coll.Insert(index, item);

            //    OnPropertyChanged(CountString);
            //    OnPropertyChanged(IndexerName);
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}
        }

        delegate NotifyCollectionChangedEventArgs WriteAccessFunc<TResult>(ref TResult result);
        private TResult WriteAccess<TResult>(bool countChanged, WriteAccessFunc<TResult> action)
        {
            TResult result = default(TResult);
            _lock.EnterWriteLock();
            try
            {
                NotifyCollectionChangedEventArgs args = action(ref result);

                if (args == null)
                    return result; //skip Update
                if (!_inBatchUpdate)
                {
                    if (countChanged)
                        OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                    OnCollectionChanged(args);
                }
                return result;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IDisposable BatchUpdate()
        {
            if (_inBatchUpdate)
                throw new Exception("BatchUpdate already in progress");
            _inBatchUpdate = true;
            return new CustomDisposable(() =>
            {
                _inBatchUpdate = false;
                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        #endregion

        public T this[int index]
        {
            get
            {
                return ReadAccess(() => _coll[index]);
                //_lock.EnterReadLock();
                //try
                //{
                //    return _coll[index];
                //}
                //finally
                //{
                //    _lock.ExitReadLock();
                //}
            }
            set
            {
                WriteAccess(false, (ref object dummy) =>
                {
                    T originalItem = this[index];
                    _coll[index] = value;
                    return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, originalItem, value, index);
                });
                //_lock.EnterWriteLock();
                //try
                //{
                //    T originalItem = this[index];
                //    _coll[index] = value;

                //    OnPropertyChanged(IndexerName);
                //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, originalItem, value, index));
                //}
                //finally
                //{
                //    _lock.ExitWriteLock();
                //}
            }
        }

        #region Read Access
        public void CopyTo(T[] array, int index)
        {
            ReadAccess<object>(() =>
            {
                _coll.CopyTo(array, index);
                return null;
            });
            //_lock.EnterReadLock();
            //try
            //{
            //    _coll.CopyTo(array, index);
            //}
            //finally
            //{
            //    _lock.ExitReadLock();
            //}
        }
        public void CopyTo(Array array, int index)
        {
            ReadAccess<object>(() =>
            {
                ((ICollection)_coll).CopyTo(array, index);
                return null;
            });
            //_lock.EnterReadLock();
            //try
            //{
            //    _coll.CopyTo(array, index);
            //}
            //finally
            //{
            //    _lock.ExitReadLock();
            //}
        }
        public bool Contains(T item)
        {
            return ReadAccess(() => _coll.Contains(item));
            //_lock.EnterReadLock();
            //try
            //{
            //    return _coll.Contains(item);
            //}
            //finally
            //{
            //    _lock.ExitReadLock();
            //}
        }
        public int IndexOf(T item)
        {
            return ReadAccess(() => _coll.IndexOf(item));
            //_lock.EnterReadLock();
            //try
            //{
            //    return _coll.IndexOf(item);
            //}
            //finally
            //{
            //    _lock.ExitReadLock();
            //}
        }

        public int Count
        {
            get
            {
                return ReadAccess(() => _coll.Count);
                //_lock.EnterReadLock();
                //try
                //{
                //    return _;
                //}
                //finally
                //{
                //    _lock.ExitReadLock();
                //}
            }
        }

        private TResult ReadAccess<TResult>(Func<TResult> action)
        {
            _lock.EnterReadLock();
            try
            {
                return action();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        public void HandleSynchonizationCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            if (!Equals(collection))
                throw new Exception("Collection does not match");
            if (_isInCollectionChanged)
            {
                if (writeAccess && !_lock.IsWriteLockHeld)
                    throw new Exception("When calling CollectionChanged you should use Write Lock");
                accessMethod();
            }
            else
            {   //wird beim Start aufgerufen, und z.B. beim Clear im Thread, dann kommt der Refresh später hier durch, 
                //bei einfachen Add etc. kommt man hier gar nicht durch, dies macht die Shadow-Liste alles selbst
                if (writeAccess)
                    _lock.EnterWriteLock();
                else
                    _lock.EnterReadLock();

                accessMethod();

                if (writeAccess)
                    _lock.ExitWriteLock();
                else
                    _lock.ExitReadLock();
            }
        }

        //#region BatchUpdate

        //private bool _inBatchUpdate;

        //public IDisposable BatchUpdate()
        //{
        //    BeginBatch();
        //    return new UpdateDisposable(this);
        //}

        //public void AddRange(IEnumerable<T> items)
        //{
        //    if (items == null)
        //        return;

        //    BeginBatch();

        //    foreach (T item in items)
        //    {
        //        Add(item);
        //    }
        //    EndBatch();
        //}

        //public void RemoveRange(IEnumerable<T> items)
        //{
        //    if (items == null)
        //        return;

        //    BeginBatch();

        //    foreach (T item in items)
        //    {
        //        Remove(item);
        //    }
        //    EndBatch();
        //}

        //private void BeginBatch()
        //{
        //    if (_inBatchUpdate)
        //    {
        //        throw new InvalidOperationException("Batch update already in progress");
        //    }
        //    _inBatchUpdate = true;
        //}

        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    if (!_inBatchUpdate)
        //    {
        //        base.OnCollectionChanged(e);
        //    }
        //}

        //protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        //{
        //    if (!_inBatchUpdate)
        //    {
        //        base.OnPropertyChanged(e);
        //    }
        //}

        //private void EndBatch()
        //{
        //    if (_inBatchUpdate)
        //    {
        //        _inBatchUpdate = false;
        //        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        //        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        //          NotifyCollectionChangedAction.Reset));
        //    }
        //}

        //private class UpdateDisposable : IDisposable
        //{
        //    private readonly ObservableCollectionSafe<T> _parent;

        //    public UpdateDisposable(ObservableCollectionSafe<T> parent)
        //    {
        //        _parent = parent;
        //    }

        //    public void Dispose()
        //    {
        //        _parent.EndBatch();
        //    }
        //}
        //#endregion

        
        public class CustomDisposable : IDisposable
        {
            private readonly Action _action;

            public CustomDisposable(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        #region Interface stuff

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ReadAccess(() => new List<T>(_coll).GetEnumerator()); //return a copy
            //_lock.EnterReadLock();
            //try
            //{
            //    return new List<T>(_coll).GetEnumerator(); //return a copy
            //}
            //finally
            //{
            //    _lock.ExitReadLock();
            //}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        int IList<T>.IndexOf(T item)
        {
            return IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count => Count;

        bool ICollection<T>.IsReadOnly => ((IList<T>)_coll).IsReadOnly;

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        bool IList.IsFixedSize => ((IList)_coll).IsFixedSize;

        bool IList.IsReadOnly => ((IList)_coll).IsReadOnly;

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        int IList.Add(object value)
        {
            return Add((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo(array, index);
        }

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => ((IList)_coll).IsFixedSize;

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException("This ObservableCollection doesn't need external synchronization"); }
        }

        T IReadOnlyList<T>.this[int index] => this[index];

        int IReadOnlyCollection<T>.Count => Count;

        #endregion

        internal const string CountString = "Count";

        internal const string IndexerName = "Item[]";

    }
}
