using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections.Tags.Serializer
{
  public class TagsCollectionSerializer
  {
        /// <summary>
        /// Serializes a tags collection to a byte array and addes the size in the first 4 bytes.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public void SerializeWithSize(TagsCollectionBase collection, Stream stream)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Tag), true);

            var tagsList = new List<Tag>(collection);
            typeModel.SerializeWithSize(stream, tagsList);
        }

        /// <summary>
        /// Deserializes a tags collection from a byte array and takes the data size from the first 4 bytes.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public TagsCollectionBase DeserializeWithSize(Stream stream)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Tag), true);
            return new TagsCollection(typeModel.DeserializeWithSize(stream,
                null, typeof(List<Tag>)) as List<Tag>);
        }
    }
}
