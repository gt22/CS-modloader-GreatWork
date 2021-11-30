using SecretHistories.Commands;
using SecretHistories.Entities;
using SecretHistories.States;

namespace GreatWorkIvory.Events.EventTypes
{
    public class SituationEvent : Event
    {
        public readonly Situation Situation;

        public SituationEvent(Situation situation)
        {
            Situation = situation;
        }

        public class StateChanged : SituationEvent
        {
            public readonly SituationState NewState;
            
            public StateChanged(Situation situation) : base(situation)
            {
                NewState = situation.State;
            }
        }

        public class TimerValueChanged : SituationEvent
        {
            public TimerValueChanged(Situation situation) : base(situation)
            {
            }
        }

        public class SphereContentChanged : SituationEvent
        {
            public SphereContentChanged(Situation situation) : base(situation)
            {
            }
        }

        public class CommandExecuted : SituationEvent
        {
            public readonly ISituationCommand Command;

            public CommandExecuted(Situation situation, ISituationCommand command) : base(situation)
            {
                Command = command;
            }

            public class Pre : CommandExecuted
            {
                public Pre(Situation situation, ISituationCommand command) : base(situation, command)
                {
                }
            }

            public class Post : CommandExecuted
            {
                public Post(Situation situation, ISituationCommand command) : base(situation, command)
                {
                }
            }
        }
    }
}