namespace Katuusagi.CSharpScriptGenerator
{
    public static class ScriptGeneratorUtils
    {
        public static string GetModifierLabel(this ModifierType modifier)
        {
            string result = string.Empty;
            if (modifier.HasFlag(ModifierType.Public))
            {
                result += "public ";
            }
            else if (modifier.HasFlag(ModifierType.Protected))
            {
                result += "protected ";
            }
            else if (modifier.HasFlag(ModifierType.Private))
            {
                result += "private ";
            }
            else if (modifier.HasFlag(ModifierType.Internal))
            {
                result += "internal ";
            }

            if (modifier.HasFlag(ModifierType.Unsafe))
            {
                result += "unsafe ";
            }

            if (modifier.HasFlag(ModifierType.Sealed))
            {
                result += "sealed ";
            }

            if (modifier.HasFlag(ModifierType.Static))
            {
                result += "static ";
            }

            if (modifier.HasFlag(ModifierType.Partial))
            {
                result += "partial ";
            }

            if (modifier.HasFlag(ModifierType.ReadOnly))
            {
                result += "readonly ";
            }

            if (modifier.HasFlag(ModifierType.Ref))
            {
                result += "ref ";
            }

            if (modifier.HasFlag(ModifierType.Const))
            {
                result += "const ";
            }

            if (modifier.HasFlag(ModifierType.Abstract))
            {
                result += "abstract ";
            }
            else if (modifier.HasFlag(ModifierType.Virtual))
            {
                result += "virtual ";
            }
            else if (modifier.HasFlag(ModifierType.Override))
            {
                result += "override ";
            }

            if (modifier.HasFlag(ModifierType.Record))
            {
                result += "record ";
            }

            if (modifier.HasFlag(ModifierType.Class))
            {
                result += "class ";
            }
            else if (modifier.HasFlag(ModifierType.Struct))
            {
                result += "struct ";
            }
            else if (modifier.HasFlag(ModifierType.Interface))
            {
                result += "interface ";
            }

            return result;
        }
    }
}
