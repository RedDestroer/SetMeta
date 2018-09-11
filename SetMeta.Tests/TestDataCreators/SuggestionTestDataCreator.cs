using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;

namespace SetMeta.Tests.TestDataCreators
{
    public class SuggestionTestDataCreator
        : ISuggestionTestDataCreator
    {
        public ISuggestionTestDataCreator WithMinLength(string value)
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithMaxLength(string value)
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithMultiline()
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithMinLines(string value)
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithMaxLines(string value)
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator Regex(string value, string validation)
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithNotifyOnChange()
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithNotifiable()
        {
            throw new System.NotImplementedException();
        }

        public ISuggestionTestDataCreator WithControl(string name)
        {
            throw new System.NotImplementedException();
        }

        public XElement Build(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}