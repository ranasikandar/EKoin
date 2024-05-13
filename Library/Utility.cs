using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library
{
    public class Utility
    {
        //public static byte[] ObjectToByteArray(Object obj)
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    using (var ms = new MemoryStream())
        //    {
        //        bf.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //public static Object ByteArrayToObject(byte[] arrBytes)
        //{
        //    using (var memStream = new MemoryStream())
        //    {
        //        var binForm = new BinaryFormatter();
        //        memStream.Write(arrBytes, 0, arrBytes.Length);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var obj = binForm.Deserialize(memStream);
        //        return obj;
        //    }
        //}


        //private static byte[] ObjectToByteArray<T>(T t)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var bf = new BinaryFormatter();
        //        bf.Serialize(ms, t);
        //        return ms.ToArray();
        //    }
        //}

        //private static T ByteArrayToObject<T>(byte[] bytes)
        //{
        //    using (var memStream = new MemoryStream())
        //    {
        //        var binForm = new BinaryFormatter();
        //        memStream.Write(bytes, 0, bytes.Length);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var obj = binForm.Deserialize(memStream);
        //        return (T)obj;
        //    }
        //}


        //public static byte[] ObjectToByteArray(object obj)
        //{
        //    if (obj == null)
        //        return null;

        //    using var stream = new MemoryStream();

        //    Serializer.Serialize(stream, obj);

        //    return stream.ToArray();
        //}

        //public static T ByteArrayToObject<T>(byte[] arrBytes)
        //{
        //    using var stream = new MemoryStream();

        //    // Ensure that our stream is at the beginning.
        //    stream.Write(arrBytes, 0, arrBytes.Length);
        //    stream.Seek(0, SeekOrigin.Begin);

        //    return Serializer.Deserialize<T>(stream);
        //}

        public static string GetMd5HashX(object obj)
        {
            using (var md5 = MD5.Create())
            {
                BinaryFormatter serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    serializer.Serialize(memoryStream, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    return Convert.ToHexString(md5.ComputeHash(memoryStream.ToArray()));
                }
            }
        }

        public static string GetMd5Hash<T>(T t)
        {
            using (var md5 = MD5.Create())
            {
                var jsonString = JsonSerializer.Serialize(t);
                return Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(jsonString)));
            }
        }

        public static byte[] TtoByteArray<T>(T t)
        {
            string jsonString = JsonSerializer.Serialize(t);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static string GetMd5HashReflective(object obj)
        {
            using (var md5 = MD5.Create())
            {
                var properties = obj.GetType().GetProperties();
                StringBuilder sb = new StringBuilder();
                foreach (var property in properties)
                {
                    sb.Append(property.GetValue(obj));
                }
                return Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
            }
        }

        


    }
}
