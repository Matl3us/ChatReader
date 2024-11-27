import { createContext, useEffect, useState, ReactNode } from "react";

export type BadgesSet = {
  set_id: string;
  versions: [
    {
      id: string;
      image_url_1x: string;
      image_url_2x: string;
      image_url_4x: string;
      title: string;
      description: string;
      click_action: string;
      click_url: string;
    }
  ];
};

export const BadgesContext = createContext<Array<BadgesSet>>([]);

export const BadgesProvider = ({ children }: { children: ReactNode }) => {
  const [globalBadges, setGlobalBadges] = useState<Array<BadgesSet>>([]);

  useEffect(() => {
    fetchGlobalBadges();
  }, []);

  const fetchGlobalBadges = async () => {
    const url = "http://localhost:5240/api/channel/badges/global";
    const response = await fetch(url);
    if (response.ok) {
      const data = await response.json();
      setGlobalBadges(data);
    }
  };

  return (
    <BadgesContext.Provider value={globalBadges}>
      {children}
    </BadgesContext.Provider>
  );
};
