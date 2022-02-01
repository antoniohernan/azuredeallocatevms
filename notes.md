var clientID = "bfcc403d-1f73–4ad4-xxxx-xxxxxxxxxxxx";
var clientSecret = "SomethingDifficult";
var tenantID = "12345678-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientID, clientSecret, tenantID, AzureEnvironment.AzureGlobalCloud);            
var azure = Azure.Authenticate(credentials).WithDefaultSubscription();
