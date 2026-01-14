using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? version;

        // Parse informational version (format: 1.0.0+commitHash)
        var parts = informationalVersion.Split('+');
        var semVer = parts[0];
        var commitHash = parts.Length > 1 ? parts[1] : "unknown";

        return Ok(new
        {
            version = semVer,
            commit = commitHash,
            buildDate = System.IO.File.GetLastWriteTimeUtc(assembly.Location).ToString("yyyy-MM-dd HH:mm:ss UTC"),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        });
    }
}
