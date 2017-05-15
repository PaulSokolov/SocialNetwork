namespace BusinessLayer.Interfaces
{
    public interface ISocialNetworkManager
    {
        string Id { get; set; }
        IFriendsCategory Friends { get; }
        IMessagesCategory Messages { get; }
        IUsersCategory Users { get; }
        IDatabaseCategory Database { get; }
    }
}