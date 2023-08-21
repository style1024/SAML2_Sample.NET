using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using SampleSP.NET.Web.Models;

namespace SampleSP.NET.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private const string RELAY_STATE_RETURN_URL = "ReturnUrl";
        private readonly ILogger<AuthController> _logger;
        private readonly Saml2Configuration _config;

        public AuthController(ILogger<AuthController> logger, IOptions<Saml2Configuration> configAccessor)
        {
            _logger = logger;
            _config = configAccessor.Value;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Entrypoint(string returnUrl)
        {
            // 創建重定向綁定並設置重定向狀態
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string>() {
                { RELAY_STATE_RETURN_URL, returnUrl ?? Url.Content("~/Home") }
            });

            // 創建SAML2身份驗證請求並綁定
            return binding.Bind(new Saml2AuthnRequest(_config)
            {
                NameIdPolicy = new NameIdPolicy { AllowCreate = false, Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress" }
            }).ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/Home"));
            }

            var binding = new Saml2RedirectBinding();
            var saml2LogoutRequest = await new Saml2LogoutRequest(_config, User).DeleteSession(HttpContext);

            var result =  binding.Bind(saml2LogoutRequest).ToActionResult();
            return result;
        }

        [AllowAnonymous]
        [HttpPost("LogoutResponse")]
        public IActionResult LogoutResponse()
        {
            var binding = new Saml2PostBinding();
            var saml2LogoutResponse = new Saml2LogoutResponse(_config);

            //binding.Unbind(Request.ToGenericHttpRequest(), saml2LogoutResponse);

            if (saml2LogoutResponse.Status != Saml2StatusCodes.Success)
            {
                _logger.LogError($"Logout failed with status: {saml2LogoutResponse.Status}");
                return View("Error", new ErrorViewModel { ShowRequestId = true });
            }

            new Saml2LogoutRequest(_config, User).DeleteSession(HttpContext);
            return Redirect("~/Home");
        }

        [AllowAnonymous]
        [HttpPost("assertion-consumer-service")]
        public async Task<IActionResult> AssertionConsumerService()
        {
            // 讀取SAML2身份驗證響應
            var binding = new Saml2PostBinding();
            var authResponse = new Saml2AuthnResponse(_config);

            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), authResponse);
            if (authResponse.Status != Saml2StatusCodes.Success)
            {
                // 如果身份驗證未成功，則拋出異常
                throw new AuthenticationException($"SAML Response status: {authResponse.Status}");
            }
            binding.Unbind(Request.ToGenericHttpRequest(), authResponse);

            // 創建會話並重定向
            await authResponse.CreateSession(HttpContext, claimsTransform: (claimsPrincipal) => ClaimsTransform.Transform(claimsPrincipal));
            return Redirect(extractRelayState(binding));
        }

        // 提取重定向狀態
        private string extractRelayState(Saml2PostBinding binding)
        {
            var relayState = binding.GetRelayStateQuery();
            return relayState.ContainsKey(RELAY_STATE_RETURN_URL) ? relayState[RELAY_STATE_RETURN_URL] : Url.Content("~/Home");
        }
    }
}
