using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Rise.Shared.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    [EnumMember(Value = "Administrator")]
    Administrator,

    [EnumMember(Value = "Guest")]
    Guest,

    [EnumMember(Value = "Member")]
    Member
}