using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using ConstantElement = SetMeta.XmlKeys.OptionSetElement.ConstantElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class ConstantTestDataCreator
        : IConstantTestDataCreator
    {
        private string _value;

        public IConstantTestDataCreator WithValue(string value)
        {
            _value = value;

            return this;
        }

        public XElement Build(string name)
        {
            var body = new XElement(ConstantElement.ElementName, new XAttribute(ConstantElement.Attrs.Name, name));

            if (_value != null)
                body.Add(new XAttribute(ConstantElement.Attrs.Value, _value));

            return body;
        }
    }
}