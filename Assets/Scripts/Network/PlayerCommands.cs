using System;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Commands;

// ReSharper disable once CheckNamespace
public static class PlayerCommands
{
    public static ITAlertClient Client { get; set; }

	public static void PickupItem(ItemBehaviour item, SubsystemBehaviour subsystem)
	{
	    var requestPickupItemCommand = new RequestPickupItemCommand
	    {
            PlayerId = Director.Player.Id,
            ItemId = item.Id,
	        LocationId = subsystem.Id
	    };
        Client.SendGameCommand(requestPickupItemCommand);
		// todo process locally too and resync later Director.RequestPickupItem(item.Id, subsystem.Id);
	}

	public static void Move(SubsystemBehaviour destination)
	{
        var requestMovePlayerCommand = new RequestMovePlayerCommand()
        {
            PlayerId = Director.Player.Id,
            DestinationId = destination.Id
        };
        Client.SendGameCommand(requestMovePlayerCommand);
        // todo process locally too and resync laterDirector.RequestMovePlayer(destination.Id);
    }

    public static void DropItem(ItemBehaviour item)
	{
        var requestDropItemCommand = new RequestDropItemCommand()
        {
            PlayerId = Director.Player.Id,
            ItemId = item.Id
        };
        Client.SendGameCommand(requestDropItemCommand);
        // todo process locally too and resync laterDirector.RequestDropItem(item.Id);
    }

    public static void ActivateItem(ItemBehaviour item)
	{
	    var requestActivateItemCommand = new RequestActivateItemCommand()
	    {
            PlayerId = Director.Player.Id,
            ItemId = item.Id
	    };
        Client.SendGameCommand(requestActivateItemCommand);
        // todo process locally too and resync later Director.RequestActivateItem(item.Id);
    }

    public static void ActivateEnhancement(EnhancementBehaviour enhancement)
	{
		throw new NotImplementedException("Send Command to Simulation");
	}

}
