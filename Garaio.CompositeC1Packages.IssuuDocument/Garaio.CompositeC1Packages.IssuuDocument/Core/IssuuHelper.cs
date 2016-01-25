using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Garaio.CompositeC1Packages.IssuuDocument.Configuration;

namespace Garaio.CompositeC1Packages.IssuuDocument.Core
{
	public static class IssuuHelper
	{
		/// <summary>
		/// Generate signatures for requests. For reference <see cref="http://developers.issuu.com/api/signingrequests.html"/>.
		/// </summary>
		/// <param name="parametersDict">
		/// Dictionary with all parameters for signing.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GenerateSignature(Dictionary<RequestParameters.RequestParamters, string> parametersDict)
		{
			if (parametersDict == null)
			{
				throw new ArgumentException("Dictionary with parameters has to be provided.", "parametersDict");
			}

			List<RequestParameters.RequestParamters> parameters = parametersDict.Keys.OrderBy(m => m.ToString()).ToList();

			StringBuilder stringForHash = new StringBuilder();
			stringForHash.Append(IssuuDocumentConfigurationFacade.Configuration.APISecretKey);
			foreach (RequestParameters.RequestParamters requestParamter in parameters)
			{
				stringForHash.Append(requestParamter + parametersDict[requestParamter]);
			}

			string ret = GetMd5Sum(stringForHash.ToString());
			return ret;
		}

		/// <summary>
		/// Generate MD5 Hash of a string. Encoding has to be utf-8. 
		/// </summary>
		/// <param name="str">
		/// String for hashing.
		/// </param>
		/// <returns>
		/// Hex output has to be lowerCase.
		/// </returns>
		public static string GetMd5Sum(string str)
		{
			// Check wether data was passed
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}

			// Calculate MD5 hash. This requires that the string is splitted into a byte[].
			byte[] result;
			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				byte[] textToHash = Encoding.UTF8.GetBytes(str);
				result = md5.ComputeHash(textToHash);
			}

			// Convert result back to string - to lowercase
			return BitConverter.ToString(result).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
		}
	}
}
