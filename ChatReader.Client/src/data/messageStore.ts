type PRIVMSGTags = {
  Id: string;
  Badges: Array<string>;
  Color: string;
};

export type MessageDTO = {
  Prefix: string;
  Command: number;
  Params: string;
  Tags: PRIVMSGTags;
  User: string;
  Channel: string;
  Content: string;
};

class MessagesStore {
  private messagesMap: Map<string, Array<MessageDTO>>;
  private listeners: Array<() => void> = [];

  public constructor() {
    this.messagesMap = new Map();
  }

  public addMessageToChannel(message: MessageDTO) {
    console.log("Adding message:", message);

    if (!this.messagesMap.has(message.Channel)) {
      this.messagesMap.set(message.Channel, []);
    }
    const messages = this.messagesMap.get(message.Channel)!;
    if (messages.length + 1 > 50) {
      messages.shift();
    }
    messages.push(message);
    this.messagesMap.set(message.Channel, messages);
    this.notifyListeners();
  }

  public getChannelMessages(channelName: string) {
    const messages = this.messagesMap.get(channelName);
    return messages ? [...messages] : [];
  }

  public clearChannelMessages(channelName: string) {
    this.messagesMap.delete(channelName);
    this.notifyListeners();
  }

  public subscribe(listener: () => void) {
    this.listeners.push(listener);
  }

  public unsubscribe(listener: () => void) {
    this.listeners = this.listeners.filter((l) => l !== listener);
  }

  private notifyListeners() {
    this.listeners.forEach((listener) => listener());
  }
}

export const messageStore = new MessagesStore();
