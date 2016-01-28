using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Garaio.CompositeC1Packages.IssuuDocument.Configuration;
using Garaio.CompositeC1Packages.IssuuDocument.DTO;
using Garaio.CompositeC1Packages.IssuuDocument.Package.App_GlobalResources;

namespace Garaio.CompositeC1Packages.IssuuDocument.Core
{
	public class IssuuController
	{
		private const string IssuuAddActionParameterValue = "issuu.document_embed.add";
		private const string IssuuListEmbedsActionParameterValue = "issuu.document_embeds.list";
		private const string IssuuListDocumentActionParameterValue = "issuu.documents.list";
		private const int PageSize = 100;
		private const int PageSizeForDocuments = 30;

		private readonly IssuuDocumentConfiguration _issuuConfig = IssuuDocumentConfigurationFacade.Configuration;

		/// <summary>
		/// Default Height for embedds.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Default Width for embedds.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Get embeds of all documents. Max pagesize is 100. If there are more elements,additional request is needed.
		///     Requests are allowed at rate 1 request/second for free account
		///     <see cref="http://developers.issuu.com/api/limits.html"/>.
		///     This method takes first embed of all documents,because of dataConfigId for dropdown.
		/// </summary>
		/// <param name="docDict">
		/// Dictionary with common-documents.
		/// </param>
		/// <returns>
		/// The
		///     <see>
		///         <cref>Dictionary</cref>
		///     </see>
		///     .
		/// </returns>
		public Dictionary<string, DocumentEmbed> GetEmbedsOfDocuments(Dictionary<string, DocumentCommon> docDict)
		{
			if (docDict == null)
			{
				throw new ArgumentException(Resources_Text.IssuDocument_Core_Exception_MissingArgument_DocumentCommon_Message, "docDict");
			}

			Dictionary<string, DocumentEmbed> resultDict = new Dictionary<string, DocumentEmbed>();
			bool more;
			int counter = 0;
			int startIndex = 0;

			do
			{
				// prepare parameters
				Dictionary<RequestParameters.RequestParamters, string> paramDict = new Dictionary<RequestParameters.RequestParamters, string>
																					{
																						{ RequestParameters.RequestParamters.action, IssuuListEmbedsActionParameterValue },
																						{ RequestParameters.RequestParamters.apiKey, _issuuConfig.APIKey },
																						{ RequestParameters.RequestParamters.pageSize, PageSize.ToString(CultureInfo.InvariantCulture) },
																						{ RequestParameters.RequestParamters.startIndex, startIndex.ToString(CultureInfo.InvariantCulture) }
																					};

				// build request
				string response = DoHttpRequest(paramDict);

				// parse xml to get data
				XDocument parsedDoc = XDocument.Parse(response);

				IEnumerable<XElement> result = parsedDoc.Descendants("rsp").Descendants("result");

				// is the pagesize big enough or there are more elements
				more = bool.Parse(result.First().Attribute("more").Value);

				IEnumerable<XElement> docsElements = parsedDoc.Descendants("rsp").Descendants("result").Descendants("documentEmbed");

				// try to get embed of document
				foreach (XElement docsElement in docsElements)
				{
					// deleted embeds don't have documentId -> find first one with id
					string docId = docsElement.Attribute("documentId") != null ? docsElement.Attribute("documentId").Value : null;
					if (docId == null)
					{
						continue;
					}

					if (docDict.ContainsKey(docId))
					{
						DocumentEmbed docEmbed = new DocumentEmbed(docDict[docId]);
						docDict.Remove(docId);
						docEmbed.EmbedId = docsElement.Attribute("id").Value;
						docEmbed.Height = docsElement.Attribute("height").Value;
						docEmbed.Width = docsElement.Attribute("width").Value;
						docEmbed.Created = docsElement.Attribute("created").Value;
						docEmbed.DataConfigId = docsElement.Attribute("dataConfigId").Value;
						resultDict.Add(docEmbed.DataConfigId, docEmbed);
					}
				}

				// if there's no embed found,check if there are more embedds and get them
				if (more)
				{
					// prepare parameters for next iterations
					// with premium licences,limits are 2 requests/sec or 3 requests/sec-> wait time needs to be adjusted (http://developers.issuu.com/api/limits.html)
					int requestsProSecond = int.Parse(IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond, CultureInfo.InvariantCulture);
					int sleepTime = (int)Math.Floor((decimal)(1000.0 / requestsProSecond));
					Thread.Sleep(sleepTime + 1);
					counter++;
					startIndex = counter * PageSize;
				}
			}
			while (more);

			// if there are still unprocessed documents (without embedds),create embed for them
			if (docDict.Any())
			{
				foreach (KeyValuePair<string, DocumentCommon> documentCommonKeyValue in docDict)
				{
					DocumentEmbed newDocEmbed = CreateEmbedForDocument(documentCommonKeyValue.Value);
					resultDict.Add(newDocEmbed.DataConfigId, newDocEmbed);
					int requestsProSecond = int.Parse(IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond, CultureInfo.InvariantCulture);
					int sleepTime = (int)Math.Floor((decimal)(1000.0 / requestsProSecond));
					Thread.Sleep(sleepTime + 1);
				}
			}

			return resultDict;
		}

		/// <summary>
		/// Request all available documents with one embed and prepare a dictionary for dropdown.
		/// </summary>
		/// <returns>
		/// The
		///     <see>
		///         <cref>Dictionary</cref>
		///     </see>
		///     .
		/// </returns>
		public Dictionary<string, string> ListDocuments()
		{
			bool more;
			int counter = 0;
			int startIndex = 0;

			Dictionary<string, DocumentCommon> commonDocsDict = new Dictionary<string, DocumentCommon>();
			do
			{
				// prepare parameters
				Dictionary<RequestParameters.RequestParamters, string> paramDict = new Dictionary<RequestParameters.RequestParamters, string>
																					{
																						{ RequestParameters.RequestParamters.action, IssuuListDocumentActionParameterValue },
																						{ RequestParameters.RequestParamters.apiKey, _issuuConfig.APIKey },
																						{ RequestParameters.RequestParamters.pageSize, PageSizeForDocuments.ToString(CultureInfo.InvariantCulture) },
																						{ RequestParameters.RequestParamters.startIndex, startIndex.ToString(CultureInfo.InvariantCulture) }
																					};

				// build request (request documents)
				string response = DoHttpRequest(paramDict);

				XDocument parsedDoc = XDocument.Parse(response);

				// is the pagesize big enough or there are more elements
				IEnumerable<XElement> result = parsedDoc.Descendants("rsp").Descendants("result").ToList();
				XAttribute statAttribute = parsedDoc.Descendants("rsp").First().Attribute("stat");

				if (!result.Any() && statAttribute.Value == "fail")
				{
					XElement errorElement = parsedDoc.Descendants("rsp").Descendants("error").FirstOrDefault();
					throw new IssuuApiException(errorElement);
				}

				// is the pagesize big enough or there are more elements
				more = bool.Parse(result.First().Attribute("more").Value);

				// parse xml to get data
				IEnumerable<XElement> docsElements = parsedDoc.Descendants("rsp").Descendants("document");

				// complex logic,because 2 separate request needs to be done - one documentId could have multiple styles (embedds) - and we need to have only the first one (with documentId)
				IEnumerable<KeyValuePair<string, DocumentCommon>> docs =
					docsElements.Select(
										x =>
											new KeyValuePair<string, DocumentCommon>(x.Attribute("documentId").Value,
																					new DocumentCommon
																					{
																						DocumentId = x.Attribute("documentId").Value,
																						DocumentName = x.Attribute("name") != null ? x.Attribute("name").Value : null,
																						OrgDocName = x.Attribute("orgDocName") != null ? x.Attribute("orgDocName").Value : null,
																						PublishDate = x.Attribute("publishDate") != null ? x.Attribute("publishDate").Value : null,
																					}));
				foreach (KeyValuePair<string, DocumentCommon> entry in docs)
				{
					commonDocsDict.Add(entry.Key, entry.Value);
				}
				counter++;
				startIndex = counter * PageSizeForDocuments;
				int requestsProSecond = int.Parse(IssuuDocumentConfigurationFacade.Configuration.RequestsProSecond, CultureInfo.InvariantCulture);
				int sleepTime = (int)Math.Floor((decimal)(1000.0 / requestsProSecond));
				Thread.Sleep(sleepTime + 1);
			}
			while (more);

			Dictionary<string, DocumentEmbed> preparedEmbeddsDict = GetEmbedsOfDocuments(commonDocsDict);

			return preparedEmbeddsDict.ToDictionary(x => x.Key, x => x.Value.LabelForDropdown);
		}

		/// <summary>
		/// Create embed for documents without embeds to have an dataConfigId (embed).
		/// </summary>
		/// <param name="document">
		/// Common documents for embed.
		/// </param>
		/// <returns>
		/// The <see cref="DocumentEmbed"/>.
		/// </returns>
		private DocumentEmbed CreateEmbedForDocument(DocumentCommon document)
		{
			// prepare parameters
			Dictionary<RequestParameters.RequestParamters, string> paramDict = new Dictionary<RequestParameters.RequestParamters, string>
																				{
																					{ RequestParameters.RequestParamters.action, IssuuAddActionParameterValue },
																					{ RequestParameters.RequestParamters.apiKey, _issuuConfig.APIKey },
																					{ RequestParameters.RequestParamters.documentId, document.DocumentId },
																					{ RequestParameters.RequestParamters.readerStartPage, "1" },
																					{ RequestParameters.RequestParamters.width, Width.ToString(CultureInfo.InvariantCulture) },
																					{ RequestParameters.RequestParamters.height, Height.ToString(CultureInfo.InvariantCulture) },
																				};

			// build request
			string response = DoHttpRequest(paramDict);

			// parse xml to get data
			XDocument parsedDoc = XDocument.Parse(response);
			IEnumerable<XElement> docsElements = parsedDoc.Descendants("rsp").Descendants("documentEmbed");

			// dictionary with embedId as key,and documents as value,in this case,there should be only one documentEmbed
			foreach (XElement docsElement in docsElements)
			{
				// existing documents
				string docId = docsElement.Attribute("documentId") != null ? docsElement.Attribute("documentId").Value : null;
				if (docId == null)
				{
					continue;
				}

				DocumentEmbed docEmbed = new DocumentEmbed(document);
				docEmbed.EmbedId = docsElement.Attribute("id").Value;
				docEmbed.Height = docsElement.Attribute("height").Value;
				docEmbed.Width = docsElement.Attribute("width").Value;
				docEmbed.Created = docsElement.Attribute("created").Value;
				docEmbed.DataConfigId = docsElement.Attribute("dataConfigId").Value;
				return docEmbed;
			}

			return null;
		}

		/// <summary>
		/// Sign the request and request data.
		/// </summary>
		/// <param name="paramDict">
		/// Dictionary with parameters to do request with.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		private string DoHttpRequest(Dictionary<RequestParameters.RequestParamters, string> paramDict)
		{
			using (HttpClient client = new HttpClient())
			{
				// fake a formdata from dictionary
				List<string> formData = paramDict.Select(keyValue => keyValue.Key + "=" + keyValue.Value).ToList();

				// add signature parameter and sign parameters from dictionary alphabetical
				formData.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}", RequestParameters.RequestParamters.signature, IssuuHelper.GenerateSignature(paramDict)));
				string requestString = _issuuConfig.APIUrl + "?" + string.Join("&", formData);

				// request
				HttpResponseMessage response = client.GetAsync(requestString).Result;
				if (!response.IsSuccessStatusCode)
				{
					return null;
				}

				// get response as string
				Task<string> resultString = response.Content.ReadAsStringAsync();
				return resultString.Result;
			}
		}
	}
}
