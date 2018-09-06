using System.Xml.Linq;
using ConstantElement = SetMeta.Entities.OptionSetElement.ConstantElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class ConstantTestDataCreator
        : IConstantTestDataCreator
    {
        public XElement Build(string name, string value, string valueType)
        {
            var element = new XElement(
                ConstantElement.ElementName, 
                new XAttribute(ConstantElement.Attrs.Name, name),
                new XAttribute(ConstantElement.Attrs.Value, value),
                new XAttribute(ConstantElement.Attrs.ValueType, valueType));

            return element;
        }
    }
}