using Authentication.Repositories.Interfaces.SLTs;

namespace Authentication.Repositories.Implementations.SLTs
{
    public class Singleton : ISingleton
    {
        public Guid Guid { get; set; }

        public Singleton()
        {
            Guid = Guid.NewGuid();
        }

        public Guid GenerateID() => Guid;
    }
}
