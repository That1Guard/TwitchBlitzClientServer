# TwitchBlitzClientProxy

The `TwitchBlitzClientProxy` repo is a game mod DLL client for Blitz3D programs, specifically for `SCP: Containment Breach`. This project also includes a C# server program that acts as a voting manager, allowing viewers to participate in "crowd control" style voting via Twitch.

## Projects

- **Game Mod DLL Client** (TwitchBlitzClientProxy): Made to integrate with Blitz3D programs built from source. An example game that integrates it can be found here: [SCP: Containment Breach - Twitch Chaos Mod](https://github.com/That1Guard/scpcb_twitch).
- **C# Server** (TwitchBlitzServer): A voting manager and server that communicates with the DLL client. This keeps track of which voting options should be polled for Twitch viewers to vote on, tracks viewer votes, supports a html page for sending voting information to OBS for tracking, and decides the winning vote to send over to the client.

## Usage

1. Start the C# server: `TwitchBlitzServer.exe`. Make sure to configure the `config.json` by copying the `config.json.example` and setting that up.

2. Run a Blitz3D built program (e.g. `SCP - Containment Breach.exe`) with the mod DLL (e.g. `TwitchBlitzClientProxy.dll`) in the same directory as the program built with Blitz3D built program.

3. Viewers can join the stream and participate by voting in the chat with the associated voting options at the time.

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests if you want.

## License

This project is licensed under the Unlicense License. See the [LICENSE](LICENSE) file for details.
