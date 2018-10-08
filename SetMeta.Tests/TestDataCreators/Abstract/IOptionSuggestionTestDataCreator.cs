using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IOptionSuggestionTestDataCreator
    {
        IOptionSuggestionTestDataCreator WithMinLength(string length);
        IOptionSuggestionTestDataCreator WithMaxLength(string length);
        IOptionSuggestionTestDataCreator WithMinLines(string length);
        IOptionSuggestionTestDataCreator WithMaxLines(string length);
        IOptionSuggestionTestDataCreator WithMultiLine();
        IOptionSuggestionTestDataCreator WithNotifiable();
        IOptionSuggestionTestDataCreator WithNotifyOnChange();
        IOptionSuggestionTestDataCreator WithRegex(string value, string validation);

        XElement Build(string name);
    }
}