#### This project offers a supply buildpack which helps applying IWA security (kerberos) for app-app/svc-svc secure communication, in PCF. 

In detail, if any service running in PCF is protected by `route service` (https://github.com/macsux/route-service-auth), the client application should be attaching a valid kerberos ticket, so as to be authenticated properly. This buildpack, together with the nuget package mentioned below will be intercepting all the egress requests and attaches a valid kerberos ticket. For more details, please listen to Andrew Stackhov's video here "youtube video link here"'

#### Specific functionalities executed by this buildpack

1. Modifies the wcf client section in web.config with endpoint behaviours required for injection of kerberos ticket
2. Adds MIT Kerberos dlls and Kerberos executables required for the operation, into application bin folder
3. Validates if `PivotalServices.WcfClient.Kerberos.Interceptor` package is installed

#### Dependency
Nuget Package to be added 'PivotalServices.WcfClient.Kerberos.Interceptor' (https://github.com/alfusinigoj/wcf-client-interceptor-egress-kerberos-auth)
