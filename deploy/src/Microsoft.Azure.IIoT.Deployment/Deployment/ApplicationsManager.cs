﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Deployment.Deployment {
    using Microsoft.Azure.IIoT.Deployment.Infrastructure;
    using Microsoft.Graph;
    using Microsoft.Rest;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    class ApplicationsManager {

        protected readonly Guid _tenantId;
        protected readonly MicrosoftGraphServiceClient _msGraphServiceClient;

        private Application _serviceApplication;
        private ServicePrincipal _serviceApplicationSP;

        private Application _clientApplication;
        private ServicePrincipal _clientApplicationSP;

        private Application _aksApplication;
        private ServicePrincipal _aksApplicationSP;
        private string _aksApplicationPasswordCredentialRbacSecret;

        public ApplicationsManager (
            Guid tenantId,
            TokenCredentials microsoftGraphTokenCredentials,
            CancellationToken cancellationToken
        ) {
            _tenantId = tenantId;

            _msGraphServiceClient = new MicrosoftGraphServiceClient(
                microsoftGraphTokenCredentials,
                cancellationToken
            );
        }

        public Application GetServiceApplication() {
            return _serviceApplication;
        }

        public ServicePrincipal GetServiceApplicationSP() {
            return _serviceApplicationSP;
        }

        public Application GetClientApplication() {
            return _clientApplication;
        }

        public ServicePrincipal GetClientApplicationSP() {
            return _clientApplicationSP;
        }

        public Application GetAKSApplication() {
            return _aksApplication;
        }

        public ServicePrincipal GetAKSApplicationSP() {
            return _aksApplicationSP;
        }

        public string GetAKSApplicationRbacSecret() {
            return _aksApplicationPasswordCredentialRbacSecret;
        }

        public async Task RegisterApplicationsAsync(
            string applicationName,
            DirectoryObject owner,
            IEnumerable<string> tags = null,
            CancellationToken cancellationToken = default
        ) {
            tags ??= new List<string>();

            var serviceApplicationName = applicationName + "-service";
            var clientApplicationName = applicationName + "-client";
            var aksApplicationName = applicationName + "-aks";

            // Service Application /////////////////////////////////////////////
            // Register service application
            _serviceApplication = await RegisterServiceApplicationAsync(
                    serviceApplicationName,
                    owner,
                    tags,
                    cancellationToken
                );

            // Find service principal for service application
            _serviceApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(_serviceApplication, cancellationToken);

            // Add current user or service principal as app owner for service
            // application, if it is not owner already.
            await _msGraphServiceClient
                .AddAsApplicationOwnerAsync(
                    _serviceApplication,
                    owner,
                    cancellationToken
                );

            // Client Application //////////////////////////////////////////////
            // Register client application
            _clientApplication = await RegisterClientApplicationAsync(
                    _serviceApplication,
                    clientApplicationName,
                    tags,
                    cancellationToken
                );

            // Find service principal for client application
            _clientApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(_clientApplication, cancellationToken);

            // Add current user or service principal as app owner for client
            // application, if it is not owner already.
            await _msGraphServiceClient
                .AddAsApplicationOwnerAsync(
                    _clientApplication,
                    owner,
                    cancellationToken
                );

            // Update service application to include client application as knownClientApplications
            _serviceApplication = await _msGraphServiceClient
                .AddAsKnownClientApplicationAsync(
                    _serviceApplication,
                    _clientApplication,
                    cancellationToken
                );

            // Grant admin consent for service application "user_impersonation" API permissions of client application
            // Grant admin consent for Microsoft Graph "User.Read" API permissions of client application
            await _msGraphServiceClient
                .GrantAdminConsentToClientApplicationAsync(
                    _serviceApplicationSP,
                    _clientApplicationSP,
                    cancellationToken
                );

            // App Registration for AKS ////////////////////////////////////////
            // Register aks application
            var registrationResult = await RegisterAKSApplicationAsync(
                    aksApplicationName,
                    tags,
                    cancellationToken
                );

            _aksApplication = registrationResult.Item1;
            _aksApplicationPasswordCredentialRbacSecret = registrationResult.Item2;

            // Find service principal for aks application
            _aksApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(_aksApplication, cancellationToken);

            // Add current user or service principal as app owner for aks
            // application, if it is not owner already.
            await _msGraphServiceClient
                .AddAsApplicationOwnerAsync(
                    _aksApplication,
                    owner,
                    cancellationToken
                );
        }

        /// <summary>
        /// Retrieve Application and Service Principal definitions from
        /// Microsoft Graph.
        /// </summary>
        /// <param name="applicationRegistrationConfiguration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task GetDefinitionsFromMicrosoftGraphAsync(
            Guid serviceApplicationId,
            Guid clientApplicationId,
            Guid aksApplicationId,
            string AksApplicationRbacSecret,
            CancellationToken cancellationToken = default
        ) {
            Log.Information("Application and Service Principal definitions will be retrieved from Microsoft Graph.");

            Log.Information("Getting service application registration ...");
            _serviceApplication = await _msGraphServiceClient
                .GetApplicationAsync(
                    serviceApplicationId,
                    cancellationToken
                );

            _serviceApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(
                    _serviceApplication,
                    cancellationToken
                );

            Log.Information("Getting client application registration ...");
            _clientApplication = await _msGraphServiceClient
                .GetApplicationAsync(
                    clientApplicationId,
                    cancellationToken
                );

            _clientApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(
                    _clientApplication,
                    cancellationToken
                );

            Log.Information("Getting AKS application registration ...");
            _aksApplication = await _msGraphServiceClient
                .GetApplicationAsync(
                    aksApplicationId,
                    cancellationToken
                );

            _aksApplicationSP = await _msGraphServiceClient
                .GetApplicationServicePrincipalAsync(
                    _aksApplication,
                    cancellationToken
                );

            _aksApplicationPasswordCredentialRbacSecret = AksApplicationRbacSecret;
        }

        /// <summary>
        /// Load Application and Service Principal definitions from provided
        /// ApplicationRegistrationDefinition object.
        /// </summary>
        /// <param name="applicationRegistrationDefinition"></param>
        public void Load(ApplicationRegistrationDefinition applicationRegistrationDefinition) {
            _serviceApplication = applicationRegistrationDefinition.ServiceApplication;
            _serviceApplicationSP = applicationRegistrationDefinition.ServiceApplicationSP;

            _clientApplication = applicationRegistrationDefinition.ClientApplication;
            _clientApplicationSP = applicationRegistrationDefinition.ClientApplicationSP;

            _aksApplication = applicationRegistrationDefinition.AksApplication;
            _aksApplicationSP = applicationRegistrationDefinition.AksApplicationSP;
            _aksApplicationPasswordCredentialRbacSecret = applicationRegistrationDefinition.AksApplicationRbacSecret;
        }

        public async Task DeleteApplicationsAsync(
            CancellationToken cancellationToken = default
        ) {
            // Service Application
            if (null != _serviceApplicationSP) {
                try {
                    await _msGraphServiceClient
                        .DeleteServicePrincipalAsync(
                            _serviceApplicationSP,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete ServicePrincipal of Service Application");
                }
            }

            if (null != _serviceApplication) {
                try {
                    await _msGraphServiceClient
                        .DeleteApplicationAsync(
                            _serviceApplication,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete Service Application");
                }
            }

            // Client Application
            if (null != _clientApplicationSP) {
                try {
                    await _msGraphServiceClient
                        .DeleteServicePrincipalAsync(
                            _clientApplicationSP,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete ServicePrincipal of Client Application");
                }
            }

            if (null != _clientApplication) {
                try {
                    await _msGraphServiceClient
                        .DeleteApplicationAsync(
                            _clientApplication,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete Client Application");
                }
            }

            // AKS Application
            if (null != _aksApplicationSP) {
                try {
                    await _msGraphServiceClient
                        .DeleteServicePrincipalAsync(
                            _aksApplicationSP,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete ServicePrincipal of AKS Application");
                }
            }

            if (null != _aksApplication) {
                try {
                    await _msGraphServiceClient
                        .DeleteApplicationAsync(
                            _aksApplication,
                            cancellationToken
                        );
                }
                catch (Exception) {
                    Log.Warning("Ignoring failure to delete AKS Application");
                }
            }
        }

        public async Task UpdateClientApplicationRedirectUrisAsync(
            string applicationURL,
            CancellationToken cancellationToken = default
        ) {
            if (null == applicationURL) {
                throw new ArgumentNullException("applicationURL");
            }

            if (applicationURL.Trim() == string.Empty) {
                throw new ArgumentException("Input cannot be empty", "applicationURL");
            }

            if (!applicationURL.StartsWith("https://") && !applicationURL.StartsWith("http://")) {
                applicationURL = $"https://{applicationURL}";
            }

            if (!applicationURL.EndsWith("/")) {
                applicationURL += "/";
            }

            Log.Information($"Updating RedirectUris of client " +
                $"application to point to '{applicationURL}'");

            var redirectUris = new List<string> {
                $"{applicationURL}",
                $"{applicationURL}registry/",
                $"{applicationURL}twin/",
                $"{applicationURL}history/",
                $"{applicationURL}ua/",
                $"{applicationURL}vault/"
            };

            _clientApplication = await _msGraphServiceClient
                .UpdateApplicationRedirectUrisAsync(
                    _clientApplication,
                    redirectUris,
                    cancellationToken
                );
        }

        protected async Task<Application> RegisterServiceApplicationAsync(
            string serviceApplicationName,
            DirectoryObject owner,
            IEnumerable<string> tags = null,
            CancellationToken cancellationToken = default
        ) {
            try {
                tags ??= new List<string>();

                Log.Information("Creating service application registration ...");

                // Setup AppRoles for service application
                var serviceApplicationAppRoles = new List<AppRole>();

                var serviceApplicationApproverRoleId = Guid.NewGuid();
                serviceApplicationAppRoles.Add(new AppRole {
                    DisplayName = "Approver",
                    Value = "Sign",
                    Description = "Approvers have the ability to issue certificates.",
                    AllowedMemberTypes = new List<string> { "User", "Application" },
                    Id = serviceApplicationApproverRoleId
                });

                var serviceApplicationWriterRoleId = Guid.NewGuid();
                serviceApplicationAppRoles.Add(new AppRole {
                    DisplayName = "Writer",
                    Value = "Write",
                    Description = "Writers Have the ability to change entities.",
                    AllowedMemberTypes = new List<string> { "User", "Application" },
                    Id = serviceApplicationWriterRoleId
                });

                var serviceApplicationAdministratorRoleId = Guid.NewGuid();
                serviceApplicationAppRoles.Add(new AppRole {
                    DisplayName = "Administrator",
                    Value = "Admin",
                    Description = "Admins can access advanced features.",
                    AllowedMemberTypes = new List<string> { "User", "Application" },
                    Id = serviceApplicationAdministratorRoleId
                });

                // Setup RequiredResourceAccess for service application

                var keyVaultUserImpersonationRequiredResourceAccess = new RequiredResourceAccess {
                    ResourceAppId = AzureAppsConstants.AzureKeyVault.AppId,
                    ResourceAccess = new List<ResourceAccess> {
                        new ResourceAccess {
                            Id = AzureAppsConstants.AzureKeyVault.ResourceAccess["user_impersonation"],
                            Type = "Scope"
                        }
                    }
                };

                var microsoftGraphUserReadRequiredResourceAccess = new RequiredResourceAccess {
                    ResourceAppId = AzureAppsConstants.MicrosoftGraph.AppId,
                    ResourceAccess = new List<ResourceAccess> {
                        new ResourceAccess {
                            Id = AzureAppsConstants.MicrosoftGraph.ResourceAccess["User.Read"],
                            Type = "Scope"
                        }
                    }
                };

                var serviceApplicationRequiredResourceAccess = new List<RequiredResourceAccess>() {
                    keyVaultUserImpersonationRequiredResourceAccess,
                    microsoftGraphUserReadRequiredResourceAccess
                };

                // Add OAuth2Permissions
                var oauth2Permissions = new List<PermissionScope> {
                    new PermissionScope {
                        AdminConsentDescription = $"Allow the app to access {serviceApplicationName} on behalf of the signed-in user.",
                        AdminConsentDisplayName = $"Access {serviceApplicationName}",
                        Id = Guid.NewGuid(),
                        IsEnabled = true,
                        Type = "User",
                        UserConsentDescription = $"Allow the application to access {serviceApplicationName} on your behalf.",
                        UserConsentDisplayName = $"Access {serviceApplicationName}",
                        Value = "user_impersonation"
                    }
                };

                var serviceApplicationApiApplication = new ApiApplication {
                    Oauth2PermissionScopes = oauth2Permissions
                };

                var serviceApplicationWebApplication = new WebApplication {
                    ImplicitGrantSettings = new ImplicitGrantSettings {
                        EnableIdTokenIssuance = true
                    }
                };

                var serviceApplicationDefinition = new Application {
                    DisplayName = serviceApplicationName,
                    IsFallbackPublicClient = false,
                    IdentifierUris = new List<string> {
                        $"https://{_tenantId.ToString()}/{serviceApplicationName}"
                    },
                    Tags = tags,
                    SignInAudience = "AzureADMyOrg",
                    AppRoles = serviceApplicationAppRoles,
                    RequiredResourceAccess = serviceApplicationRequiredResourceAccess,
                    Api = serviceApplicationApiApplication,
                    Web = serviceApplicationWebApplication,
                    PasswordCredentials = new List<PasswordCredential> { }
                };

                var serviceApplication = await _msGraphServiceClient
                    .CreateApplicationAsync(
                        serviceApplicationDefinition,
                        cancellationToken
                    );

                // Add Service Key PasswordCredential
                var serviceKeyName = "Service Key";
                await _msGraphServiceClient
                    .AddApplication2YPasswordCredentialAsync(
                        serviceApplication,
                        serviceKeyName,
                        cancellationToken
                    );

                // We need to create ServicePrincipal for this application.
                var serviceApplicationSP = await _msGraphServiceClient
                    .CreateApplicationServicePrincipalAsync(
                        serviceApplication,
                        tags,
                        cancellationToken
                    );

                // Add app role assignment to owner as Approver, Writer and Administrator.
                string PrincipalType;

                if (owner is User) {
                    PrincipalType = "User";
                }
                else if (owner is ServicePrincipal) {
                    PrincipalType = "ServicePrincipal";
                }
                else if (owner is Group) {
                    PrincipalType = "Group";
                }
                else {
                    throw new ArgumentException($"Owner is of unknown type: {owner.GetType()}", "owner");
                }

                var approverAppRoleAssignmentDefinition = new AppRoleAssignment {
                    PrincipalType = PrincipalType,
                    PrincipalId = new Guid(owner.Id),
                    ResourceId = new Guid(serviceApplicationSP.Id),
                    ResourceDisplayName = "Approver",
                    Id = serviceApplicationApproverRoleId.ToString(),
                    AppRoleId = serviceApplicationApproverRoleId
                };

                var writerAppRoleAssignmentDefinition = new AppRoleAssignment {
                    PrincipalType = PrincipalType,
                    PrincipalId = new Guid(owner.Id),
                    ResourceId = new Guid(serviceApplicationSP.Id),
                    ResourceDisplayName = "Writer",
                    Id = serviceApplicationWriterRoleId.ToString(),
                    AppRoleId = serviceApplicationWriterRoleId
                };

                var administratorAppRoleAssignmentDefinition = new AppRoleAssignment {
                    PrincipalType = PrincipalType,
                    PrincipalId = new Guid(owner.Id),
                    ResourceId = new Guid(serviceApplicationSP.Id),
                    ResourceDisplayName = "Administrator",
                    Id = serviceApplicationAdministratorRoleId.ToString(),
                    AppRoleId = serviceApplicationAdministratorRoleId
                };

                await _msGraphServiceClient
                    .AddServicePrincipalAppRoleAssignmentAsync(
                        serviceApplicationSP,
                        approverAppRoleAssignmentDefinition,
                        cancellationToken
                    );

                await _msGraphServiceClient
                    .AddServicePrincipalAppRoleAssignmentAsync(
                        serviceApplicationSP,
                        writerAppRoleAssignmentDefinition,
                        cancellationToken
                    );

                await _msGraphServiceClient
                    .AddServicePrincipalAppRoleAssignmentAsync(
                        serviceApplicationSP,
                        administratorAppRoleAssignmentDefinition,
                        cancellationToken
                    );

                // Get updated definition
                serviceApplication = await _msGraphServiceClient
                    .GetApplicationAsync(
                        new Guid(serviceApplication.Id),
                        cancellationToken
                    );

                Log.Information("Created service application registration.");

                return serviceApplication;
            }
            catch (Exception) {
                Log.Error("Failed to create service application registration.");
                throw;
            }
        }

        public async Task<Application> RegisterClientApplicationAsync(
            Application serviceApplication,
            string clientApplicationName,
            IEnumerable<string> tags = null,
            CancellationToken cancellationToken = default
        ) {
            try {
                tags ??= new List<string>();

                Log.Information("Creating client application registration ...");

                // Extract id of Oauth2PermissionScope for user impersonation
                var saApiOauth2PermissionScopeUserImpersonationList = serviceApplication
                    .Api
                    .Oauth2PermissionScopes
                    .Where(scope => "User" == scope.Type
                        && "user_impersonation" == scope.Value
                        && scope.IsEnabled.GetValueOrDefault(false)
                    )
                    .ToList();

                if (saApiOauth2PermissionScopeUserImpersonationList.Count != 1
                    || !saApiOauth2PermissionScopeUserImpersonationList.First().Id.HasValue) {
                    throw new Exception("Service appplication does not expose Oauth2PermissionScope for user impersonation.");
                }

                var saApiOauth2PermissionScopeUserImpersonationId =
                    saApiOauth2PermissionScopeUserImpersonationList.First().Id.Value;

                var serviceApplicationUserImpersonationRequiredResourceAccess = new RequiredResourceAccess {
                    ResourceAppId = serviceApplication.AppId,  // service application
                    ResourceAccess = new List<ResourceAccess> {
                        new ResourceAccess {
                            Id = saApiOauth2PermissionScopeUserImpersonationId,  // "user_impersonation"
                            Type = "Scope"
                        }
                    }
                };

                var microsoftGraphUserReadRequiredResourceAccess = new RequiredResourceAccess {
                    ResourceAppId = AzureAppsConstants.MicrosoftGraph.AppId,
                    ResourceAccess = new List<ResourceAccess> {
                        new ResourceAccess {
                            Id = AzureAppsConstants.MicrosoftGraph.ResourceAccess["User.Read"],
                            Type = "Scope"
                        }
                    }
                };

                var clientApplicationRequiredResourceAccess = new List<RequiredResourceAccess>() {
                    serviceApplicationUserImpersonationRequiredResourceAccess,
                    microsoftGraphUserReadRequiredResourceAccess
                };

                var clientApplicationPublicClientApplication = new Microsoft.Graph.PublicClientApplication {
                    RedirectUris = new List<string> {
                        "urn:ietf:wg:oauth:2.0:oob"
                    }
                };

                // Note: Oauth2AllowImplicitFlow will be enabled automatically since both
                // EnableIdTokenIssuance and EnableAccessTokenIssuance are set to true.

                var clientApplicationWebApplication = new WebApplication {
                    //Oauth2AllowImplicitFlow = true,
                    ImplicitGrantSettings = new ImplicitGrantSettings {
                        EnableIdTokenIssuance = true,
                        EnableAccessTokenIssuance = true
                    }
                };

                var clientApplicationDefinition = new Application {
                    DisplayName = clientApplicationName,
                    IsFallbackPublicClient = true,
                    IdentifierUris = new List<string> {
                        $"https://{_tenantId.ToString()}/{clientApplicationName}"
                    },
                    Tags = tags,
                    SignInAudience = "AzureADMyOrg",
                    RequiredResourceAccess = clientApplicationRequiredResourceAccess,
                    PublicClient = clientApplicationPublicClientApplication,
                    Web = clientApplicationWebApplication,
                    PasswordCredentials = new List<PasswordCredential> { }
                };

                var clientApplication = await _msGraphServiceClient
                    .CreateApplicationAsync(
                        clientApplicationDefinition,
                        cancellationToken
                    );

                // Add Client Key PasswordCredential
                var clientKeyName = "Client Key";
                await _msGraphServiceClient
                    .AddApplication2YPasswordCredentialAsync(
                        clientApplication,
                        clientKeyName,
                        cancellationToken
                    );

                // We need to create ServicePrincipal for this application.
                await _msGraphServiceClient
                    .CreateApplicationServicePrincipalAsync(
                        clientApplication,
                        tags,
                        cancellationToken
                    );

                // Get updated definition
                clientApplication = await _msGraphServiceClient
                    .GetApplicationAsync(
                        new Guid(clientApplication.Id),
                        cancellationToken
                    );

                Log.Information("Created client application registration.");

                return clientApplication;
            }
            catch (Exception) {
                Log.Error("Failed to created client application registration.");
                throw;
            }
        }

        public async Task<Tuple<Application, string>> RegisterAKSApplicationAsync(
            string aksApplicationName,
            IEnumerable<string> tags = null,
            CancellationToken cancellationToken = default
        ) {
            try {
                tags ??= new List<string>();

                Log.Information("Creating AKS application registration ...");

                // Add OAuth2Permissions for user impersonation
                var aksOauth2Permissions = new List<PermissionScope> {
                    new PermissionScope {
                        AdminConsentDescription = $"Allow the app to access {aksApplicationName} on behalf of the signed-in user.",
                        AdminConsentDisplayName = $"Access {aksApplicationName}",
                        Id = Guid.NewGuid(),
                        IsEnabled = true,
                        Type = "User",
                        UserConsentDescription = $"Allow the application to access {aksApplicationName} on your behalf.",
                        UserConsentDisplayName = $"Access {aksApplicationName}",
                        Value = "user_impersonation"
                    }
                };

                var aksApplicationApiApplication = new ApiApplication {
                    Oauth2PermissionScopes = aksOauth2Permissions
                };

                var aksApplicationWebApplication = new WebApplication {
                    ImplicitGrantSettings = new ImplicitGrantSettings {
                        EnableIdTokenIssuance = true
                    }
                };

                var aksApplicationDefinition = new Application {
                    DisplayName = aksApplicationName,
                    IsFallbackPublicClient = false,
                    IdentifierUris = new List<string> {
                        $"https://{_tenantId.ToString()}/{aksApplicationName}"
                    },
                    Tags = tags,
                    SignInAudience = "AzureADMyOrg",
                    AppRoles = new List<AppRole>(),
                    RequiredResourceAccess = new List<RequiredResourceAccess>(),
                    Api = aksApplicationApiApplication,
                    Web = aksApplicationWebApplication,
                    PasswordCredentials = new List<PasswordCredential> { }
                };

                var aksApplication = await _msGraphServiceClient
                    .CreateApplicationAsync(
                        aksApplicationDefinition,
                        cancellationToken
                    );

                // Add RBAC Key PasswordCredential
                var rbacKeyName = "rbac";
                var aksApplicationRBACPasswordCredential = await _msGraphServiceClient
                    .AddApplication2YPasswordCredentialAsync(
                        aksApplication,
                        rbacKeyName,
                        cancellationToken
                    );

                if (string.IsNullOrEmpty(aksApplicationRBACPasswordCredential.SecretText)) {
                    throw new Exception($"Failed to get password credentials for AKS Application: {rbacKeyName}");
                }

                var aksApplicationRBACPasswordCredentialSecret = aksApplicationRBACPasswordCredential.SecretText;

                // We need to create ServicePrincipal for this application.
                await _msGraphServiceClient
                    .CreateApplicationServicePrincipalAsync(
                        aksApplication,
                        tags,
                        cancellationToken
                    );

                // Get updated definition
                aksApplication = await _msGraphServiceClient
                    .GetApplicationAsync(
                        new Guid(aksApplication.Id),
                        cancellationToken
                    );

                var result = new Tuple<Application, string>(
                    aksApplication,
                    aksApplicationRBACPasswordCredentialSecret
                );

                Log.Information("Created AKS application registration.");

                return result;
            }
            catch (Exception) {
                Log.Error("Failed to create AKS application registration.");
                throw;
            }
        }
    }
}
