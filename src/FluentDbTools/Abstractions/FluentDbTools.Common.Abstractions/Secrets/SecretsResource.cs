using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FluentDbTools.Common.Abstractions.Secrets
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class SecretsResource
    {
        private static readonly Assembly CurrentAssembly = typeof(SecretsResource).Assembly;

        internal static readonly string Location = typeof(SecretsResource).Namespace;

        internal static string KeyBase64String() => CurrentAssembly.GetStringFromEmbeddedResource($"{Location}.{nameof(Key512Bytes)}.txt");
        internal static string IVBase64String() => CurrentAssembly.GetStringFromEmbeddedResource($"{Location}.{nameof(IV512Bytes)}.txt");

        internal static byte[] Key512Bytes() => Convert.FromBase64String(KeyBase64String());
        internal static byte[] IV512Bytes() => Convert.FromBase64String(IVBase64String());

    }
}