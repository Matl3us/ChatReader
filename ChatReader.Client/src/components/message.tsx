import { MessageDTO } from "@/data/messageStore";

const Message = ({ msg }: { msg: MessageDTO }) => {
  return (
    <div>
      <p className="inline font-semibold" style={{ color: msg.Tags.Color }}>
        {msg.User}
      </p>
      <p className="inline">: {msg.Content}</p>
    </div>
  );
};

export default Message;
