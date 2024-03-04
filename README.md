# Azure API Management - Chaos Monkey 🐒

Bringing chaos to your Azure API Management gateways 🐒

## What chaos does it bring?

A sample that will automatically simulate gateway failures by randomly enabling/disabling a gateway in a given location.

This allows you to simulate gateway failures and see how your platform reacts to it and how it recovers.

## Deployment

You can deploy the sample to an Azure Container App as following:

```shell
az deployment group create --resource-group building-resilient-api-platform --template-file ./deploy/azure-container-apps.bicep --parameters appId=c609aad8-d5b0-46f0-8e26-8a0f3224723f --parameters tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47 --parameters appSecret=foo
```

## License Information

This is licensed under The MIT License (MIT). Which means that you can use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the web application. But you always need to state that Tom Kerkhove is the original author of this application.

Read the full license [here](LICENSE).
