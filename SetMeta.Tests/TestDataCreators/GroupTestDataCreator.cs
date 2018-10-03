using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using GroupElement = SetMeta.XmlKeys.OptionSetElement.GroupElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class GroupTestDataCreator
        : IGroupTestDataCreator
    {
        private readonly IList<XElement> _elements = new List<XElement>();

        private string _displayName;
        private string _description;
        
        public IGroupTestDataCreator WithElement(XElement element)
        {
            _elements.Add(element);

            return this;
        }

        public IGroupTestDataCreator WithDisplayName(string displayName)
        {
            _displayName = displayName;

            return this;
        }

        public IGroupTestDataCreator WithDescription(string description)
        {
            _description = description;

            return this;
        }

        public XElement Build(string name)
        {
            var element = new XElement(GroupElement.ElementName, new XAttribute(GroupElement.Attrs.Name, name));

            if (_displayName != null)
                element.Add(new XAttribute(GroupElement.Attrs.DisplayName, _displayName));
            if (_description != null)
                element.Add(new XAttribute(GroupElement.Attrs.Description, _description));

            foreach (var xElement in _elements)
            {
                element.Add(xElement);
            }

            return element;
        }
    }
}