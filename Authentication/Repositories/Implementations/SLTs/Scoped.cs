using Authentication.Repositories.Interfaces.SLTs;

namespace Authentication.Repositories.Implementations.SLTs
{
    public class Scoped : IScoped
    {
        public Guid Guid { get; set; }

        public Scoped()
        {
            Guid = Guid.NewGuid();
        }

        public Guid GenerateID() => Guid;
    }
}
