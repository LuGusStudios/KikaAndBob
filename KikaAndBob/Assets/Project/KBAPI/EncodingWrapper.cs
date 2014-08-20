using UnityEngine;
using System.Collections;

[NotConverted]
[NotRenamed]
public static class EncodingWrapper 
{
	public static string Base64Encode(string subject)
	{
		var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(subject);
		return System.Convert.ToBase64String(plainTextBytes);
	}

	public static string Base64Decode(string subject)
	{  
		var base64EncodedBytes = System.Convert.FromBase64String(subject);
		return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
	}

    [NotRenamed]
    public static string MD5Sum(string strToEncrypt)
    {

        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		//System.Text.ASCIIEncoding ue = new System.Text.ASCIIEncoding();
		
        byte[] bytes = ue.GetBytes(strToEncrypt);

     

        // encrypt bytes

        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

        byte[] hashBytes = md5.ComputeHash(bytes);

     

        // Convert the encrypted bytes back to a string (base 16)

        string hashString = "";

     

        for (int i = 0; i < hashBytes.Length; i++)

        {

            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');

        }

     

        return hashString.PadLeft(32, '0');

 

}
}