using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Discogs.Dump.Model;
using Discogs.Dump.Model.Artists;
using Discogs.Dump.Model.Releases;
using Artist = Discogs.Dump.Model.Releases.Artist;

namespace Discogs.Dump
{
    public static class DumpParser
    {
        sealed class EmptyReadOnlyList<T> : IReadOnlyList<T>
        {
            public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public int Count => 0;
            public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index));
        }

        static IReadOnlyList<T> EmptyList<T>() => new EmptyReadOnlyList<T>();

        static Image ParseImage(XElement elm)
        {
            elm.ShouldHaveName("image");
            var height = elm.GetRequiredIntAttribute("height");
            var type = elm.GetRequiredStringAttribute("type");
            var uri = elm.GetRequiredStringAttribute("uri");
            var uri150 = elm.GetRequiredStringAttribute("uri150");
            var width = elm.GetRequiredIntAttribute("width");
            return new Image(height, type, uri, uri150, width);
        }

        static class ReleaseParser
        {
            static Artist ParseArtist(XElement elm)
            {
                var id = elm.GetRequiredIntChild("id");
                var name = elm.GetRequiredStringChild("name");
                var anv = elm.GetRequiredStringChild("anv");
                var join = elm.GetRequiredStringChild("join");
                var role = elm.GetRequiredStringChild("role");
                var tracks = elm.GetRequiredStringChild("tracks");
                return new Artist(id, name, anv, join, role, tracks);
            }

            static Label ParseLabel(XElement elm)
            {
                elm.ShouldHaveName("label");
                var catNo = elm.GetRequiredStringAttribute("catno");
                var id = elm.GetRequiredIntAttribute("id");
                var name = elm.GetRequiredStringAttribute("name");
                return new Label(catNo, id, name);
            }

            static Format ParseFormat(XElement elm)
            {
                var name = elm.GetRequiredStringAttribute("name");
                int qty;
                try
                {
                    qty = elm.GetRequiredIntAttribute("qty");
                }
                // there are values like '999999999999' and '1000000000000000000000000000000000000000000000000000000000000001'
                catch (DiscogsDumpParseException)
                {
                    qty = -1;
                }

                var text = elm.GetRequiredStringAttribute("text");
                var descriptions = elm
                    .Element("descriptions")
                    ?.ThatShouldBeListOfElementsWithName("description")
                    .Select(x => x.GetText())
                    .ToList()
                    ?? EmptyList<string>();
                return new Format(name, qty, text, descriptions);
            }

            static Track ParseTrack(XElement elm)
            {
                var position = elm.GetRequiredStringChild("position");
                var title = elm.GetRequiredStringChild("title");
                var duration = elm.GetRequiredStringChild("duration");
                var artists = elm
                    .Element("artists")
                    ?.ThatShouldBeListOfElementsWithName("artist")
                    .Select(ParseArtist)
                    .ToList()
                    ?? EmptyList<Artist>();
                return new Track(position, title, duration, artists);
            }

            static Identifier ParseIdentifier(XElement elm)
            {
                elm.ShouldHaveName("identifier");
                var description = elm.GetOptionalStringAttribute("description");
                var type = elm.GetRequiredStringAttribute("type");
                var value = elm.GetRequiredStringAttribute("value");
                return new Identifier(description, type, value);
            }

            static Video ParseVideo(XElement elm)
            {
                elm.ShouldHaveName("video");
                var duration = elm.GetRequiredIntAttribute("duration");
                var embed = elm.GetRequiredBoolAttribute("embed");
                var src = elm.GetRequiredStringAttribute("src");
                var title = elm.GetRequiredStringChild("title");
                var description = elm.GetRequiredStringChild("description");
                return new Video(duration, embed, src, title, description);
            }

            static Company ParseCompany(XElement elm)
            {
                var id = elm.GetRequiredIntChild("id");
                var name = elm.GetRequiredStringChild("name");
                var catNo = elm.GetRequiredStringChild("catno");
                var entityType = elm.GetRequiredIntChild("entity_type");
                var entityTypeName = elm.GetRequiredStringChild("entity_type_name");
                return new Company(id, name, catNo, entityType, entityTypeName);
            }

            public static Release ParseRelease(XElement elm)
            {
                elm.ShouldHaveName("release");
                var id = elm.GetRequiredIntAttribute("id");
                if (id == 13442199) Console.WriteLine(elm.ToString());
                var status = elm.GetRequiredEnumAttribute<Status>("status");
                var images = elm
                    .Element("images")
                    ?.Elements()
                    .Select(ParseImage)
                    .ToList()
                    ?? EmptyList<Image>();
                var artists = elm
                    .GetSingleChildWithName("artists")
                    .ThatShouldBeListOfElementsWithName("artist")
                    .Select(ParseArtist)
                    .ToList();
                var extraArtists = elm
                    .GetSingleChildWithName("extraartists")
                    .ThatShouldBeListOfElementsWithName("artist")
                    .Select(ParseArtist)
                    .ToList();
                var title = elm.GetRequiredStringChild("title");
                var labels = elm
                    .GetSingleChildWithName("labels")
                    .Elements()
                    .Select(ParseLabel)
                    .ToList();
                var formats = elm
                    .GetSingleChildWithName("formats")
                    .Elements()
                    .Select(ParseFormat)
                    .ToList();
                var genres = elm
                    .Element("genres")
                    ?.ThatShouldBeListOfElementsWithName("genre")
                    .Select(x => x.GetText())
                    .ToList()
                    ?? EmptyList<string>();
                var styles = elm
                    .Element("styles")
                    ?.ThatShouldBeListOfElementsWithName("style")
                    .Select(x => x.GetText())
                    .ToList()
                    ?? EmptyList<string>();
                var country = elm.GetOptionalStringChild("country");
                var released = elm.GetOptionalStringChild("released");
                var notes = elm.GetOptionalStringChild("notes");
                var dataQuality = elm.GetRequiredStringChild("data_quality");
                var masterIdStr = elm.Element("master_id")?.Value;
                var masterId = masterIdStr == null ? (int?) null :
                    int.TryParse(masterIdStr, out var res) ? res : throw new DiscogsDumpParseException(
                        $"Can not cast {masterIdStr} to int."
                    );
                var trackList = elm
                    .GetSingleChildWithName("tracklist")
                    .ThatShouldBeListOfElementsWithName("track")
                    .Select(ParseTrack)
                    .ToList();
                var identifiers = elm
                    .GetSingleChildWithName("identifiers")
                    .Elements()
                    .Select(ParseIdentifier)
                    .ToList();
                var videos = elm
                    .Element("videos")
                    ?.Elements()
                    .Select(ParseVideo)
                    .ToList()
                    ?? EmptyList<Video>();
                var companies = elm
                    .GetSingleChildWithName("companies")
                    .ThatShouldBeListOfElementsWithName("company")
                    .Select(ParseCompany)
                    .ToList();

                return new Release(
                    id,
                    status,
                    images,
                    artists,
                    extraArtists,
                    title,
                    labels,
                    formats,
                    genres,
                    styles,
                    country,
                    released,
                    notes,
                    dataQuality,
                    masterId,
                    trackList,
                    identifiers,
                    videos,
                    companies
                );
            }
        }

        static class ArtistParser
        {
            static Ref ParseRef(XElement elm)
            {
                elm.ShouldHaveName("name");
                var id = elm.GetRequiredIntAttribute("id");
                var name = elm.GetText();
                return new Ref(id, name);
            }

            public static Model.Artists.Artist ParseArtist(XElement elm)
            {
                var images = elm
                    .Element("images")
                    ?.Elements()
                    .Select(ParseImage)
                    .ToList()
                    ?? EmptyList<Image>();
                var id = elm.GetRequiredIntChild("id");
                var name = elm.GetRequiredStringChild("name");
                var realName = elm.GetOptionalStringChild("realname");
                var profile = elm.GetRequiredStringChild("profile");
                var dataQuality = elm.GetRequiredStringChild("data_quality");
                var urls = elm
                    .Element("urls")
                    ?.ThatShouldBeListOfElementsWithName("url")
                    .Select(x => x.GetText())
                    .ToList()
                    ?? EmptyList<string>();
                var nameVariations = elm
                    .Element("namevariations")
                    ?.ThatShouldBeListOfElementsWithName("name")
                    .Select(x => x.GetText())
                    .ToList()
                    ?? EmptyList<string>();
                var aliases = elm
                    .Element("aliases")
                    ?.Elements()
                    .Select(ParseRef)
                    .ToList()
                    ?? EmptyList<Ref>();
                var members = elm
                    .Element("members")
                    ?.Elements("name") // ignore <id> tags
                    .Select(ParseRef)
                    .ToList()
                    ?? EmptyList<Ref>();
                var groups = elm
                    .Element("groups")
                    ?.Elements()
                    .Select(ParseRef)
                    .ToList()
                    ?? EmptyList<Ref>();
                return new Model.Artists.Artist(
                    images,
                    id,
                    name,
                    realName,
                    profile,
                    dataQuality,
                    urls,
                    nameVariations,
                    aliases,
                    members,
                    groups
                );
            }
        }

        static XmlReader CreateXmlReader(Stream stream) =>
            XmlReader.Create(stream, new XmlReaderSettings { Async = true });

        public static async IAsyncEnumerable<Release> ParseReleasesDumpStream(Stream stream, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var xmlReader = CreateXmlReader(stream);

            await xmlReader.MoveToContentAsync().ConfigureAwait(false);
            xmlReader.ReadStartElement("releases");
            while (true)
            {
                await xmlReader.MoveToContentAsync().ConfigureAwait(false);
                if (xmlReader.NodeType == XmlNodeType.EndElement) break;

                var releaseNode = await XNode.ReadFromAsync(xmlReader, ct).ConfigureAwait(false);
                yield return ReleaseParser.ParseRelease((XElement) releaseNode);
            }
            xmlReader.ReadEndElement();
        }

        public static async IAsyncEnumerable<Model.Artists.Artist> ParseArtistsDumpStream(Stream stream, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var xmlReader = CreateXmlReader(stream);

            await xmlReader.MoveToContentAsync().ConfigureAwait(false);
            xmlReader.ReadStartElement("artists");
            while (true)
            {
                await xmlReader.MoveToContentAsync().ConfigureAwait(false);
                if (xmlReader.NodeType == XmlNodeType.EndElement) break;

                var releaseNode = await XNode.ReadFromAsync(xmlReader, ct).ConfigureAwait(false);
                yield return ArtistParser.ParseArtist((XElement) releaseNode);
            }

            xmlReader.ReadEndElement();
        }
    }
}
