﻿using System;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
#endif

namespace MarkerMetro.Unity.WinLegacy.Cryptography
{
    public static class EncryptionProvider
    {

        public static string GetMD5HexString(string input)
        {
#if NETFX_CORE
            return CryptographicBuffer.EncodeToHexString(GetMD5Hash(input));

#elif WINDOWS_PHONE
            return MD5CryptoServiceProvider.GetMd5String(input);
#else
            throw new System.PlatformNotSupportedException();
#endif
        }

        public static string GetSHA1HexString(string input)
        {
#if NETFX_CORE
            return CryptographicBuffer.EncodeToHexString(GetSHA1Hash(input));
#elif WINDOWS_PHONE
            var sha = new System.Security.Cryptography.SHA1Managed();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var bytesHash = sha.ComputeHash(bytes);
            return System.Text.Encoding.UTF8.GetString(bytesHash, 0, bytesHash.Length);
#else
            throw new System.PlatformNotSupportedException();
#endif
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Returns an encrypted text.
        /// </summary>
        /// <param name="toEncrypt">String to be encrypted</param>
        /// <param name="key">Unique key for encryption/decryption</param>m>
        /// <returns>Returns encrypted string.</returns>
        public static string Encrypt(string toEncrypt, string key)
        {
#if NETFX_CORE
            try
            {
                // Get the MD5 key hash (you can as well use the binary of the key string)
                var keyHash = GetMD5Hash(key);

                // Create a buffer that contains the encoded message to be encrypted.
                var toDecryptBuffer = CryptographicBuffer.ConvertStringToBinary(toEncrypt, BinaryStringEncoding.Utf8);

                // Open a symmetric algorithm provider for the specified algorithm.
                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);

                // Create a symmetric key.
                var symetricKey = aes.CreateSymmetricKey(keyHash);

                // The input key must be securely shared between the sender of the cryptic message
                // and the recipient. The initialization vector must also be shared but does not
                // need to be shared in a secure manner. If the sender encodes a message string
                // to a buffer, the binary encoding method must also be shared with the recipient.
                var buffEncrypted = CryptographicEngine.Encrypt(symetricKey, toDecryptBuffer, null);

                // Convert the encrypted buffer to a string (for display).
                // We are using Base64 to convert bytes to string since you might get unmatched characters
                // in the encrypted buffer that we cannot convert to string with UTF8.
                var strEncrypted = CryptographicBuffer.EncodeToBase64String(buffEncrypted);

                return strEncrypted;
            }
            catch (Exception)
            {
                // MetroEventSource.Log.Error(ex.Message);
                return "";
            }
#elif WINDOWS_PHONE
            var encoding = new System.Text.UTF8Encoding();

            byte[] Key = DESCrytography.DoPadWithString(encoding.GetBytes(key), 24, (byte)0);
            byte[] plainText = new byte[1024];
            plainText = DESCrytography.DoPadWithString(encoding.GetBytes(toEncrypt), 8, (byte)0);

            byte[] cipherText = null;
            DESCrytography.TripleDES(plainText, ref cipherText, Key, true);

            string result = Convert.ToBase64String(cipherText);
            result = result.Replace("+", "-").Replace("/", "_");

            return result;
#else
            throw new System.PlatformNotSupportedException();
#endif
        }

        /// <summary>
        /// Decrypt a string using dual encryption method. Return a Decrypted clear string
        /// </summary>
        /// <param name="cipherString">Encrypted string</param>
        /// <param name="key">Unique key for encryption/decryption</param>
        /// <returns>Returns decrypted text.</returns>
        public static string Decrypt(string cipherString, string key)
        {
#if NETFX_CORE
            try
            {
                // Get the MD5 key hash (you can as well use the binary of the key string)
                var keyHash = GetMD5Hash(key);

                // Create a buffer that contains the encoded message to be decrypted.
                IBuffer toDecryptBuffer = CryptographicBuffer.DecodeFromBase64String(cipherString);

                // Open a symmetric algorithm provider for the specified algorithm.
                SymmetricKeyAlgorithmProvider aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);

                // Create a symmetric key.
                var symetricKey = aes.CreateSymmetricKey(keyHash);

                var buffDecrypted = CryptographicEngine.Decrypt(symetricKey, toDecryptBuffer, null);

                string strDecrypted = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);

                return strDecrypted;
            }
            catch (Exception)
            {
                // MetroEventSource.Log.Error(ex.Message);
                //throw;
                return "";
            }
#elif WINDOWS_PHONE
            var encoding = new System.Text.UTF8Encoding();

            byte[] Key = DESCrytography.DoPadWithString(encoding.GetBytes(key), 24, (byte)0);
            byte[] plainText = Convert.FromBase64String(cipherString.Replace("-", "+").Replace("_", "/"));

            byte[] cipherText = null;
            DESCrytography.TripleDES(plainText, ref cipherText, Key, false);

            string result = encoding.GetString(cipherText, 0, cipherText.Length).Replace(Convert.ToChar(0x0).ToString(), "");

            return result;
#else
            throw new System.PlatformNotSupportedException();
#endif
        }

#if NETFX_CORE
        internal static IBuffer GetMD5Hash(string key)
        {
            return GetHash(HashAlgorithmNames.Md5, key);
        }
        internal static IBuffer GetSHA1Hash(string key)
        {
            return GetHash(HashAlgorithmNames.Sha1, key);
        }

        private static IBuffer GetHash(string algorithm, string key)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algorithm);

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            return buffHash;
        }
#endif
    }
}
