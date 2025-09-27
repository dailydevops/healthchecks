# Azure IoT Hub Integration Tests

Integration tests for Azure IoT Hub health checks are currently not implemented because:

1. **No Local Emulator**: Unlike Azure Blob Storage, Tables, and Queues (which use Azurite), Azure IoT Hub does not have a local emulator available for testing.

2. **Testcontainers Limitation**: There are no testcontainers available for Azure IoT Hub, unlike other Azure services such as Service Bus.

3. **Real Service Dependency**: Integration tests would require a real Azure IoT Hub instance with valid credentials, which is not suitable for automated testing in this environment.

## Future Considerations

If Azure IoT Hub emulator or testcontainers become available in the future, integration tests should be added following the same patterns used by other Azure services in this test suite.

For now, comprehensive unit tests provide adequate coverage of the health check functionality, including:
- Configuration validation
- Client factory behavior  
- Health check execution logic
- Error handling scenarios