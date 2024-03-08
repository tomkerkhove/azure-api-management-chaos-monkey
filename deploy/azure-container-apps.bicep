param location string = resourceGroup().location
param appName string = 'api-management-chaos-monkey'
param environmentName string = 'resilient-api-environment'
param imageName string = 'ghcr.io/tomkerkhove/api-management-chaos-monkey-api'
param imageTag string = 'latest'
param appInsightsName string = 'building-resilient-api-platform'
param tenantId string
param appId string
@secure()
param appSecret string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource environment 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: environmentName
  location: location
  properties: {
    daprAIInstrumentationKey: appInsights.properties.InstrumentationKey
  }
}

resource app 'Microsoft.App/containerApps@2022-03-01' = {
  name: appName
  location: location
  properties: {
    managedEnvironmentId: environment.id
    template: {
      containers: [
        {
          name: 'chaos-monkey-api'
          image: '${imageName}:${imageTag}'
          resources: {
            cpu: '1.0'
            memory: '2Gi'
          }
        }
      ]
    }
  }
}
