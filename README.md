# Twitch Chat Client

> A web application connecting users to Twitch chats for selected channels, allowing them to log in, send messages, and follow conversations in real-time.

## Features

- **Integration with Twitch OAuth**: Uses Twitch OAuth system to authenticate users and obtain tokens.
- **Channel Chat Connection**: Allows users to connect to the chat of selected Twitch channels after logging in.
- **Real-time Messaging**: Supports sending and displaying messages in real-time using WebSocket protocol.
- **Message Analysis**: Analyzes and parses Twitch IRC messages to identify message types and deliver them to the user.

## Tech Stack

- **Backend**: ASP.NET Core
- **Frontend**: React, Tailwind CSS

### Prerequisites

- `.NET 8` and `Node.js` installed.
- Obtain a Twitch Developer client ID and secret from the [Twitch Developer Portal](https://dev.twitch.tv/).

## Installation

1. **Clone the repository**:
```bash
https://github.com/Matl3us/ChatReader.git
```
2.  **Install NPM packages**
```bash
cd ChatReader.Client
npm install
```
3. **Build the frontend app**
```bash
npm run build
```
4. **Start the app**
```bash
cd ChatReader
dotnet run
```