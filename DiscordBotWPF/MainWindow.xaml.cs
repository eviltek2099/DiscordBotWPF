using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotWPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    Server svr = new Server();
    public MainWindow()
    {
      InitializeComponent();
      DataContext = svr;
      svr.Start(new[] {
        "https://worldmatrix.xyz/discordapi/"
      });


      MainAsync().GetAwaiter().GetResult();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      string DiscordUri = "https://discord.com/api/oauth2/authorize";
      string Client_id = "905885667026944040";
      string URL = DiscordUri + "?client_id=" + Client_id +
        "&redirect_uri=https%3A%2F%2Fworldmatrix.xyz%2Fdiscordapi%2F&response_type=code&scope=identify";

      Process.Start(URL);
    }

    public async Task MainAsync()
    {
      var Client = new DiscordClient(new DiscordConfiguration()
      {
        Token = "OTA1ODg1NjY3MDI2OTQ0MDQw.YYQlxQ.HRKtit66aB_LGwQwjcKNMcdn4MQ",
        TokenType = TokenType.Bot,
        AutoReconnect = true
      });

      Client.Ready += Client_Ready;
      Client.GuildAvailable += Client_GuildAvailable;
      Client.ClientErrored += Client_ClientError;

      Client.MessageCreated += Handlers.MessageCreated;
      Client.GuildMemberAdded += Handlers.MemberAdded;
      Client.MessageUpdated += Handlers.MessageUpdated;
      Client.ComponentInteractionCreated += Handlers.ComponentInteraction;

      var commands = Client.UseCommandsNext(new CommandsNextConfiguration
      {
        StringPrefixes = new[] { "!" },
        EnableMentionPrefix = false,
        EnableDms = true,
        EnableDefaultHelp = true,
        DmHelp = true,
        CaseSensitive = false,
        IgnoreExtraArguments = false

      });
      commands.RegisterCommands<BotCommands>();
      commands.SetHelpFormatter<CustomHelpFormatter>();

      Client.UseInteractivity(new InteractivityConfiguration()
      {
        PollBehaviour = PollBehaviour.KeepEmojis,
        Timeout = TimeSpan.FromSeconds(30),
        ResponseBehavior = InteractionResponseBehavior.Ignore
      });

      await Client.ConnectAsync( new DiscordActivity("anything you say!", ActivityType.ListeningTo) , UserStatus.Online);
      await Task.Delay(-1);
    }
    private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
    {
      // let's log the fact that this event occured
      //sender.Logger.LogInformation(BotEventId, "Client is ready to process events.");

      // since this method is not async, let's return
      // a completed task, so that no additional work
      // is done
      return Task.CompletedTask;
    }
    private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
      // let's log the name of the guild that was just
      // sent to our client
      //sender.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");

      // since this method is not async, let's return
      // a completed task, so that no additional work
      // is done
      return Task.CompletedTask;
    }
    private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
    {
      // let's log the details of the error that just 
      // occured in our client
      //sender.Logger.LogError(BotEventId, e.Exception, "Exception occured");

      // since this method is not async, let's return
      // a completed task, so that no additional work
      // is done
      return Task.CompletedTask;
    }
  }
}
