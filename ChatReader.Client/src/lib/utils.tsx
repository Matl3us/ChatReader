import { BadgesSet } from "@/providers/badges-provider";
import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export const mapBadgesArray = (
  badgeArray: Array<string>,
  badgesList: Array<BadgesSet>
) => {
  const badgeElements = badgeArray
    .map((badge) => mapBadge(badge, badgesList))
    .filter((value) => Boolean(value));

  return <>{badgeElements}</>;
};

const mapBadge = (badgeString: string, badgesList: Array<BadgesSet>) => {
  const [badgeName, badgeVersion] = badgeString.split("/");
  const filteredBadgeName = badgesList.filter((b) => b.set_id === badgeName);
  if (filteredBadgeName.length === 0) {
    return null;
  }

  const filteredBadgeVersion = filteredBadgeName[0].versions.filter(
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
