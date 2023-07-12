# Realm Helper

**Realm Helper** - an ASP.NET Core 7.0 Open Source project which helps you to view and manage your Minecraft Java and Bedrock Realms online easily.

## Features

* View basic Realm information
* View recent posts *(Bedrock only)*
* View online and recently seen members with pretty formatted timestamps
* Invite players to your Realm and change their permissions *(Owner only)*
* Block and unblock players *(Bedrock only) (Owner only)*
* Download and upload world backups *(Owner only)*
* Edit world and slot settings *(Owner only)*

![Screenshot of a non-owned Bedrock Realm](https://user-images.githubusercontent.com/83646375/252860406-a7d5026e-45cf-4006-860f-b1cda910df63.png)

## Run Locally

If you want to run this project locally, then you have to create an [Azure AD application](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) (Choose `Personal Microsoft accounts only` in `Supported account types` section) and fill out the `ClientId` and `ClientSecret` fields in the `AzureAD` section.

*Note: Don't forget to specify a redirect URI that must be identical to the `CallbackPath` in the `appsettings.json` file.*
