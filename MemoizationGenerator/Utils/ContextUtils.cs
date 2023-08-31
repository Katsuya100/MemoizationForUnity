using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Katuusagi.MemoizationForUnity.SourceGenerator.Utils
{
    public static class ContextUtils
    {
        private static StringBuilder _logs = new StringBuilder();

        public static LogHandle LogScope(GeneratorExecutionContext context)
        {
            return new LogHandle(context);
        }

        public static void ClearLog()
        {
            _logs.Clear();
        }

        public static void OutputLog(GeneratorExecutionContext context)
        {
            if (_logs.Length > 0)
            {
                context.AddSource("Log.Output.cs", SourceText.From(_logs.ToString(), Encoding.UTF8));
            }
        }

        public static void Log(object log)
        {
            var stack = new StackTrace(true);
            var frames = stack.GetFrames();
            var frameIndex = Array.FindIndex(frames, v => v.GetMethod().Name == "Execute" && typeof(ISourceGenerator).IsAssignableFrom(v.GetMethod().ReflectedType));
            var validStack = frames.Take(frameIndex + 1).Skip(1);
            var stackLog = string.Concat(validStack.Select(v => $"{v.GetMethod().ReflectedType}:{v.GetMethod()} (at {v.GetFileName()}:{v.GetFileLineNumber()})\n"));
            _logs.AppendLine($"// {log}\n{stackLog}".Replace("\n", "\n// "));
        }

        public static void Error(this GeneratorExecutionContext context, string errorId, string title, object log, Location location = null)
        {
            var desc = new DiagnosticDescriptor(
                id: errorId,
                title: title,
                messageFormat: log.ToString(),
                category: "Generator",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);
            var diagnostic = Diagnostic.Create(desc, location);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
