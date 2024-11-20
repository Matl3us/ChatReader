import { useEffect, useState } from "react";
import { SocketProvider } from "./providers/socket-provider";
import Login from "./components/login";
import Chat from "./components/chat";

export default function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const checkAuth = async () => {
    const url = "http://localhost:5240/api/auth/check";
    const response = await fetch(url);
    if (response.ok) {
      setIsAuthenticated(true);
    }
  };

  useEffect(() => {
    checkAuth();
  }, []);

  if (!isAuthenticated) {
    return <Login />;
  }

  return (
    <SocketProvider>
      <Chat />
    </SocketProvider>
  );
}
