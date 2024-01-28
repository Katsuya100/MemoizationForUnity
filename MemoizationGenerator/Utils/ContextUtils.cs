using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;

namespace Katuusagi.SourceGeneratorCommon
{
    public static class ContextUtils
    {
        [ThreadStatic]
        private static Logger _logger;

        public static void InitLog<T>(GeneratorExecutionContext context)
            where T : ISourceGenerator
        {
            _logger = new Logger(typeof(T), context);
        }

        public static void Log(object log)
        {
            _logger.Log(DiagnosticSeverity.Info, log);
        }

        public static void LogWarning(object log)
        {
            _logger.Log(DiagnosticSeverity.Warning, log);
        }

        public static void LogWarning(string id, string title, object log)
        {
            _logger.Log(DiagnosticSeverity.Warning, id, title, log);
        }

        public static void LogWarning(string id, string title, object log, CSharpSyntaxNode syntax)
        {
            _logger.Log(DiagnosticSeverity.Warning, id, title, log, syntax);
        }

        public static void LogWarning(string id, string title, object log, Location location)
        {
            _logger.Log(DiagnosticSeverity.Warning, id, title, log, location);
        }

        public static void LogWarning(string id, string title, object log, string file, int line, int column)
        {
            _logger.Log(DiagnosticSeverity.Warning, id, title, log, file, line, column);
        }

        public static void LogError(object log)
        {
            _logger.Log(DiagnosticSeverity.Error, log);
        }

        public static void LogError(string id, string title, object log)
        {
            _logger.Log(DiagnosticSeverity.Error, id, title, log);
        }

        public static void LogError(string id, string title, object log, CSharpSyntaxNode syntax)
        {
            _logger.Log(DiagnosticSeverity.Error, id, title, log, syntax);
        }

        public static void LogError(string id, string title, object log, Location location)
        {
            _logger.Log(DiagnosticSeverity.Error, id, title, log, location);
        }

        public static void LogError(string id, string title, object log, string file, int line, int column)
        {
            _logger.Log(DiagnosticSeverity.Error, id, title, log, file, line, column);
        }

        public static void LogException(Exception e)
        {
            _logger.Log(DiagnosticSeverity.Error, e);
        }

        public static string GetRootPath(this GeneratorExecutionContext context)
        {
            var scriptPath = context.Compilation.SyntaxTrees.Select(v => v.FilePath.Replace("\\", "/")).FirstOrDefault(v => v.Contains("/Assets/") || v.Contains("/Library/") || v.Contains("/Packages/"));
            var index = scriptPath.IndexOf("/Assets/");
            if (index < 0)
            {
                index = scriptPath.IndexOf("/Library/");
                if (index < 0)
                {
                    index = scriptPath.IndexOf("/Packages/");
                }
            }

            var rootPath = scriptPath.Substring(0, index + 1);
            return rootPath;
        }

        public static bool IsAvailableAssembly(this GeneratorExecutionContext self, string assemblyName)
        {
            return self.IsReferencedAssembly(assemblyName) ||
                   self.Compilation.AssemblyName == assemblyName;
        }

        public static bool IsReferencedAssembly(this GeneratorExecutionContext self, string assemblyName)
        {
            return self.Compilation.ReferencedAssemblyNames.Any(v => v.Name == assemblyName);
        }
    }
}
