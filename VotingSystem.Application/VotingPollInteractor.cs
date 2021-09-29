namespace VotingSystem.Application
{
    public class VotingPollInteractor
    {
        private readonly IVotingPollFactory _factory;
        private readonly IVotingSystemPersistance _persistance;

        public VotingPollInteractor(IVotingPollFactory _factory, IVotingSystemPersistance _persistance)
        {
            this._factory = _factory;
            this._persistance = _persistance;
        }

        public void CreateVotingPoll(VotingPollFactory.Request request)
        {
           var poll = _factory.Create(request);

            _persistance.SaveVotingPoll(poll);
        }
    }

    
}
