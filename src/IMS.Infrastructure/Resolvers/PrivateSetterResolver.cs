using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace IMS.Infrastructure.Resolvers;

// This resolver is used to serialize and deserialize objects with private setters.
public sealed class PrivateSetterResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(
      MemberInfo member,
      MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (!property.Writable)
        {
            var prop = member as PropertyInfo;

            if (prop is not null)
            {
                var hasPrivateSetter = prop.GetSetMethod(true) is not null;
                property.Writable = hasPrivateSetter;
            }
        }

        return property;
    }
}
