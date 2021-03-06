﻿using Engine.API.StandardAPI.ClientCommands;
using Engine.Helpers;
using Engine.Model.Entities;
using Engine.Model.Server;
using System;
using System.Linq;

namespace Engine.API.StandardAPI.ServerCommands
{
  class ServerAddFileToRoomCommand :
      BaseServerCommand,
      ICommand<ServerCommandArgs>
  {
    public void Run(ServerCommandArgs args)
    {
      var receivedContent = Serializer.Deserialize<MessageContent>(args.Message);

      if (receivedContent.File == null)
        throw new ArgumentNullException("File");

      if (string.IsNullOrEmpty(receivedContent.RoomName))
        throw new ArgumentException("RoomName");

      if (!RoomExists(receivedContent.RoomName, args.ConnectionId))
        return;

      using (var context = ServerModel.Get())
      {
        var room = context.Rooms[receivedContent.RoomName];

        if (!room.Users.Contains(args.ConnectionId))
        {
          ServerModel.API.SendSystemMessage(args.ConnectionId, "Вы не входите в состав этой комнаты.");
          return;
        }

        if (room.Files.FirstOrDefault(file => file.Equals(receivedContent.File)) == null)
          room.Files.Add(receivedContent.File);

        var sendingContent = new ClientFilePostedCommand.MessageContent
        {
          File = receivedContent.File,
          RoomName = receivedContent.RoomName
        };

        foreach (string user in room.Users)
          ServerModel.Server.SendMessage(user, ClientFilePostedCommand.Id, sendingContent);
      }
    }

    [Serializable]
    public class MessageContent
    {
      string roomName;
      FileDescription file;

      public string RoomName { get { return roomName; } set { roomName = value; } }
      public FileDescription File { get { return file; } set { file = value; } }
    }

    public const ushort Id = (ushort)ServerCommand.AddFileToRoom;
  }
}
