using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IOptionTestDataCreator
    {
        IOptionTestDataCreator WithDisplayName(string displayName);
        IOptionTestDataCreator WithDescription(string description);
        IOptionTestDataCreator WithDefaultValue(string defaultValue, bool asElement = true);
        IOptionTestDataCreator WithValueType(OptionValueType valueType);
        IOptionTestDataCreator WithBehaviour(XElement behaviourElement);
        IOptionTestDataCreator WithAttribute(XAttribute attribute);
        IOptionTestDataCreator WithElement(XElement element);
        XElement Build(string name);
    }
}