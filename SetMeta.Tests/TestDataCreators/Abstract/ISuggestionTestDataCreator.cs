using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface ISuggestionTestDataCreator
    {
        ISuggestionTestDataCreator WithMinLength(string value);
        ISuggestionTestDataCreator WithMaxLength(string value);
        ISuggestionTestDataCreator WithMultiline();
        ISuggestionTestDataCreator WithMinLines(string value);
        ISuggestionTestDataCreator WithMaxLines(string value);
        ISuggestionTestDataCreator Regex(string value, string validation);
        ISuggestionTestDataCreator WithNotifyOnChange();
        ISuggestionTestDataCreator WithNotifiable();
        ISuggestionTestDataCreator WithControl(string name);
        XElement Build(string name);
    }
}