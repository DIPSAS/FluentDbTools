using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using Xunit;

namespace Test.FluentDbTools
{
    public class CryptDecryptTests
    {
        [Fact]
        public void EncryptAndDecrypt_SameInstance_ShouldBeDecryptedOk()
        {
            var plainText = "Arild";
            var symmetricCryptoProvider = new SymmetricCryptoProvider();
            var ecrypted = symmetricCryptoProvider.Encrypt(plainText);
            Encoding.UTF8.GetString(symmetricCryptoProvider.Decrypt(ecrypted)).Should().Be(plainText);

        }


        [Fact]
        public void EncryptAndDecrypt_DifferenceInstances_ShouldBeDecryptedOk()
        {
            var plainText = "Arild";
            var ecrypted = new SymmetricCryptoProvider().Encrypt(plainText);
            Encoding.UTF8.GetString(new SymmetricCryptoProvider().Decrypt(ecrypted)).Should().Be(plainText);


        }
        [Theory]
        [InlineData(null,true)]
        [InlineData("DES",true)]
        [InlineData("3DES",true)]
        [InlineData("Rijndael",true)]
        [InlineData("AesManaged",true)]
        [InlineData("RC2",true)]
        [InlineData("MD5",false)]
        [InlineData("SHA",false)]
        [InlineData("SHA1",false)]
        [InlineData("SHA256",false)]
        [InlineData("SHA512",false)]
        public void EncryptAndDecryptWithAlgorithmName_DifferenceInstances_ShouldBeDecryptedWithOkOrFalse(string algorithmName, bool ok)
        {
            var plainText = "Arild";
            Action action = () =>
            {
                var ecrypted = new SymmetricCryptoProvider(getConfigValue).Encrypt(plainText);
                Encoding.UTF8.GetString(new SymmetricCryptoProvider(getConfigValue).Decrypt(ecrypted)).Should()
                    .Be(plainText);
            };

            if (ok)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<Exception>();

            }
            string getConfigValue(string key)
            {
                new Dictionary<string, string>
                {
                    {"algorithm:name", algorithmName}
                }.TryGetValue(key, out var value);

                return value;
            }
        }

        [Fact]
        public void EncryptAndDecryptWithSupportedAlgorithms_DifferenceInstances_ShouldBeDecryptedOk()
        {
            foreach (var algorithm in SymmetricCryptoProvider.SupportedAlgorithms)
            {
                EncryptAndDecryptWithAlgorithmName_DifferenceInstances_ShouldBeDecryptedWithOkOrFalse(algorithm,true);
            }   
        }

    }
}