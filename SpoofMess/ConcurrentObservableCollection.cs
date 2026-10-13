using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SpoofMess;

public class ConcurrentObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly List<T> _collection = [];

    public int Count => _collection.Count;

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => _collection[index];
        set
        {
            T oldItem = _collection[index];
            _collection[index] = value;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
        }
    }

    public void Add(T item)
    {
        lock (_collection)
        {
            _collection.Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, _collection.Count - 1);
            OnPropertyChanged(nameof(Count));
        }
    }

    public int IndexOf(T item)
    {
        lock (_collection)
        {
            return _collection.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_collection)
        {
            _collection.Insert(index, item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            OnPropertyChanged(nameof(Count));
        }
    }

    public void RemoveAt(int index)
    {
        lock (_collection)
        {
            T item = _collection[index];
            _collection.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }
    }

    public void Clear()
    {
        lock (_collection)
        {
            _collection.Clear();
        }
        OnCollectionReset();
        OnPropertyChanged(nameof(Count));
    }

    public bool Contains(T item)
    {
        lock (_collection)
        {
            return _collection.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_collection)
        {
            _collection.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(T item)
    {
        bool result;
        lock (_collection)
        {
            result = _collection.Remove(item);
        }
        if (result)
            OnPropertyChanged(nameof(Count));
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_collection)
        {
            return _collection.GetEnumerator();
        }
    }

    public void Move(int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex)
            return;
        lock (_collection)
        {
            T item = _collection[oldIndex];
            _collection.RemoveAt(oldIndex);
            _collection.Insert(newIndex, item);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index, int oldIndex)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionReset()
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
