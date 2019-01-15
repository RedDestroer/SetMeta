using System.Collections.Generic;
using System.Xml;
using SetMeta.Abstract;
using SetMeta.Util;

namespace SetMeta.Impl
{
    public class DefaultOptionSetValidator
        : IOptionSetValidator
    {
        private readonly IList<KeyValuePair<string, IXmlLineInfo>> _validationResults;

        public DefaultOptionSetValidator()
        {
            _validationResults = new List<KeyValuePair<string, IXmlLineInfo>>();
        }

        public IList<KeyValuePair<string, IXmlLineInfo>> ValidationResults => _validationResults;

        /// <inheritdoc />
        public void AddError(string message, IXmlLineInfo xmlLineInfo = null)
        {
            Validate.NotNull(message, nameof(message));

            _validationResults.Add(new KeyValuePair<string, IXmlLineInfo>(message, xmlLineInfo));
        }
    }
}