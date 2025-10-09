using System.Collections.Concurrent;

namespace ClassScheduler.WPF.Utils;

public class FixedSizedQueue<T> : ConcurrentQueue<T>
{
    private readonly object syncObject = new();

    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);

        lock (syncObject)
        {
            while (Count > Size)
            {
                TryDequeue(out T outObj);
            }
        }
    }
}
