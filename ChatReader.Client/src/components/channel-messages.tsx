import { MessageDTO, messageStore } from "@/data/messageStore";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import Message from "./message";

const ChannelMessages = ({ channelName }: { channelName: string }) => {
  const [messages, setMessages] = useState<MessageDTO[]>(
    messageStore.getChannelMessages(channelName)
  );

  const [isInView, setIsInView] = useState(true);
  const bottomRef = useRef<HTMLDivElement>(null);

  const checkInView = () => {
    if (bottomRef.current) {
      const rect = bottomRef.current.getBoundingClientRect();
      setIsInView(rect.top < window.innerHeight - 56 && rect.bottom >= 0);
    }
  };

  useLayoutEffect(() => {
    checkInView();

    const updateMessages = () => {
      const updatedMessages = messageStore.getChannelMessages(channelName);
      setMessages(updatedMessages);

      if (!isInView && bottomRef.current) {
        bottomRef.current.scrollIntoView({ behavior: "smooth" });
      }
    };

    messageStore.subscribe(updateMessages);

    return () => {
      messageStore.unsubscribe(updateMessages);
    };
  }, [channelName, isInView]);

  useEffect(() => {
    if (isInView && bottomRef.current) {
      bottomRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages, isInView]);

  return (
    <div>
      <p className="italic font-semibold mb-2">Joined channel {channelName}</p>
      {messages.map((msg) => (
        <Message key={msg.Tags.Id} msg={msg} />
      ))}
      <div ref={bottomRef} />
    </div>
  );
};

export default ChannelMessages;
