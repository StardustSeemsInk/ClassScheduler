namespace ClassScheduler.WPF;

internal class Events
{
    public delegate void OnSetRootPathHandler();

    public static event OnSetRootPathHandler? OnSetRootPath;

    public static void InitEvents()
    {
        OnSetRootPath += () => { };
    }

    public static void Invoke(string name)
    {
        switch (name)
        {
            case nameof(OnSetRootPath):
                OnSetRootPath?.Invoke();
                break;
        }
    }
}
