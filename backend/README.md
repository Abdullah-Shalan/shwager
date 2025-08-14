
# Explanation of each entity in the system

## 1. `Hr` (Human Resources)

### What it does:

Represents the HR user — the person managing job positions and overseeing the onboarding process.

### Contribution:

* Creates jobs and their associated onboarding tasks.
* Invites candidates and assigns them to jobs.
* Monitors each candidate’s progress through their onboarding checklist.

### Why it's needed:

Without an HR actor, there’s no source of task creation, job posting, or candidate assignment — they’re the driver of the onboarding process.

---

## 2. `Job`

### What it does:

Represents a job opening or position, such as “Software Engineer” or “Sales Intern”.

### Contribution:

* Serves as the anchor point for onboarding — each job has a custom set of onboarding tasks.
* Candidates are assigned to specific jobs (which determine what tasks they must complete).

### Why it's needed:

Different jobs often have different onboarding requirements (e.g. legal, training, equipment setup). This entity makes onboarding customizable per role.

---

## 3. `Candidate`

### What it does:

Represents a person going through onboarding for a job.

### Contribution:

* Receives onboarding tasks via their assigned job.
* Marks their tasks as completed (progress tracking).
* Is linked to HR through the job they are assigned to.

### Why it's needed:

They are the core participant in the onboarding process — the system revolves around helping candidates complete their setup to join the company.

---

## 4. `JopTask`

### What it does:

Represents a task required for onboarding, such as:

* “Upload National ID”
* “Sign NDA”
* “Complete security training”

Each task belongs to a specific `Job`.

### Contribution:

* Defines the requirements that candidates must complete to be considered fully onboarded.
* Is reused across multiple candidates applying for the same job.

### Why it's needed:

Formalizes onboarding into structured steps — no ambiguity, everything is trackable and clear.

---

## 5. `CandidateTask`

### What it does:

A link between a candidate and an onboarding task. Tracks:

* Whether a candidate has started/completed a specific task
* When they finished it
* What status it’s in (e.g. NotStarted, InProgress, Completed)

### Contribution:

* Provides individual task tracking for each candidate.
* Allows HR to view granular progress (who is stuck, who is done).

### Why it's needed:

Without this, you couldn't track progress at a per-candidate level — it adds personalization and traceability to the process.

---

## 6. `TaskStatus` (Enum)

### What it does:

Defines allowed states of a task:

* NotStarted
* InProgress
* Completed

### Contribution:

* Standardizes how the system understands progress.
* Used in reporting and conditional logic (e.g. notify HR when all tasks are completed).

### Why it's needed:

Helps track lifecycle of each task — it's essential for dashboards, reminders, and analytics.

---

## 🧩 Summary — How They Work Together

| Action                              | Entity Involved                   |
| ----------------------------------- | --------------------------------- |
| HR logs in and creates a job        | `Hr`, `Job`                       |
| HR adds onboarding tasks            | `Job`, `JopTask`           |
| HR invites a candidate to the job   | `Candidate` linked to `Job`       |
| Candidate views and completes tasks | `CandidateTask`, `TaskStatus`     |
| HR monitors progress                | `CandidateTask`, `JopTask` |

# Features for each entity

## HR

- Create HR account
- Login to dashboard
- Create a Job with title and description
- Create job tasks for each Job
- Invite/Register a Candidate to a Job
- View candidate profile and task progress
- Verify a completed task
- Delete a candidate
- Delete a job (with all tasks and candidates)
- Edit Job title or tasks
- Receive notifications when candidates complete tasks (optional)
- Resend invitation email to candidate (optional)

## Candidate

- Accept invitation / register
- Login to candidate portal
- View assigned tasks
- Mark task as completed
- Upload required documents
- See verification status for each task
- View job title and HR contact info
- Edit profile details (optional)
- Add comments or questions on tasks (optional)


## Job

- Belongs to one HR
- Has many onboarding tasks
- Can have multiple candidates
- Can be edited or deleted by HR
- Defines onboarding requirements per position


## JopTask

- Created by HR under a Job
- Assigned to all candidates of the job
- Can be reordered or deleted by HR
- Can require a file upload (optional)
- Can be marked as AutoVerifiable (optional)


## CandidateTask

- Tracks task status: NotStarted, InProgress, Completed
- Stores timestamp when completed
- Tracks if verified by HR
- Triggers notification to HR when completed (optional)
- Stores document path (optional)