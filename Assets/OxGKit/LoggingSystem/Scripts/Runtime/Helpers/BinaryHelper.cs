using System;
using System.Text;
using UnityEngine;

namespace OxGKit.LoggingSystem
{
    public static class BinaryHelper
    {
        public struct ConfigInfo
        {
            public ConfigFileType type;
            public string content;
        }

        public static byte[] EncryptToBytes(string content)
        {
            byte[] writeBuffer;
            byte[] data = Encoding.UTF8.GetBytes(content);

            // Encrypt
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= LoggersConfig.CIPHER << 1;
            }

            // Write data with header
            int pos = 0;
            byte[] dataWithHeader = new byte[data.Length + 2];
            // Write header (non-encrypted)
            WriteInt16(LoggersConfig.CIPHER_HEADER, dataWithHeader, ref pos);
            Buffer.BlockCopy(data, 0, dataWithHeader, pos, data.Length);
            writeBuffer = dataWithHeader;
            return writeBuffer;
        }

        public static ConfigInfo DecryptToString(byte[] data)
        {
            int pos = 0;
            ConfigInfo info = new ConfigInfo();

            // Read header (non-encrypted)
            var header = ReadInt16(data, ref pos);
            if (header == LoggersConfig.CIPHER_HEADER)
            {
                info.type = ConfigFileType.Bytes;

                // Read data without header
                byte[] dataWithoutHeader = new byte[data.Length - 2];
                Buffer.BlockCopy(data, pos, dataWithoutHeader, 0, data.Length - pos);
                // Decrypt
                for (int i = 0; i < dataWithoutHeader.Length; i++)
                {
                    dataWithoutHeader[i] ^= LoggersConfig.CIPHER << 1;
                }

                // To string
                info.content = Encoding.UTF8.GetString(dataWithoutHeader);
                Debug.Log($"<color=#4eff9e>[Source is Cipher] Check -> {LoggersConfig.LOGGERS_CONFIG_FILE_NAME}</color>");
            }
            else
            {
                info.type = ConfigFileType.Json;

                // To string
                info.content = Encoding.UTF8.GetString(data);
                Debug.Log($"<color=#4eff9e>[Source is Plaintext] Check -> {LoggersConfig.LOGGERS_CONFIG_FILE_NAME}</color>");
            }

            return info;
        }

        public static void WriteInt16(short value, byte[] buffer, ref int pos)
        {
            WriteUInt16((ushort)value, buffer, ref pos);
        }

        internal static void WriteUInt16(ushort value, byte[] buffer, ref int pos)
        {
            buffer[pos++] = (byte)value;
            buffer[pos++] = (byte)(value >> 8);
        }

        public static short ReadInt16(byte[] buffer, ref int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                short value = (short)((buffer[pos]) | (buffer[pos + 1] << 8));
                pos += 2;
                return value;
            }
            else
            {
                short value = (short)((buffer[pos] << 8) | (buffer[pos + 1]));
                pos += 2;
                return value;
            }
        }

        internal static ushort ReadUInt16(byte[] buffer, ref int pos)
        {
            return (ushort)ReadInt16(buffer, ref pos);
        }
    }
}