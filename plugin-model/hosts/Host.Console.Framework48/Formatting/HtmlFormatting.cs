using System.Text;
using Host.Console.Framework48.Pretty;

namespace Host.Console.Framework48.Formatting;

public static class HtmlFormatting
{
    extension(PrettyPluginExecutionTest test)
    {
        public string Html()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Plugin Execution Test Results</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
            sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f5f5f5; padding: 20px; }");
            sb.AppendLine("        .container { max-width: 1200px; margin: 0 auto; }");
            sb.AppendLine("        .plugin-card { background: white; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px; overflow: hidden; }");
            sb.AppendLine("        .plugin-header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; }");
            sb.AppendLine("        .plugin-name { font-size: 24px; font-weight: bold; margin-bottom: 5px; }");
            sb.AppendLine("        .plugin-info { opacity: 0.9; font-size: 14px; }");
            sb.AppendLine("        .section { border-bottom: 1px solid #e0e0e0; }");
            sb.AppendLine("        .section:last-child { border-bottom: none; }");
            sb.AppendLine("        .section-header { background: #f8f9fa; padding: 15px 20px; font-weight: 600; color: #333; border-left: 4px solid #667eea; }");
            sb.AppendLine("        .section-content { padding: 20px; }");
            sb.AppendLine("        .info-grid { display: grid; grid-template-columns: auto 1fr; gap: 10px 20px; margin-bottom: 20px; }");
            sb.AppendLine("        .info-label { font-weight: 600; color: #666; }");
            sb.AppendLine("        .info-value { color: #333; word-break: break-all; }");
            sb.AppendLine("        .assembly-list { margin-top: 15px; }");
            sb.AppendLine("        .assembly-item { background: #f8f9fa; border-left: 3px solid #28a745; padding: 10px 15px; margin-bottom: 10px; border-radius: 4px; }");
            sb.AppendLine("        .assembly-name { font-weight: 600; color: #333; margin-bottom: 5px; }");
            sb.AppendLine("        .assembly-location { font-size: 12px; color: #666; font-family: 'Courier New', monospace; }");
            sb.AppendLine("        .resolved-item { background: #fff3cd; border-left: 3px solid #ffc107; padding: 10px 15px; margin-bottom: 10px; border-radius: 4px; }");
            sb.AppendLine("        .resolved-header { font-weight: 600; color: #856404; margin-bottom: 8px; }");
            sb.AppendLine("        .resolved-detail { font-size: 13px; color: #666; margin-left: 15px; margin-bottom: 5px; }");
            sb.AppendLine("        .badge { display: inline-block; padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: 600; }");
            sb.AppendLine("        .badge-load { background: #e3f2fd; color: #1976d2; }");
            sb.AppendLine("        .badge-execute { background: #f3e5f5; color: #7b1fa2; }");
            sb.AppendLine("        .empty-state { color: #999; font-style: italic; padding: 10px 0; }");
            sb.AppendLine("        h3 { color: #333; margin-bottom: 10px; font-size: 16px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class=\"container\">");
            sb.AppendLine("        <div class=\"plugin-card\">");

            // Plugin Header
            sb.AppendLine("            <div class=\"plugin-header\">");
            sb.AppendLine($"                <div class=\"plugin-name\">{EscapeHtml(test.Plugin.Name)}</div>");
            sb.AppendLine($"                <div class=\"plugin-info\">Version: {EscapeHtml(test.Plugin.Version)} | Path: {EscapeHtml(test.Plugin.FullPath)}</div>");
            sb.AppendLine("            </div>");

            // Sections
            foreach (var section in test.Sections)
            {
                if (section is PrettyPluginLoadAssemblySection loadSection)
                {
                    sb.AppendLine("            <div class=\"section\">");
                    sb.AppendLine("                <div class=\"section-header\">");
                    sb.AppendLine("                    <span class=\"badge badge-load\">LOAD ASSEMBLY</span>");
                    sb.AppendLine($"                    Load Function: {EscapeHtml(loadSection.LoadFunctionType)}");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("                <div class=\"section-content\">");

                    // Main Assembly
                    sb.AppendLine("                    <div class=\"info-grid\">");
                    sb.AppendLine("                        <div class=\"info-label\">Assembly:</div>");
                    sb.AppendLine($"                        <div class=\"info-value\">{EscapeHtml(loadSection.Assembly.FullName)}</div>");
                    sb.AppendLine("                        <div class=\"info-label\">Location:</div>");
                    sb.AppendLine($"                        <div class=\"info-value\">{EscapeHtml(loadSection.Assembly.Location ?? "(none)")}</div>");
                    sb.AppendLine("                    </div>");

                    // Loaded Assemblies
                    if (loadSection.LoadedAssemblies.Any())
                    {
                        sb.AppendLine("                    <h3>Loaded Assemblies</h3>");
                        sb.AppendLine("                    <div class=\"assembly-list\">");
                        foreach (var assembly in loadSection.LoadedAssemblies)
                        {
                            sb.AppendLine("                        <div class=\"assembly-item\">");
                            sb.AppendLine($"                            <div class=\"assembly-name\">{EscapeHtml(assembly.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"assembly-location\">{EscapeHtml(assembly.Location ?? "(no location)")}</div>");
                            sb.AppendLine("                        </div>");
                        }
                        sb.AppendLine("                    </div>");
                    }

                    // Resolved Assemblies
                    if (loadSection.ResolvedAssemblies.Any())
                    {
                        sb.AppendLine("                    <h3>Resolved Assemblies</h3>");
                        sb.AppendLine("                    <div class=\"assembly-list\">");
                        foreach (var resolved in loadSection.ResolvedAssemblies)
                        {
                            sb.AppendLine("                        <div class=\"resolved-item\">");
                            sb.AppendLine($"                            <div class=\"resolved-header\">Load Function: {EscapeHtml(resolved.LoadFunction)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Requesting:</strong> {EscapeHtml(resolved.RequestingAssembly.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Resolved:</strong> {EscapeHtml(resolved.Assembly.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Location:</strong> {EscapeHtml(resolved.Assembly.Location ?? "(none)")}</div>");
                            sb.AppendLine("                        </div>");
                        }
                        sb.AppendLine("                    </div>");
                    }
                    else
                    {
                        sb.AppendLine("                    <div class=\"empty-state\">No assemblies were resolved during loading</div>");
                    }

                    sb.AppendLine("                </div>");
                    sb.AppendLine("            </div>");
                }
                else if (section is PrettyPluginFunctionExecutionSection execSection)
                {
                    sb.AppendLine("            <div class=\"section\">");
                    sb.AppendLine("                <div class=\"section-header\">");
                    sb.AppendLine("                    <span class=\"badge badge-execute\">EXECUTE FUNCTION</span>");
                    sb.AppendLine($"                    Load Function: {EscapeHtml(execSection.LoadFunctionType)}");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("                <div class=\"section-content\">");

                    // Loaded Assemblies
                    if (execSection.LoadedAssemblies.Any())
                    {
                        sb.AppendLine("                    <h3>Loaded Assemblies</h3>");
                        sb.AppendLine("                    <div class=\"assembly-list\">");
                        foreach (var assembly in execSection.LoadedAssemblies)
                        {
                            sb.AppendLine("                        <div class=\"assembly-item\">");
                            sb.AppendLine($"                            <div class=\"assembly-name\">{EscapeHtml(assembly.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"assembly-location\">{EscapeHtml(assembly.Location ?? "(no location)")}</div>");
                            sb.AppendLine("                        </div>");
                        }
                        sb.AppendLine("                    </div>");
                    }
                    else
                    {
                        sb.AppendLine("                    <div class=\"empty-state\">No assemblies were loaded during execution</div>");
                    }

                    // Resolved Assemblies
                    if (execSection.ResolvedAssemblies.Any())
                    {
                        sb.AppendLine("                    <h3>Resolved Assemblies</h3>");
                        sb.AppendLine("                    <div class=\"assembly-list\">");
                        foreach (var resolved in execSection.ResolvedAssemblies)
                        {
                            sb.AppendLine("                        <div class=\"resolved-item\">");
                            sb.AppendLine($"                            <div class=\"resolved-header\">Load Function: {EscapeHtml(resolved.LoadFunction)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Requesting:</strong> {EscapeHtml(resolved.RequestingAssembly?.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Resolved:</strong> {EscapeHtml(resolved.Assembly.FullName)}</div>");
                            sb.AppendLine($"                            <div class=\"resolved-detail\"><strong>Location:</strong> {EscapeHtml(resolved.Assembly.Location ?? "(none)")}</div>");
                            sb.AppendLine("                        </div>");
                        }
                        sb.AppendLine("                    </div>");
                    }
                    else
                    {
                        sb.AppendLine("                    <div class=\"empty-state\">No assemblies were resolved during execution</div>");
                    }

                    sb.AppendLine("                </div>");
                    sb.AppendLine("            </div>");
                }
            }

            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }

    private static string EscapeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
            
        }

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}