using System.Collections.Generic;
using System.Xml.Linq;
using OptionElement = SetMeta.XmlKeys.OptionSetElement.GroupElement.OptionElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class GroupOptionTestDataCreator
        : IGroupOptionTestDataCreator
    {
        private readonly IList<XElement> _elements = new List<XElement>();

        public IGroupOptionTestDataCreator WithGroupOption(XElement element)
        {
            _elements.Add(element);

            return this;
        }

        public XElement Build(string name)
        {
            var element = new XElement(OptionElement.ElementName, new XAttribute(OptionElement.Attrs.Name, name));

            foreach (var xElement in _elements)
            {
                element.Add(xElement);
            }

            return element;
        }
    }
}