using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PotatoBot.Discord.Modules;

namespace PotatoBot.Discord
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private Timer _tickTimer;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _commands = new CommandService();
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig {
                    AlwaysDownloadUsers = true
                });

                _client.Log += Log;

                //  You can assign your bot token to a string, and pass that in to connect.
                //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
                var token = _configuration.GetValue<string>("DiscordToken");
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
                
                // Hook the MessageReceived event into our command handler
                _client.MessageReceived += HandleCommandAsync;

                // Here we discover all of the command modules in the entry 
                // assembly and load them. Starting from Discord.NET 2.0, a
                // service provider is required to be passed into the
                // module registration method to inject the 
                // required dependencies.
                //
                // If you do not use Dependency Injection, pass null.
                // See Dependency Injection guide for more information.
                
                 await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
                                                 services: _serviceProvider);

                _tickTimer = new Timer(
                    new TimerCallback(TickTimer), 
                    null, 
                    1000, 
                    2000);


                // Block this task until the program is closed.
                await Task.Delay(-1);
            }
        }

        private async void TickTimer(object state)
        {
            var guildGameRepo = _serviceProvider.GetService(typeof(GuildGameRepository)) as GuildGameRepository;
            var guildGames = guildGameRepo.GetAllGuildGames();
            foreach(var guildGame in guildGames)
            {
                try 
                {
                    if (guildGame.CurrentGame.IsOver) continue;
                    guildGame.Tick();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error during tick timer");
                }
            }
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix("potato ", ref argPos, StringComparison.InvariantCultureIgnoreCase) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: _serviceProvider);
        }

        private Task Log(LogMessage msg)
        {
            _logger.LogInformation(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
