using System;

namespace Katuusagi.CSharpScriptGenerator
{
    public static class ScriptGeneratorUtils
    {
        public static ModifierType GetModifierType(string label)
        {
            var result = ModifierType.None;
            if (label == null)
            {
                return result;
            }

            var splitedLabels = label.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var splitedLabel in splitedLabels)
            {
                switch (splitedLabel)
                {
                    case "public":
                        result |= ModifierType.Public;
                        break;
                    case "private":
                        result |= ModifierType.Private;
                        break;
                    case "protected":
                        result |= ModifierType.Protected;
                        break;
                    case "internal":
                        result |= ModifierType.Internal;
                        break;
                    case "unsafe":
                        result |= ModifierType.Unsafe;
                        break;
                    case "sealed":
                        result |= ModifierType.Sealed;
                        break;
                    case "static":
                        result |= ModifierType.Static;
                        break;
                    case "const":
                        result |= ModifierType.Const;
                        break;
                    case "volatile":
                        result |= ModifierType.Volatile;
                        break;
                    case "extern":
                        result |= ModifierType.Extern;
                        break;
                    case "abstract":
                        result |= ModifierType.Abstract;
                        break;
                    case "virtual":
                        result |= ModifierType.Virtual;
                        break;
                    case "override":
                        result |= ModifierType.Override;
                        break;
                    case "partial":
                        result |= ModifierType.Partial;
                        break;
                    case "new":
                        result |= ModifierType.New;
                        break;
                    case "async":
                        result |= ModifierType.Async;
                        break;
                    case "readonly":
                        if (result >= ModifierType.ReadOnly)
                        {
                            result |= ModifierType.ReturnReadOnly;
                        }
                        else
                        {
                            result |= ModifierType.ReadOnly;
                        }
                        break;
                    case "this":
                        result |= ModifierType.This;
                        break;
                    case "ref":
                        result |= ModifierType.Ref;
                        break;
                    case "in":
                        result |= ModifierType.In;
                        break;
                    case "out":
                        result |= ModifierType.Out;
                        break;
                    case "params":
                        result |= ModifierType.Params;
                        break;
                    case "record":
                        result |= ModifierType.Record;
                        break;
                    case "class":
                        result |= ModifierType.Class;
                        break;
                    case "struct":
                        result |= ModifierType.Struct;
                        break;
                    case "interface":
                        result |= ModifierType.Interface;
                        break;
                }
            }
            return result;
        }

        public static string GetModifierLabel(this ModifierType modifier)
        {
            string result = string.Empty;
            if (modifier.HasFlag(ModifierType.Public))
            {
                result += "public ";
            }
            else if (modifier.HasFlag(ModifierType.Private))
            {
                result += "private ";
                if (modifier.HasFlag(ModifierType.Protected))
                {
                    result += "protected ";
                }
            }
            else if (modifier.HasFlag(ModifierType.Protected))
            {
                result += "protected ";
                if (modifier.HasFlag(ModifierType.Internal))
                {
                    result += "internal ";
                }
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

            if (modifier.HasFlag(ModifierType.Const))
            {
                result += "const ";
            }

            if (modifier.HasFlag(ModifierType.Volatile))
            {
                result += "volatile ";
            }

            if (modifier.HasFlag(ModifierType.Extern))
            {
                result += "extern ";
            }
            else if (modifier.HasFlag(ModifierType.Abstract))
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
            else if (modifier.HasFlag(ModifierType.New))
            {
                result += "new ";
            }

            if (modifier.HasFlag(ModifierType.Async))
            {
                result += "async ";
            }

            if (modifier.HasFlag(ModifierType.ReadOnly))
            {
                result += "readonly ";
            }

            if (modifier.HasFlag(ModifierType.This))
            {
                result += "this ";
            }

            if (modifier.HasFlag(ModifierType.Ref))
            {
                result += "ref ";
            }
            else if (modifier.HasFlag(ModifierType.In))
            {
                result += "in ";
            }
            else if (modifier.HasFlag(ModifierType.Out))
            {
                result += "out ";
            }

            if (modifier.HasFlag(ModifierType.Params))
            {
                result += "params ";
            }

            if (modifier.HasFlag(ModifierType.ReturnReadOnly))
            {
                result += "readonly ";
            }

            if (modifier.HasFlag(ModifierType.Partial))
            {
                result += "partial ";
            }

            if (modifier.HasFlag(ModifierType.Record))
            {
                result += "record ";
            }
            else if (modifier.HasFlag(ModifierType.Class))
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
