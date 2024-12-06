import { BadgesSet, BadgeVersion } from "@/providers/badges-provider";
import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const mapBadgesArray = (
  badgeArray: Array<string>,
  badgesList: Map<string, Array<BadgesSet>>,
  channelName: string
) => {
  if (!badgesList.has("global") || !badgesList.has(channelName)) {
    return <></>;
  }
  const globalBadges = badgesList.get("global")!;
  const channelBadges = badgesList.get(channelName)!;

  const badgeElements = badgeArray
    .map((badge) => mapBadge(badge, globalBadges, channelBadges))
    .filter((value) => Boolean(value));

  return <>{badgeElements}</>;
};

const mapBadge = (
  badgeString: string,
  globalBadges: Array<BadgesSet>,
  channelBadges: Array<BadgesSet>
) => {
  const [badgeName, badgeVersion] = badgeString.split("/");
  const badges = globalBadges.concat(channelBadges);

  const filteredBadgeName = badges.filter((b) => b.set_id === badgeName);
  if (filteredBadgeName.length === 0) {
    return null;
  }

  const filteredBadges = {
    set_id: filteredBadgeName[0].set_id,
    versions: filteredBadgeName.reduce<Array<BadgeVersion>>(
      (a, b) => a.concat(b.versions),
      []
    ),
  };

  const filteredBadgeVersion = filteredBadges.versions.filter(
    (b) => b.id === badgeVersion
  );
  if (filteredBadgeVersion.length === 0) {
    return null;
  }

  const finalBadge = filteredBadgeVersion[0];

  return (
    <img
      key={badgeString}
      src={finalBadge.image_url_1x}
      alt={finalBadge.title}
      className="w-[18px] h-[18px] inline-block"
    />
  );
};
