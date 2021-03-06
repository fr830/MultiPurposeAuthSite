﻿//**********************************************************************************
//* テンプレート
//**********************************************************************************

// 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
// https://github.com/OpenTouryoProject/MultiPurposeAuthSite/blob/master/license/LicenseForTemplates.txt

//**********************************************************************************
//* クラス名        ：HomeController
//* クラス日本語名  ：HomeController
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/04/24  西野 大介         新規
//**********************************************************************************

using MultiPurposeAuthSite.Co;
#if NETFX
using MultiPurposeAuthSite.Entity;
#else
using MultiPurposeAuthSite;
#endif
using MultiPurposeAuthSite.Data;
using MultiPurposeAuthSite.Notifications;
using MultiPurposeAuthSite.Extensions.OAuth2;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.StdMigration;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Str;

namespace MultiPurposeAuthSite.Controllers
{
    [Authorize]
    public class HomeController : MyBaseMVControllerCore
    {
        #region Action Method

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Home/Scroll
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Scroll()
        {
            return View();
        }

        #endregion

        #region Test OAuth2

        #region Common

        /// <summary>認可エンドポイント</summary>
        private string OAuthAuthorizeEndpoint = "";

        /// <summary>client_id</summary>
        private string ClientId = "";

        /// <summary>state (nonce)</summary>
        private string State = "";

        /// <summary>nonce</summary>
        private string Nonce = "";

        /// <summary>code_verifier</summary>
        private string CodeVerifier = "";

        /// <summary>code_verifier</summary>
        private string CodeChallenge = "";

        /// <summary>OAuth2スターターを組み立てて返す</summary>
        /// <param name="response_type">string</param>
        /// <returns>組み立てたOAuth2スターター</returns>
        private string AssembleOAuth2Starter(string response_type)
        {
            return this.OAuthAuthorizeEndpoint +
                string.Format(
                    "?client_id={0}&response_type={1}&scope={2}&state={3}",
                    this.ClientId, response_type, Const.StandardScopes, this.State);
        }

        /// <summary>OIDCスターターを組み立てて返す</summary>
        /// <param name="response_type">string</param>
        /// <returns>組み立てたOIDCスターター</returns>
        private string AssembleOidcStarter(string response_type)
        {
            return this.OAuthAuthorizeEndpoint +
                string.Format(
                    "?client_id={0}&response_type={1}&scope={2}&state={3}",
                    this.ClientId, response_type, Const.OidcScopes, this.State)
                    + "&nonce=" + this.Nonce;
        }

        /// <summary>FAPI1スターターを組み立てて返す</summary>
        /// <param name="response_type">string</param>
        /// <returns>組み立てたFAPI1スターター</returns>
        private string AssembleFAPI1Starter(string response_type)
        {
            return this.OAuthAuthorizeEndpoint +
                string.Format(
                    "?client_id={0}&response_type={1}&scope={2}&state={3}",
                    this.ClientId, response_type, Const.StandardScopes, "fapi1:" + this.State);
        }

        /// <summary>初期化</summary>
        private void Init()
        {
            this.OAuthAuthorizeEndpoint =
            Config.OAuth2AuthorizationServerEndpointsRootURI
            + Config.OAuth2AuthorizeEndpoint;

            this.ClientId = Helper.GetInstance().GetClientIdByName("TestClient");
            this.State = GetPassword.Generate(10, 0); // 記号は入れない。
            this.Nonce = GetPassword.Generate(20, 0); // 記号は入れない。

            this.CodeVerifier = "";
            this.CodeChallenge = "";
        }

        /// <summary>保存</summary>
        private void Save()
        {
            // テスト用にstate, nonce, code_verifierを、Session, Cookieに保存
            // ・Session : サイト分割時
            // ・Cookie : 同一サイト時

            IRequestCookieCollection requestCookies = MyHttpContext.Current.Request.Cookies;
            IResponseCookies responseCookies = MyHttpContext.Current.Response.Cookies;

            // state
            HttpContext.Session.SetString("test_state", this.State);
            if (requestCookies.Get("test_state") == null)
            {
                responseCookies.Set("test_state", this.State);
            }
            else
            {
                if (string.IsNullOrEmpty(requestCookies.Get("test_state")))
                {
                    responseCookies.Set("test_state", this.State);
                }
            }

            // nonce
            HttpContext.Session.SetString("test_nonce", this.State);
            if (requestCookies.Get("test_nonce") == null)
            {
                responseCookies.Set("test_nonce", this.State);
            }
            else
            {
                if (string.IsNullOrEmpty(requestCookies.Get("test_nonce")))
                {
                    responseCookies.Set("test_nonce", this.State);
                }
            }

            // code_verifier
            HttpContext.Session.SetString("test_code_verifier", this.State);
            if (requestCookies.Get("test_code_verifier") == null)
            {
                responseCookies.Set("test_code_verifier", this.State);
            }
            else
            {
                if (string.IsNullOrEmpty(requestCookies.Get("test_code_verifier")))
                {
                    responseCookies.Set("test_code_verifier", this.State);
                }
            }
        }

        #endregion

        #region Action Method

        /// <summary>OAuthStarters</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult OAuth2Starters()
        {
            return View();
        }

        #region Authorization Code Flow

        #region OAuth2

        /// <summary>Test Authorization Code Flow</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode()
        {
            this.Init();
            this.Save();

            // Authorization Code Flow
            return Redirect(this.AssembleOAuth2Starter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType));
        }

        /// <summary>Test Authorization Code Flow (form_post)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode_FormPost()
        {
            this.Init();
            this.Save();

            // Authorization Code Flow (form_post)
            return Redirect(this.AssembleOAuth2Starter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType)
                + "&response_mode=form_post");
        }

        #endregion

        #region OIDC

        /// <summary>Test Authorization Code Flow (OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode_OIDC()
        {
            this.Init();
            this.Save();

            // Authorization Code Flow (OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType)
                + "&prompt=none");
        }

        /// <summary>Test Authorization Code Flow (OIDC, form_post)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode_OIDC_FormPost()
        {
            this.Init();
            this.Save();

            // Authorization Code Flow (OIDC, form_post)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType)
                + "&prompt=none"
                + "&response_mode=form_post");
        }

        #endregion

        #region PKCE(FAPI1)

        /// <summary>Test Authorization Code Flow (PKCE plain)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode_PKCE_Plain()
        {
            this.Init();
            this.CodeVerifier = GetPassword.Base64UrlSecret(50);
            this.CodeChallenge = this.CodeVerifier;
            this.Save();

            // Authorization Code Flow (PKCE plain)
            return Redirect(this.AssembleOAuth2Starter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType)
                + "&code_challenge=" + this.CodeChallenge
                + "&code_challenge_method=" + OAuth2AndOIDCConst.PKCE_plain);
        }

        /// <summary>Test Authorization Code Flow (PKCE S256)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthorizationCode_PKCE_S256()
        {
            this.Init();
            this.CodeVerifier = GetPassword.Base64UrlSecret(50);
            this.CodeChallenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(this.CodeVerifier);
            this.Save();

            // Authorization Code Flow (PKCE S256)
            return Redirect(this.AssembleOAuth2Starter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType)
                + "&code_challenge=" + this.CodeChallenge
                + "&code_challenge_method=" + OAuth2AndOIDCConst.PKCE_S256);
        }

        #endregion

        #region FAPI1

        /// <summary>Test Authorization Code Flow (FAPI1)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult FAPI1AuthorizationCode()
        {
            this.Init();
            this.Save();

            // Authorization Code Flow
            return Redirect(this.AssembleFAPI1Starter(
                OAuth2AndOIDCConst.AuthorizationCodeResponseType));
        }

        #endregion

        #endregion

        #region Implicit Flow

        #region OAuth2

        /// <summary>Test Implicit Flow</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Implicit()
        {
            this.Init();
            this.Save();

            // Implicit Flow
            return Redirect(this.AssembleOAuth2Starter(
                OAuth2AndOIDCConst.ImplicitResponseType));
        }

        #endregion

        #region OIDC

        /// <summary>Test Implicit Flow 'id_token'(OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Implicit_OIDC1()
        {
            this.Init();
            this.Save();

            // Implicit Flow 'id_token'(OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.OidcImplicit1_ResponseType));
        }


        /// <summary>Test Implicit Flow 'id_token token'(OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Implicit_OIDC2()
        {
            this.Init();
            this.Save();

            // Implicit Flow 'id_token token'(OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.OidcImplicit2_ResponseType));
        }

        #endregion

        #endregion

        #region Hybrid Flow

        #region OIDC

        /// <summary>Test Hybrid Flow 'code id_token'(OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Hybrid_OIDC1()
        {
            this.Init();
            this.Save();

            // Hybrid Flow 'code id_token'(OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.OidcHybrid2_IdToken_ResponseType));
        }

        /// <summary>Test Hybrid Flow 'code token'(OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Hybrid_OIDC2()
        {
            this.Init();
            this.Save();

            // Hybrid Flow 'code token'(OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.OidcHybrid2_Token_ResponseType));
        }

        /// <summary>Test Hybrid Flow 'code id_token token'(OIDC)</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Hybrid_OIDC3()
        {
            this.Init();
            this.Save();

            // Hybrid Flow 'code id_token token'(OIDC)
            return Redirect(this.AssembleOidcStarter(
                OAuth2AndOIDCConst.OidcHybrid3_ResponseType));
        }

        #endregion

        #region FAPI2

        #endregion

        #endregion

        #region Client Authentication Flow

        #region Client Credentials Flow

        /// <summary>TestClientCredentialsFlow</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> TestClientCredentialsFlow()
        {
            // Tokenエンドポイントにアクセス
            string aud = Config.OAuth2AuthorizationServerEndpointsRootURI
                     + Config.OAuth2BearerTokenEndpoint;

            // ClientNameから、client_id, client_secretを取得。
            string client_id = "";
            string client_secret = "";

            if (User.Identity.IsAuthenticated)
            {
                // User Accountの場合、
                client_id = Helper.GetInstance().GetClientIdByName(User.Identity.Name);
                client_secret = Helper.GetInstance().GetClientSecret(client_id);
            }
            else
            {
                // Client Accountの場合、
                client_id = Helper.GetInstance().GetClientIdByName("TestClient");
                client_secret = Helper.GetInstance().GetClientSecret(client_id);
            }

            string response = await Helper.GetInstance()
                .ClientCredentialsGrantAsync(new Uri(
                    Config.OAuth2AuthorizationServerEndpointsRootURI
                     + Config.OAuth2BearerTokenEndpoint),
                     client_id, client_secret, Const.StandardScopes);

            ViewBag.Response = response;
            ViewBag.AccessToken = ((JObject)JsonConvert.DeserializeObject(response))[OAuth2AndOIDCConst.AccessToken];

            return View("OAuth2ClientAuthenticationFlow");
        }

        #endregion

        #region JWT Bearer Token Flow

        /// <summary>TestJWTBearerTokenFlow</summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> TestJWTBearerTokenFlow()
        {
            // Token2エンドポイントにアクセス
            string aud = Config.OAuth2AuthorizationServerEndpointsRootURI
                     + Config.OAuth2BearerTokenEndpoint2;

            // ClientNameから、client_id(iss)を取得。
            string iss = "";

            if (User.Identity.IsAuthenticated)
            {
                // User Accountの場合、
                iss = Helper.GetInstance().GetClientIdByName(User.Identity.Name);
            }
            else
            {
                // Client Accountの場合、
                iss = Helper.GetInstance().GetClientIdByName("TestClient");
            }

            // テストなので秘密鍵は共通とする。
            string privateKey = OAuth2AndOIDCParams.OAuth2JwtAssertionPrivatekey;
            privateKey = CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(privateKey), CustomEncode.us_ascii);

            string response = await Helper.GetInstance()
                .JwtBearerTokenFlowAsync(new Uri(
                    Config.OAuth2AuthorizationServerEndpointsRootURI
                     + Config.OAuth2BearerTokenEndpoint2),
                     JwtAssertion.CreateJwtBearerTokenFlowAssertionJWK(
                         iss, aud, new TimeSpan(0, 0, 30), Const.StandardScopes, privateKey));

            ViewBag.Response = response;
            ViewBag.AccessToken = ((JObject)JsonConvert.DeserializeObject(response))[OAuth2AndOIDCConst.AccessToken];

            return View("OAuth2ClientAuthenticationFlow");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
