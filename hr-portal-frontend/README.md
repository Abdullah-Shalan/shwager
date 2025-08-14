# HR Portal Frontend

A frontend application for the HR Portal RESTful API, built with vanilla JavaScript and Vite.

## Features

- **Authentication**: Login and registration for both candidates and HR users
- **Candidate Portal**: View available jobs, manage assigned tasks, and update profile
- **HR Portal**: Manage jobs, tasks, and candidates
- **Type Safety**: Full DTO mapping with TypeScript definitions

## DTO Usage

All API functions now use proper DTOs for input/output requests. The DTOs are defined in `src/dtos.d.ts` and provide type safety and consistency with your backend API.

### Authentication DTOs

#### Login
```javascript
import { login } from './api/auth.js';

// Using LoginRequest DTO
const loginData = {
  email: "user@example.com",
  password: "P@ssw0rd"
};

const token = await login(loginData, 'candidate');
```

#### Candidate Registration
```javascript
import { candidateRegister } from './api/auth.js';

// Using CandidateRegisterRequest DTO
const candidateData = {
  firstName: "John",
  lastName: "Doe",
  email: "john.doe@example.com",
  password: "P@ssw0rd",
  resumeUrl: "https://example.com/resume.pdf"
};

const response = await candidateRegister(candidateData);
// Returns CandidateRegisterResponse
```

#### HR Registration
```javascript
import { hrRegister } from './api/auth.js';

// Using HrRegisterRequest DTO
const hrData = {
  firstName: "Jane",
  lastName: "Smith",
  email: "jane.smith@example.com",
  password: "P@ssw0rd"
};

const response = await hrRegister(hrData);
// Returns HrRegisterResponse
```

### Candidate API DTOs

#### Get Available Jobs
```javascript
import { getAvailableJobs } from './api/candidate.js';

const jobs = await getAvailableJobs();
// Returns JobSummaryDto[]
// Each job: { id, title, description, jobTasks? }
```

#### Get Assigned Job
```javascript
import { getAssignedJob } from './api/candidate.js';

const job = await getAssignedJob();
// Returns JobSummaryDto
```

#### Get Tasks
```javascript
import { getTasks } from './api/candidate.js';

const tasks = await getTasks();
// Returns CandidateTaskDto[]
// Each task: { id, order, description, requiresVerification, isVerifiedByHr, requiresFile, filePath, status, completedAt }
```

#### View Profile
```javascript
import { viewProfile } from './api/candidate.js';

const profile = await viewProfile();
// Returns CandidateProfileDto
// { id, email, firstName, lastName, resumeUrl, assignedJobTitle }
```

#### Edit Profile
```javascript
import { editProfile } from './api/candidate.js';

// Using CandidateDto
const profileUpdate = {
  firstName: "John",
  lastName: "Doe"
};

await editProfile(profileUpdate);
```

### HR API DTOs

#### Create Job
```javascript
import { createJob } from './api/hr.js';

// Using JobDto
const jobData = {
  title: "Software Developer",
  description: "Full-stack development position"
};

const response = await createJob(jobData);
// Returns JobResponse
// { id, title, description, assignedHrName }
```

#### Create Job Task
```javascript
import { createJobTask } from './api/hr.js';

// Using JobTaskDto
const taskData = {
  description: "Complete coding assignment",
  requiresFile: true,
  requiresVerification: true
};

const response = await createJobTask(jobId, taskData);
// Returns TaskResponse
// { id, forJobTitle, task, requiresFile, requiresVerification, order, assignedHrName }
```

#### Reorder Tasks
```javascript
import { reorderJobTasks } from './api/hr.js';

// Using ReorderRequest[]
const reorderData = [
  { taskId: 1, order: 1 },
  { taskId: 2, order: 2 },
  { taskId: 3, order: 3 }
];

await reorderJobTasks(jobId, reorderData);
```

#### Get Candidates
```javascript
import { listCandidates } from './api/hr.js';

const candidates = await listCandidates();
// Returns CandidateProfileDto[]
```

## DTO Structure Reference

### Status Enum
```typescript
type Status = 0 | 1 | 2 | 'NotStarted' | 'InProgress' | 'Completed';
// 0 = NotStarted, 1 = InProgress, 2 = Completed
```

### Core DTOs

#### CandidateDto
```typescript
interface CandidateDto {
  firstName: string;
  lastName: string;
}
```

#### CandidateProfileDto
```typescript
interface CandidateProfileDto {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  resumeUrl: string;
  assignedJobTitle: string;
}
```

#### JobSummaryDto
```typescript
interface JobSummaryDto {
  id: number;
  title: string;
  description: string;
  jobTasks?: JobTaskSummaryDto[] | null;
}
```

#### JobTaskDto
```typescript
interface JobTaskDto {
  description: string;
  requiresFile: boolean;
  requiresVerification: boolean;
}
```

#### CandidateTaskDto
```typescript
interface CandidateTaskDto {
  id: number;
  order: number;
  description: string;
  requiresVerification: boolean;
  isVerifiedByHr: boolean;
  requiresFile: boolean;
  filePath: string;
  status: Status;
  completedAt: string | null;
}
```

## Examples

See `src/examples.js` for comprehensive examples of how to use all the DTOs with the API functions.

## Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

## API Endpoints

The frontend communicates with your backend API at the following endpoints:

- **Auth**: `/api/auth/*`
- **Candidate**: `/api/candidate/*`
- **HR**: `/api/hr/*`

All endpoints expect and return data in the format defined by the DTOs.

## Type Safety

The project uses JSDoc comments with TypeScript definitions to provide type safety and IntelliSense support in modern IDEs. All API functions are properly typed with their corresponding DTOs.
