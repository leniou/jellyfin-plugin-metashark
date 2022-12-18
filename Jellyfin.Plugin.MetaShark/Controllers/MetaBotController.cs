using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller;
using MediaBrowser.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediaBrowser.Model.IO;
using MediaBrowser.Controller.Providers;
using Jellyfin.Plugin.MetaShark.Providers;
using System.Runtime.InteropServices;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Common.Net;

namespace Jellyfin.Plugin.MetaShark.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/plugin/metashark")]
    public class MetaSharkController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaSharkController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
        public MetaSharkController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        /// <summary>
        /// 代理访问图片.
        /// </summary>
        [Route("proxy/image")]
        [HttpGet]
        public async Task<Stream> ProxyImage(string url)
        {

            if (string.IsNullOrEmpty(url))
            {
                throw new ResourceNotFoundException();
            }

            var httpClient = GetHttpClient();
            var response = await httpClient.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();

            Response.StatusCode = (int)response.StatusCode;
            if (response.Content.Headers.ContentType != null)
            {
                Response.ContentType = response.Content.Headers.ContentType.ToString();
            }
            Response.ContentLength = response.Content.Headers.ContentLength;

            foreach (var header in response.Headers)
            {
                Response.Headers.Add(header.Key, header.Value.First());
            }

            return stream;
        }

        private HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient(NamedClient.Default);
            return client;
        }
    }
}
