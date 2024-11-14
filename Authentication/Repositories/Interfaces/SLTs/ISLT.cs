namespace Authentication.Repositories.Interfaces.SLTs
{
    /// <summary>
    /// Service Life Time Interface
    /// </summary>
    public interface ISLT : IExample
    {
        Guid GenerateID();
    }
}
