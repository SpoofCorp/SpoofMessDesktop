namespace SpoofMess;

public static class CollectionHelper
{
    extension<T>(ICollection<T> target)
    {
        public void ClearAndAddRange(ICollection<T> newCollection)
        {
            target.Clear();
            foreach (T item in newCollection)
                target.Add(item);
        }
    }
}