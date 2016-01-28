using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Garaio.CompositeC1Packages.IssuuDocument.Package.App_GlobalResources;

namespace Garaio.CompositeC1Packages.IssuuDocument.Core
{
	[Serializable]
	public sealed class IssuuApiException : Exception
	{
		private const string UnknownCodeAttributeValue = "unbekannt";
		private const string CodeAttributeName = "code";
		private const string MessageAttributeName = "message";
		private readonly XElement _errorElement;

		public IssuuApiException()
		{}

		public IssuuApiException(string message) : base(message)
		{}

		public IssuuApiException(string message, Exception innerException) : base(message, innerException)
		{}

		private IssuuApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{}

		public IssuuApiException(XElement errorElement)
			: base(string.Format(CultureInfo.CurrentCulture, Resources_Text.IssuDocument_Core_IssuuApiException_MessageFormat_Message, GetErrorMessageFromElement(errorElement), GetErrorCodeFromElement(errorElement)))
		{
			_errorElement = errorElement;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);
		}

		public string ErrorCode
		{
			get { return GetErrorCodeFromElement(_errorElement); }
		}

		public string ErrorMessage
		{
			get { return GetErrorMessageFromElement(_errorElement); }
		}

		private static string GetErrorCodeFromElement(XElement errorElement)
		{
			XAttribute codeAttribute = errorElement.Attribute(CodeAttributeName);
			if (codeAttribute != null && !string.IsNullOrEmpty(codeAttribute.Value))
			{
				return codeAttribute.Value;
			}

			return UnknownCodeAttributeValue;
		}

		private static string GetErrorMessageFromElement(XElement errorElement)
		{
			XAttribute messageAttribute = errorElement.Attribute(MessageAttributeName);
			if (messageAttribute != null && !string.IsNullOrEmpty(messageAttribute.Value))
			{
				return messageAttribute.Value;
			}

			return Resources_Text.IssuDocument_Core_IssuuApiException_UnknownException_Message;
		}
	}
}
