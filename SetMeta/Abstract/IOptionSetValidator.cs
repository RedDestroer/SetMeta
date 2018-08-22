using System.Xml;

namespace SetMeta.Abstract
{
    public interface IOptionSetValidator
    {
        void AddInformation(string message, IXmlLineInfo xmlLineInfo = null);
        void AddWarning(string message, IXmlLineInfo xmlLineInfo = null);
        void AddError(string message, IXmlLineInfo xmlLineInfo = null);
    }
}