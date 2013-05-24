#region Copyright (c) 2012 SmartVault, Inc.
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using Freshbooks.Library.Model;
using System.Collections.Specialized;
using System.Diagnostics;
// ReSharper disable InconsistentNaming

namespace Freshbooks.Library
{
    /// <summary>
    /// Freshbooks API authentication and wrapper class.  Use the read/write property TokenState to
    /// persist and restore the state of the object.
    /// </summary>
    public class FreshbooksApi
    {
        private readonly WebClient _webClient = new WebClient();
        private readonly string _accountName, _consumerKey, _oauthSecret;

        private readonly Uri _baseUri;
        private Uri _serviceUri;

        private string oauth_callback;
        private string req_token, oauth_token, oauth_token_secret, user_token;

        /// <summary>
        /// Constructs the Freshbooks API authentication and wrapper class
        /// </summary>
        /// <param name="accountName">The account name for which you are trying to access</param>
        /// <param name="consumerKey">The developer's account name</param>
        public FreshbooksApi(string accountName, string consumerKey) : this(accountName, consumerKey, String.Empty) { }
        /// <summary>
        /// Constructs the Freshbooks API authentication and wrapper class
        /// </summary>
        /// <param name="accountName">The account name for which you are trying to access</param>
        /// <param name="consumerKey">The developer's freshbooks account name</param>
        /// <param name="oauthSecret">The developer's oauth-secret provided by freshbooks</param>
        public FreshbooksApi(string accountName, string consumerKey, string oauthSecret)
        {
            _accountName = accountName;
            _consumerKey = consumerKey;
            _oauthSecret = oauthSecret ?? String.Empty;

            _baseUri = new UriBuilder { Scheme = "https", Host = accountName + ".freshbooks.com" }.Uri;
            _serviceUri = new Uri(_baseUri, "/api/2.1/xml-in");
            Clear();

            UserAgent = String.Format("{0}/{1} ({2})", 
                GetType().Name,
                GetType().Assembly.GetName().Version.ToString(2), 
                GetType().Assembly.FullName);

            Callback = new CallbackService(new ServiceProxy(this, "callback"));
            Category = new CategoryService(new ServiceProxy(this, "category"));
            Client = new ClientService(new ServiceProxy(this, "client"));
            Estimate = new EstimateService(new ServiceProxy(this, "estimate"));
            Expense = new ExpenseService(new ServiceProxy(this, "expense"));
            Gateway = new GatewayService(new ServiceProxy(this, "gateway"));
            Invoice = new InvoiceService(new ServiceProxy(this, "invoice"));
            Item = new ItemService(new ServiceProxy(this, "item"));
            Language = new LanguageService(new ServiceProxy(this, "language"));
            Payment = new PaymentService(new ServiceProxy(this, "payment"));
            Project = new ProjectService(new ServiceProxy(this, "project"));
            Recurring = new RecurringService(new ServiceProxy(this, "recurring"));
            System = new SystemService(new ServiceProxy(this, "system"));
            Staff = new StaffService(new ServiceProxy(this, "staff"));
            Task = new TaskService(new ServiceProxy(this, "task"));
            Tax = new TaxService(new ServiceProxy(this, "tax"));
            TimeEntry = new TimeEntryService(new ServiceProxy(this, "time_entry"));
        }

        #region Services
        public ICallbackService Callback { get; private set; }
        public ICategoryService Category { get; private set; }
        public IClientService Client { get; private set; }
        public IEstimateService Estimate { get; private set; }
        public IExpenseService Expense { get; private set; }
        public IGatewayService Gateway { get; private set; }
        public IInvoiceService Invoice { get; private set; }
        public IItemService Item { get; private set; }
        public ILanguageService Language { get; private set; }
        public IPaymentService Payment { get; private set; }
        public IProjectService Project { get; private set; }
        public IRecurringService Recurring { get; private set; }
        public IStaffService Staff { get; private set; }
        public ISystemService System { get; private set; }
        public ITaskService Task { get; private set; }
        public ITaxService Tax { get; private set; }
        public ITimeEntryService TimeEntry { get; private set; }
        #endregion

        /// <summary>
        /// An event to recieve status updates
        /// </summary>
        public event EventHandler<BeginRequestEventArgs> OnBeginRequest;
        public event EventHandler<CompleteRequestEventArgs> OnEndRequest;
        /// <summary>
        /// An event to recieve exceptions raised by the http client
        /// </summary>
        public event EventHandler<ServerErrorEventArgs> OnException;

        /// <summary>
        /// Clears all authentication information and reset all values to their defaults.
        /// </summary>
        public void Clear()
        {
            oauth_callback = String.Empty;
            req_token = oauth_token = oauth_token_secret = user_token = null;
        }

        /// <summary>
        /// The account name this instance is bound to
        /// </summary>
        public string AccountName { get { return _accountName; } }
        /// <summary>
        /// The root site of the target host, defaults to "https://[account-name].freshbooks.com/"
        /// </summary>
        public Uri AccountUri { get { return _baseUri; } }
        /// <summary>
        /// The rest services Uri for the account, defaults to "https://[account-name].freshbooks.com/api/2.1/xml-in"
        /// </summary>
        public Uri ServiceUri { get { return _serviceUri; } set { _serviceUri = new Uri(_baseUri, value); } }
        /// <summary>
        /// The HTTP "User-Agent" header value to be used.
        /// </summary>
        public string UserAgent
        {
            get { return _webClient.Headers[HttpRequestHeader.UserAgent]; }
            set { _webClient.Headers[HttpRequestHeader.UserAgent] = value; }
        }

        /// <summary>
        /// The callback URI to be used for completing the oauth negotiation.  When this page is requested
        /// hand the full URL to the AuthorizeToken(Uri redirection) method to complete the authentication process.  
        /// If not set or empty, the user will be required to enter the verication value directly which must then
        /// be passed to the AuthorizeToken(string oauth_verifier) to complete the authentication process.
        /// </summary>
        public string OAuthCallback
        {
            get { return oauth_callback; }
            set { oauth_callback = value ?? String.Empty; }
        }

        /// <summary>
        /// Returns true if the token has sufficient information to make an authenticated request; however,
        /// this does not verify that the token information is still valid.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(oauth_token) || !String.IsNullOrEmpty(user_token); }
        }

        /// <summary>
        /// Used to persist the state of the oauth negotiation between calls to a web server or other
        /// stateless process model.  May also be used to store an authenticated token for later use.
        /// </summary>
        public string TokenState
        {
            get
            {
                StringBuilder state = new StringBuilder();
                if (!String.IsNullOrEmpty(user_token))
                    state.AppendFormat("user_token={0}", Uri.EscapeDataString(user_token));
                else if (!String.IsNullOrEmpty(req_token))
                    state.AppendFormat("req_token={0}", Uri.EscapeDataString(req_token));
                else if (!String.IsNullOrEmpty(oauth_token))
                    state.AppendFormat("oauth_token={0}&oauth_token_secret={1}",
                                       Uri.EscapeDataString(oauth_token),
                                       Uri.EscapeDataString(oauth_token_secret)
                        );
                return state.ToString();
            }
            set
            {
                if(String.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                NameValueCollection nvc = HttpUtility.ParseQueryString(value);
                user_token = nvc["user_token"];
                req_token = nvc["req_token"];
                oauth_token = nvc["oauth_token"];
                oauth_token_secret = nvc["oauth_token_secret"];
            }
        }
		
		/// <summary>
		/// Sets the legacy user-specific token found on the user's My Account -> Freshbooks API page
		/// <summary>
		/// <remarks>This is an alternative to the UseLegacyToken method</remarks>
		///
		
		public string LegacyToken{
			private get
			{
				return user_token;
			}
			set
			{
				user_token = value;
				req_token = null;
				oauth_token = null;
				oauth_token_secret = null;
			}
		}

        private long Timestamp
        {
            get { return (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds; }
        }
        private string Nonce
        {
            get { return Guid.NewGuid().ToString("N"); }
        }

        internal string Post(string contents)
        {
            using (WebClient client = new WebClient())
                return HttpPost(client, AuthorizationHeader, ServiceUri.PathAndQuery, contents);
        }

        private string Request(string relativeUri)
        {
            return HttpPost(_webClient, null, relativeUri, String.Empty);
        }

        internal string HttpPost(WebClient client, string auth, string relativeUri, string contents)
        {
            Uri uri = new Uri(_baseUri, relativeUri);
            HttpStatusCode hstatus = HttpStatusCode.OK;
            string message = null;

            client.Encoding = new UTF8Encoding(false);
            client.UseDefaultCredentials = false;
            client.Headers[HttpRequestHeader.UserAgent] = UserAgent;

            if(!String.IsNullOrEmpty(auth))
                client.Headers[HttpRequestHeader.Authorization] = auth;

            try
            {
                if (OnBeginRequest != null)
                    OnBeginRequest(this, new BeginRequestEventArgs(uri, contents));

                return client.UploadString(uri, contents);
            }
            catch(WebException we)
            {
                message = we.Status.ToString();
                hstatus = HttpStatusCode.ServiceUnavailable;
                HttpWebResponse webResponse = we.Response as HttpWebResponse;
                if (webResponse != null)
                {
                    hstatus = webResponse.StatusCode;
                    try
                    {
                        using (Stream response = webResponse.GetResponseStream())
                            if (response != null)
                                message = new StreamReader(response).ReadToEnd();
                    }
                    catch (Exception e)
                    { Debug.WriteLine(e); }
                }

                Exception raise;
                if (!String.IsNullOrEmpty(auth))
                    raise = new Properties.FreshbooksServerException((int)hstatus, message, we);
                else
                    raise = new Properties.ServerAuthenticationException((int)hstatus, message, we);

                if (OnException != null)
                    OnException(this, new ServerErrorEventArgs(hstatus, uri, contents, raise));

                throw raise;
            }
            finally
            {
                if (OnEndRequest != null)
                    OnEndRequest(this, new CompleteRequestEventArgs(hstatus, uri, String.Empty, message));
            }
        }

        private string RequestToken()
        {
            string requestTokenUri = "/oauth/oauth_request.php" +
                                     "?oauth_consumer_key=" + Uri.EscapeDataString(_consumerKey) +
                                     "&oauth_signature_method=PLAINTEXT" +
                                     "&oauth_signature=" + Uri.EscapeDataString(_oauthSecret + "&") +
                                     "&oauth_timestamp=" + Timestamp +
                                     "&oauth_nonce=" + Nonce +
                                     "&oauth_version=1.0" +
                                     "&oauth_callback=" + Uri.EscapeDataString(oauth_callback);

            string responseText = Request(requestTokenUri);
            var responseArgs = HttpUtility.ParseQueryString(responseText);
            return responseArgs["oauth_token"];
            //oauth_token_secret = responseArgs["oauth_token_secret"];
        }

        /// <summary>
        /// Step 1: Creates an oauth request and returns the URL the user should be redirected to.
        /// </summary>
        public Uri GetAuthroizationUrl()
        {
            if (req_token == null)
                req_token = RequestToken();
            return new Uri(_baseUri, "/oauth/oauth_authorize.php?oauth_token=" + req_token);
        }

        /// <summary>
        /// Step 2: Called upon redirection to the URL provided by the OAuthCallback property
        /// </summary>
        /// <param name="redirection">The complete Uri of the page</param>
        public void AuthorizeToken(Uri redirection)
        {
            var responseArgs = HttpUtility.ParseQueryString(redirection.Query);
            req_token = req_token ?? responseArgs["oauth_token"];
            AuthorizeToken(responseArgs["oauth_verifier"]);
        }

        /// <summary>
        /// Step 2: Called upon redirection to the callback uri, or after the consumer has
        /// manually entered the verification code.
        /// </summary>
        /// <param name="oauth_verifier">The verification code or the oauth_verifier uri parameter</param>
        public void AuthorizeToken(string oauth_verifier)
        {
            if (String.IsNullOrEmpty(req_token))
                throw new InvalidOperationException();

            string authorizeTokenUri = "/oauth/oauth_access.php" +
                                       "?oauth_consumer_key=" + Uri.EscapeDataString(_consumerKey) +
                                       "&oauth_token=" + Uri.EscapeDataString(req_token) +
                                       "&oauth_signature_method=PLAINTEXT" +
                                       "&oauth_signature=" + Uri.EscapeDataString(_oauthSecret + "&") +
                                       "&oauth_timestamp=" + Timestamp +
                                       "&oauth_nonce=" + Nonce +
                                       "&oauth_version=1.0" +
                                       "&oauth_verifier=" + oauth_verifier;

            string responseText = Request(authorizeTokenUri);
            var responseArgs = HttpUtility.ParseQueryString(responseText);

            user_token = null;
            req_token = null;
            oauth_token = responseArgs["oauth_token"];
            oauth_token_secret = responseArgs["oauth_token_secret"];
        }

        /// <summary>
        /// Authenticates with the legacy user-specific token provided to the user
        /// </summary>
        /// <param name="userToken">A user-specific string found on the user's My Account -> Freshbooks API page</param>
        public void UseLegacyToken(string userToken)
        {
            user_token = userToken;
            req_token = null;
            oauth_token = null;
            oauth_token_secret = null;
        }

        /// <summary>
        /// Returns the string to use for the HTTP "Authorization" header.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Raised when IsAuthenticated is false</exception>
        public string AuthorizationHeader
        {
            get
            {
                if (!IsAuthenticated)
                    throw new UnauthorizedAccessException();

                if (!String.IsNullOrEmpty(user_token))
                    return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(user_token + ":"));

                return "OAuth realm=\"\"" +
                       ",oauth_consumer_key=\"" + Uri.EscapeDataString(_consumerKey) + "\"" +
                       ",oauth_token=\"" + Uri.EscapeDataString(oauth_token) + "\"" +
                       ",oauth_signature_method=\"PLAINTEXT\"" +
                       ",oauth_signature=\"" + Uri.EscapeDataString(_oauthSecret + "&" + oauth_token_secret) + "\"" +
                       ",oauth_timestamp=\"" + Timestamp + "\"" +
                       ",oauth_nonce=\"" + Nonce + "\"" +
                       ",oauth_version=\"1.0\"";
            }
        }
    }
}