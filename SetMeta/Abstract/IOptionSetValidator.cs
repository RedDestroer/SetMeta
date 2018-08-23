using System.Xml;

namespace SetMeta.Abstract
{
    public interface IOptionSetValidator
    {
        void AddError(string message, IXmlLineInfo xmlLineInfo = null);
    }
}