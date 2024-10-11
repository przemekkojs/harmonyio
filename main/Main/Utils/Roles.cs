namespace Main.Utils;

public enum Role
{
    Admin,
    Creator,
    Standard
}

public static class Roles
{
    private static readonly Dictionary<Role, string> RoleNames = new()
    {
        [Role.Admin] = "Admin",
        [Role.Creator] = "Creator",
        [Role.Standard] = "Standard",
    };

    public static string GetRoleName(Role role) => RoleNames[role];
}