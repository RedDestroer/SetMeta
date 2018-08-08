﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SetMeta.Entities;

namespace SetMeta.Abstract
{
    public abstract class OptionSetParser
    {
        private static readonly IDictionary<string, OptionSetParser> OptionSetParsers = new ConcurrentDictionary<string, OptionSetParser>();

        public abstract string Version { get; }

        public static OptionSetParser Create(string version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));

            lock (OptionSetParsers)
            {
                FillParsers();

                if (OptionSetParsers.TryGetValue(version, out OptionSetParser result))
                {
                    return result;
                }
            }
            
            throw new InvalidOperationException($"Can't create '{nameof(OptionSetParser)}' of given version '{version}'");
        }

        public static OptionSetParser Create(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var reader = new XmlTextReader(stream))
            {
                return Create(reader);
            }
        }

        public static OptionSetParser Create(XmlTextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            try
            {
                if (!reader.Read())
                    throw new InvalidOperationException("Can't read data as correct xml.");
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Can't read data as correct xml.", exception);
            }

            if (!reader.IsStartElement())
            {
                while (reader.Read() && !reader.IsStartElement())
                {
                }
            }

            var name = reader.Name;
            if (name != "optionSet")
                throw new InvalidOperationException("Data is not 'optionSet'.");

            if (!reader.MoveToAttribute("version"))
                throw new InvalidOperationException("There is no 'version' attribute.");

            var versionString = reader.GetAttribute("version");
            if (string.IsNullOrEmpty(versionString))
                throw new InvalidOperationException("Serializer version is not set.");

            try
            {
                FillParsers();

                lock (OptionSetParsers)
                {
                    if (OptionSetParsers.TryGetValue(versionString, out OptionSetParser parser))
                        return parser;
                }

                throw new InvalidOperationException($"Serializer for version '{versionString}' not found.");
            }
            finally
            {
                reader.ResetState();
            }
        }

        public OptionSet Parse(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var reader = new XmlTextReader(stream))
            {
                return Parse(reader);
            }
        }

        public OptionSet Parse(string data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var textReader = new StringReader(data))
            using (var reader = new XmlTextReader(textReader))
            {
                return Parse(reader);
            }
        }

        public abstract OptionSet Parse(XmlTextReader reader);

        private static void FillParsers()
        {
            lock (OptionSetParsers)
            {
                if (OptionSetParsers.Count < 1)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var marker = typeof(OptionSetParser);
                    var types = assembly.GetTypes().Where(t => !t.IsAbstract && marker.IsAssignableFrom(t));

                    foreach (var type in types)
                    {
                        var instance = (OptionSetParser)Activator.CreateInstance(type);
                        OptionSetParsers[instance.Version] = instance;
                    }
                }
            }
        }
    }
}