using System;
using GreatWorkIvory.Events.EventTypes;
using SecretHistories.Commands;
using SecretHistories.Entities;
using SecretHistories.States;

namespace GreatWorkIvory.Events.Links
{
    public static class SituationLinks
    {

        [EventLink(typeof(SituationEvent.StateChanged))]
        public class StateEntered<T> : IEventLink where T : SituationState
        {
            public readonly Situation Situation;
            public readonly SituationState NewState;

            public StateEntered(SituationEvent.StateChanged e)
            {
                Situation = e.Situation;
                NewState = e.NewState;
            }

            public bool IsValid()
            {
                return NewState is T;
            }
        }

        [EventLink(typeof(SituationEvent.CommandExecuted))]
        public class CommandExecuted<T> : IEventLink where T : ISituationCommand
        {
            public readonly Situation Situation;
            private readonly ISituationCommand _command;
            public T Command => (T) _command;

            public CommandExecuted(SituationEvent.CommandExecuted e)
            {
                Situation = e.Situation;
                _command = e.Command;
            }

            public bool IsValid()
            {
                return _command is T;
            }
 
            [EventLink(typeof(SituationEvent.CommandExecuted.Pre))]
            public class Pre : CommandExecuted<T>
            {
                public Pre(SituationEvent.CommandExecuted e) : base(e)
                {
                }
            }
            
            [EventLink(typeof(SituationEvent.CommandExecuted.Post))]
            public class Post : CommandExecuted<T>
            {
                public Post(SituationEvent.CommandExecuted e) : base(e)
                {
                }
            }
        }
        
    }
}