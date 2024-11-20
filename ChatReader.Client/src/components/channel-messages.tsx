import { MessageDTO, messageStore } from "@/data/messageStore";
import { useEffect, useState } from "react";

const ChannelMessages = ({ channelName }: { channelName: string }) => {
  const [messages, setMessages] = useState<MessageDTO[]>(
    messageStore.getChannelMessages(channelName)
  );

  useEffect(() => {
    const updateMessages = () => {
      const updatedMessages = messageStore.getChannelMessages(channelName);
      setMessages(updatedMessages);
    };

    setMessages(messageStore.getChannelMessages(channelName));
    messageStore.subscribe(updateMessages);

    return () => {
      messageStore.unsubscribe(updateMessages);
    };
  }, [channelName]);

  return (
    <div>
      <p>Joined channel {channelName}</p>
      {messages.map((msg) => (
        <div key={msg.Tags.Id} className="flex">
          <p style={{ color: msg.Tags.Color }}>{msg.User}</p>
          <p>: {msg.Content}</p>
        </div>
      ))}
    </div>
  );
};

export default ChannelMessages;
