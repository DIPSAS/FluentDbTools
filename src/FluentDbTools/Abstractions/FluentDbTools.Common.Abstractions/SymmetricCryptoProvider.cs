using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Handles Symmetric Cryptography Encryption and Decryption
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SymmetricCryptoProvider
    {
        //var keys = new byte[512];
        //var iv = new byte[512];

        //System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(keys);
        //System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(iv);
        //var keysS = Convert.ToBase64String(keys);
        //var ivS = Convert.ToBase64String(iv);

        private readonly byte[] Key;
        private readonly byte[] IV;
        private readonly string AlgorithmName;

        /// <summary>
        /// Known Supported Symmetric Cryptography algorithms
        /// </summary>
        [SuppressMessage("ReSharper", "StringLiteralTypo")] 
        public static string[] SupportedAlgorithms = {
            "Aes",
            "AesManaged",
            "Rijndael",
            "DES",
            "3DES",
            "RC2"
        };

        /// <summary>
        /// Default Symmetric Cryptography algorithm is First value from [<see cref="SupportedAlgorithms"/>]
        /// </summary>
        public static string DefaultAlgorithm = SupportedAlgorithms.FirstOrDefault();

        /// <summary>
        /// Create SymmetricCryptoProvider by <paramref name="algorithmName"/>, <paramref name="key"/> and <paramref name="iv"/>
        /// </summary>
        /// <param name="algorithmName">Leave null to use <see cref="DefaultAlgorithm"/></param>
        /// <param name="key">Leave null to use Internal Key from <see cref="Secrets.SecretsResource.Key512Bytes()"/></param>
        /// <param name="iv">Leave null to use Internal IV from <see cref="Secrets.SecretsResource.IV512Bytes()"/></param>
        public SymmetricCryptoProvider(
            string algorithmName, 
            byte[] key, 
            byte[] iv)
        {
            AlgorithmName = algorithmName.WithDefault(DefaultAlgorithm);
            Key = key ?? Secrets.SecretsResource.Key512Bytes();
            IV = iv ?? Secrets.SecretsResource.IV512Bytes();
        }

        /// <summary>
        /// Create SymmetricCryptoProvider by values in config-section:
        /// algorithmName : From algorithm:name
        /// key : From algorithm:key
        /// iv : From algorithm:iv
        /// </summary>
        /// <param name="getConfigValue"></param>
        public SymmetricCryptoProvider(Func<string, string> getConfigValue = null)
        : this(
            getConfigValue?.Invoke("algorithm:name"),
            FromBase64String(getConfigValue?.Invoke( "algorithm:key")),
            FromBase64String(getConfigValue?.Invoke( "algorithm:iv")))
        {
        }

        /// <summary>
        /// Return Encrypted <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] bytes)
        {
            return Protect(AlgorithmName, bytes, Key, IV);
        }

        /// <summary>
        /// Return Encrypted <paramref name="stringToEncrypt"/>
        /// </summary>
        /// <param name="stringToEncrypt"></param>
        /// <returns></returns>
        public byte[] Encrypt(string stringToEncrypt)
        {
            return Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt));
        }

        /// <summary>
        /// Return Decrypted <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] bytes)
        {
            return Unprotect(AlgorithmName, bytes, Key, IV);
        }

        private static byte[] Protect(string algorithmName, byte[] bytes, byte[] key, byte[] iv)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(bytes), $"{nameof(bytes)} is null or empty");
            }

            using (var cryptoServiceProvider = GetAesSymmetricAlgorithm(algorithmName, key, iv))
            {
                return cryptoServiceProvider
                    .CreateEncryptor()
                    .TransformFinalBlock(bytes, 0, bytes.Length);
            }
        }

        private static byte[] Unprotect(string algorithmName,byte[] bytes, byte[] key, byte[] iv)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new ArgumentNullException(nameof(bytes), $"{nameof(bytes)} is null or empty");
            }

            using (var cryptoServiceProvider = GetAesSymmetricAlgorithm(algorithmName, key, iv))
            {
                return cryptoServiceProvider
                    .CreateDecryptor()
                    .TransformFinalBlock(bytes, 0, bytes.Length);
            }
        }


        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static SymmetricAlgorithm GetAesSymmetricAlgorithm(string algorithmName, byte[] key, byte[] iv)
        {
            var keyBitsLength = key.Length * 8;
            var ivBitsLength = iv.Length * 8;

            var symAlg = SymmetricAlgorithm.Create(algorithmName.WithDefault(DefaultAlgorithm)) ?? Aes.Create();

            if (!symAlg.ValidKeySize(keyBitsLength))
            {
                Array.Resize(ref key, symAlg.LegalKeySizes.Max(x => x.MaxSize) / 8);
            }

            if (ivBitsLength != symAlg.BlockSize)
            {
                Array.Resize(ref iv, symAlg.BlockSize / 8);
            }

            symAlg.IV = iv;
            symAlg.Key = key;

            return symAlg;
        }

        private static byte[] FromBase64String(string base64String)
        {
            return base64String.IsNotEmpty() ? Convert.FromBase64String(base64String) : null;
        }
    }
}