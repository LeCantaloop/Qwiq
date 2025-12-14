# Qwiq.WireMock.Tests Component Guide

## Component Overview

**Qwiq.WireMock.Tests** provides HTTP traffic recording and playback capabilities for testing REST API interactions without hitting real Azure DevOps servers.

## Purpose

- Record actual HTTP REST API traffic from Azure DevOps
- Replay recorded traffic for offline testing
- Validate REST client behavior without server dependency
- Support fast, deterministic integration tests

## Key Characteristics

- **Target Frameworks**: Test project TFMs
- **Test Framework**: MSTest
- **Tool**: WireMock.Net for HTTP mocking
- **Pattern**: Record/Playback HTTP proxy

## Usage Modes

### Record Mode

Captures real HTTP traffic and saves responses:

```csharp
// Start WireMock in record mode
var server = WireMockServer.Start();
server.StartRecording();

// Make real API calls - traffic is recorded
var store = CreateRestStore(proxyUrl: server.Url);
var items = store.Query("SELECT [System.Id] FROM WorkItems");

// Save recordings
server.SaveMappings();
```

### Playback Mode

Replays previously recorded HTTP responses:

```csharp
// Start WireMock with saved mappings
var server = WireMockServer.Start();
server.ReadStaticMappings();

// Make API calls - responses come from recordings
var store = CreateRestStore(proxyUrl: server.Url);
var items = store.Query("SELECT [System.Id] FROM WorkItems");
```

## Benefits

- **Offline testing**: No network connectivity required
- **Deterministic**: Same responses every time
- **Fast**: No actual HTTP calls
- **Repeatable**: Captured once, replayed many times

## Common Patterns

### Recording REST API Calls

1. Connect to real Azure DevOps with PAT
2. Start WireMock in record mode
3. Execute API operations to record
4. Save mappings to disk
5. Commit recordings to repository

### Testing with Recordings

1. Load saved WireMock mappings
2. Configure REST client to use WireMock proxy
3. Execute tests - responses from recordings
4. Verify behavior without real server

## Related Components

- **Qwiq.Core.Rest** - REST client being tested
- **Qwiq.Integration.Tests** - Can be enhanced with WireMock
- **Qwiq.Tests.Common** - Shared test infrastructure

## Common Mistakes to Avoid

❌ **Don't commit sensitive data** - Sanitize PATs/tokens from recordings

❌ **Don't record and playback in same test** - Choose one mode

✅ **Do sanitize recordings** - Remove auth headers, tokens

✅ **Do use for REST API tests** - Not applicable to SOAP

✅ **Do version recordings** - Track changes in API responses
