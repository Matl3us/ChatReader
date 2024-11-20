import { useState } from "react";
import { Input } from "./ui/input";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { useSocket } from "../providers/socket-provider";
import { messageStore } from "@/data/messageStore";
import ChannelMessages from "./channel-messages";

const Chat = () => {
  const { socket } = useSocket();
  const [channelName, setChannelName] = useState("");
  const [message, setMessage] = useState("");
  const [joinedChannels, setJoinedChannels] = useState<Array<string>>([]);

  const addChannel = (channelName: string) => {
    setChannelName("");
    if (!joinedChannels.includes(channelName) && channelName.length > 0) {
      setJoinedChannels(joinedChannels.concat(channelName));
      socket?.send(`JOIN #${channelName}`);
    }
  };

  const removeChannel = (channelName: string) => {
    setJoinedChannels(
      joinedChannels.filter((channel) => channel != channelName)
    );
    messageStore.clearChannelMessages(channelName);
    socket?.send(`PART #${channelName}`);
  };

  return (
    <div className="p-6 h-screen flex flex-col">
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
      <div className="p-1 mt-4 mb-1 flex-1 overflow-auto">
        {joinedChannels.length > 0 ? (
          <Tabs defaultValue={joinedChannels[0]}>
            <TabsList>
              {joinedChannels.map((channel) => (
                <div
                  key={channel}
                  className="border border-stone-300 hover:border-2 hover:border-button rounded-md"
                >
                  <TabsTrigger value={channel}>{channel}</TabsTrigger>
                  <button
                    className="px-2 font-bold text-button"
                    onClick={() => removeChannel(channel)}
                  >
                    X
                  </button>
                </div>
              ))}
            </TabsList>
            {joinedChannels.map((channel) => (
              <TabsContent
                key={channel}
                value={channel}
                className="p-2 rounded-md mt-8 bg-stone-900"
              >
                <ChannelMessages channelName={channel} />
              </TabsContent>
            ))}
          </Tabs>
        ) : (
          <div>
            <p className="text-text">Join a Twitch chat</p>
          </div>
        )}
      </div>
      <div className="flex gap-2">
        <Input
          placeholder="Send message"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />
        <button className="bg-button rounded-md p-2 text-text">Send</button>
      </div>
    </div>
  );
};

export default Chat;
