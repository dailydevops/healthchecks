# Azure IoT Hub Integration Tests

This directory contains integration tests for Azure IoT Hub health checks using testcontainers.

## Implementation Details

### Mock Container Approach

The integration tests use a `IoTHubMockContainer` that runs WireMock in a Docker container to simulate Azure IoT Hub REST API endpoints. This approach allows us to test the health check infrastructure while working around the limitations of Azure IoT Hub SDK.

### Test Limitations

Due to the nature of Azure IoT Hub SDK, full integration testing has several limitations:

1. **Hostname Validation**: The Azure IoT Hub SDK validates that hostnames follow the pattern `*.azure-devices.net`
2. **Certificate Requirements**: The SDK requires valid SSL/TLS certificates from Azure
3. **Authentication Flow**: The SDK performs complex authentication that cannot be easily mocked

### Available Tests

- ✅ **Configuration Validation**: Tests invalid connection strings and configuration scenarios
- ✅ **Timeout Handling**: Tests timeout scenarios that result in degraded health status  
- ⚠️ **Mock Server Tests**: Skipped due to SDK hostname validation (documented with Skip attribute)

### Test Coverage

While we cannot test against a fully functional IoT Hub mock due to SDK limitations, the integration tests provide coverage for:

- Health check registration and dependency injection
- Configuration validation and error handling
- Timeout behavior and degraded status responses
- Integration with the health check infrastructure

The comprehensive unit tests provide detailed coverage of the core health check functionality, making this integration test approach sufficient for validating the overall system behavior.

## Usage

The tests run automatically as part of the integration test suite. The `IoTHubMockContainer` is shared across tests using the TUnit framework's shared instance pattern.