using Authentication.Repositories.Interfaces.SLTs;

namespace Authentication.Repositories.Implementations.SLTs
{
    public class Transient : ITransient
    {
        public Guid Guid { get; set; }

        public Transient()
        {
            Guid = Guid.NewGuid();
        }

        public Guid GenerateID() => Guid;
    }
}
