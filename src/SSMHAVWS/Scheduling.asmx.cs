/* Web Service that does various things with scheduling forms.
 * Version 00.91.00.161206
 */

/*
 * This code is very much beta, and will require modification to work in your environment. Additional notes
 * and changelog is located at the end of this file.
 *
 * This code is over-commented, the intention being that it's abundently clear as to what it does, and how
 * it works.
*/

/* The MIT License(MIT)
 *
 * Copyright(c) 2016 A Pretty Cool Program
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions
 * of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using NTST.ScriptLinkService.Objects;
using System;
using System.Web.Services;

namespace SSMHAVWS
{
    /// <summary>Summary description for Schedule.</summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Schedule : System.Web.Services.WebService
    {
        /// <summary>Main logic for the Web Service.</summary>
        /// <param name="sentOptionObject">The OptionObject2 from Avatar.</param>
        /// <param name="scriptAction">The passed script action (AppointmentDurationCheck).</param>
        /// <returns>A completed OptionObject2.</returns>
        /// <remarks>This method is required by Avatar.</remarks>
        [WebMethod]
        public OptionObject2 RunScript(OptionObject2 sentOptionObject, string scriptAction)
        {
            switch (scriptAction)
            {
                case "SchedCalApptDurCheck":
                    return AppointmentDurationCheck(sentOptionObject);

                default: // ERROR!
                    return sentOptionObject;
            }
        }

        /// <summary>Provides the version number of this script.</summary>
        /// <returns>The script version number./returns>
        /// <remarks>This method is required by Avatar.</remarks>
        [WebMethod]
        public string GetVersion()
        {
            return "VERSION 00.91.00 BUILD 161216";
        }

        /// <summary>Verifies that an appointment's duration matches the Service Code definition.</summary>
        /// <param name="sentOptionObject">The Optionbject2 object.</param>
        /// <returns>A completed OptionObject2.</returns>
        public static OptionObject2 AppointmentDurationCheck(OptionObject2 sentOptionObject)
        {
            /* This method is used with the Scheduling Calendar form, and checks to make sure that an appointment's
             * duration falls within the defined paramaters of its Service Code definition. This method will have
             * one of four results:
             *      1. The appointment is "Per Event", and therefore has no set duration. The user will not recieve
             *         a message, and the appointment is scheduled normally.
             *      2. The appointment duration falls outside the minimum/maximum minutes, as defined by the System
             *         Code. The user is notified, and then returned to the scheduling calendar to modify the
             *         appointment start/end times until they fall withing the System Code definition.
             *      3. The appointment duration exceeds the maximum minutes, as defined by the System Code. The user
             *         recieves a warning, which they can choose to ignore. If the warning is ignored, the appointment
             *         is scheduled normally, otherwise the user is returned to the form to modify the appointment
             *         start/end times.
             *      4. The appointment falls withing the minimum/maximum minutes, as defined by the System Code. The
             *         user does not recieve a message, the appointment is scheduled normally.
             */

            // These are the ID numbers for the Service Code, Appointment Start Time, and Appointment End Time fields.
            // You will need to modify these to match your environment.
            const string serviceCodeField          = "10002";
            const string appointmentStartTimeField = "10107";
            const string appointmentEndTimeField   = "10108";

            // Placeholders for the Service Code and Appointment Start/End times.
            var serviceCode = 0;
            var appointmentStartTime = default(DateTime);
            var appointmentEndTime = default(DateTime);

            // Loop through the forms in the sent option, and if we hit a the "Service Code", "Appointment Start
            // Time", or "Appointment End Time" fields, store their values.
            // TODO - More efficient way of doing this? Maybe just going to the specific field?
            foreach (var form in sentOptionObject.Forms)
            {
                foreach (var field in form.CurrentRow.Fields)
                {
                    switch (field.FieldNumber)
                    {
                        case serviceCodeField:
                            serviceCode = int.Parse(field.FieldValue);
                            break;

                        case appointmentStartTimeField:
                            appointmentStartTime = Convert.ToDateTime(field.FieldValue);
                            break;

                        case appointmentEndTimeField:
                            appointmentEndTime = Convert.ToDateTime(field.FieldValue);
                            break;

                        default:
                            break;
                    }
                }
            }

            // Placeholders for Service Code details.
            var serviceCodeName         = string.Empty;
            var serviceCodeMinDuration  = 0;
            var serviceCodeMaxDuration  = 0;
            var serviceCodeMinSoftLimit = 0;
            var serviceCodeMaxSoftLimit = 0;

            /* This is where we set the details for each Service Code. You'll need to modify these  to match your
             * environment. Each Service Code will need a case statement that contains the following:
             *      serviceCodeName         = Name of the Service Code (i.e. "Family Therapy")
             *      serviceCodeMinDuration  = Minimum appointment duration, in minutes (i.e. "15")
             *      serviceCodeMaxDuration  = Maximum appointment duration, in minutes (i.e. "60")
             *      serviceCodeMinSoftLimit = Lower value for a soft limit, in minutes (i.e. "15") - optional, not used
             *      serviceCodeMaxSoftLimit = Upper value for a soft limit, in minutes (i.e. "60") - optional
             * The "soft limit" values are optional, and are used when a user should be warned that an appointment
             * duration falls outside of the Service Code definition, but does not require the duration to be
             * changed. For instance, if a Service Code defines the maximum duration of an appointment as 60 minutes,
             * but exceeding that duration is allowed, the user can ignore the warning and schedule the appointment
             * normally.
             */
            switch (serviceCode)
            {
                case 10:
                    // No maximum duration, but if it's >60, warn the user. Currently scheduled in Avatar.
                    serviceCodeName         = "Individual Therapy";
                    serviceCodeMinDuration  = 38;
                    serviceCodeMaxDuration  = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 60;
                    break;

                case 15:
                    // Currently scheduled in Avatar.
                    serviceCodeName         = "Individual Therapy";
                    serviceCodeMinDuration  = 16;
                    serviceCodeMaxDuration  = 37;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 20:
                case 25:
                    // Per event, no warning necessary. Currently scheduled in Avatar.
                    serviceCodeName         = "Family/Couple Therapy";
                    serviceCodeMinDuration  = 45;
                    serviceCodeMaxDuration  = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 90;
                    break;

                case 26:
                    // Per event, no warning necessary. Currently not scheduled in Avatar.
                    serviceCodeName = "Family/Couple Therapy";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 90;
                    break;

                case 30:
                    // Has a guarantor version - maximum duration TBD. Currently scheduled in Avatar.
                    serviceCodeName = "Collateral Consultation";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 0; // TODO
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 360;
                    break;

                case 35:
                    // Has a guarantor version - maximum duration TBD. Currently scheduled in Avatar.
                    serviceCodeName = "Family Consultation";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 120; // TODO
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 360;
                    break;

                case 37:
                    // Per event, no warning necessary. Currently not scheduled in Avatar.
                    serviceCodeName = "Bridge Consultation";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 180;
                    break;

                case 38:
                    // Maximum duration TBD. Currently scheduled in Avatar.
                    serviceCodeName = "Collateral Case Contact";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 0; // TODO
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 360;
                    break;

                case 40:
                    // Per event, no warning necessary. Currently scheduled in Avatar.
                    serviceCodeName = "Group Therapy";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 120;
                    break;

                case 50:
                case 52:
                    // No maximum duration, but if it's >90, warn the user. Currently scheduled in Avatar.
                    serviceCodeName = "Diagnostic";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 90;
                    break;

                case 70:
                    // Per event, not scheduled. Currently not scheduled in Avatar.
                    serviceCodeName = "Crisis Intervention";
                    serviceCodeMinDuration = 0;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 77:
                    // Not scheduled. Currently not scheduled in Avatar.
                    serviceCodeName = "Mobile Crisis Intervention";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 1440;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 78:
                    // Per event, not scheduled. Currently not scheduled in Avatar.
                    serviceCodeName = "Crisis Bed";
                    serviceCodeMinDuration = 0;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 79:
                    // Per event, not scheduled. Currently not scheduled in Avatar.
                    serviceCodeName = "Crisis Bed";
                    serviceCodeMinDuration = 0;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 80:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Day Treatment Full Day";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 480;
                    break;

                case 85:
                    // No maximum duration, not scheduled. Currently not scheduled in Avatar.
                    serviceCodeName = "PACT SERVICES";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 1440;
                    break;

                case (801):
                    // UPDATE
                    serviceCodeName = "TBD";
                    serviceCodeMinDuration = 38;
                    serviceCodeMaxDuration = 0;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 60;
                    break;

                case (820 - 890):
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Psychological Testing";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 480;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 870:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Nuero psch";
                    serviceCodeMinDuration = 45;
                    serviceCodeMaxDuration = 480;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 949:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Community Support/Case Mnmt.";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 1440;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 952:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "None";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 1440;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 960:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "None";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 1440;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 961:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "None";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 1440;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1511:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Home Visit";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 120;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1521:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Center Based Individual";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 120;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1531:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Child-Focus Group";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 150;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1532:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Community Child Group";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 150;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1541:
                    // Currently not scheduled in Avatar.
                    serviceCodeName = "Parent-Focus Group";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 120;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1561:
                    // Double entry. Currently not scheduled in Avatar.
                    serviceCodeName = "Follow-up Screening";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 360;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                case 1562:
                    // Double entry. Currently not scheduled in Avatar.
                    serviceCodeName = "Follow-up Assessment";
                    serviceCodeMinDuration = 8;
                    serviceCodeMaxDuration = 120;
                    serviceCodeMinSoftLimit = 0;
                    serviceCodeMaxSoftLimit = 0;
                    break;

                default:
                    break;
            }

            // Placeholders for potential error message information
            var errorMessageBody = string.Empty;
            var errorMessageCode = 0;

            // Determine the length of the appointment, in minutes.
            var totalMinutes = NumberOfMinutes(appointmentStartTime, appointmentEndTime);

            // If the Service Code minimum duration is "0", then this is a "Per Event" duration.
            var durationIsPerEvent = (serviceCodeMinDuration == 0) ? true : false;

            /* If the appointment duration defined by the Service Code is "Per Event", then the appointment will be
             * scheduled normally, otherwise we will end up displaying a message to the user, depending on a few
             * variables.
             */
            if (!durationIsPerEvent)
            {
                /* If the Service Code DOESN'T have a soft limit for the maximum appointment duration, AND
                 * the total minutes is less than the defined minimum, OR more than the defined maximum, a message will
                 * be displayed informing the user that the appiontment falls outside of the defined paramaters, and they
                 * will be returned to the form to make corrections. If the duration falls withing the limits, the
                 * appointment will be scheduled normally.
                 */
                if (serviceCodeMaxSoftLimit == 0)
                {
                    if ((totalMinutes < serviceCodeMinDuration) || (totalMinutes > serviceCodeMaxDuration))
                    {
                        errorMessageBody = CreateScheduleErrorMessage(serviceCode, totalMinutes, serviceCodeMinDuration, serviceCodeMaxDuration, false);
                        errorMessageCode = 1;
                    }
                }

                /* If the Service Code DOES have a soft limit for the maximum appointment duration, and
                 * the total minutes is less than the defined minimum, a message will
                 * be displayed informing the user that the appiontment falls outside of the defined paramaters, and they
                 * will be returned to the form to make corrections. If the duration falls withing the limits, the
                 * appointment will be scheduled normally. If the duration falls outside of the soft limit, the user will
                 * be given a warning with the option to ignore or return.
                 */
                // TODO - The way that the soft limit replaces the max duration is a bit hacky, and should be changed.
                else if (serviceCodeMaxSoftLimit >= 1)
                {
                    if ((totalMinutes < serviceCodeMinDuration))
                    {
                        errorMessageBody = CreateScheduleErrorMessage(serviceCode, totalMinutes, serviceCodeMinDuration, serviceCodeMaxSoftLimit, false);
                        errorMessageCode = 1;
                    }
                    else if ((totalMinutes > serviceCodeMaxSoftLimit))
                    {
                        errorMessageBody = CreateScheduleErrorMessage(serviceCode, totalMinutes, serviceCodeMinDuration, serviceCodeMaxSoftLimit, true);
                        errorMessageCode = 2;
                    }
                }
            }

            OptionObject2 returnOptionObject = new OptionObject2();

            // As long as there is an error code, add the error message info to the return object.
            if (errorMessageCode != 0)
            {
                returnOptionObject.ErrorCode = errorMessageCode;
                returnOptionObject.ErrorMesg = errorMessageBody;
            }

            // Complete the OptionObject2 that we are going to return, then return it.
            return CompleteOptionObject(sentOptionObject, returnOptionObject, true, false);
        }

        /// <summary>Completes the content of an OptionObject2 object.</summary>
        /// <param name="sentOptionObject">A complete OptionObject that contains the original data.</param>
        /// <param name="returnOptionObject">Data that will add to, or overwrite, data in the sentOptionObject.</param>
        /// <param name="recommended">Fields that are recommended to set (true/false) [true].</param>
        /// <param name="notRecommended">Fields that not recommended to set (true/false) [false].</param>
        /// <returns></returns>
        public static OptionObject2 CompleteOptionObject(OptionObject2 sentOptionObject, OptionObject2 returnOptionObject, bool recommended, bool notRecommended)
        {
            /* This method will make sure that all of the fields of an OptionObject2 object that are not explicitly set
             * are set to whatever the original values in "sentOptionObject" were. This ensures that the OptionObject2
             *  that is returned to Avatar contains the required information. Currently this is done using brute force,
             *  but eventually it will be accomplished with a loop.
             */

            // Init new object, duh.
            OptionObject2 completedOptionObject = new OptionObject2();

            // Since these fields MUST be explicitly set prior to returning the OptionObject2, they are always set. If
            // these fields are null when returned to Avatar, the script will fail.
            completedOptionObject.EntityID = sentOptionObject.EntityID;
            completedOptionObject.Facility = sentOptionObject.Facility;
            completedOptionObject.NamespaceName = sentOptionObject.NamespaceName;
            completedOptionObject.OptionId = sentOptionObject.OptionId;
            completedOptionObject.ParentNamespace = sentOptionObject.ParentNamespace;
            completedOptionObject.ServerName = sentOptionObject.ServerName;
            completedOptionObject.SystemCode = sentOptionObject.SystemCode;

            // Since it is recommended that these be explicitly set prior to returning the OptionObject2, they should
            // always be set by passing "true" as the value for the "recommended" argument. The if statement does its
            // best job to catch any invalid argument values.
            if (recommended != false)
            {
                completedOptionObject.EpisodeNumber = sentOptionObject.EpisodeNumber;
                completedOptionObject.OptionStaffId = sentOptionObject.OptionStaffId;
                completedOptionObject.OptionUserId = sentOptionObject.OptionUserId;

                // If the returnOptionObject has data, use that to complete the completedOptionObject. Otherwise, use
                // the data that exists in the sentOptionObject.
                if (returnOptionObject.ErrorCode >= 1)
                {
                    completedOptionObject.ErrorCode = returnOptionObject.ErrorCode;
                    completedOptionObject.ErrorMesg = returnOptionObject.ErrorMesg;
                }
                else
                {
                    completedOptionObject.ErrorCode = sentOptionObject.ErrorCode;
                    completedOptionObject.ErrorMesg = sentOptionObject.ErrorMesg;
                }
            }

            // Since it is recommended that these NOT BE explicitly set prior to returning the OptionObject2, avoid
            // setting them by passing "false" as the value for the "recommended" argument. Generally, if these fields
            // contiain data when returned to Avater, this script will fail. The if statement does its  best job to
            // catch any invalid argument values.
            if (notRecommended == true)
            {
                completedOptionObject.Forms = sentOptionObject.Forms;
            }

            return completedOptionObject;
        }

        /// <summary>Determine the number of minutes in an appointment.</summary>
        /// <param name="appointmentStartTime">Appointment start time.</param>
        /// <param name="appointmentEndTime">Appointment end time.</param>
        /// <returns>The appointment duration, in minutes.</returns>
        public static int NumberOfMinutes(DateTime appointmentStartTime, DateTime appointmentEndTime)
        {
            // Get the total timespan for the appointment, convert it to minutes, then build a descriptive error message.
            TimeSpan totalTimeSpan = appointmentEndTime - appointmentStartTime;
            return Convert.ToInt32(totalTimeSpan.TotalMinutes);
        }

        /// <summary>Create the error message.</summary>
        /// <param name="serviceCode">The Service Code.</param>
        /// <param name="totalMinutes">Appointment duration, in minutes.</param>
        /// <param name="serviceCodeMinDuration">Appointment duration minimum.</param>
        /// <param name="serviceCodeMaxDuration">Appointment duration maximum.</param>
        /// <param name="softLimitWarning">If this has a soft limit (true/false) [false].</param>
        /// <returns></returns>
        public static string CreateScheduleErrorMessage(int serviceCode, int totalMinutes, int serviceCodeMinDuration, int serviceCodeMaxDuration, bool softLimitWarning)
        {
            if (softLimitWarning)
            {
                return "The appointment duration of " + totalMinutes + " minutes exceeds the recommended maximum duration of " + serviceCodeMaxDuration + " minutes for Service Code " + serviceCode + ".\n\rWould you like to continue to schedule this appointment (YES), or return to the schedule (CANCEL)?";
            }
            else
            {
                return "The appointment duration of " + totalMinutes + " minutes is not valid for Service Code " + serviceCode + ".\n\rThe appoinment duration for Service Code " + serviceCode + " must be between " + serviceCodeMinDuration + " and " + serviceCodeMaxDuration + ".\n\rPlease recheck the Service Code.";
            }
        }
    }
}

/* ADDITIONAL NOTES
 *
 * These are the possible Error Codes that can be used with Avatar:
 *      1: Returns an Error Message and stops further processing of scripts (if set)
 *      2: Returns an Error Message with OK/Cancel buttons (further scripts are stopped if Cancelled)
 *      3: Returns an Error Message with OK button
 *      4: Returns an Error Message with Yes/No buttons (further scripts are stopped if No)
 *      5: Returns a URL to be opened in a new browser
 */

/* CHANGELOG
 * =========
 *
 * 00.91.00.161206
 * ---------------
 * * Renamed "Schedule.asmx" to "Scheduling.asmx"
 * * Renamed the "AppointmentDurationCheck" scriptAction to "SchedCalApptDurCheck"
 *
 * 00.90.00.161107
 * ---------------
 * > Initial release
 */
