using System;
using System.Linq;
using System.Xml.Linq;

namespace SetMeta.Util
{
    public static class XElementExtension
    {
        public static XElement GetNode(this XElement element, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!TryGetNode(element, name, out var answer))
                throw new InvalidOperationException($"Не найдена нода '{name}'.");

            return answer;
        }

        public static XElement GetNode(this XElement element, string name, string attributeName, string attributeValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!TryGetNode(element, name, attributeName, attributeValue, out var answer))
                throw new InvalidOperationException($"Не найдена нода '{name}' содержащая аттрибут '{attributeName}' со значением '{attributeValue}'.");

            return answer;
        }

        public static bool TryGetNode(this XElement root, string name, out XElement result)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            result = IsNodeExists(root, name)
                         ? root.Elements(name).FirstOrDefault()
                         : null;

            return result != null;
        }

        public static bool TryGetNode(this XElement root, string name, string attributeName, string attributeValue, out XElement result)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));

            result = IsNodeExists(root, name, attributeName, attributeValue)
                ? root.Elements(name)
                    .FirstOrDefault(x =>
                    {
                        var attr = x.Attribute(attributeName);
                        if (attr == null)
                            return false;

                        return attr.Value == attributeValue;
                    })
                : null;

            return result != null;
        }

        public static bool HaveAttributeWithValue(this XElement element, string attributeName, string attributeValue)
        {
            if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));

            return element
                .Attributes()
                .Any(at => at.Name == attributeName && at.Value == attributeValue);
        }

        public static bool IsAttributeExists(this XElement parent, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return parent.Attributes().Any(obj => obj.Name == name);
        }

        public static bool IsNodeExists(this XElement parent, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return parent.Elements(name).Any();
        }

        public static bool IsNodeExists(this XElement parent, string name, string attributeName, string attributeValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));

            return parent.Elements(name)
                .Any(el => el.HaveAttributeWithValue(attributeName, attributeValue));
        }

        public static T GetNodeValue<T>(this XElement root, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var node = GetNode(root, name);

            return DataConversion.Convert<T>(node.Value);
        }

        public static T TryGetNodeValue<T>(this XElement root, string name, T defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var node = IsNodeExists(root, name)
                         ? root.Elements(name).FirstOrDefault()
                         : null;

            if (node == null)
                return defaultValue;

            return DataConversion.Convert<T>(node.Value);
        }

        public static bool TryGetNodeValue<T>(this XElement root, string name, T defaultValue, out T nodeValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            try
            {
                nodeValue = TryGetNodeValue(root, name, defaultValue);
                return true;
            }
            catch (Exception)
            {
                nodeValue = default(T);
                return false;
            }
        }

        public static XElement GetOrAddNode(this XElement root, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!IsNodeExists(root, name))
            {
                var xElement = new XElement(name);

                root.Add(xElement);

                return xElement;
            }

            return GetNode(root, name);
        }

        public static bool TryGetOrAddNode(this XElement root, string name, out XElement result)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!IsNodeExists(root, name))
                root.Add(new XElement(name));

            result = root.Elements(name).FirstOrDefault();

            return result != null;
        }

        public static XAttribute GetAttribute(this XElement root, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return root.Attributes(name).First();
        }

        public static bool TryGetAttribute(this XElement root, string name, out XAttribute result)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            result = IsAttributeExists(root, name)
                         ? root.Attributes(name).FirstOrDefault()
                         : null;

            return result != null;
        }

        public static T GetAttributeValue<T>(this XElement root, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!TryGetAttribute(root, name, out var attribute))
                throw new InvalidOperationException($"Не удалось найти ноду с именем '{name}'.");

            return DataConversion.Convert<T>(attribute.Value);
        }

        public static bool TryGetAttributeValue<T>(this XElement root, string name, out T attributeValue)
        {
            return root.TryGetAttributeValue(name, default(T), out attributeValue);
        }

        public static bool TryGetAttributeValue<T>(this XElement root, string name, T defaultValue, out T attributeValue)
        {
            if (name == null) throw new ArgumentNullException(
                nameof(name));

            try
            {
                attributeValue = TryGetAttributeValue(root, name, defaultValue);
                return true;
            }
            catch (Exception)
            {
                attributeValue = default(T);
                return false;
            }
        }

        public static T TryGetAttributeValue<T>(this XElement root, string name, T defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            try
            {
                if (!TryGetAttribute(root, name, out var attribute))
                    return defaultValue;

                if (DataConversion.TryConvert(attribute.Value, out T result))
                    return result;

                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string AsString(this XElement element)
        {
            return element?.ToString();
        }
    }
}