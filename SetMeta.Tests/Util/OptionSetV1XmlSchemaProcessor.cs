using System.Collections.Generic;
using System.Xml.Schema;
using XsdIterator;

namespace SetMeta.Tests.Util
{
    public class OptionSetV1XmlSchemaProcessor
        : XmlSchemaDefaultProcessor, IOptionInformant
    {
        private bool _readOptionAttributes;

        public OptionSetV1XmlSchemaProcessor()
        {
            OptionAttributes = new List<XmlSchemaAttribute>();
        }

        public List<XmlSchemaAttribute> OptionAttributes { get; }

        public override bool StartProcessing(XmlSchemaComplexType obj)
        {
            if (obj.Name == "optionType")
            {
                _readOptionAttributes = true;
            }

            return true;
        }

        public override void EndProcessing(XmlSchemaComplexType obj)
        {
            if (obj.Name == "optionType")
            {
                _readOptionAttributes = false;
            }
        }

        public override bool StartProcessing(XmlSchemaAttribute obj)
        {
            if (!_readOptionAttributes)
                return true;

            OptionAttributes.Add(obj);

            return true;
        }
    }

    public interface IOptionInformant
    {
        List<XmlSchemaAttribute> OptionAttributes { get; }
    }
}