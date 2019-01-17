using SetMeta.Abstract;

namespace SetMeta.Impl
{
    public class DefaultIdFactory
        : IIdFactory
    {
        /// <inheritdoc />
        public string CreateId(string data)
        {
            return (data ?? string.Empty).ToLowerInvariant();
        }
    }
}