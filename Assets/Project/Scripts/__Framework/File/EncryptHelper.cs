/**
 * @file    EncryptHelper.cs
 * @brief
 *
 * @author  $Author$
 * @date    $Date$
 * @version $Rev$
 */
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Security.Cryptography;

// オープンソースの暗号モジュールBouncyCastleを使用
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;

namespace SmileLab {
/// <summary>
/// ヘルパークラス：暗号化
/// </summary>
public static class EncryptHelper
{
    /// <summary>
    /// MD5ハッシュ文字列取得
    /// </summary>
    public static string ToHashMD5(this byte[] data)
    {
        byte[] hash = null;
        using(var md5 = new MD5CryptoServiceProvider()){
            hash = md5.ComputeHash(data);
        }
        return BitConverter.ToString(hash).ToLower().Replace("-","") ;
    }
    /// <summary>
    /// UTF-16 文字列からMD5ハッシュ文字列取得
    /// </summary>
    public static string ToHashMD5(this string str)
    {
        return ToHashMD5(Encoding.UTF8.GetBytes(str));
    }

    /// <summary>
    /// MD5ハッシュ
    /// </summary>
    public static Hash128 ToHash128MD5(this byte[] data)
    {
        byte[] hash = null;
        using(var md5 = new MD5CryptoServiceProvider()){
            hash = md5.ComputeHash(data);
        }
        
        return new Hash128(
            BitConverter.ToUInt32(hash, 0),
            BitConverter.ToUInt32(hash, 4),
            BitConverter.ToUInt32(hash, 8),
            BitConverter.ToUInt32(hash, 12)
        );
    }
    /// <summary>
    /// UTF-16 文字列からMD5ハッシュ
    /// </summary>
    public static Hash128 ToHash128MD5(this string str)
    {
        return ToHash128MD5(Encoding.UTF8.GetBytes(str));
    }

    /// <summary>
    /// SHA256ハッシュ文字列取得
    /// </summary>
    public static string ToHashSHA256(this byte[] data)
    {
        byte[] hash = null;
        using(var has256 = SHA256.Create()){
            hash = has256.ComputeHash(data);    
        }
        return BitConverter.ToString(hash).ToLower().Replace("-", "");
    }
    /// <summary>
    /// UTF-16 文字列からSHA256ハッシュ文字列取得
    /// </summary>
    public static string ToHashSHA256(this string str)
    {
        return ToHashSHA256(Encoding.UTF8.GetBytes(str));
    }


    /// <summary>
    /// 指定パスのファイルに、指定データを3DES暗号化して書きこむ.
    /// </summary>
    public static void WriteToFile(byte[] data, string key,string filePath)
    {
        using(var tDes = new TripleDESCryptoServiceProvider()){
            tDes.Key = GenerateKey(key, tDes.Key.Length);
            tDes.IV = GenerateKey(key, tDes.IV.Length);

            using(var fStream = File.Create(filePath)){
                using(var cStream = new CryptoStream(fStream, tDes.CreateEncryptor(), CryptoStreamMode.Write) ){
                    cStream.Write(data, 0,data.Length);
                }
            }
        }
        //CSDebug.Log("[MCEncryptHelper] Write To File : " + filePath + ", length = " + data.Length);
    }
    /// <summary>
    /// 3DES暗号：指定キーでデータを暗号化して返す.
    /// </summary>
    public static byte[] EncryptData(string key,byte[] sources)
    {
        byte[] data = null;
        using(var tDes = new TripleDESCryptoServiceProvider()){
            tDes.Key = GenerateKey(key, tDes.Key.Length);
            tDes.IV = GenerateKey(key, tDes.IV.Length);

            using(var memStream = new MemoryStream()){
                using(var cStream = new CryptoStream(memStream, tDes.CreateEncryptor(), CryptoStreamMode.Write) ){
                    cStream.Write(sources, 0,sources.Length);
                }
                data = memStream.ToArray();
            }
        }
        return data;
    }


    /// <summary>
    /// 3DES暗号：指定パスのファイルを読み込んで、指定キーで復号化して返す.
    /// </summary>
    public static byte[] ReadFromFile(string key,string filePath)
    {
        if( !File.Exists(filePath)){
            return null;
        }

        byte[] data = null;
        using(var stream = new FileStream(filePath,FileMode.Open)){
            data = DecryptData(key,stream);
        }
        return data;
    }
    /// <summary>
    /// 3DES暗号：指定キーでデータを復号化して返す.
    /// </summary>
    public static byte[] DecryptData(string key,byte[] sources)
    {
        byte[] data = null;
        using(var stream = new MemoryStream(sources)){
            data = DecryptData(key,stream);
        }
        return data;
    }
    /// <summary>3DES暗号：指定キーでデータを復号化して返す.</summary>
    public static byte[] DecryptData(string key,Stream sourceStream)
    {
        byte[] data = null;
        using(var tDes = new TripleDESCryptoServiceProvider()){
            tDes.Key = GenerateKey(key, tDes.Key.Length);
            tDes.IV = GenerateKey(key, tDes.IV.Length);

            using(var cStream = new CryptoStream(sourceStream, tDes.CreateDecryptor(), CryptoStreamMode.Read) ){
                using(var reader = new BinaryReader(cStream)){
                    data = reader.ReadBytes((int)sourceStream.Length);
                }
            }
        }
        //Debug.Log("[Encrypthelper] Read From File : " + filePath + ", length = " + data.Length);
        return data;
    }

    /// <summary>
    /// TripleDESで暗号化されたバイト列を文字列として復号化する
    /// </summary>
    public static string Decrypt3DESToString(byte[] key, byte[] iv, byte[] source)
    {
        byte[] data = null;

        using(var memStream = new MemoryStream(source)) {
            using(var tDes = new TripleDESCryptoServiceProvider()) {
                tDes.Key = key;
                tDes.IV = iv;

                using(var cStream = new CryptoStream(memStream, tDes.CreateDecryptor(), CryptoStreamMode.Read)) {
                    using(var reader = new BinaryReader(cStream)) {
                        data = reader.ReadBytes((int)memStream.Length);
                    }
                }
            }
        }

        return Encoding.UTF8.GetString(data);
    }

    /// <summary>
    /// 3DES暗号：指定キー文字列から、暗号化バイト列を返す
    /// </summary>
    public static byte[] GenerateKey(string key,int length)
    {
        var res = new byte[length];
        for(int i = 0; i < length; i++){
            res[i] = (byte)((key[i % key.Length] + i) & 0xff);
        }
        return res;
    }

    /// <summary>
    /// RSA暗号 : 公開鍵で暗号化します。
    /// </summary>
    public static byte[] RSAEncryptWithPublickey(byte[] source, string key)
    {
        var rsa = new Pkcs1Encoding(new RsaEngine());
        var keyParam = (AsymmetricKeyParameter)new PemReader(new StringReader(key)).ReadObject();
        rsa.Init(true, keyParam);
        return rsa.ProcessBlock(source, 0, source.Length);
    }

    /// <summary>
    /// RSA暗号 : 秘密鍵で暗号化します。
    /// </summary>
    public static byte[] RSAEncryptWithPrivatekey(byte[] source, string key)
    {
        var rsa = new Pkcs1Encoding(new RsaEngine());
        var keyParam = (AsymmetricCipherKeyPair)new PemReader(new StringReader(key)).ReadObject();
        rsa.Init(true, keyParam.Private);
        return rsa.ProcessBlock(source, 0, source.Length);
    }

    /// <summary>
    /// RSA暗号 : 公開鍵で復号化します。
    /// </summary>
    public static byte[] RSADecryptWithPublickey(byte[] source, string key)
    {
        var rsa = new Pkcs1Encoding(new RsaEngine());
        var keyParam = (AsymmetricKeyParameter)new PemReader(new StringReader(key)).ReadObject();
        rsa.Init(false, keyParam);
        return rsa.ProcessBlock(source, 0, source.Length);
    }

    /// <summary>
    /// RSA暗号 : 秘密鍵で復号化します。
    /// </summary>
    public static byte[] RSADecryptWithPrivatekey(byte[] source, string key)
    {
        var rsa = new Pkcs1Encoding(new RsaEngine());
        var keyParam = (AsymmetricCipherKeyPair)new PemReader(new StringReader(key)).ReadObject();
        rsa.Init(false, keyParam.Private);
        return rsa.ProcessBlock(source, 0, source.Length);
    }
}
}
