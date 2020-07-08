# Azure Function Proxy Tutorial

## What is Azure Function Proxy?
Azure Function Proxies are part of Azure Function App that helps you to manage the API endpoints on your function app that are implemented by other resources. Your APIs can be deployed in App Service API App, Function App, Virtual machines or other servers. When you deploy a large application (typically a microservice application) into different servers, they will be exposed by different API endpoints. You can use the Azure Function proxy to provide a common host name for app APIs running on different backend services. Using function proxies you can also override the reqeust and response objects. If you want to implement a mock API for testing, you can use the function proxy without a backend url property.
> [!IMPORTANT]
> Standard Functions billing applies to proxy executions.

![Architecture](images/architecture.png)

### Modify the request and response
You can modify the request object to the backend api service using proxy. You can use the *Appication Settings* values with in the request. For example, you can make the backend url value as a dynamic value by refering to the AppSetting value. In the below proxy configuration you can see the `GetCountries` proxy is sending requests to the backend service defined by the %UTILS_HOST% settings value.
```
"CountriesList": {
    "matchCondition": {
        "route": "/api/utils/countries",
        "methods": [ "GET" ]
    },
    "backendUri": "%UTILS_HOST%/api/GetCountries"
}
```
> [!TIP]
> You can use `localhost` to reference a function inside the same function app directly, without a roundtrip proxy request. 
> `"backendurl": "https://localhost/api/GetCountries"` will reference a local HTTP triggered function at the route `/api/GetCountries`.

You can also use the Url parameters of the incoming requests to the backend uri. For example, in the below configuration we are accepting `{code}` as a url parameter for proxy url and the same is passed to the backend url.
```
"CountryByCode": {
    "matchCondition": {
        "route": "/api/utils/countries/{code}", 
        "methods": [ "GET" ]
    },
    "backendUri": "%UTILS_HOST%/api/GetCountryByCode/{code}"
}
```
You can override the reqeust methods too. For example, If the backend api is implemeted as a `POST` request uri, then you can define a proxy for that backend service with a `GET` uri. You will be overriding the `POST` method with a `GET` request method. 
```
"ServerTime": {
    "matchCondition": {
        "route": "/api/utils/time",
        "methods": [ "GET" ]
    },
    "requestOverrides": {
        "backend.request.method": "POST"
    },
    "backendUri": "%UTILS_HOST%/api/GetCurrentTime"
},
```
You can also create mock APIs for development and testing purposes using function proxies. You can override the response object and provide a static response to the caller. In such cases it is not necessary to define the `backendUri` property of the proxy configuration. Find the below configuration for defining a mock API.
```
"PaymentStatus": {
    "matchCondition": {
        "route": "/api/eshop/payments/status/{orderId}",
        "methods": [ "GET" ]
    },
    "responseOverrides": {
        "response.body": {
            "OrderId": "{orderId}",
            "Status": "Completed"
        },
        "response.statusCode": "200",
        "response.headers.Content-Type": "application/json"      
    }
}
```
## Creating and configuring Function Proxies using Azure Portal
1) Open Azure Portal and create a new Function App.
2) Open the Function App and select `Proxies` from the Functions section. Click on the Add button to create a new Proxy.

    ![Portal1](images/portal1.png)
3) In the `New Proxy` dialog window, specify the name of the Proxy, Route Template, Http methods and the backend URL. Optionally you can specify the `Request override` and `Response override` configuration. Click `Create` to create the new proxy.

    ![Portal2](images/portal2.png)
4) After the proxy is created you can click on the proxy name to see the details.

    ![Portal3](images/portal3.png)
5) Click on the `Advanced Editor` to open the proxy configuration in a new window.

    ![Portal4](images/portal4.png) 
6) You can see a JSON file that contains the proxy configuration. Note that the proxy configuration is stored in `proxies.json` file.

    ![Portal5](images/portal5.png)
7) Now, you can copy the proxy URL and test using a browser or any other tool like Postman.

## Creating Function Proxy using Visual Studio
1) To create function proxies in your Visual Studio  fucntion App project, you need to add `proxies.json` file in the project.
2) You need to explicitly add the following lines to the `*.csproj` file to include the `proxies.json` in the package file while publishing the project to Azure.
    ```
    <None Update="proxies.json">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    ```
3) In the `proxies.json` file you can define the list of proxies. Find the sample proxy configurations below.
    ```
    {
      "$schema": "http://json.schemastore.org/proxies",
      "proxies": {
        "PlaceOrder": {
          "matchCondition": {
            "methods": [ "POST" ],
            "route": "/api/eshop/order"
          },
          "backendUri": "%ESHOP_ORDERS_HOST%/api/PostOrder"
        },
        "CountriesList": {
          "matchCondition": {
            "route": "/api/utils/countries",
            "methods": [ "GET" ]
          },
          "backendUri": "%UTILS_HOST%/api/GetCountries"
        },
        "CountryByCode": {
          "matchCondition": {
            "route": "/api/utils/countries/{code}", 
            "methods": [ "GET" ],
          },
          "backendUri": "%UTILS_HOST%/api/GetCountryByCode/{code}"
        },
        "ServerTime": {
          "matchCondition": {
            "route": "/api/utils/time",
            "methods": [ "GET" ]
          },
          "requestOverrides": {
            "backend.request.method": "POST"
          },
          "backendUri": "%UTILS_HOST%/api/GetCurrentTime"
        },
        "PaymentStatus": {
          "matchCondition": {
            "route": "/api/eshop/payments/status/{orderId}",
            "methods": [ "GET" ]
          },
          "responseOverrides": {
            "response.body": {
              "OrderId": "{orderId}",
              "Status": "Completed"
            },
            "response.statusCode": "200",
            "response.headers.Content-Type": "application/json"      
          }
        }
      }
    }
    ```
## Sample case study
1) Download or clone the repository from [https://github.com/sonusathyadas/azure-function-proxy-tutorial](https://github.com/sonusathyadas/azure-function-proxy-tutorial).
2) Open the project in Visual Studio 2019. 
3) The project contains some sample HttpTrigger functions configured with function proxy.
4) The function `PostOrder` uses an `Azure ServiceBus Queue`. The function adds order data to the `orders` queue. 
5) Open Azure portal and create a Service Bus namespace. Then creates an `orders` queue in the service bus namespace.
6) Update the `local.settings.json` file to include the Service Bus connection string. Update the value of the `servicebus_connection` property. Replace *SERVICEBUS_CONNECTIONSTRING* with your service bus connection string.
7) You can test the functions and proxies locally.
8) For publishing the application to Azure function app, right click on the project and select publish. Select the Azure AppService Function App (Windows) to publish.
9) In the publish window, click on the `Manage Azure App Service Settings` link.

    ![local1](images/local1.png)    
10) In the dialog box, find the `servicebus_connection` property and click on the `Insert value from local` to copy the local connection string value to remote.

    ![local2](images/local2.png)  
11) Save the changes and click on the Publish button to deploy the application on Azure function App.

