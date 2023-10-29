using Discord;
using Discord.Interactions;

namespace Aidoneus.API.Preconditions;

public class RequiresVoiceConnection : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        if (context.Guild == null) {
            return PreconditionResult.FromError("This command must be run in a guild");
        }
        var guildUser = await context.Guild.GetUserAsync(context.User.Id);
        if (guildUser.VoiceChannel == null) {
            return PreconditionResult.FromError("You must be in a voice channel to use this command.");
        }
        return PreconditionResult.FromSuccess();
    }
}