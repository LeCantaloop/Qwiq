using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Extension methods for configuring WireMock to respond like Azure DevOps REST APIs.
    /// </summary>
    public static class AzureDevOpsWireMockExtensions
    {
        /// <summary>
        /// Loads WireMock stub mappings from a JSON file captured from real Azure DevOps traffic.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="stubsFilePath">Path to the JSON file containing stub mappings.</param>
        /// <returns>The server for fluent chaining.</returns>
        /// <remarks>
        /// The stubs file should be generated using the Convert-HarToWireMock.ps1 script
        /// from a Fiddler HAR capture of real Azure DevOps traffic.
        /// </remarks>
        public static WireMockServer LoadStubsFromFile(this WireMockServer server, string stubsFilePath)
        {
            if (!File.Exists(stubsFilePath))
            {
                throw new FileNotFoundException($"Stubs file not found: {stubsFilePath}");
            }

            var json = File.ReadAllText(stubsFilePath);
            var stubs = JsonConvert.DeserializeObject<StubsFile>(json);

            if (stubs?.Mappings == null)
            {
                throw new InvalidOperationException("Invalid stubs file format - no mappings found");
            }

            foreach (var mapping in stubs.Mappings)
            {
                if (mapping.Request == null)
                {
                    continue;
                }

                var request = Request.Create();

                // Configure request matcher
                var urlPath = mapping.Request.UrlPath;
                var urlPathPattern = mapping.Request.UrlPathPattern;

                if (!string.IsNullOrEmpty(urlPath))
                {
                    request = request.WithPath(urlPath!);
                }
                else if (!string.IsNullOrEmpty(urlPathPattern))
                {
                    request = request.WithPath(new global::WireMock.Matchers.RegexMatcher(urlPathPattern!));
                }

                var method = mapping.Request.Method;
                if (!string.IsNullOrEmpty(method))
                {
                    request = method!.ToUpperInvariant() switch
                    {
                        "GET" => request.UsingGet(),
                        "POST" => request.UsingPost(),
                        "PUT" => request.UsingPut(),
                        "DELETE" => request.UsingDelete(),
                        "PATCH" => request.UsingPatch(),
                        _ => request.UsingAnyMethod()
                    };
                }

                // Configure response
                var response = Response.Create()
                    .WithStatusCode(mapping.Response?.Status ?? 200);

                var headers = mapping.Response?.Headers;
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        response = response.WithHeader(header.Key, header.Value);
                    }
                }

                var body = mapping.Response?.Body;
                if (!string.IsNullOrEmpty(body))
                {
                    response = response.WithBody(body!);
                }

                server.Given(request).RespondWith(response);
            }

            return server;
        }

        /// <summary>
        /// Gets the default path to the Azure DevOps stubs file.
        /// </summary>
        public static string GetDefaultStubsFilePath()
        {
            // Look for the stubs file relative to the test assembly
            var assemblyLocation = typeof(AzureDevOpsWireMockExtensions).Assembly.Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation)!;

            // Try several possible locations
            var possiblePaths = new[]
            {
                Path.Combine(assemblyDir, "WireMock", "Stubs", "azure-devops-stubs.json"),
                Path.Combine(assemblyDir, "azure-devops-stubs.json"),
                Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "..", "WireMock", "Stubs", "azure-devops-stubs.json")),
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException(
                "Could not find azure-devops-stubs.json. Searched in: " +
                string.Join(", ", possiblePaths));
        }

        /// <summary>
        /// Configures WireMock to respond to VssConnection handshake requests.
        /// This must be called before any other Azure DevOps API calls.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <returns>The server for fluent chaining.</returns>
        /// <remarks>
        /// The VssConnection SDK performs a complex handshake when connecting:
        /// 1. GET /_apis/connectionData - Returns authenticated user and location service data
        /// 2. GET /_apis/resourceAreas - Returns available API resource areas
        ///
        /// Without these endpoints configured, VssConnection.GetClient&lt;T&gt;() will fail.
        /// </remarks>
        public static WireMockServer SetupVssConnectionHandshake(this WireMockServer server)
        {
            // Connection Data - Required for VssConnection initialization
            // Note: The descriptor field must be an object with identityType and identifier properties
            // The IdentityDescriptor class has a custom JSON converter that expects this format
            var connectionDataResponse = $@"{{
                ""authenticatedUser"": {{
                    ""id"": ""00000000-0000-0000-0000-000000000001"",
                    ""descriptor"": {{
                        ""identityType"": ""Microsoft.IdentityModel.Claims.ClaimsIdentity"",
                        ""identifier"": ""00000000-0000-0000-0000-000000000001""
                    }},
                    ""subjectDescriptor"": {{
                        ""subjectType"": ""msa"",
                        ""identifier"": ""00000000-0000-0000-0000-000000000001""
                    }},
                    ""providerDisplayName"": ""Test User"",
                    ""isActive"": true,
                    ""properties"": {{}}
                }},
                ""authorizedUser"": {{
                    ""id"": ""00000000-0000-0000-0000-000000000001"",
                    ""descriptor"": {{
                        ""identityType"": ""Microsoft.IdentityModel.Claims.ClaimsIdentity"",
                        ""identifier"": ""00000000-0000-0000-0000-000000000001""
                    }},
                    ""subjectDescriptor"": {{
                        ""subjectType"": ""msa"",
                        ""identifier"": ""00000000-0000-0000-0000-000000000001""
                    }},
                    ""providerDisplayName"": ""Test User"",
                    ""isActive"": true,
                    ""properties"": {{}}
                }},
                ""instanceId"": ""00000000-0000-0000-0000-000000000000"",
                ""deploymentId"": ""00000000-0000-0000-0000-000000000000"",
                ""deploymentType"": ""hosted"",
                ""locationServiceData"": {{
                    ""serviceOwner"": ""00000000-0000-0000-0000-000000000000"",
                    ""defaultAccessMappingMoniker"": ""PublicAccessMapping"",
                    ""lastChangeId"": 1,
                    ""lastChangeId64"": 1,
                    ""accessMappings"": [
                        {{
                            ""moniker"": ""PublicAccessMapping"",
                            ""accessPoint"": ""{server.Url}/"",
                            ""displayName"": ""Public Access Mapping"",
                            ""virtualDirectory"": ""/""
                        }}
                    ],
                    ""serviceDefinitions"": []
                }}
            }}";

            server
                .Given(Request.Create()
                    .WithPath("/_apis/connectionData")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(connectionDataResponse));

            // Resource Areas - Required for locating API endpoints
            var resourceAreasResponse = $@"{{
                ""count"": 2,
                ""value"": [
                    {{
                        ""id"": ""5264459e-e5e0-4bd8-b118-0985e68a4ec5"",
                        ""name"": ""wit"",
                        ""locationUrl"": ""{server.Url}/""
                    }},
                    {{
                        ""id"": ""79134c72-4a58-4b42-976c-04e7115f32bf"",
                        ""name"": ""core"",
                        ""locationUrl"": ""{server.Url}/""
                    }}
                ]
            }}";

            server
                .Given(Request.Create()
                    .WithPath("/_apis/resourceAreas")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(resourceAreasResponse));

            // Also handle the resource area lookup by ID
            server
                .Given(Request.Create()
                    .WithPath("/_apis/resourceAreas/*")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody($@"{{
                        ""id"": ""5264459e-e5e0-4bd8-b118-0985e68a4ec5"",
                        ""name"": ""wit"",
                        ""locationUrl"": ""{server.Url}/""
                    }}"));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to WIQL query requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="workItemIds">The work item IDs to return in the query result.</param>
        /// <param name="queryType">The query type (flat, oneHop, tree). Default is "flat".</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupWiqlQueryResponse(
            this WireMockServer server,
            IEnumerable<int> workItemIds,
            string queryType = "flat")
        {
            var ids = workItemIds.ToArray();
            var workItemsJson = string.Join(",\n", ids.Select(id =>
                $@"{{ ""id"": {id}, ""url"": ""{server.Url}/_apis/wit/workItems/{id}"" }}"));

            var responseBody = $@"{{
                ""queryType"": ""{queryType}"",
                ""queryResultType"": ""workItem"",
                ""asOf"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}"",
                ""columns"": [
                    {{ ""referenceName"": ""System.Id"", ""name"": ""ID"", ""url"": ""{server.Url}/_apis/wit/fields/System.Id"" }},
                    {{ ""referenceName"": ""System.WorkItemType"", ""name"": ""Work Item Type"", ""url"": ""{server.Url}/_apis/wit/fields/System.WorkItemType"" }},
                    {{ ""referenceName"": ""System.Title"", ""name"": ""Title"", ""url"": ""{server.Url}/_apis/wit/fields/System.Title"" }}
                ],
                ""workItems"": [{workItemsJson}]
            }}";

            server
                .Given(Request.Create()
                    .WithPath("/*/_apis/wit/wiql")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(responseBody));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to work item batch requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupWorkItemsBatchResponse(
            this WireMockServer server,
            IEnumerable<MockWorkItemData> workItems)
        {
            var items = workItems.ToArray();
            var workItemsJson = string.Join(",\n", items.Select(wi => wi.ToJson(server.Url!)));

            var responseBody = $@"{{
                ""count"": {items.Length},
                ""value"": [{workItemsJson}]
            }}";

            server
                .Given(Request.Create()
                    .WithPath("/*/_apis/wit/workitems")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(responseBody));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to single work item requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="workItem">The work item to return.</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupWorkItemResponse(
            this WireMockServer server,
            MockWorkItemData workItem)
        {
            server
                .Given(Request.Create()
                    .WithPath($"/*/_apis/wit/workitems/{workItem.Id}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(workItem.ToJson(server.Url!)));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to project requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="projectName">The project name.</param>
        /// <param name="projectId">The project GUID.</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupProjectResponse(
            this WireMockServer server,
            string projectName,
            Guid projectId)
        {
            var responseBody = $@"{{
                ""id"": ""{projectId}"",
                ""name"": ""{projectName}"",
                ""url"": ""{server.Url}/_apis/projects/{projectId}"",
                ""state"": ""wellFormed"",
                ""revision"": 1,
                ""visibility"": ""private"",
                ""lastUpdateTime"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}""
            }}";

            server
                .Given(Request.Create()
                    .WithPath($"/_apis/projects/{projectName}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(responseBody));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to field definitions requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupFieldDefinitionsResponse(this WireMockServer server)
        {
            var responseBody = $@"{{
                ""count"": 5,
                ""value"": [
                    {{ ""name"": ""ID"", ""referenceName"": ""System.Id"", ""type"": ""integer"", ""url"": ""{server.Url}/_apis/wit/fields/System.Id"" }},
                    {{ ""name"": ""Work Item Type"", ""referenceName"": ""System.WorkItemType"", ""type"": ""string"", ""url"": ""{server.Url}/_apis/wit/fields/System.WorkItemType"" }},
                    {{ ""name"": ""Title"", ""referenceName"": ""System.Title"", ""type"": ""string"", ""url"": ""{server.Url}/_apis/wit/fields/System.Title"" }},
                    {{ ""name"": ""State"", ""referenceName"": ""System.State"", ""type"": ""string"", ""url"": ""{server.Url}/_apis/wit/fields/System.State"" }},
                    {{ ""name"": ""Team Project"", ""referenceName"": ""System.TeamProject"", ""type"": ""string"", ""url"": ""{server.Url}/_apis/wit/fields/System.TeamProject"" }}
                ]
            }}";

            server
                .Given(Request.Create()
                    .WithPath("/_apis/wit/fields")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(responseBody));

            return server;
        }

        /// <summary>
        /// Configures WireMock to respond to work item type requests.
        /// </summary>
        /// <param name="server">The WireMock server.</param>
        /// <param name="projectName">The project name.</param>
        /// <param name="typeName">The work item type name.</param>
        /// <returns>The server for fluent chaining.</returns>
        public static WireMockServer SetupWorkItemTypeResponse(
            this WireMockServer server,
            string projectName,
            string typeName)
        {
            var responseBody = $@"{{
                ""name"": ""{typeName}"",
                ""referenceName"": ""Microsoft.VSTS.WorkItemTypes.{typeName}"",
                ""description"": ""A {typeName} work item"",
                ""color"": ""CC293D"",
                ""icon"": {{ ""id"": ""icon_bug"", ""url"": ""{server.Url}/_apis/wit/workItemIcons/icon_bug"" }},
                ""isDisabled"": false,
                ""xmlForm"": """",
                ""fields"": [],
                ""fieldInstances"": [],
                ""transitions"": {{}},
                ""states"": [],
                ""url"": ""{server.Url}/{projectName}/_apis/wit/workItemTypes/{typeName}""
            }}";

            server
                .Given(Request.Create()
                    .WithPath($"/{projectName}/_apis/wit/workitemtypes/{typeName}")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(responseBody));

            return server;
        }
    }

    /// <summary>
    /// Represents mock work item data for WireMock responses.
    /// </summary>
    public class MockWorkItemData
    {
        public int Id { get; set; }
        public int Rev { get; set; } = 1;
        public string WorkItemType { get; set; } = "Bug";
        public string Title { get; set; } = "Test Work Item";
        public string State { get; set; } = "Active";
        public string TeamProject { get; set; } = "TestProject";
        public Dictionary<string, object> AdditionalFields { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Converts this mock data to Azure DevOps REST API JSON format.
        /// </summary>
        public string ToJson(string baseUrl)
        {
            var additionalFieldsJson = string.Join(",\n",
                AdditionalFields.Select(kvp => $@"""{kvp.Key}"": {FormatValue(kvp.Value)}"));

            var fieldsJson = $@"
                ""System.Id"": {Id},
                ""System.Rev"": {Rev},
                ""System.WorkItemType"": ""{WorkItemType}"",
                ""System.Title"": ""{EscapeJson(Title)}"",
                ""System.State"": ""{State}"",
                ""System.TeamProject"": ""{TeamProject}""";

            if (!string.IsNullOrEmpty(additionalFieldsJson))
            {
                fieldsJson += ",\n" + additionalFieldsJson;
            }

            return $@"{{
                ""id"": {Id},
                ""rev"": {Rev},
                ""fields"": {{{fieldsJson}
                }},
                ""url"": ""{baseUrl}/_apis/wit/workItems/{Id}""
            }}";
        }

        private static string FormatValue(object value)
        {
            return value switch
            {
                string s => $"\"{EscapeJson(s)}\"",
                bool b => b.ToString().ToLowerInvariant(),
                null => "null",
                _ => value.ToString() ?? "null"
            };
        }

        private static string EscapeJson(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }

    /// <summary>
    /// Represents the structure of a WireMock stubs JSON file.
    /// </summary>
    internal class StubsFile
    {
        public List<StubMapping>? Mappings { get; set; }
    }

    /// <summary>
    /// Represents a single stub mapping.
    /// </summary>
    internal class StubMapping
    {
        public string? Name { get; set; }
        public StubRequest? Request { get; set; }
        public StubResponse? Response { get; set; }
    }

    /// <summary>
    /// Represents the request matcher for a stub.
    /// </summary>
    internal class StubRequest
    {
        public string? Method { get; set; }
        public string? UrlPath { get; set; }
        public string? UrlPathPattern { get; set; }
    }

    /// <summary>
    /// Represents the response for a stub.
    /// </summary>
    internal class StubResponse
    {
        public int Status { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Body { get; set; }
    }
}
