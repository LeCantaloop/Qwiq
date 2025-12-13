using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Loads WireMock stub mappings from a JSON file captured from real Azure DevOps traffic.
        /// </summary>
        public static WireMockServer LoadStubsFromFile(this WireMockServer server, string stubsFilePath)
        {
            if (!File.Exists(stubsFilePath))
            {
                throw new FileNotFoundException($"Stubs file not found: {stubsFilePath}");
            }

            var json = File.ReadAllText(stubsFilePath);
            var stubs = JsonSerializer.Deserialize<StubsFile>(json, JsonOptions);

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
            var assemblyLocation = typeof(AzureDevOpsWireMockExtensions).Assembly.Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation)!;

            // Prefer the extracted stubs which contain full API responses including resource locations
            var possiblePaths = new[]
            {
                Path.Combine(assemblyDir, "WireMock", "Stubs", "azure-devops-stubs-extracted.json"),
                Path.Combine(assemblyDir, "WireMock", "Stubs", "azure-devops-stubs.json"),
                Path.Combine(assemblyDir, "azure-devops-stubs-extracted.json"),
                Path.Combine(assemblyDir, "azure-devops-stubs.json"),
                Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "..", "WireMock", "Stubs", "azure-devops-stubs-extracted.json")),
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
        /// </summary>
        /// <remarks>
        /// The IdentityDescriptor must be a string in format "identityType;identifier"
        /// NOT a JSON object. The Azure DevOps SDK uses a custom JSON converter.
        /// </remarks>
        public static WireMockServer SetupVssConnectionHandshake(this WireMockServer server)
        {
            // IdentityDescriptor format: "identityType;identifier" as a string
            var connectionDataResponse = $@"{{
                ""authenticatedUser"": {{
                    ""id"": ""00000000-0000-0000-0000-000000000001"",
                    ""descriptor"": ""Microsoft.IdentityModel.Claims.ClaimsIdentity;00000000-0000-0000-0000-000000000001"",
                    ""subjectDescriptor"": ""msa.00000000-0000-0000-0000-000000000001"",
                    ""providerDisplayName"": ""Test User"",
                    ""isActive"": true,
                    ""properties"": {{}}
                }},
                ""authorizedUser"": {{
                    ""id"": ""00000000-0000-0000-0000-000000000001"",
                    ""descriptor"": ""Microsoft.IdentityModel.Claims.ClaimsIdentity;00000000-0000-0000-0000-000000000001"",
                    ""subjectDescriptor"": ""msa.00000000-0000-0000-0000-000000000001"",
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

            // Handle OPTIONS preflight requests for CORS and resource discovery
            // The SDK makes OPTIONS requests to discover API capabilities
            server
                .Given(Request.Create()
                    .WithPath("/_apis/*")
                    .UsingMethod("OPTIONS"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Allow", "DELETE, GET, HEAD, OPTIONS, PATCH, POST, PUT")
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{}"));

            // Also handle the root OPTIONS for full service discovery
            server
                .Given(Request.Create()
                    .WithPath("/_apis/")
                    .UsingMethod("OPTIONS"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Allow", "DELETE, GET, HEAD, OPTIONS, PATCH, POST, PUT")
                    .WithHeader("Content-Type", "application/json")
                    .WithBody("{}"));

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
        public Dictionary<string, object> AdditionalFields { get; set; } = new();

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

        private static string FormatValue(object value) => value switch
        {
            string s => $"\"{EscapeJson(s)}\"",
            bool b => b.ToString().ToLowerInvariant(),
            null => "null",
            _ => value.ToString() ?? "null"
        };

        private static string EscapeJson(string s) =>
            s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    internal class StubsFile
    {
        public List<StubMapping>? Mappings { get; set; }
    }

    internal class StubMapping
    {
        public string? Name { get; set; }
        public StubRequest? Request { get; set; }
        public StubResponse? Response { get; set; }
    }

    internal class StubRequest
    {
        public string? Method { get; set; }
        public string? UrlPath { get; set; }
        public string? UrlPathPattern { get; set; }
    }

    internal class StubResponse
    {
        public int Status { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Body { get; set; }
    }
}
