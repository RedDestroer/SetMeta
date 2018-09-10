using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IListItemTestDataCreator
    {
        IListItemTestDataCreator WithValue(string value);
        IListItemTestDataCreator WithDisplayValue(string displayValue);
        XElement Build();
    }
}