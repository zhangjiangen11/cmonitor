﻿using cmonitor.plugins.command.report;
using cmonitor.server;
using common.libs;
using MemoryPack;
using cmonitor.plugins.sapi;

namespace cmonitor.plugins.command.messenger
{
    public sealed class CommandClientMessenger : IMessenger
    {

        private readonly CommandReport commandReport;
        public CommandClientMessenger(CommandReport commandReport)
        {
            this.commandReport = commandReport;
        }


        [MessengerId((ushort)CommandMessengerIds.Exec)]
        public void Exec(IConnection connection)
        {
            string[] commands = MemoryPackSerializer.Deserialize<string[]>(connection.ReceiveRequestWrap.Payload.Span);
            Task.Run(() =>
            {
                string result = CommandHelper.Windows(string.Empty, commands);
            });

            connection.Write(Helper.TrueArray);
        }

        [MessengerId((ushort)CommandMessengerIds.CommandStart)]
        public void CommandStart(IConnection connection)
        {
            int id = commandReport.CommandStart();
            connection.Write(BitConverter.GetBytes(id));
        }

        [MessengerId((ushort)CommandMessengerIds.CommandWrite)]
        public void CommandWrite(IConnection connection)
        {
            CommandLineWriteInfo commandLineWriteInfo = MemoryPackSerializer.Deserialize<CommandLineWriteInfo>(connection.ReceiveRequestWrap.Payload.Span);
            commandReport.CommandWrite(commandLineWriteInfo);
        }

        [MessengerId((ushort)CommandMessengerIds.CommandStop)]
        public void CommandStop(IConnection connection)
        {
            commandReport.CommandStop(BitConverter.ToInt32(connection.ReceiveRequestWrap.Payload.Span));
        }

        [MessengerId((ushort)CommandMessengerIds.CommandAlive)]
        public void CommandAlive(IConnection connection)
        {
            commandReport.CommandAlive(BitConverter.ToInt32(connection.ReceiveRequestWrap.Payload.Span));
        }
    }


    public sealed class CommandServerMessenger : IMessenger
    {

        private readonly IApiServerServer clientServer;
        public CommandServerMessenger(IApiServerServer clientServer)
        {
            this.clientServer = clientServer;
        }

        [MessengerId((ushort)CommandMessengerIds.CommandData)]
        public void CommandData(IConnection connection)
        {
            CommandLineDataInfo commandLineDataInfo = MemoryPackSerializer.Deserialize<CommandLineDataInfo>(connection.ReceiveRequestWrap.Payload.Span);
            clientServer.Notify("/notify/command/data", commandLineDataInfo);
        }
    }
}
