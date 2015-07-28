## Overview ##
This project provides a single assembly wrapper library around the Freshbooks
API.  Freshbooks.Library.dll exposes a class 'FreshbooksApi' that ecapsulates
the OAuth 1.0a negotiation as well as exposing service interfaces for all the
supported entities in the Freshbooks API.

## Entities Available ##
  * Callback - [goto documentation](http://developers.freshbooks.com/docs/callbacks/)
  * Category - [goto documentation](http://developers.freshbooks.com/docs/categories/)
  * Client - [goto documentation](http://developers.freshbooks.com/docs/clients/)
  * Estimate - [goto documentation](http://developers.freshbooks.com/docs/estimates/)
  * Expense - [goto documentation](http://developers.freshbooks.com/docs/expenses/)
  * Gateway - [goto documentation](http://developers.freshbooks.com/docs/gateway/)
  * Invoice - [goto documentation](http://developers.freshbooks.com/docs/invoices/)
  * Item - [goto documentation](http://developers.freshbooks.com/docs/items/)
  * Language - [goto documentation](http://developers.freshbooks.com/docs/languages/)
  * Payment - [goto documentation](http://developers.freshbooks.com/docs/payments/)
  * Project - [goto documentation](http://developers.freshbooks.com/docs/projects/)
  * Recurring - [goto documentation](http://developers.freshbooks.com/docs/recurring/)
  * Staff - [goto documentation](http://developers.freshbooks.com/docs/staff/)
  * System - [goto documentation](http://developers.freshbooks.com/docs/system/)
  * Task - [goto documentation](http://developers.freshbooks.com/docs/tasks/)
  * Tax - [goto documentation](http://developers.freshbooks.com/docs/taxes/)
  * Time Entry - [goto documentation](http://developers.freshbooks.com/docs/time-entries/)

## Example Code ##
### Connecting with Legacy Token ###
The following uses the legacy user-provided tokens to authenticate requests.  The "user-account" is the account you are accessing data for.  The "developer-account" is your Freshbooks account.  This authentication scheme has been deprecated in favor of OAuth 1.0a; however, it is still currently supported.

```
    FreshbooksApi api = new FreshbooksApi("user-account", "developer-account");
    api.UseLegacyToken("user-provided-token");
    //or
    //api.LegacyToken = "user-provided-token";
    var self = api.Staff.Current().Staff;
```

### Connecting with OAuth 1.0a ###
The following uses OAuth 1.0a to authenticate requests.  The "user-account" is the account you are accessing data for.  The "developer-account" is your Freshbooks account.  You may either omit providing the "optional-oauth-secret", or provide the OAuth secret provided by freshbooks.com for your developer account.  If you omit this value a warning will be displayed to the user, for more information see [Freshbooks Authentication](http://developers.freshbooks.com/authentication-2/).

```
    FreshbooksApi api = new FreshbooksApi("user-account", "developer-account", "optional-oauth-secret");

    if (Request.Url.Query == String.Empty)
    {
        // When the user grants us access have freshbooks redirect back to this page
        api.OAuthCallback = Request.Url.AbsoluteUri;

        // Redirect the user to their freshbooks.com OAuth logon page...
        Uri redirect = api.GetAuthroizationUrl();
        Response.Redirect(redirect.AbsoluteUri);
    }
    else
    {
        // Authorize the token based on the query-string values in this request
        api.AuthorizeToken(Request.Url);

        // Preserve the TokenState and redirect to the page, or just use the api...
        HttpContext.Current.Session["freshbooks-auth"] = api.TokenState;
        Response.Redirect("/authenticated/");
    }
```

### Accessing API Methods ###
Each entity has an interface to interact with the Freshbooks server.  For instance, if you want to access Client records, use the `FreshbooksApi.Client` service.  Here is a simple example of creating and updating a Client record.

```
    // Once the api is authenticate (see above) we can start using it...
    FreshbooksApi api = GetAuthenticatedApi();

    // Define the client properties:
    var client = new Client
                     {
                         Organization = "Sample Client",
                         FirstName = "Sample",
                         LastName = "Client",
                         Email = SampleClientEmail,
                     };

    // Tell Freshbooks to create the client:
    ClientIdentity id = api.Client.Create(new ClientRequest{Client = client});

    // Fetch the client we create and modify it:
    client = api.Client.Get(id).Client;
    client.Notes = "Modified.";

    // Tell Freshbooks to apply the changes:
    api.Client.Update(new ClientRequest {Client = client});
```