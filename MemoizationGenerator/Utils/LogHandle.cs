using Microsoft.CodeAnalysis;
using System;

namespace Katuusagi.MemoizationForUnity.SourceGenerator.Utils
{
    public struct LogHandle : IDisposable
    {
        private GeneratorExecutionContext _context;

        public LogHandle(GeneratorExecutionContext context)
        {
            _context = context;
            ContextUtils.ClearLog();
        }

        public void Dispose()
        {
            ContextUtils.OutputLog(_context);
        }
    }
}
