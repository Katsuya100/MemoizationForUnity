using System.Text;

namespace Katuusagi.CSharpScriptGenerator
{
    public abstract class ScriptBuilderBase
    {
        private StringBuilder _builder = new StringBuilder();
        private int _indent = 0;

        public override string ToString()
        {
            return _builder.ToString();
        }

        protected void Append(string str)
        {
            if (_builder.Length > 0 &&
                _builder[_builder.Length - 1] == '\n')
            {
                for (int i = 0; i < _indent; ++i)
                {
                    _builder.Append("    ");
                }
            }

            _builder.Append(str);
        }

        protected void AppendLine(string str = "")
        {
            if (_builder.Length > 0 &&
                _builder[_builder.Length - 1] == '\n')
            {
                for (int i = 0; i < _indent; ++i)
                {
                    _builder.Append("    ");
                }
            }

            _builder.AppendLine(str);
        }

        protected void RemoveBack(int count)
        {
            _builder = _builder.Remove(_builder.Length - count, count);
        }

        protected virtual void StartScope()
        {
            AppendLine("{");
            IncrementIndent();
        }

        protected virtual void EndScope()
        {
            DecrementIndent();
            AppendLine("}");
        }

        protected void IncrementIndent()
        {
            ++_indent;
        }

        protected void DecrementIndent()
        {
            --_indent;
        }
    }
}
