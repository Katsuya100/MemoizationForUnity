﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Katuusagi.MemoizationForUnity.SourceGenerator.Utils
{
    public static class SyntaxUtils
    {
        public static AttributeSyntax GetAttribute(this MemberDeclarationSyntax self, string name)
        {
            return self.AttributeLists.SelectMany(v => v.Attributes).FirstOrDefault(v => v.Name.ToString() == name);
        }

        public static AttributeArgumentSyntax GetArgument(this AttributeSyntax self, string name)
        {
            var argments = self.ArgumentList?.Arguments;
            return argments?.FirstOrDefault(v => v.NameEquals?.Name?.ToString() == name || v.NameColon?.Name?.ToString() == name);
        }

        public static bool HasModifier(this MemberDeclarationSyntax self, string name)
        {
            return self.Modifiers.Any(v => v.ValueText == name);
        }

        public static bool IsPerfectPartial(this TypeDeclarationSyntax self)
        {
            var parents = GetAncestorsAndSelf<TypeDeclarationSyntax>(self);
            return parents.All(v => v.HasModifier("partial"));
        }

        public static IEnumerable<string> GetTypeNames(this TypeDeclarationSyntax self, string fullNameSpace, string typeName)
        {
            var typeFullName = $"{fullNameSpace}.{typeName}";
            var usings = self.GetAncestorUsings();
            yield return typeFullName;

            var parentNameSpaces = new HashSet<string>(self.GetNamespaces());
            var typeNameSpaces = GetNameSpaces(fullNameSpace);
            foreach (var typeNameSpace in typeNameSpaces)
            {
                if (!parentNameSpaces.Contains(typeNameSpace))
                {
                    continue;
                }

                yield return typeFullName.Replace($"{typeNameSpace}.", string.Empty);
            }

            var usingNameSpaces = new HashSet<string>(usings.Where(v => string.IsNullOrEmpty(v.StaticKeyword.Text) && v.Alias == null).Select(v => v.Name.ToString()));
            if (usingNameSpaces.Contains(fullNameSpace))
            {
                yield return typeName;
            }

            var aliases = usings.Where(v => v.Name.ToString() == typeFullName && v.Alias != null).Select(v => v.Name.ToString()).ToArray();
            foreach (var alias in aliases)
            {
                yield return alias;
            }
        }

        public static IEnumerable<string> GetNameSpaces(string fullNamespace)
        {
            var namespaces = fullNamespace.Split('.');
            var result = string.Empty;
            foreach (var @namespace in namespaces)
            {
                result += @namespace;
                yield return result;
                result += ".";
            }
        }

        public static IEnumerable<string> GetNamespaces(this TypeDeclarationSyntax self)
        {
            var typeNameSpace = self.GetNameSpace();
            return GetNameSpaces(typeNameSpace);
        }

        public static IEnumerable<UsingDirectiveSyntax> GetAncestorUsings(this MemberDeclarationSyntax self)
        {
            var ancestors = self.GetAncestors<CSharpSyntaxNode>().Reverse();
            foreach (var ancestor in ancestors)
            {
                if (ancestor is NamespaceDeclarationSyntax @namespace)
                {
                    foreach (var @using in @namespace.Usings)
                    {
                        yield return @using;
                    }
                }
                else if (ancestor is CompilationUnitSyntax root)
                {
                    foreach (var @using in root.Usings)
                    {
                        yield return @using;
                    }
                }
            }
        }

        public static string GetNameSpace(this TypeDeclarationSyntax self)
        {
            string result = string.Empty;
            var namespaces = GetAncestors<NamespaceDeclarationSyntax>(self).Reverse();
            foreach (var @namespace in namespaces)
            {
                result = $"{result}{@namespace.Name}.";
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Remove(result.Length - 1, 1);
            }
            return result;
        }

        public static string GetAncestorPath(this TypeDeclarationSyntax self)
        {
            var ancestors = self.GetAncestors<TypeDeclarationSyntax>().Reverse();
            var ancestorPath = string.Concat(ancestors.Select(v => $"{v.Identifier}-"));
            if (!string.IsNullOrEmpty(ancestorPath))
            {
                ancestorPath = ancestorPath.Remove(ancestorPath.Length - 1);
            }

            return ancestorPath;
        }

        public static string GetFullName(this TypeDeclarationSyntax self)
        {
            string result = self.Identifier.ToString();
            var ancestorPath = GetAncestorPath(self);
            if (!string.IsNullOrEmpty(ancestorPath))
            {
                result = $"{ancestorPath}-{result}";
            }

            var nameSpace = GetNameSpace(self);
            if (!string.IsNullOrEmpty(nameSpace))
            {
                result = $"{nameSpace}.{result}";
            }

            return result;
        }

        public static IEnumerable<T> GetAncestors<T>(this MemberDeclarationSyntax self)
        {
            if (self == null)
            {
                yield break;
            }

            if (self.Parent is CompilationUnitSyntax root &&
                root is T rootResult)
            {
                yield return rootResult;
                yield break;
            }

            foreach (var ancestor in GetAncestorsAndSelf<T>(self.Parent as MemberDeclarationSyntax))
            {
                yield return ancestor;
            }
        }

        public static IEnumerable<T> GetAncestorsAndSelf<T>(this MemberDeclarationSyntax self)
        {
            if (self == null)
            {
                yield break;
            }

            if (self is T result)
            {
                yield return result;
            }

            if (self.Parent is CompilationUnitSyntax root &&
                root is T rootResult)
            {
                yield return rootResult;
                yield break;

            }

            var parents = GetAncestorsAndSelf<T>(self.Parent as MemberDeclarationSyntax);
            foreach (var parent in parents)
            {
                yield return parent;
            }
        }

        public static IEnumerable<TypeDeclarationSyntax> GetStructuredTypes(this CompilationUnitSyntax self)
        {
            return GetTypes(self.Members).Where(v => v is TypeDeclarationSyntax);
        }

        public static IEnumerable<T> GetTypes<T>(this CompilationUnitSyntax self)
            where T : TypeDeclarationSyntax
        {
            return GetTypes(self.Members).OfType<T>();
        }

        public static IEnumerable<T> GetTypes<T>(this NamespaceDeclarationSyntax self)
            where T : TypeDeclarationSyntax
        {
            return GetTypes(self.Members).OfType<T>();
        }

        public static IEnumerable<T> GetTypes<T>(this TypeDeclarationSyntax self)
            where T : TypeDeclarationSyntax
        {
            return GetTypes(self.Members).OfType<T>();
        }

        public static IEnumerable<TypeDeclarationSyntax> GetTypes(this CompilationUnitSyntax self)
        {
            return GetTypes(self.Members);
        }

        public static IEnumerable<TypeDeclarationSyntax> GetTypes(this NamespaceDeclarationSyntax self)
        {
            return GetTypes(self.Members);
        }

        public static IEnumerable<TypeDeclarationSyntax> GetTypes(this TypeDeclarationSyntax self)
        {
            return GetTypes(self.Members);
        }

        private static IEnumerable<TypeDeclarationSyntax> GetTypes(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (var member in members)
            {
                if (member is NamespaceDeclarationSyntax @namespace)
                {
                    foreach (var elem in @namespace.GetTypes())
                    {
                        yield return elem;
                    }

                    continue;
                }

                if (member is TypeDeclarationSyntax type)
                {
                    yield return type;

                    foreach (var elem in type.GetTypes())
                    {
                        yield return elem;
                    }

                    continue;
                }
            }
        }

        public static IEnumerable<FieldDeclarationSyntax> GetFields(this TypeDeclarationSyntax self)
        {
            return self.Members.OfType<FieldDeclarationSyntax>();
        }

        public static IEnumerable<PropertyDeclarationSyntax> GetProperties(this TypeDeclarationSyntax self)
        {
            return self.Members.OfType<PropertyDeclarationSyntax>();
        }

        public static IEnumerable<EventDeclarationSyntax> GetEvents(this TypeDeclarationSyntax self)
        {
            return self.Members.OfType<EventDeclarationSyntax>();
        }

        public static IEnumerable<MethodDeclarationSyntax> GetMethods(this TypeDeclarationSyntax self)
        {
            return self.Members.OfType<MethodDeclarationSyntax>();
        }
    }
}
