using System;

namespace HotDocs.Sdk.Server
{
    /// <summary>
    /// An InterviewWorkItem represents an interview that will be (or has been) presented to the end user in a web browser.
    /// In includes a method to retrieve the interview HTML fragment (when delivering the interview to a browser),
    /// and another to "finish" the interview once answers have been posted back from the browser.
    /// </summary>
    [Serializable]
    public class InterviewWorkItem : WorkItem
    {
        /// <summary>
        /// The constructor is internal; it is only called from the WorkSession class.  The WorkSession
        /// is in charge of adding work items to itself.
        /// </summary>
        /// <param name="template">The template upon which the work item is based.</param>
        internal InterviewWorkItem(Template template)
            : base(template)
        {
        }

        /* methods */

        /// <summary>
        /// Called by the host application when it needs to deliver an interview down to the end user in a web page.
        /// Wraps the underlying IServices.GetInterview call (for convenience).
        /// </summary>
        /// <param name="options">The InterviewOptions to use for this interview.  This parameter makes the WorkSession's
        /// DefaultInterviewOptions redundant/useless. TODO: We need to decide whether to use a parameter, or only the defaults.</param>
        /// <returns></returns>
        public InterviewResult GetInterview(HotDocs.Sdk.Server.Contracts.InterviewOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called by the host application when answers have been posted back from a browser interview.
        /// </summary>
        /// <param name="interviewAnswers">The answers that were posted back from the interview.</param>
        public void FinishInterview(string interviewAnswers)
        {
            // pseudocode:
            // overlay interviewAnswers over the session answer set,
            // if the current template is an interview template
            //     "assemble" it
            //     add pending assemblies to the queue as necessary
            // mark this interview workitem as complete.  (This will cause the WorkSession to advance to the next workItem.)

            throw new NotImplementedException();
        }
    }
}