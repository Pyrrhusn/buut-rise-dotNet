using Rise.Shared.Users;

namespace Rise.Server.Tests.Utils
{

    public static class UserRoleExtensions
    {
        public static string GetEmail(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => "test@guest.com",
                UserRole.Member => "test@member.com",
                UserRole.Administrator => "test@admin.com",
                _ => throw new ArgumentException($"No email defined for role {role}")
            };
        }
        public static string GetPassword(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => "Test@1234",
                UserRole.Member => "Test@1234",
                UserRole.Administrator => "Test@1234",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }
        public static string GetRole(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => "Guest",
                UserRole.Member => "Member",
                UserRole.Administrator => "Administrator",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }

        public static string GetUserName(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => "Guest",
                UserRole.Member => "Member",
                UserRole.Administrator => "Administrator",
                _ => throw new ArgumentException($"No password defined for role {role}")
            };
        }

        public static int GetId(this UserRole role)
        {
            return role switch
            {
                UserRole.Guest => 3,
                UserRole.Member => 2,
                UserRole.Administrator => 1,
                _ => throw new ArgumentException($"No id defined for role {role}")
            };
        }

    }
}