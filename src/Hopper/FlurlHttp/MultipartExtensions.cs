﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http.Content;

namespace Flurl.Http
{
	/// <summary>
	/// Fluent extension methods for sending multipart/form-data requests.
	/// </summary>
	public static class MultipartExtensions
	{
		/// <summary>
		/// Sends an asynchronous multipart/form-data POST request.
		/// </summary>
		/// <param name="buildContent">A delegate for building the content parts.</param>
		/// <param name="request">The IFlurlRequest.</param>
		/// <param name="completionOption">The HttpCompletionOption used in the request. Optional.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A Task whose result is the received IFlurlResponse.</returns>
		public static Task<IFlurlResponse> PostMultipartAsync(this IFlurlRequest request, Action<CapturedMultipartContent> buildContent, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, CancellationToken cancellationToken = default) {
			var cmc = new CapturedMultipartContent(request.Settings);
			buildContent(cmc);
			return request.SendAsync(HttpMethod.Post, cmc, completionOption, cancellationToken);
		}
	}
}
