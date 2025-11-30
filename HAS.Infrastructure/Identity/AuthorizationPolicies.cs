namespace HAS.Infrastructure.Identity;

public static class AuthorizationPolicies
{
    // Policy names
    public const string AdminOnly = "AdminOnly";
    public const string DoctorOnly = "DoctorOnly";
    public const string PatientOnly = "PatientOnly";
    public const string ReceptionistOnly = "ReceptionistOnly";
    public const string AdminOrReceptionist = "AdminOrReceptionist";
    public const string DoctorOrReceptionist = "DoctorOrReceptionist";
    public const string AdminOrDoctor = "AdminOrDoctor";
    public const string AllRoles = "AllRoles";

    // Role claim type
    public const string RoleClaimType = "role";
}
