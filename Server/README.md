Running the server as a standalone application:

1. Build CounterPublisher.
2. Build LoadBalancing (3 times to resolve all .dll dependencies - this may be overkill though).
3. Run \deploy\bin_Win64\_run-Photon-as-application.start.cmd to start the server.


Notes:

- Run \deploy\bin_Win64\_run-Photon-as-application.stop.cmd to stop the server.
- Logs located in \deploy\bin_Win64\log.