using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Specialized;

namespace Elmah
{
  public class NameValueCollectionSerializer : SerializerBase<NameValueCollection>
  {
    private static readonly NameValueCollectionSerializer instance = new NameValueCollectionSerializer();

    public static NameValueCollectionSerializer Instance
    {
      get { return instance; }
    }

    public override NameValueCollection Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
      var bsonReader = context.Reader;
      var bsonType = bsonReader.GetCurrentBsonType();
      if (bsonType == BsonType.Null)
      {
        bsonReader.ReadNull();
        return null;
      }

      var nvc = new NameValueCollection();

      bsonReader.ReadStartArray();
      while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
      {
        bsonReader.ReadStartArray();
        var key = bsonReader.ReadString();
        var val = bsonReader.ReadString();
        bsonReader.ReadEndArray();
        nvc.Add(key, val);
      }
      bsonReader.ReadEndArray();

      return nvc;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NameValueCollection value)
    {
      var bsonWriter = context.Writer;
      if (value == null)
      {
        bsonWriter.WriteNull();
        return;
      }

      var nvc = (NameValueCollection)value;
      
      bsonWriter.WriteStartArray();
      foreach (var key in nvc.AllKeys)
      {
        foreach (var val in nvc.GetValues(key))
        {
          bsonWriter.WriteStartArray();
          bsonWriter.WriteString(key);
          bsonWriter.WriteString(val);
          bsonWriter.WriteEndArray();
        }
      }
      bsonWriter.WriteEndArray();
    }
    
  }
}