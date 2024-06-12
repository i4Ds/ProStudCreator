# ProStudCreator

ProStudCreator is a task management and notification system designed for managing and tracking various tasks related to student projects, grades, and administrative tasks within a university setting.

## Table of Contents

- [Features](#features)
- [Task Types](#task-types)
- [Email Notifications](#email-notifications)
  - [Which Emails are Sent](#which-emails-are-sent)
  - [To Which Audience](#to-which-audience)
  - [At Which Date / At Which Regularity / After Which Event](#at-which-date--at-which-regularity--after-which-event)
- [Database Schema](#database-schema)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- Automated task checks and reminders
- Email notifications for various events and deadlines
- Management of project states and transitions
- Support for multiple task types and responsible users
- Detailed logging and error handling

## Task Types

The system manages the following task types:

1. RegisterGrades
2. CheckWebsummary
3. InfoStartProject
4. InfoFinishProject
5. UploadResults
6. PlanDefenses
7. UpdateDefenseDates
8. PayExperts
9. InsertNewSemesters
10. SendGrades
11. SendMarKomBrochure
12. InvoiceCustomers
13. EnterAssignedStudents
14. DoubleCheckMarKomBrochureData
15. CheckBillingStatus
16. SetProjectLanguage
17. SendThesisTitles
18. FinishProject
19. ThesisTitleHint2Weeks
20. ThesisTitleHint2Days
21. GradeDeadlineIP5
22. GradeDeadlineIP6

## Email Notifications

### Which Emails are Sent

1. **Exception Handling Email**
   - **Subject**: "TaskCheck has thrown an Exception"

2. **Thesis Title Reminder Emails**
   - **Subject**: "Informatikprojekte P6: Thesis-Titel Erinnerung" (2 weeks reminder)
   - **Subject**: "Informatikprojekte P6: Thesis-Titel Erinnerung" (2 days reminder)

3. **Grade Deadline Reminder Emails**
   - **Subject**: "Informatikprojekte IP5: Notenabgabe Erinnerung"
   - **Subject**: "Informatikprojekte IP6: Notenabgabe Erinnerung"

4. **Thesis Titles to Admin Email**
   - **Subject**: "Informatikprojekte P6: Thesis-Titel"

5. **Pay Experts Email**
   - **Subject**: "Informatikprojekte P5/P6: Experten-Honorare auszahlen"

6. **Double Check MarKom Brochure Data Email**
   - **Not explicitly named but sent to verify MarKom data.**

7. **MarKom Brochure Email**
   - **Subject**: "Informatikprojekte P6: Projektliste für Broschüre"

8. **Reminder for Responsible Users Email**
   - **Subject**: "Erinnerung von ProStud"

9. **Task Check Mail to Web Admin**
   - **Subject**: "TaskCheck has been run"

10. **Debug Email**
    - **Subject**: "DEBUG: ProStud" (subject varies based on context)

### To Which Audience

1. **Exception Handling Email**
   - **To**: `Global.WebAdmin`

2. **Thesis Title Reminder Emails**
   - **To**: Advisors of ongoing thesis projects (Advisor1, and optionally Advisor2)

3. **Grade Deadline Reminder Emails**
   - **To**: Advisors of IP5 and IP6 projects (Advisor1, and optionally Advisor2)

4. **Thesis Titles to Admin Email**
   - **To**: `Global.WebAdmin` (should be `Global.GradeAdmin`)
   - **To**: `marketing.technik@fhnw.ch`
   - **To**: `nicole.stamm@fhnw.ch`
   - **CC**: `regula.scherrer@fhnw.ch`, `daria.zgraggen@fhnw.ch`, `sibylle.peter@fhnw.ch`

5. **Pay Experts Email**
   - **To**: `Global.WebAdmin` (potentially to other addresses like `Global.PayExpertAdmin` or specific individuals)

6. **Double Check MarKom Brochure Data Email**
   - **To**: Not explicitly stated but intended for verifying MarKom data

7. **MarKom Brochure Email**
   - **To**: `Global.WebAdmin` (should be `Global.MarKomAdmin`)

8. **Reminder for Responsible Users Email**
   - **To**: Responsible users for tasks (mostly advisors or department managers)

9. **Task Check Mail to Web Admin**
   - **To**: `Global.WebAdmin`

10. **Debug Email**
    - **To**: `Global.WebAdmin`

### At Which Date / At Which Regularity / After Which Event

1. **Exception Handling Email**
   - **Event**: When an exception occurs in `RunAllTasks`.

2. **Thesis Title Reminder Emails**
   - **2 weeks reminder**: 2 weeks before the submission deadline.
   - **2 days reminder**: 2 days before the submission deadline.

3. **Grade Deadline Reminder Emails**
   - **IP5 reminder**: 7 days before the IP5 grade submission deadline.
   - **IP6 reminder**: 7 days before the IP6 grade submission deadline.

4. **Thesis Titles to Admin Email**
   - **Event**: After the thesis titles are finalized (a day after the allowed title changes deadline).

5. **Pay Experts Email**
   - **Event**: After the semester ends and all ongoing projects for the semester are finished.

6. **Double Check MarKom Brochure Data Email**
   - **Event**: Projects published between the last and current year’s May 1st require double-checking data for the MarKom brochure.

7. **MarKom Brochure Email**
   - **Event**: Around June 1st each year, before the project list is finalized for the brochure.

8. **Reminder for Responsible Users Email**
   - **Event**: Periodically, based on the days between reminders configured for each task type.

9. **Task Check Mail to Web Admin**
   - **Event**: After the `CheckAllTasks` or `ForceCheckAllTasks` method runs.

## Database Schema

### Tables

- **Projects**: Contains project details including state, advisors, and log information.
- **Tasks**: Contains tasks with references to projects, responsible users, and task types.
- **Semesters**: Contains semester details including start and end dates.
- **TaskRuns**: Logs each run of the task check process.
- **UserDepartmentMap**: Maps users to departments and roles.

