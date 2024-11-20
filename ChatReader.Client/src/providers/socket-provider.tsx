import { MessageDTO, messageStore } from "@/data/messageStore";
import { createContext, useContext, useEffect, useState } from "react";

type SocketContextType = {
  socket: WebSocket | null;
  isConnected: boolean;
};

const SocketContext = createContext<SocketContextType>({
  socket: null,
  isConnected: false,
});

export const useSocket = () => {
  return useContext(SocketContext);
};

export const SocketProvider = ({ children }: { children: React.ReactNode }) => {
  const [socket, setSocket] = useState<WebSocket | null>(null);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const socket = new WebSocket("ws://localhost:5240/api/ws");

    socket.addEventListener("open", () => {
      setIsConnected(true);
      console.log("Connected");
    });

    socket.addEventListener("close", () => {
      setIsConnected(false);
      console.log("Disconnected");
    });

    socket.addEventListener("message", (event: MessageEvent) => {
      messageStore.addMessageToChannel(JSON.parse(event.data) as MessageDTO);
    });

    socket.addEventListener("error", () => {
      console.log("Error");
    });

    setSocket(socket);

    return () => {
      socket.close();
    };
  }, []);

  return (
    <SocketContext.Provider value={{ socket, isConnected }}>
      {children}
    </SocketContext.Provider>
  );
};
