import { useState } from "react";
import { Input } from "./ui/input";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { useSocket } from "../providers/socket-provider";

const Chat = () => {
  const { socket } = useSocket();
  const [channelName, setChannelName] = useState("");
  const [joinedChannels, setJoinedChannels] = useState<Array<string>>([]);

  const addChannel = (channelName: string) => {
    setChannelName("");
    if (!joinedChannels.includes(channelName)) {
      setJoinedChannels(joinedChannels.concat(channelName));
      socket?.send(`JOIN #${channelName}`)
    }
  };

  const removeChannel = (channelName: string) => {
    setJoinedChannels(
      joinedChannels.filter((channel) => channel != channelName)
    );
    socket?.send(`PART #${channelName}`)
  };

  return (
    <div className="p-4">
      <h1 className="text-3xl font-semibold text-text mb-2">Chat reader</h1>
      <div className="w-64 flex gap-2">
        <Input
          placeholder="Channel name"
          value={channelName}
          onChange={(e) => setChannelName(e.target.value)}
        />
        <button
          className="bg-button rounded-md p-2 text-text"
          onClick={() => {
            addChannel(channelName);
          }}
        >
          Add
        </button>
      </div>
      <div className="mt-4">
        {joinedChannels.length > 0 ? (
          <Tabs defaultValue={joinedChannels[0]}>
            <TabsList>
              {joinedChannels.map((channel) => (
                <div key={channel} className="text-text">
                  <TabsTrigger value={channel}>{channel}</TabsTrigger>
                  <button
                    className="font-bold"
                    onClick={() => removeChannel(channel)}
                  >
                    X
                  </button>
                </div>
              ))}
            </TabsList>
            {joinedChannels.map((channel) => (
              <TabsContent key={channel} value={channel}>
                Joined channel {channel}
              </TabsContent>
            ))}
          </Tabs>
        ) : (
          <div>
            <p className="text-text">Join a Twitch chat</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Chat;
