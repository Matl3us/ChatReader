import { createContext, useEffect, useState, ReactNode } from "react";

export type BadgesSet = {
  set_id: string;
  versions: Array<BadgeVersion>;
};

export type BadgeVersion = {
  id: string;
  image_url_1x: string;
  image_url_2x: string;
  image_url_4x: string;
  title: string;
  description: string;
  click_action: string;
  click_url: string;
};

type BadgesContextType = {
  badgesList: Map<string, Array<BadgesSet>>;
  fetchChannelBadges: (channelName: string) => Promise<void>;
};

export const BadgesContext = createContext<BadgesContextType>({
  badgesList: new Map<string, Array<BadgesSet>>(),
  fetchChannelBadges: async () => {},
});

export const BadgesProvider = ({ children }: { children: ReactNode }) => {
  const [badgesList, setBadgesList] = useState<Map<string, Array<BadgesSet>>>(
    new Map<string, Array<BadgesSet>>()
  );

  useEffect(() => {
    fetchGlobalBadges();
  }, []);

  const fetchGlobalBadges = async () => {
    const url = "http://localhost:5240/api/channel/badges/global";
    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setBadgesList(
        new Map<string, Array<BadgesSet>>(badgesList).set("global", data)
      );
    }
  };

  const fetchChannelBadges = async (channelName: string) => {
    const url = `http://localhost:5240/api/channel/badges?username=${channelName}`;
    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setBadgesList(
        new Map<string, Array<BadgesSet>>(badgesList).set(channelName, data)
      );
    }
  };

  return (
    <BadgesContext.Provider value={{ badgesList, fetchChannelBadges }}>
      {children}
    </BadgesContext.Provider>
  );
};
