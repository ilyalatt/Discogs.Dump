using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Discogs.Dump.Model.Releases;

namespace Discogs.Dump
{
    static class XExtensions
    {
        public static XElement ShouldHaveName(this XElement elm, string name) => elm.Name == name
            ? elm
            : throw new DiscogsDumpParseException($"Element '{elm.Name}' is expected to have name '{name}'.");

        public static string GetText(this XElement elm) => !elm.HasElements
            ? elm.Value
            : throw new DiscogsDumpParseException($"Element '{elm.Name}' is expected to have no elements.");

        public static string GetRequiredStringAttribute(this XElement elm, string name)
        {
            var attr = elm.Attribute(name)
                ?? throw new DiscogsDumpParseException($"Attribute '{name}' is not found.");
            return attr.Value;
        }

        public static int GetRequiredIntAttribute(this XElement elm, string name)
        {
            var value = elm.GetRequiredStringAttribute(name);
            return int.TryParse(value, out var val)
                ? val
                : throw new DiscogsDumpParseException($"Attribute '{name}' value '{value}' can not be cast to int.");
        }

        public static bool GetRequiredBoolAttribute(this XElement elm, string name)
        {
            var value = elm.GetRequiredStringAttribute(name);
            return bool.TryParse(value, out var val)
                ? val
                : throw new DiscogsDumpParseException($"Attribute '{name}' value '{value}' can not be cast to bool.");
        }

        public static string GetRequiredStringChild(this XElement elm, string name)
        {
            var element = elm.Element(name)
                ?? throw new DiscogsDumpParseException($"Child '{name}' is not found.");
            return element.GetText();
        }

        public static int GetRequiredIntChild(this XElement elm, string name)
        {
            var value = elm.GetRequiredStringChild(name);
            return int.TryParse(value, out var val)
                ? val
                : throw new DiscogsDumpParseException($"Child '{name}' value '{value}' can not be cast to int.");
        }

        public static T GetRequiredEnumAttribute<T>(this XElement elm, string name) where T : struct
        {
            var value = elm.GetRequiredStringAttribute(name);
            return Enum.TryParse(value, out T res) ? res : throw new DiscogsDumpParseException(
                $"Can not cast '{value}' to {typeof(Status).Name}."
            );
        }

        public static string? GetOptionalStringAttribute(this XElement elm, string name)
        {
            return elm.Attribute(name)?.Value;
        }

        public static string? GetOptionalStringChild(this XElement elm, string name)
        {
            return elm.Element(name)?.GetText();
        }

        public static XNode GetSingleChild(this XElement elm, string name)
        {
            var children = elm.Nodes().ToList();
            return children.Count == 1 ? children.Single() : throw new DiscogsDumpParseException(
                $"Element '{elm.Name}' is expected to have single child '{name}' but got {children.Count} children."
            );
        }

        public static XElement ThatShouldBeElement(this XNode node) =>
            node as XElement ?? throw new DiscogsDumpParseException(
                "Node is not an element."
            );

        public static IEnumerable<XElement> ThatShouldBeListOfElementsWithName(this XElement elm, string name) =>
            elm.Elements().Select(x => x.ShouldHaveName(name));

        public static XElement GetSingleChildWithName(this XElement elm, string name)
        {
            var children = elm.Elements(name).ToList();
            return children.Count == 1 ? children.Single() : throw new DiscogsDumpParseException(
                $"Element '{elm.Name}' is expected to have single child '{name}' but got {children.Count} children."
            );
        }
    }
}
