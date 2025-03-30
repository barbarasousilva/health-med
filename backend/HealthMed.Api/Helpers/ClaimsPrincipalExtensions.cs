using System.Security.Claims;

namespace HealthMed.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetMedicoId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue("id");
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }

    public static string GetCRM(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("crm") ?? string.Empty;
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("role") ?? string.Empty;
    }

    public static string GetNome(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }
}
