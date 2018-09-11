using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Entities;
using SetMeta.Tests.TestDataCreators.Abstract;
using SetMeta.Util;
using SetMeta.XmlKeys;

namespace SetMeta.Tests.TestDataCreators
{
    public class OptionSetV1TestDataCreator
        : IOptionSetV1TestDataCreator
    {
        private readonly IList<XElement> _elements;

        public OptionSetV1TestDataCreator()
        {
            _elements = new List<XElement>();
        }

        public IOptionSetV1TestDataCreator WithElement(XElement element)
        {
            _elements.Add(element);

            return this;
        }

        public XDocument Build()
        {
            var body = new XElement(OptionSetElement.ElementName,
                new XAttribute(OptionSetElement.Attrs.Version, "1"));
            ////new XAttribute("xmlns", "http://tempuri.org")
            ////new XAttribute("xmlns:xsi", "http://tempuri.org/2018/Option/XMLSchema-instance"),
            ////new XAttribute("xsi:schemaLocation", "http://tempuri.org OptionSetV1.xsd"));

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _elements.ForEach<XElement>(element => body.Add(element));

            var document = new XDocument();
            document.Add(body);

            return document;
        }
    }
}