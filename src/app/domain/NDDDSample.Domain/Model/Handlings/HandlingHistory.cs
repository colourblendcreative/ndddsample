﻿namespace NDDDSample.Domain.Model.Handlings
{
    #region Usings

    using System.Collections.Generic;
    using JavaRelated;
    using Shared;

    #endregion

    /// <summary>
    /// The handling history of a cargo.
    /// </summary>
    public class HandlingHistory : IValueObject<HandlingHistory>
    {
        public static readonly HandlingHistory EMPTY = new HandlingHistory(new List<HandlingEvent>());
        private readonly List<HandlingEvent> handlingEvents;

        public HandlingHistory(IEnumerable<HandlingEvent> handlingEvents)
        {
            Validate.notNull(handlingEvents, "Handling events are required");

            this.handlingEvents = new List<HandlingEvent>(handlingEvents);
        }

        #region IValueObject<HandlingHistory> Members

        /// <summary>
        /// Value objects compare by the values of their attributes, they don't have an identity.
        /// </summary>
        /// <param name="other">The other value object.</param>
        /// <returns>true if the given value object's and this value object's attributes are the same.</returns>
        public bool SameValueAs(HandlingHistory other)
        {
            return other != null && handlingEvents.Equals(other.handlingEvents);
        }

        #endregion

        /// <summary>
        /// A distinct list (no duplicate registrations) of handling events, ordered by completion time.
        /// </summary>
        /// <returns></returns>
        public IList<HandlingEvent> DistinctEventsByCompletionTime()
        {
            var ordered = new List<HandlingEvent>(new HashSet<HandlingEvent>(handlingEvents));

            ordered.Sort((he1, he2) => he1.CompletionTime().CompareTo(he2.CompletionTime()));

            return new List<HandlingEvent>(ordered).AsReadOnly();
        }

        /// <summary>
        ///  Most recently completed event, or null if the delivery history is empty.
        /// </summary>
        /// <returns></returns>
        public HandlingEvent MostRecentlyCompletedEvent()
        {
            IList<HandlingEvent> distinctEvents = DistinctEventsByCompletionTime();
            if (distinctEvents.IsEmpty())
            {
                return null;
            }
            return distinctEvents[distinctEvents.Count - 1];
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (HandlingHistory) obj;
            return SameValueAs(other);
        }

        public override int GetHashCode()
        {
            //TODO: atrosin revise if for the list hash code work like in java
            return handlingEvents.GetHashCode();
        }
    }
}