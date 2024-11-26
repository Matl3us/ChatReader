import { useEffect, useRef, useState } from "react";
import { Input } from "./ui/input";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { useSocket } from "../providers/socket-provider";
import { messageStore } from "@/data/messageStore";
import ChannelMessages from "./channel-messages";

type User = {
  id: string;
  username: string;
};

type UserInfo = {
  id: string;
  login: string;
  display_name: string;
  type: string;
  description: string;
  profile_image_url: string;
  offline_image_url: string;
  created_at: string;
};

type UserColor = {
  user_id: string;
  user_login: string;
  user_name: string;
  color: string;
};

const Chat = () => {
  const { socket } = useSocket();
  const [channelName, setChannelName] = useState("");
  const [message, setMessage] = useState("");
  const [joinedChannels, setJoinedChannels] = useState<Array<string>>([]);
  const [userInfo, setUserInfo] = useState<UserInfo | null>(null);
  const [userColor, setUserColor] = useState<UserColor | null>(null);

  const tabsRootRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const userInfoStr = localStorage.getItem("user")!;
    const user = JSON.parse(userInfoStr) as User;
    getUserInfo(user.username);
    getUserColor(user.username);
  }, []);

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      addChannel(channelName);
    }
  };

  const getUserInfo = async (username: string) => {
    const url = `http://localhost:5240/api/user?username=${username}`;
    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setUserInfo(data);
    }
  };

  const getUserColor = async (username: string) => {
    const url = `http://localhost:5240/api/user/color?username=${username}`;
    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setUserColor(data);
    }
  };

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

  const sendMessage = () => {
    if (tabsRootRef.current && userInfo) {
      const children = tabsRootRef.current.childNodes;
      for (const child of children) {
        if (child instanceof HTMLElement) {
          if (child.getAttribute("data-state") === "active") {
            const activeChannel = child.getAttribute("id")?.split("-").pop();
            if (activeChannel) {
              socket?.send(`PRIVMSG #${activeChannel} :${message}`);
              messageStore.addMessageToChannel({
                Prefix: "",
                Command: 7,
                Params: "",
                Tags: {
                  Id: "",
                  Badges: [],
                  Color: userColor?.color ?? "#595959",
                },
                User: userColor?.user_name ?? "#Your message#",
                Channel: activeChannel,
                Content: message,
              });
              setMessage("");
            }
          }
        }
      }
    }
  };

  return (
    <div className="p-4 h-screen flex flex-col">
      <div className="h-32 flex justify-between bg-stone-950 rounded-md px-6 py-4">
        <div>
          <h1 className="text-3xl font-semibold text-text mb-2">Chat reader</h1>
          <div className="w-64 flex gap-2">
            <Input
              placeholder="Channel name"
              value={channelName}
              onChange={(e) => setChannelName(e.target.value)}
              onKeyDown={handleKeyDown}
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
        </div>
        {userInfo && (
          <div className="flex flex-col gap-2 items-center justify-center">
            <img
              src={userInfo.profile_image_url}
              alt="Profile image"
              className="w-14 h-14 rounded-lg"
            />
            <p className="text-text font-semibold">{userInfo.display_name}</p>
          </div>
        )}
      </div>
      {joinedChannels.length > 0 ? (
        <div className="p-1 flex flex-1 h-4/6">
          <Tabs
            className="flex flex-col flex-1"
            ref={tabsRootRef}
            defaultValue={joinedChannels[0]}
          >
            <TabsList className="overflow-x-auto max-w-md whitespace-nowrap">
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
                className="flex-1 overflow-auto p-4 rounded-md mb-4 bg-stone-950"
              >
                <ChannelMessages channelName={channel} />
              </TabsContent>
            ))}
            <div className="flex gap-2">
              <Input
                placeholder="Send message"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
              />
              <button
                className="bg-button rounded-md p-2 text-text"
                onClick={() => sendMessage()}
              >
                Send
              </button>
            </div>
          </Tabs>
        </div>
      ) : (
        <div>
          <p className="p-2 mt-6 text-text italic font-semibold">
            Join a Twitch chat
          </p>
        </div>
      )}
    </div>
  );
};

export default Chat;
