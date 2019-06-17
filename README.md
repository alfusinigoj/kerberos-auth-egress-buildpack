#### This project offers a supply buildpack which helps applying IWA security (kerberos) for app-app/svc-svc secure communication, in PCF. 

In detail, if any service running in PCF is protected by `route service` (https://github.com/macsux/route-service-auth), the client application should be attaching a valid kerberos ticket, so as to be authenticated properly. This buildpack, together with the nuget package mentioned below will be intercepting all the egress requests and attaches a valid kerberos ticket. For more details, please listen to Andrew Stackhov's video here "youtube video link here"'

#### Specific functionalities executed by this buildpack

  1. Modifies the wcf client section in web.config with endpoint behaviours required for injection of kerberos ticket
  2. Adds MIT Kerberos dlls and Kerberos executables required for the operation, into application bin folder
  3. Validates if `PivotalServices.WcfClient.Kerberos.Interceptor` package is installed

#### Dependency
Nuget Package to be added `PivotalServices.WcfClient.Kerberos.Interceptor` (https://github.com/alfusinigoj/wcf-client-interceptor-egress-kerberos-auth)

#### To manually build and release this buildpack using command `nuke <target>` from powershell window.
*Targets*
  1. `Clean`    --> cleans bin/obj/artifacts folder
  2. `Restore`  --> Restores all nuget dependencies (depends on `Clean`)
  3. `Compile`  --> Compiles the buildpack (depends on `Restore`)
  4. `Test`     --> Runs all the tests for the buildpack (depends on `Compile`)
  5. `Publish`  --> Publishes the buildpack artifact under `artifacts` directory (depends on `Test`)
  6. `Release`  --> Create a new buildpack release under the github repo (depends on `Publish`). You will be prompted for `Github ApiToken` which has release rights to the github repo, alternatively you can pass as commandline param `--GitHubToken`
