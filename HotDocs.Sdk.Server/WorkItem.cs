/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

namespace HotDocs.Sdk.Server
{
    /// <summary>
    ///     WorkItem is an abstract class representing either a browser interview OR an assembled document.
    ///     Work Sessions (also known as assembly queues) in the Open SDK are composed of one or more WorkItems.
    /// </summary>
    public abstract class WorkItem
    {
        /* Other properties I thought we may need, but which are not implemented currently. */

        //public string ID { get; } // Don't know if we would want or need something like this, maybe if a host app has need to refer to a specific work item in the future?
        //public WorkSession Parent { get; private set; } // If we had this, InterviewWorkItem could fetch the default InterviewOptions from the WorkSession.
        // Without it, we can't, and that's inconvenient.  But we would need to do custom serialization to have this... maybe we'll need to anyway.
        private string _title;

        /// <summary>
        ///     The WorkItem constructor is protected; it is only called from derived WorkItem classes.
        /// </summary>
        /// <param name="template">The template upon which the work item is based.</param>
        protected WorkItem(Template template)
        {
            Template = template;
        }

        /* properties/state */

        /// <summary>
        ///     The title of the work item.  This is used by a host application when presenting user interface showing the
        ///     completed and pending work items.  Right now each work item maintains its own title internally, but eventually
        ///     the title could be derived from elsewhere (for example, the Title property of InterviewOptions, or if that's not
        ///     set then the title associated with a template).
        /// </summary>
        public string Title
        {
            get { return Template.Title; }
            private set { _title = value; }
        }

        /// <summary>
        ///     The template associated with this work item.  For interview work items, this is the template that delivers the
        ///     interview.  For document work items, this is the template that generates the document.  A single template can
        ///     be (and often is) associated with both an interview work item and a document work item.
        /// </summary>
        public Template Template { get; }

        /// <summary>
        ///     A flag indicating whether this work item has been completed or not.  This property is only set by the WorkSession
        ///     to which this WorkItem belongs.
        /// </summary>
        public bool IsCompleted { get; internal set; }
    }
}