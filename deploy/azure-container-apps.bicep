param location string = resourceGroup().location
param appName string = 'api-management-chaos-monkey'
param environmentName string = 'building-resilient-api-platform'
param imageName string = 'ghcr.io/tomkerkhove/api-management-chaos-monkey-api'
param imageTag string = 'latest'
param appInsightsName string = 'building-resilient-api-platform'

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource environment 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: environmentName
  location: location
  tags: {
    app: 'chaos-monkey'
  }
  properties: {
    daprAIInstrumentationKey: appInsights.properties.InstrumentationKey
  }
}

resource app 'Microsoft.App/containerApps@2022-03-01' = {
  name: appName
  location: location
  tags: {
    app: 'chaos-monkey'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      ingress: {
        targetPort: 8080
        external: true
      }
    }
    template: {
      containers: [
        {
          name: 'chaos-monkey-api'
          image: '${imageName}:${imageTag}'
          env: [
            {
              name: 'CHAOS_MONKEY_AUTH_MODE'
              value: 'ManagedIdentity'
            }
          ]
          probes: [
            {
              type: 'Readiness'
              httpGet: {
                path: '/api/v1/health'
                port: 8080
              }
            }
            {
              type: 'Liveness'
              httpGet: {
                path: '/api/v1/health'
                port: 8080
              }
            }
          ]
          resources: {
            cpu: json('1.0')
            memory: '2Gi'
          }
        }
      ]
    }
  }
}
