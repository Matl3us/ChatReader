import { MessageDTO } from "@/data/messageStore";
import { mapBadgesArray } from "@/lib/utils";
import { BadgesContext } from "@/providers/badges-provider";
import { useContext } from "react";

const Message = ({ msg }: { msg: MessageDTO }) => {
  const { badgesList } = useContext(BadgesContext);

  return (
    <div className="my-1">
      <p
        className="inline-flex items-center gap-1 font-semibold align-middle"
        style={{ color: msg.Tags.Color }}
      >
        {mapBadgesArray(msg.Tags.Badges, badgesList, msg.Channel)}
        {msg.User}
      </p>
      <p className="inline align-middle">: {msg.Content}</p>
    </div>
  );
};

export default Message;
