/// <summary>
/// Interface for marking a class to be able to be poolable.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when the object was gotten from a pool.
    /// </summary>
    void Show();

    /// <summary>
    /// Called when the object is placed back into a pool.
    /// </summary>
    void Hide();

    /// <summary>
    /// Called when the object should be deleted.
    /// </summary>
    void Delete();
}
