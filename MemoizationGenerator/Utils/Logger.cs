using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Katuusagi.SourceGeneratorCommon
{
    public class Logger
    {
        private struct Frame
        {
            public string File;
            public string Method;
            public int Line;
            public int Column;
        }

        private Type _rootType;
        private GeneratorExecutionContext _context;
        private string _currentPath;
        private string _logPath;

        public Logger(Type type, GeneratorExecutionContext context)
        {
            _rootType = type;
            _context = context;
            _currentPath = context.GetRootPath();
            _logPath = $"{_currentPath}/Logs/{type.Name}/{context.Compilation.AssemblyName}.txt";
            var dir = Path.GetDirectoryName(_logPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Open(_logPath, FileMode.Create, FileAccess.Write, FileShare.Read).Dispose();
        }

        private void Write(object o)
        {
            using (var f = File.AppendText(_logPath))
            {
                f.WriteLine(o);
            }
        }

        private void LogRaw(DiagnosticSeverity type, string id, string title, object o, Location location, IEnumerable<Frame> frames)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = type.ToString();
            }

            if (string.IsNullOrEmpty(title))
            {
                title = type.ToString();
            }

            if (frames == null)
            {
                frames = GetStack();
            }

            frames = TrimStack(frames);

            string file;
            int line;
            int column;
            if (location == null)
            {
                var frame = frames.FirstOrDefault(v => !string.IsNullOrEmpty(v.File));
                file = frame.File;
                if (string.IsNullOrEmpty(file))
                {
                    file = " ";
                }

                line = frame.Line;
                column = frame.Column;

                var textSpan = new TextSpan(line, column);
                var lineSpan = new LinePositionSpan(new LinePosition(line, column), new LinePosition(line, column));
                location = Location.Create(file, textSpan, lineSpan);
            }
            else
            {
                var lineSpan = location.GetLineSpan();
                file = lineSpan.Path;
                if (string.IsNullOrEmpty(file))
                {
                    file = " ";
                }

                var start = lineSpan.StartLinePosition;
                line = start.Line;
                column = start.Character;
            }


            file = file.Replace("\\", "/").Replace(_currentPath, string.Empty).Trim();
            var stacktrace = FormatStack(frames);

            var log = $"{file}({line},{column}):{type} {id}:{o}\n{stacktrace}";
            Write(log);

            var desc = new DiagnosticDescriptor(id, title, o?.ToString() ?? string.Empty, "Generator", type, true);
            var diagnostic = Diagnostic.Create(desc, location);
            _context.ReportDiagnostic(diagnostic);
        }

        private void LogInternal(DiagnosticSeverity type, string id, string title, object o, string file, int line, int column, IEnumerable<Frame> frames = null)
        {
            if (file == null)
            {
                file = string.Empty;
            }

            var textSpan = new TextSpan(line, column);
            var lineSpan = new LinePositionSpan(new LinePosition(line, column), new LinePosition(line, column));
            var location = Location.Create(file, textSpan, lineSpan);

            LogRaw(type, id, title, o, location, frames);
        }

        private void LogInternal(DiagnosticSeverity type, string id, string title, object o, Location location = null, IEnumerable<Frame> frames = null)
        {
            LogRaw(type, id, title, o, location, frames);
        }

        public void Log(DiagnosticSeverity type, object o)
        {
            LogInternal(type, string.Empty, string.Empty, o);
        }

        public void Log(DiagnosticSeverity type, string id, string title, object o)
        {
            LogInternal(type, id, title, o);
        }

        public void Log(DiagnosticSeverity type, string id, string title, object o, CSharpSyntaxNode syntax)
        {
            LogInternal(type, id, title, o, syntax?.GetLocation());
        }

        public void Log(DiagnosticSeverity type, string id, string title, object o, Location location)
        {
            LogInternal(type, id, title, o, location);
        }

        public void Log(DiagnosticSeverity type, string id, string title, object o, string file, int line, int column)
        {
            LogInternal(type, id, title, o, file, line, column);
        }

        public void Log(DiagnosticSeverity type, Exception e)
        {
            if (e == null)
            {
                LogInternal(type, "Exception", "Exception", "Unknown Exception");
                return;
            }

            var frames = ParseStack(e.StackTrace);
            LogInternal(type, "Exception", "Exception", $"{e.GetType()}:{e.Message}", null, frames);
        }

        private IEnumerable<Frame> ParseStack(string stackTrace)
        {
            var splitedStackTraces = stackTrace.Split('\n');
            return splitedStackTraces.Select(Parse);
        }

        private IEnumerable<Frame> GetStack()
        {
            var stack = new StackTrace(true);
            return stack.GetFrames().Select(v => new Frame()
            {
                File = v?.GetFileName(),
                Method = GetMethodName(v?.GetMethod()),
                Line = v?.GetFileLineNumber() ?? 0,
                Column = v?.GetFileColumnNumber() ?? 0
            });
        }

        private IEnumerable<Frame> TrimStack(IEnumerable<Frame> frames)
        {
            var indices = frames.Select((v, i) => (v, i));
            var firstIndex = indices.FirstOrDefault(v => !IsLogMethod(v.v)).i;
            var lastIndex = indices.FirstOrDefault(v => IsRoot(v.v)).i;
            var result = frames.Take(lastIndex + 1).Skip(firstIndex);
            return result;
        }

        private string FormatStack(IEnumerable<Frame> frames)
        {
            var result = string.Concat(frames.Select(FormatStack));
            return result;
        }

        private string FormatStack(Frame frame)
        {
            var methodName = frame.Method;
            var fileName = frame.File?.Replace("\\", "/")?.Replace(_currentPath, string.Empty).Trim() ?? string.Empty;
            var lineNumber = frame.Line;
            return $"{methodName} (at {fileName}:{lineNumber})\n";
        }

        private bool IsLogMethod(Frame frame)
        {
            return frame.Method.StartsWith($"{typeof(ContextUtils).FullName}.Log") ||
                   frame.Method.StartsWith(typeof(Logger).FullName);
        }

        private bool IsRoot(Frame frame)
        {
            return frame.Method.StartsWith($"{_rootType.FullName}.{nameof(ISourceGenerator.Execute)}({nameof(GeneratorExecutionContext)}") ||
                   frame.Method.StartsWith($"{_rootType.FullName}.{nameof(ISourceGenerator.Initialize)}({nameof(GeneratorInitializationContext)}");
        }

        private Frame Parse(string stack)
        {
            string file = string.Empty;
            string method = string.Empty;
            int line = 0;
            try
            {
                var methodTop = stack.Substring(6, stack.Length - 6);
                var splited = methodTop.Split(new string[] { ") in " }, StringSplitOptions.None);
                if (splited.Length <= 1)
                {
                    method = methodTop.Trim();
                }
                else
                {
                    method = $"{splited[0]})";
                    splited = splited[1].Split(new string[] { ":line " }, StringSplitOptions.None);
                    file = splited[0].Trim();
                    int.TryParse(splited[1], out line);
                }
            }
            catch
            {
            }

            return new Frame()
            {
                File = file,
                Method = method,
                Line = line,
            };
        }

        public static string GetMethodName(System.Reflection.MethodBase method)
        {
            if (method == null)
            {
                return string.Empty;
            }

            var parameters = string.Join(",", method.GetParameters().Select(v => v.ParameterType.Name));
            return $"{GetTypeName(method.ReflectedType)}.{method.Name}({parameters})";
        }

        public static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string generic = string.Empty;
                foreach (var arg in type.GetGenericArguments())
                {
                    generic += $"{GetTypeName(arg)},";
                }
                generic = generic.Remove(generic.Length - 1, 1);

                string parentName;
                if (type.ReflectedType != null)
                {
                    parentName = $"{GetTypeName(type.ReflectedType)}/";
                }
                else
                {
                    parentName = $"{type.Namespace}.";
                }

                return $"{parentName}{type.Name}<{generic}>";
            }

            if (type.FullName == null)
            {
                return type.Name;
            }
            return type.FullName.Replace("+", "/");
        }
    }
}
